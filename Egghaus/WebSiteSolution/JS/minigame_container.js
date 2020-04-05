/*global Hangout, window, document, $, FB, FB_RequireFeatures */

Hangout.require("Hangout.config");
Hangout.require("Hangout.log");
Hangout.require("Hangout.metrics");
Hangout.require("Hangout.unityDispatcher");
Hangout.require("Hangout.util");

// Unity -> JS query for a single config value
Hangout.unityDispatcher.exposeFunctionToUnity("requestConfigValue", true, function (keystring)
{
    Hangout.log.debug("requestConfigValue(" + keystring + ") -> " + Hangout.config[keystring]);
    return Hangout.config[keystring];
});

// Unity -> JS query for the entire config dictionary
Hangout.unityDispatcher.exposeFunctionToUnity("requestConfigObject", true, function ()
{
    Hangout.log.debug("requestConfigObject() -> " + JSON.stringify(Hangout.config));
    return JSON.stringify(Hangout.config);
});

// Fashion game uses this to post a feed update
Hangout.unityDispatcher.exposeFunctionToUnity("requestFeedPost",
					      false,
					      function (callback,
							targetId,
							userMessagePrompt,
							defaultUserMessage,
							actionLinkName,
							actionLinkURL,
							attachmentName,
							attachmentURL,
							attachmentCaption,
							attachmentDescription,
							attachmentImageSrcURL,
							attachmentImageLinkURL)
{
    $("#unity_background").show();
    Hangout.util.hideUnityContainer();
    
    var attachment =
	{
	    "name" : attachmentName,
	    "href" : Hangout.util.getInstanceURL(Hangout.config.facebook_base_url,
						 attachmentURL),
	    "caption" : attachmentCaption,
	    "description" : attachmentDescription,
	    "properties" : {},
	    "media" : [{'type': 'image',
			'src': Hangout.util.getInstanceURL(Hangout.config.web_host_base_url,
							   attachmentImageSrcURL),
			'href': Hangout.util.getInstanceURL(Hangout.config.facebook_base_url,
							    attachmentImageLinkURL)
		       }]
	};
    var action_links =
	[{
	    "text" : actionLinkName,
	    "href" : Hangout.util.getInstanceURL(Hangout.config.facebook_base_url,
						 actionLinkURL)
	}];

    var completeFeedPost = function()
    {
	Hangout.util.showUnityContainer();
	$("#unity_background").hide();
	callback("");
    };
/*
    alert("FB.Connect.streamPublish(" + defaultUserMessage + "\n" +
	  JSON.stringify(attachment) + "\n" +
	  JSON.stringify(action_links) + "\n" +
	  targetId + "\n" +
	  userMessagePrompt + "\n" +
	  "false\n\"\"\n);");
*/	  

    FB.ensureInit(function()
    {
	FB.Connect.streamPublish(defaultUserMessage,
				 attachment,
				 action_links,
				 targetId,
				 userMessagePrompt,
				 completeFeedPost,
				 false,
				 "");
    });
});

// Open the cash store so we can buy!
Hangout.unityDispatcher.exposeFunctionToUnity("requestCashStore", false, function (callback)
{
    Hangout.util.hideUnityContainer();
    $("#cash_store_frame").attr("src", "fake_billing.html");
    $("#cash_store_container").show();
    FB.CanvasClient.setCanvasHeight("5000px");
    Hangout.cashStoreFinished = function ()
    {
	delete Hangout.cashStoreFinished;
	$("#cash_store_container").hide();
	Hangout.util.showUnityContainer();
	callback("");
	FB.CanvasClient.setCanvasHeight(Hangout.config.fb_canvas_height);
    };

});

// Open a URL in a new tab, leaving FB and Unity intact.
Hangout.unityDispatcher.exposeFunctionToUnity("openNewBrowserTab", true, function (url)
{
    window.open(url);
    return "";
});

Hangout.unityDispatcher.exposeFunctionToUnity("logMetricsEvent",
					      false,
					      function (callback,
							eventName,
							extraProperties)
{
    Hangout.metrics.track(eventName, JSON.parse(extraProperties), callback);
});

Hangout.unityDispatcher.exposeFunctionToUnity("logMetricsFunnelStep",
					      false,
					      function (callback,
							funnelId,
							stepNumber,
							stepName,
							extraProperties)
{
    Hangout.metrics.track_custom_funnel(funnelId, stepNumber, stepName, JSON.parse(extraProperties), callback);
});

Hangout.unityDispatcher.exposeFunctionToUnity("logToConsole",
					      true,
					      function (level,
							message)
{
    Hangout.log.log(level, message);
    return "";
});
/*
Hangout.unityDispatcher.exposeFunctionToUnity("toggleConsole",
					      true,
					      function ()
{
    $("#hangout_log_container").toggle();
    return "";
});
*/
$(document).ready(function()
{
    Hangout.config.campaign_id = $.cookie('hangout_campaign');
    Hangout.config.selected_avatar = $.cookie('hangout_selected_avatar');
    Hangout.config.fb_session_id = $.query.get('fb_sig_session_key');
    Hangout.config.fb_account_id = $.query.get('fb_sig_user');

    Hangout.metrics.track_funnel(4);
    Hangout.metrics.track_funnel(5);
    Hangout.metrics.track_funnel(6);

    FB_RequireFeatures(["XFBML", "CanvasUtil"], function()
    {
        FB.init(Hangout.config.fb_api_key, "xd_receiver.htm");
        FB.Connect.requireSession();
	FB.CanvasClient.setCanvasHeight(Hangout.config.fb_canvas_height);
    }); 

    if (Hangout.config.instance_name === "DEV")
    {
	Hangout.log.options.debug = "on";
	Hangout.log.options.info = "on";
	Hangout.log.options.warning = "on";
	Hangout.log.options.error = "on";

	$("#hangout_log_container").show();

	$("#invite_prompt").show();
	$("#invite_prompt").find("a").click(function ()
	{
	    Hangout.util.hideUnityContainer();
	    $("#support_footer").hide();
	    $("#friend_invite_frame").attr("src", "friend_invite.aspx");
	    $("#friend_invite_container").show();
	    FB.CanvasClient.setCanvasHeight("5000px");
	    Hangout.closeChildFrame = function ()
	    {
		delete Hangout.closeChildFrame;
		$("#friend_invite_container").hide();
		Hangout.util.showUnityContainer();
		$("#support_footer").show();
		FB.CanvasClient.setCanvasHeight(Hangout.config.fb_canvas_height);
	    };
	});
    }
});
