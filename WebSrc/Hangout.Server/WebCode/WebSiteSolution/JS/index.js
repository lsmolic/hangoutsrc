/*global Hangout, window, document, $ */

Hangout.require("Hangout.config");
Hangout.require("Hangout.util");

$(document).ready(function(){

    // Forward to the proper starting page, based on FB app install status and
    // Unity client install status.  Also pass along the GET query args we received.

    var fbArgs = window.location.search;

    if (window.top === window)
    {
	// We're not in an iframe--someone must have directly accessed via not-facebook
	// Redirect to apps.facebook URL
	window.location = Hangout.config.facebook_base_url;
    }
    else if (Hangout.util.isFacebookAppInstalled())
    {
	if(Hangout.util.isUnityInstalled())
	{
	    window.location = "minigame_container.html" + fbArgs;
	}
	else
	{
	    window.location = "unity_install.html" + fbArgs;
	}
    }
    else
    {
	window.location = "avatar_select.html" + fbArgs;
    }
});
