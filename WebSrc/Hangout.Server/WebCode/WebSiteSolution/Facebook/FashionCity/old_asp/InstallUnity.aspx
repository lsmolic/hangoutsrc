<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="InstallUnity.aspx.cs" Inherits="InstallUnity" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" type="text/css" href="/CSS/Facebook/avselect.css" />
	<script type="text/javascript" src="http://api.mixpanel.com/site_media/js/api/mpmetrics.js"></script> 
	<script type="text/javascript" src="/JS/unity_install.js"></script>        
    <script type="text/javascript" src="/JS/LoadPlugin.js"></script>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <div id="mf">
            <img id="bg" src="/Images/Facebook/FFrenzy/LandingPagePolariodFrame.jpg">

	  <script language="javascript1.1" type="text/javaScript">
				
				if (DetectUnityWebPlayer()) {
					
					document.write('<object id="UnityObject" classid="clsid:444785F1-DE89-4295-863A-D46C3A781394" width="300" height="300"> \n');
					document.write('  <param name="src" value="/TempUnityLocation/install_reporter.unity3d" /> \n');
					document.write('  <embed id="UnityEmbed" src="/TempUnityLocation/install_reporter.unity3d" width="300" height="300" type="application/vnd.unity" pluginspage="http://www.unity3d.com/unity-web-player-2.x" /> \n');
					document.write('</object>');
				}
				else {
				
					var installerPath = GetInstallerPath();
					if (installerPath != "") {
						// Place a link to the right installer depending on the platform we are on. The iframe is very important! Our goals are:
						// 1. Don't have to popup new page
						// 2. This page still remains active, so our automatic reload script will refresh the page when the plugin is installed
						//document.write('<div align="center" id="UnityPrompt"> \n');
						//document.write('  <a href= ' + installerPath + '><img src="http://webplayer.unity3d.com/installation/getunity.png" border="0"/></a> \n');
						document.write('  <a href= ' + installerPath + '><img id="bgphoto" src="/Images/Facebook/FFrenzy/UnityDownloadPage.jpg" border="0"/></a> \n');
						//document.write('</div> \n');
                        document.write('  <a id="getstartedtext" href= ' + installerPath + '>Get Started!</a><\n');
						
						document.write('<iframe name="InstallerFrame" height="0" width="0" frameborder="0">\n');
					}
					else {
						//document.write('<div align="center" id="UnityPrompt"> \n');
						//document.write('  <a href="javascript: window.open("http://www.unity3d.com/unity-web-player-2.x"); "><img src="http://webplayer.unity3d.com/installation/getunity.png" border="0"/></a> \n');
						document.write('  <a href="javascript: window.open("http://www.unity3d.com/unity-web-player-2.x"); "><img id="bgphoto" src="/Images/Facebook/FFrenzy/UnityDownloadPage.jpg" border="0"/></a> \n');
						//document.write('</div> \n');
					}
					
					AutomaticReload();
				}
			
			</script>
			<noscript>
                          This content requires a browser with Javascript enabled.
			</noscript>
          </div>
</asp:Content>

