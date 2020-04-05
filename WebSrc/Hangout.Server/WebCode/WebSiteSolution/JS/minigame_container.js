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

// Request from Unity client to post a feed update
Hangout.unityDispatcher.exposeFunctionToUnity("requestFeedPost",
					      false,
					      function (callback,
							targetId,
							feedType,
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


    // Called back when popup dialog closes
    var completedFeedPost = function(post_id, exception)
    {
        Hangout.metrics.track("feed_popup_result",
			      {
				  "feed_type" : feedType, 
				  "posted" : (post_id !== null && post_id !== "null"),
				  "post_id" : post_id,
				  "exception" : exception
			      },
			      function(){});
	Hangout.util.showUnityContainer();
	$("#unity_background").hide();
	callback("");
    };

    Hangout.metrics.track("feed_popup_opened", { "feed_type" : feedType }, function(){});

    FB.ensureInit(function()
    {
	FB.Connect.streamPublish(defaultUserMessage,
				 attachment,
				 action_links,
				 targetId,
				 userMessagePrompt,
				 completedFeedPost,
				 false,
				 "");
    });
});

// Open the cash store so we can buy!
Hangout.unityDispatcher.exposeFunctionToUnity("requestCashStore", false, function (callback, urlParams)
{
    Hangout.log.debug("rcs: " + urlParams);

    Hangout.util.hideUnityContainer();
    $("#button_bottom_row").hide();    
    $("#button_top_row").hide();
    $("#cash_store_frame").attr("src", Hangout.util.getInstanceURL(Hangout.config.secure_host_base_url,"PurchaseGw.aspx?"+urlParams));
    $("#cash_store_container").show();
    FB.CanvasClient.setCanvasHeight("5000px");

    // Click method for "back" button that should close the store and return us to Unity
    $("#cash_store_container").find("a").click(function ()
    {
	delete Hangout.cashStoreFinished;
	$("#cash_store_container").hide();
	Hangout.util.showUnityContainer();
	$("#button_bottom_row").show();
	$("#button_top_row").show();
	FB.CanvasClient.setCanvasHeight(Hangout.config.fb_canvas_height);
	callback("");
    });

});

// Open the friend inviter
// This function is also assigned to the Invite button so store it for reuse
Hangout.unityDispatcher.openFriendInviter = function (callback, invite_type)
{
    Hangout.config.invite_type = invite_type; // iframe grabs this value later
    Hangout.util.hideUnityContainer();
    $("#button_bottom_row").hide();    
    $("#button_top_row").hide();
    $("#friend_invite_frame").attr("src", "friend_invite.aspx?invite_type=" + invite_type + "&" + window.location.search.slice(1));
    $("#friend_invite_container").show();
    FB.CanvasClient.setCanvasHeight("5000px");

    // Called by the child frame to tell us to kill it and wrap up
    Hangout.closeChildFrame = function ()
    {
	delete Hangout.closeChildFrame;
	$("#friend_invite_container").hide();
	Hangout.util.showUnityContainer();
	$("#button_bottom_row").show();
	$("#button_top_row").show();
	FB.CanvasClient.setCanvasHeight(Hangout.config.fb_canvas_height);
	callback("");
    };
};
Hangout.unityDispatcher.exposeFunctionToUnity("requestFriendInviter", false, Hangout.unityDispatcher.openFriendInviter);

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

Hangout.unityDispatcher.exposeFunctionToUnity("toggleJSConsole",
					      true,
					      function ()
{
    $("#hangout_log_container").toggle();
    return "";
});


// --------------------------------------


$(document).ready(function()
{
    Hangout.config.campaign_id = $.cookie('hangout_campaign');
    Hangout.config.inviter_id = $.cookie('hangout_inviter_id');
    Hangout.config.selected_avatar = parseInt($.cookie('hangout_selected_avatar'));
    Hangout.config.fb_session_key = $.query.get('fb_sig_session_key');
    Hangout.config.fb_account_id = $.query.get('fb_sig_user');

    if (Hangout.config.campaign_id === null || Hangout.config.campaign_id === "")
    {
	Hangout.config.campaign_id = "NO_COOKIE_FOUND";
    }

    if (Hangout.config.inviter_id === null || Hangout.config.inviter_id === "")
    {
	Hangout.config.inviter_id = "NO_COOKIE_FOUND";
    }

    Hangout.metrics.track_funnel(4);
    Hangout.metrics.track_funnel(5);

    // Set up "become a fan" button
    $("#fan_frame").attr("src",
			 "http://www.facebook.com/connect/connect.php?id=" +
			 Hangout.config.fb_page_id + 
			 "&connections=0&stream=0&css=" +
			 Hangout.util.getInstanceURL(Hangout.config.web_host_base_url,
						     "/CSS/Facebook/fan_frame.css&") +
			 window.location.search.slice(1)
			);

    // Invite a friend button
    $("#invite_prompt").find("a").click(function ()
    {
	Hangout.unityDispatcher.openFriendInviter(function (callback) {}, "invite_web_button");
    });
    $("#invite_prompt").show();

    // Add bookmark button
    $("#bookmark_prompt").find("a").click(function ()
    {
	$("#unity_background").show();
	Hangout.util.hideUnityContainer();
	FB.ensureInit(function()
        {
	    FB.Connect.showBookmarkDialog(function ()
	    {
		Hangout.util.showUnityContainer();
		$("#unity_background").hide();
	    });
	});
    });
    $("#bookmark_prompt").show();

    $("#blog_button").find("a").click(function ()
    {
	window.open($(this).attr("href"));
	return false;
    });
    $("#blog_button").show();
				      
    $("#forums_button").find("a").click(function ()
    {
	window.open($(this).attr("href"));
	return false;
    });
    $("#forums_button").show();


    Hangout.log.options.debug = "on";
    Hangout.log.options.info = "on";
    Hangout.log.options.warning = "on";
    Hangout.log.options.error = "on";

    // Enable JS console iff in DEV environment (configify this later)
    if (Hangout.config.instance_name === "DEV")
    {
	$("#hangout_log_container").show();
    }
});
