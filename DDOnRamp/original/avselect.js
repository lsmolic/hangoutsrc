$(document).ready(function(){

mpmetrics.init(metrics_token);
mpmetrics.track_funnel(metrics_funnel_id, 1, 'start', {'userAgent' : navigator.userAgent, 'token' : metrics_token});
//alert("TRACKED");

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
    // Hide
    $(this).unbind('click');
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").fadeOut(0);
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    
    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '338px', bottom: '152px'}, 200);

    var sh = $("#cs1")
    sh.fadeIn(0);
    sh.animate({ left: '377px', bottom: '146px'}, 200);
    
    // Report
    mpmetrics.track_funnel(metrics_funnel_id, 2, 'select_avatar', {'userAgent' : navigator.userAgent, 'token' : metrics_token});
    mpmetrics.track('avatar_selected', {'avatar' : '1', 'token' : metrics_token});    
});

$("#choose2").click(function()
{
    // Hide
    $(this).unbind('click');
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").fadeOut(0);
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    
    var sh = $("#cs2")
    sh.fadeIn(0);
    sh.animate({ left: '377px', bottom: '146px'}, 200);
    
    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '338px', bottom: '152px'}, 200);

    // Report
    mpmetrics.track_funnel(metrics_funnel_id, 2, 'select_avatar', {'userAgent' : navigator.userAgent, 'token' : metrics_token});
    mpmetrics.track('avatar_selected', {'avatar' : '2', 'token' : metrics_token});
});

$("#choose3").click(function()
{
    // Hide
    $(this).unbind('click');
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").fadeOut(0);
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    
    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '338px', bottom: '152px'}, 200);

    var sh = $("#cs3")
    sh.fadeIn(0);
    sh.animate({ left: '357px', bottom: '137px'}, 200);
    
    // Report
    mpmetrics.track_funnel(metrics_funnel_id, 2, 'select_avatar', {'userAgent' : navigator.userAgent, 'token' : metrics_token});
    mpmetrics.track('avatar_selected', {'avatar' : '3', 'token' : metrics_token});
});

$("#choose4").click(function()
{
    // Hide
    $(this).unbind('click');
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").fadeOut(0);
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    
    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '338px', bottom: '152px'}, 200);

    var sh = $("#cs4")
    sh.fadeIn(0);
    sh.animate({ left: '357px', bottom: '146px'}, 200);
    
    // Report
    mpmetrics.track_funnel(metrics_funnel_id, 2, 'select_avatar', {'userAgent' : navigator.userAgent, 'token' : metrics_token});
    mpmetrics.track('avatar_selected', {'avatar' : '4', 'token' : metrics_token});
});

$("#choose5").click(function()
{
    // Hide
    $(this).unbind('click');
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").fadeOut(0);
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    
    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '358px', bottom: '152px'}, 200);

    var sh = $("#cs5")
    sh.fadeIn(0);
    sh.animate({ left: '367px', bottom: '146px'}, 200);
    
    // Report
    mpmetrics.track_funnel(metrics_funnel_id, 2, 'select_avatar', {'userAgent' : navigator.userAgent, 'token' : metrics_token});
    mpmetrics.track('avatar_selected', {'avatar' : '5', 'token' : metrics_token});
});



});

function redirectToUnityInstall()
{
    window.location="unity_install.html";
}

function getFBLogin()
{
    mpmetrics.track_funnel(metrics_funnel_id, 3, 'open_allow_popup', {'userAgent' : navigator.userAgent, 'token' : metrics_token});

    FB.Connect.requireSession();
    FB.Connect.get_status().waitUntilReady(function(status)
    {
	switch(status)
	{
	    case FB.ConnectState.connected:
                redirectToUnityInstall();
	        break;
	    case FB.ConnectState.appNotAuthorized:
                FB.Connect.requireSession(redirectToUnityInstall);
                break;
            case FB.ConnectState.userNotLoggedIn:
                FB.Connect.requireSession(redirectToUnityInstall);
                break;
	};
    });
    return false;
}
