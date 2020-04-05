/*global Hangout, window, document, navigator, top, mpmetrics, $ */

Hangout.require("Hangout.config");
Hangout.require("Hangout.metrics");

$(document).ready(function(){

var camp = $.query.get('campaign');
var inviter = $.query.get('inviter_id');

if (camp === null || camp === "")
{
    camp = "NONE";
}

if (inviter === null || inviter === "")
{
    inviter = "NONE";
}

Hangout.metrics.track("arrived_from_campaign", { "campaign" : camp, "inviter" : inviter }, function(){})

$.cookie('hangout_campaign', camp, { expires: 2 });
$.cookie('hangout_inviter_id', inviter, { expires: 2 });

if ($.cookie('hangout_campaign') !== camp)
{
    Hangout.config.campaign_id = "COOKIE_DENIED";
    Hangout.config.inviter_id = "COOKIE_DENIED";
}
else
{
    // Cookie
    Hangout.config.campaign_id = camp;
    Hangout.config.inviter_id = inviter;
}

Hangout.metrics.track_funnel(1);

// Redirects the browser to FB allow prompt
Hangout.loginRedirect = function()
{
    var triggered = false;
    var beginLogin = function()
    {
	if (!triggered)
	{
	    triggered = true;
	    if (Hangout.config.login_use_connect)
	    {
		FB.ensureInit(function ()
			      {
				  FB.Connect.requireSession(function ()
							    {
								top.location = Hangout.config.facebook_base_url;
							    },
							    null,
							    true);
			      });
	    }
	    else
	    {
		top.location = "http://www.facebook.com/login.php?canvas&v=1.0&api_key=" + Hangout.config.fb_api_key;
	    }
	}
    };

    Hangout.metrics.track_funnel(3, {}, beginLogin);

    // If the metrics callback doesn't come within three seconds, redirect anyway
    window.setTimeout(beginLogin, 3000);

    return false;
};

$("#submitbutton").find("a").click(Hangout.loginRedirect);
//$("#submitbutton").find("a").attr("href", "http://www.facebook.com/login.php?canvas&v=1.0&api_key=" + Hangout.config.fb_api_key);

$("#choose1").attr("href", "http://www.facebook.com/login.php?canvas&v=1.0&api_key=" + Hangout.config.fb_api_key);
$("#choose2").attr("href", "http://www.facebook.com/login.php?canvas&v=1.0&api_key=" + Hangout.config.fb_api_key);
$("#choose3").attr("href", "http://www.facebook.com/login.php?canvas&v=1.0&api_key=" + Hangout.config.fb_api_key);
$("#choose4").attr("href", "http://www.facebook.com/login.php?canvas&v=1.0&api_key=" + Hangout.config.fb_api_key);
$("#choose5").attr("href", "http://www.facebook.com/login.php?canvas&v=1.0&api_key=" + Hangout.config.fb_api_key);

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
    $.cookie('hangout_selected_avatar', 1, { expires : 2 });

    // Hide
    $(this).unbind('click');
    $(this).click(Hangout.loginRedirect);
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").css("border", "none");
    $(".ahover").css("z-index", "-500");
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    $("#submitbutton").fadeIn("slow");

    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '358px', bottom: '152px'}, 200);

    var sh = $("#cs1");
    sh.fadeIn(0);
    sh.animate({ left: '377px', bottom: '146px'}, 200);
    
    // Report
    Hangout.metrics.track_funnel(2, {'avatar' : '1'});
    return false;
});

$("#choose2").click(function()
{
    $.cookie('hangout_selected_avatar', 2, { expires : 2 });

    // Hide
    $(this).unbind('click');
    $(this).click(Hangout.loginRedirect);
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").css("border", "none");
    $(".ahover").css("z-index", "-500");
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    $("#submitbutton").fadeIn("slow");

    var sh = $("#cs2");
    sh.fadeIn(0);
    sh.animate({ left: '377px', bottom: '146px'}, 200);
    
    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '363px', bottom: '147px'}, 200);

    // Report
    Hangout.metrics.track_funnel(2, {'avatar' : '2'});
    return false;
});

$("#choose3").click(function()
{
    $.cookie('hangout_selected_avatar', 3, { expires : 2 });

    // Hide
    $(this).unbind('click');
    $(this).click(Hangout.loginRedirect);
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").css("border", "none");
    $(".ahover").css("z-index", "-500");
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    $("#submitbutton").fadeIn("slow");
    
    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '323px', bottom: '137px'}, 200);

    var sh = $("#cs3");
    sh.fadeIn(0);
    sh.animate({ left: '357px', bottom: '137px'}, 200);
    
    // Report
    Hangout.metrics.track_funnel(2, {'avatar' : '3'});
    return false;
});

$("#choose4").click(function()
{
    $.cookie('hangout_selected_avatar', 4, { expires : 2 });

    // Hide
    $(this).unbind('click');
    $(this).click(Hangout.loginRedirect);
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").css("border", "none");
    $(".ahover").css("z-index", "-500");
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    $("#submitbutton").fadeIn("slow");
    
    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '338px', bottom: '122px'}, 200);

    var sh = $("#cs4");
    sh.fadeIn(0);
    sh.animate({ left: '357px', bottom: '146px'}, 200);
    
    // Report
    Hangout.metrics.track_funnel(2, {'avatar' : '4'});
    return false;
});

$("#choose5").click(function()
{
    $.cookie('hangout_selected_avatar', 5, { expires : 2 });

    // Hide
    $(this).unbind('click');
    $(this).click(Hangout.loginRedirect);
    $(".av_rollover").fadeOut(0);
    $(".shadow").fadeOut(0);
    $("h1").fadeOut(0);
    $(".ahover").css("border", "none");
    $(".ahover").css("z-index", "-500");
    $("#bgphoto").fadeOut(0);
    
    // Show
    $("#submitframe").fadeIn("slow");
    $("#submitbutton").fadeIn("slow");

    var im = $(this).find('img');
    im.unbind("mouseenter mouseleave");
    im.attr("src", im.attr("src").replace(/_s.gif/, "_a.gif"));
    im.fadeIn(0);
    im.animate({ left: '353px', bottom: '147px'}, 200);

    var sh = $("#cs5");
    sh.fadeIn(0);
    sh.animate({ left: '367px', bottom: '146px'}, 200);
    
    // Report
    Hangout.metrics.track_funnel(2, {'avatar' : '5'});
    return false;
});

});
