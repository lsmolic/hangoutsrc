$(document).ready(function(){

mpmetrics.init(metrics_token);
mpmetrics.track_funnel(metrics_funnel_id, 4, 'begin_unity_install', {'userAgent' : navigator.userAgent, 'token' : metrics_token});
//alert("TRACKED 4");

});

// Unity calls this function directly
function reportInstall(arg)
{
    mpmetrics.track('hardware_report', {'graphics_card': arg, 'token' : metrics_token});
    mpmetrics.track_funnel(metrics_funnel_id, 4, 'complete_unity_install', {'campaign': campaign, 'userAgent' : navigator.userAgent});
    //alert("TRACKED 5");
    fbArgs = window.location.search;
    window.location = "fb_install.html" + fbArgs;
}