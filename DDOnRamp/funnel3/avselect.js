$(document).ready(function(){	
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

	// Attach click events to avatars
	$("#choose1").click(function()
	{
	    redirectToUnityInstall('1');
	    
	    return false;
	});

	$("#choose2").click(function()
	{
	    redirectToUnityInstall('2');
	    
	    return false;
	});

	$("#choose3").click(function()
	{
	    redirectToUnityInstall('3');
	    
	    return false;
	});

	$("#choose4").click(function()
	{
	    redirectToUnityInstall('4');
	    
	    return false;
	});

	$("#choose5").click(function()
	{
	    redirectToUnityInstall('5');
	    
	    return false;
	});

	// report first step being hit
	mpmetrics.track_funnel(metrics_funnel_id, 1, 'start', {'check':'check', 'campaign':campaign, 'userAgent':navigator.userAgent});
});

function redirectToUnityInstall(selectedAvatar) {
	// additional tracking for debugging... not to be confused with avatar_selection
	mpmetrics.track('avatar_selection_click', {'check':'check', 'avatar':selectedAvatar});
    
    var fbArgs = window.location.search;
    if (fbArgs == "") {
    	connector = "?";
    } else {
    	connector = "&";
    }
    window.location = "unity_install.html" + fbArgs + connector + "SelectedAvatar=" + selectedAvatar;
}