/*global Hangout, window, document, navigator, mpmetrics, $ */

Hangout.require("Hangout.config");
Hangout.require("Hangout.metrics");
Hangout.require("Hangout.util");


function GetInstallerPath ()
{
    var tDownloadURL = "";
    //var hasXpi = navigator.userAgent.toLowerCase().indexOf( "firefox" ) != -1;
    
    // Use standalone installer
    if (navigator.platform == "MacIntel")
    {
	tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/webplayer-i386.dmg";
    }
    else if (navigator.platform == "MacPPC")
    {
	tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/webplayer-ppc.dmg";
    }
    else if (navigator.platform.toLowerCase().indexOf("win") != -1)
    {
	tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayer.exe";
    }
    return tDownloadURL;
}


// Auto-reload the page
function AutomaticReload ()
{
    navigator.plugins.refresh();
    if (Hangout.util.isUnityInstalled())
    {
	    var beginLogin = function()
	    {
			window.location = "oops.html" + window.location.search;
	    };

	    Hangout.metrics.track_funnel(7);

	    // If the metrics callback doesn't come within three seconds, redirect anyway
	    window.setTimeout(beginLogin, 3000);

    }
    else
    {
	var reloaded = $.query.get("reloaded");
	if (reloaded === "")
	{
	    reloaded = 1;
	}
	else
	{
	    reloaded++;
	}
	window.location = window.location.href.split("?")[0] + $.query.set("reloaded", reloaded).toString();
    }

}


function startRefreshCheck(a){
	//Hangout.metrics.track_funnel(6);
	if(a)
	{
		window.setTimeout(AutomaticReload, 5);
	}
	else
	{
		window.setTimeout(AutomaticReload, 5000);
	}


    // if ($.query.get("reloaded") === "")
    //    {
    // 	window.setTimeout(AutomaticReload, 20000);
    //    }
    //    else
    //    {
    // 	window.setTimeout(AutomaticReload, 5000);
    //    }
}


function isMac()
{
	if (navigator.platform == "MacIntel")
    {
		return true;
    }
    else if (navigator.platform == "MacPPC")
    {
		return true;
    }
    else if (navigator.platform.toLowerCase().indexOf("win") != -1)
    {
		return false;
    }
}

function redirectToInstructions(){	
	if(isMac()){
		window.location.replace("download_os_x_safari.html" +  window.location.search);
	}else{
		window.location.replace("download_win_generic.html" +  window.location.search);			
	}
}

$(document).ready(function()
{
    Hangout.config.campaign_id = $.cookie('hangout_campaign');
    Hangout.metrics.track_funnel(4);
	//alert(Hangout.util.browserDetect.browser);
	if($.query.get("reloaded")!="")
	{
		startRefreshCheck();
	}
	
	$("#install_unity").click(function()
	{
		
		Hangout.metrics.track_funnel(5);
		var install_path = GetInstallerPath();
		
		if(install_path == ""){
			install_path = "http://www.unity3d.com/unity-web-player-2.x";
		}
		
		if(isMac())
		{
			document.open();
			document.write('<iframe src ="'+ install_path+'" width="100" height="100" style="display:none;"></iframe>');
			document.close();
			if(Hangout.util.browserDetect.browser == "Firefox"){
				redirectToInstructions();
			}
			else{
				window.setTimeout("redirectToInstructions()", 500);			
			}	
		}
		else{
			var win = window.open(install_path,'unity_download_window');
		
			window.setTimeout("win.close()", 1000);			
			
			window.setTimeout("redirectToInstructions()", 1500);			
		}
		
		
		// Hangout.metrics.track_funnel(6);
		// 		
		// 		if(isMac())
		// 		{
		// 			document.open();
		// 			document.write('<iframe src ="'+ install_path+'" width="100" height="100" style="display:none;"></iframe>');
		// 			document.close();
		// 			if(Hangout.util.browserDetect.browser == "Firefox"){
		// 				window.location = window.location.href.split("?")[0] + $.query.set("reloaded", 1).toString();
		// 			}
		// 			else{
		// 				window.setTimeout("startRefreshCheck(true)", 500);			
		// 			}	
		// 		}
		// 		else{
		// 			var win = window.open(install_path,'unity_download_window');
		// 		
		// 			window.setTimeout("win.close()", 1000);			
		// 			
		// 			window.setTimeout("startRefreshCheck()", 1500);			
		// 		}
		// 
	    // Report
	    return false;
	});

	$("#getstartedtext").click(function()
	{
		var install_path = GetInstallerPath();
		Hangout.metrics.track_funnel(5);

		if(isMac())
		{
			document.open();
			document.write('<iframe src ="'+ install_path+'" width="100" height="100" style="display:none;"></iframe>');
			document.close();
			//$('InstallerFrame').attr("src",install_path);
			if(Hangout.util.browserDetect.browser == "Firefox"){
				window.location = window.location.href.split("?")[0] + $.query.set("reloaded", 1).toString();
			}
			else{
				window.setTimeout("startRefreshCheck(true)", 200);			
			}	
		}
		else{
			var win = window.open(install_path,'unity_download_window');

			window.setTimeout("win.close()", 1000);			
	
			window.setTimeout("startRefreshCheck()", 1500);			
		}
		return false;
	});
	
	$("#getstartedbg").click(function()
	{
		var install_path = GetInstallerPath();
		Hangout.metrics.track_funnel(5);

		if(isMac())
		{
			document.open();
			document.write('<iframe src ="'+ install_path+'" width="100" height="100" style="display:none;"></iframe>');
			document.close();
			//$('InstallerFrame').attr("src",install_path);
			if(Hangout.util.browserDetect.browser == "Firefox"){
				window.location = window.location.href.split("?")[0] + $.query.set("reloaded", 1).toString();
			}
			else{
				window.setTimeout("startRefreshCheck(true)", 200);			
			}	
		}
		else{
			var win = window.open(install_path,'unity_download_window');

			window.setTimeout("win.close()", 1000);			
	
			window.setTimeout("startRefreshCheck()", 1500);			
		}
		return false;
	});

});
