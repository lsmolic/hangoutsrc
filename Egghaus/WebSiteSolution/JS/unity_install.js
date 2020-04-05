/*global Hangout, window, document, navigator, mpmetrics, $ */

Hangout.require("Hangout.config");
Hangout.require("Hangout.metrics");
Hangout.require("Hangout.util");

// Where should we go to fetch the Unity installer file?
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
	window.location = "oops.html" + window.location.search;
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

$("#install_unity").click(function()
{
	Hangout.metrics.track_funnel(5);
    var install_path = GetInstallerPath();
	if(install_path != ""){
		window.location = install_path;
	}
	else{
		window.open("http://www.unity3d.com/unity-web-player-2.x");
		
	}
    // Report
    return false;
});
$("#getstartedtext").click(function()
{
	Hangout.metrics.track_funnel(5);
    var install_path = GetInstallerPath();
	if(install_path != ""){
		window.location = install_path;
	}
	else{
		window.open("http://www.unity3d.com/unity-web-player-2.x");
		
	}
    // Report
    return false;
});


$(document).ready(function()
{
    Hangout.config.campaign_id = $.cookie('hangout_campaign');
    Hangout.metrics.track_funnel(4);
    if ($.query.get("reloaded") === "")
    {
	window.setTimeout(AutomaticReload, 20000);
    }
    else
    {
	window.setTimeout(AutomaticReload, 5000);
    }
});
