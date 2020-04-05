/*global Hangout, window, document, $ */

Hangout.require("Hangout.config");
Hangout.require("Hangout.metrics");
Hangout.require("Hangout.util");

$(document).ready(function(){

    // Forward to the proper starting page, based on FB app install status and
    // Unity client install status.  Also pass along the GET query args we received.



    var fbArgs = window.location.search;
    Hangout.metrics.MP.register({"referrer": fbArgs});
	//Hangout.metrics.MP.register()
	
	
	//alert(fbArgs);
    if (window.top === window)
    {
	// We're not in an iframe--someone must have directly accessed via not-facebook
	// Redirect to apps.facebook URL
	window.location = Hangout.config.facebook_base_url;
    }
    else if (Hangout.util.isFacebookAppInstalled())
    {
		// if(Hangout.util.isUnityInstalled())
		// {
		//     window.location = "oops.html" + fbArgs;
		// }
		// else
		// {
		//     window.location = "download_cta.html" + fbArgs;
		// }
    	if(Hangout.util.isUnityInstalled())
		{
		    window.location = "oops.html" + fbArgs;
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
