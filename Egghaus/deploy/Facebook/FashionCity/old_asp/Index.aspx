<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Index.aspx.cs" Inherits="Facebook_FFrenzy_Index" Title="Pick an Avatar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="Stylesheet" type="text/css" href="/CSS/Facebook/minigame_container.css" />
    <script type="text/javascript" src="/JS/jquery.dimensions.js"></script>
    <script type="text/javascript" src="/JS/jquery.ahover.js"></script>
    <link rel="Stylesheet" type="text/css" href="/CSS/Facebook/avselect.css" />
    <script type="text/javascript" src="http://api.mixpanel.com/site_media/js/api/mpmetrics.js"></script>
    <script type="text/javascript" src="/JS/avselect.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
    <div id="FB_HiddenContainer" style="position: absolute; top: -10000px; width: 0px;
        height: 0px;">
    </div>

    <script src="http://static.ak.facebook.com/js/api_lib/v0.4/FeatureLoader.js.php"
        type="text/javascript"></script>

    <script type="text/javascript">FB_RequireFeatures(["XFBML"], function(){ FB.Facebook.init('<%=WebConfig.FacebookAPIKey %>', "/Facebook/FFrenzy/xd_receiver.htm"); });</script>

    <div id="mf">
        <h1>
            Pick Your Look!</h1>
        <img id="bg" src="/Images/Facebook/FFrenzy/LandingPagePolariodFrame.jpg">
        <img id="bgphoto" src="/Images/Facebook/FFrenzy/LandingPageImageEnvironment.jpg" alt="bgphoto" />
        <a id="choose1" href="#">
            <img id="c1" class="av_rollover" src="/Images/Facebook/FFrenzy/av001_s.gif" alt="1" />
        </a><a id="choose2" href="#">
            <img id="c2" class="av_rollover" src="/Images/Facebook/FFrenzy/av002_s.gif" alt="2" />
        </a><a id="choose3" href="#">
            <img id="c3" class="av_rollover" src="/Images/Facebook/FFrenzy/av003_s.gif" alt="3" />
        </a><a id="choose4" href="#">
            <img id="c4" class="av_rollover" src="/Images/Facebook/FFrenzy/av004_s.gif" alt="4" />
        </a><a id="choose5" href="#">
            <img id="c5" class="av_rollover" src="/Images/Facebook/FFrenzy/av005_s.gif" alt="5" />
        </a>
        <img class="shadow" id="cs1" src="/Images/Facebook/FFrenzy/shadow.png" />
        <img class="shadow" id="cs2" src="/Images/Facebook/FFrenzy/shadow.png" />
        <img class="shadow" id="cs3" src="/Images/Facebook/FFrenzy/shadow.png" />
        <img class="shadow" id="cs4" src="/Images/Facebook/FFrenzy/shadow.png" />
        <img class="shadow" id="cs5" src="/Images/Facebook/FFrenzy/shadow.png" />
        <div id="submitframe">
            <div id="submitbutton">
                <a href="#" onclick="getFBLogin(); return false;">
                    <img id="fb_login_image" src="http://static.ak.fbcdn.net/images/fbconnect/login-buttons/connect_light_small_short.gif"
                        alt="Connect" />
                    Save Your Look</a>
            </div>
        </div>
    </div>
</asp:Content>
