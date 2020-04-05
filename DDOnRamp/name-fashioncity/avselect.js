function AppIsAdded ()
{
    return window.location.search.indexOf("fb_sig_added=1") != -1;
}

$(document).ready(function(){

mpmetrics.init(metrics_token);
mpmetrics.track_funnel(metrics_funnel_id, 1, 'start', {'userAgent' : navigator.userAgent});

// Wait until after the DOM is in place to start preloading animations.  This helps behavior in IE.
$(".preload").each(function()
{
    $(this).attr("src", $(this).attr("src").replace(/_s.gif/, "_a.gif"));
});

// Animate avatars on mouseover
$(".av_rollover").hover
(
    function ()
    {
	$(this).attr("src", $(this).attr("src").replace(/_s.gif/, "_a.gif"));	
    },
    function ()
    {
	$(this).attr("src", $(this).attr("src").replace(/_a.gif/, "_s.gif"));
    }
);

// Hover frame around avatar on mouseover
$(".av_rollover").ahover({toggleSpeed: 500, moveSpeed: 250});

$("#choose1").click(function()
{
    selectedAvatar = 1;
        
    // Report
    mpmetrics.track_funnel(metrics_funnel_id, 2, 'avatar_selected', {'campaign': campaign, 'userAgent' : navigator.userAgent});
    mpmetrics.track('avatar_selected', {'avatar' : '1'});

	// Redirect
    redirectToUnityInstall();
    
    return false;
});

$("#choose2").click(function()
{
    selectedAvatar = 2;
    
    // Report
	mpmetrics.track_funnel(metrics_funnel_id, 2, 'avatar_selected', {'campaign': campaign, 'userAgent' : navigator.userAgent});
    mpmetrics.track('avatar_selected', {'avatar' : '2'});
    
	// Redirect
    redirectToUnityInstall();
    
    return false;
});

$("#choose3").click(function()
{
    selectedAvatar = 3;
       
    // Report
 	mpmetrics.track_funnel(metrics_funnel_id, 2, 'avatar_selected', {'campaign': campaign, 'userAgent' : navigator.userAgent});
    mpmetrics.track('avatar_selected', {'avatar' : '3'});
    
	// Redirect
    redirectToUnityInstall();
    
    return false;
});

$("#choose4").click(function()
{
    selectedAvatar = 4;
        
    // Report
	mpmetrics.track_funnel(metrics_funnel_id, 2, 'avatar_selected', {'campaign': campaign, 'userAgent' : navigator.userAgent});
    mpmetrics.track('avatar_selected', {'avatar' : '4'});
    
	// Redirect
    redirectToUnityInstall();
    
    return false;
});

$("#choose5").click(function()
{
    selectedAvatar = 5;
        
    // Report
	mpmetrics.track_funnel(metrics_funnel_id, 2, 'avatar_selected', {'campaign': campaign, 'userAgent' : navigator.userAgent});
    mpmetrics.track('avatar_selected', {'avatar' : '5'});
    
	// Redirect
    redirectToUnityInstall();
    
    return false;
});

});


function redirectToUnityInstall() {
    fbArgs = window.location.search;
    if (fbArgs == "") {
    	connector = "?";
    } else {
    	connector = "&";
    }
    window.location = nextStep + fbArgs + connector + "SelectedAvatar=" + selectedAvatar;
}