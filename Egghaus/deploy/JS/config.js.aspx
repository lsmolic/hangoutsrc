<%@ Page Language="C#" ResponseEncoding="ISO-8859-1" %>

<%
Response.Cache.SetCacheability(HttpCacheability.NoCache);
Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
Response.Cache.SetNoStore();
Response.AppendHeader("Pragma", "no-cache");
%>

/*global Hangout, window, navigator */

/*
This is our interface to server-generated config values.
Some items that need to be derived from client data, such as FB info,
may be added to this object or get their values populated in JS code 
later on (see minigame_container.js).
*/

// <%= DateTime.UtcNow.ToString() %>

Hangout.require("Hangout.log");
Hangout.require("Hangout.util");

Hangout.config = 
{
	state_server_location : "<%= WebConfig.StateServerIp %>",
	asset_base_url : "<%= WebConfig.AssetBaseUrl %>",
	web_services_base_url : "<%= WebConfig.WebServicesBaseUrl %>",
	fb_api_key : "<%= WebConfig.FacebookAPIKey %>",
	
	facebook_base_url : "<%= WebConfig.FacebookBaseURL %>",
	web_host_base_url : "<%= WebConfig.WebHostBaseURL %>",
	unity3d_fileloc : "<%= WebConfig.Unity3DFileLoc %>",

	error_mode : "<%= WebConfig.JavascriptErrorMode %>",
	instance_name : "<%= WebConfig.InstanceName %>",

	metrics_api_key : "<%= WebConfig.MetricsAPIKey %>",

	metrics_funnel_id : "ho_onramp_3",
/*	metrics_funnel_step_names: {
			    	   1: "started",
			       	   2: "selected_avatar",
				   3: "clicked_save_look",
				   4: "showed_get_started",
				   5: "clicked_get_started",
				   6: "loaded_unity_install_prompt",
				   7: "completed_unity_install"
			       	   },
*/
    metrics_funnel_step_names: {
                   1: "started",
			       2: "selected_avatar",
				   3: "showed_get_started",
				   4: "clicked_get_started",
				   5: "loaded_unity_install_prompt",
				   6: "completed_unity_install"
			       	   },
	web_entry_point : "FashionMinigame",

	fb_canvas_height : "1300px",

	login_use_connect: false,

	// These may or may not be populated by JS logic that runs later on
	selected_avatar: null,
	fb_session_id: null,
	fb_account_id: null,
	campaign_id: null
};

Hangout.createModule("Hangout.config");

if (Hangout.config.error_mode === "DEBUG")
{
	window.onerror = function(msg, url, line)
	{
		window.alert("Unhandled error: " + msg + "\n" + url.split("?")[0] + "\nLine " + line);
	};
}

var MixpanelLib;

if (MixpanelLib && (typeof MixpanelLib === "function"))
{
	Hangout.createModule("Hangout.metrics");
	Hangout.metrics.MP = new MixpanelLib(Hangout.config.metrics_api_key, "Hangout.metrics.MP");
	Hangout.metrics.track = function (eventName, extraProperties, callback)
        {
		var props = {};
		props["campaign"] = Hangout.config.campaign_id;
		//props["userAgent"] = navigator.userAgent;
		props["browserName"] = Hangout.util.browserDetect.browser + " " + Hangout.util.browserDetect.version;
		props["OS"] = Hangout.util.browserDetect.OS;
		for (var prop in extraProperties)
		{
			props[prop] = extraProperties[prop];
		}
		Hangout.log.debug("metrics.track " + eventName + ": " + JSON.stringify(props));
		return Hangout.metrics.MP.track(eventName, props, callback);
	};

	Hangout.metrics.track_funnel = function (stepNumber, extraProperties, callback)
	{
		var stepName = Hangout.config.metrics_funnel_step_names[stepNumber];
		var props = {};
		props["campaign"] = Hangout.config.campaign_id;
		//props["userAgent"] = navigator.userAgent;
		props["browserName"] = Hangout.util.browserDetect.browser + " " + Hangout.util.browserDetect.version;
		props["OS"] = Hangout.util.browserDetect.OS;
		for (var prop in extraProperties)
		{
			props[prop] = extraProperties[prop];
		}
		Hangout.log.debug("metrics.track_funnel" + stepName + ": " + JSON.stringify(props));
		return Hangout.metrics.MP.track_funnel(Hangout.config.metrics_funnel_id, stepNumber, stepName, props, callback);
	};

	Hangout.metrics.track_custom_funnel = function (funnelId, stepNumber, stepName, extraProperties, callback)
	{
		var props = {};
		props["campaign"] = Hangout.config.campaign_id;
		//props["userAgent"] = navigator.userAgent;
		props["browserName"] = Hangout.util.browserDetect.browser + " " + Hangout.util.browserDetect.version;
		props["OS"] = Hangout.util.browserDetect.OS;
		for (var prop in extraProperties)
		{
			props[prop] = extraProperties[prop];
		}
		Hangout.log.debug("metrics.track_custom_funnel" + stepName + ": " + JSON.stringify(props));
		return Hangout.metrics.MP.track_funnel(funnelId, stepNumber, stepName, props, callback);
	};
}
