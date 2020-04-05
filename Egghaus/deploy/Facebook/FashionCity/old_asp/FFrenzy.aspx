<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FFrenzy.aspx.cs" Inherits="FFrenzy" Title="Fashion Frenzy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="Stylesheet" type="text/css" href="/CSS/Facebook/minigame_container.css" />
    <script type="text/javascript" src="/JS/unity_install.js"></script>  
    <script type="text/javascript" src="/JS/minigame_container.js"></script>        
    <script type="text/javascript" src="/JS/LoadPlugin.js"></script>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <div id="FB_HiddenContainer"  style="position:absolute; top:-10000px; width:0px; height:0px;" ></div>
    <script src="http://static.ak.connect.facebook.com/js/api_lib/v0.4/FeatureLoader.js.php" type="text/javascript"></script>
<!--    <script type="text/javascript">FB_RequireFeatures(["XFBML"], function(){ FB.Facebook.init('<%=ConfigurationSettings.AppSettings["d8001936716010b89e6d985dd7bd757a"] %>', "/iansandbox/xd_receiver.htm"); });</script>  -->
    <div id="unity_container">
        <script language="javascript1.1" type="text/javaScript">	
            if (DetectUnityWebPlayer())
            {
                document.write('<object id="UnityObject" classid="clsid:444785F1-DE89-4295-863A-D46C3A781394" width="100%" height="100%"> \n');
                document.write('  <param name="src" value="<%= UnityFileLocation %>" /> \n');
                document.write('  <embed id="UnityEmbed" src="<%= UnityFileLocation %>" width="100%" height="100%" type="application/vnd.unity" pluginspage="http://www.unity3d.com/unity-web-player-2.x" /> \n');
                document.write('</object>');
            }
            else
            {
                  alert("OMG NO UNITY PANIC");
            }
        </script>
        <noscript>
            This content requires a browser with Javascript enabled.
        </noscript>
    </div>
    <div id="cash_store_container">
        <iframe id="cash_store_frame" src="" width="100%" height="100%" frameborder="0" scrolling="auto"></iframe>
    </div>
</asp:Content>

