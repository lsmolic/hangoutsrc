//Requires unity_file , height , width, div (to replace)
//Finds the location of the unity object on the page
function GetUnity () {
	if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1)
		return document.getElementById("UnityObject");
	else if (navigator.appVersion.toLowerCase().indexOf("safari") != -1)
		return document.getElementById("UnityObject");
	else
		return document.getElementById("UnityEmbed");
}

//Has the user installed the hangout plugin?
function DetectUnityWebPlayer () {
	var tInstalled = false;
	if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1) {
		tInstalled = DetectUnityWebPlayerActiveX();
	}
	else {
		if (navigator.mimeTypes && navigator.mimeTypes["application/vnd.unity"]) {
			if (navigator.mimeTypes["application/vnd.unity"].enabledPlugin && navigator.plugins && navigator.plugins["Unity Player"]) {
				tInstalled = true;	
			}
		}	
	}
	return tInstalled;	
}

var sMacIntelInstallerFile = "";
var sWinInstallerFile = "";
var sPluginDirectoryLocation = "";

//determine the installer path, based on which OS user has and which site iteration they are currently using
function GetInstallerPath () {
	var tDownloadURL = "";
	var hasXpi = navigator.userAgent.toLowerCase().indexOf( "firefox" ) != -1;
	
	// Use standalone installer
	if (navigator.platform == "MacIntel"){
		tDownloadURL = sPluginDirectoryLocation + sMacIntelInstallerFile;
	}else if (navigator.platform == "MacPPC"){
		tDownloadURL = "Platform not supported.";
	}else if (navigator.platform.toLowerCase().indexOf("win") != -1){
		tDownloadURL = sPluginDirectoryLocation + sWinInstallerFile;
	}
	return tDownloadURL;
			
}

function GotoInstallerPath() {
    var tDownloadURL = GetInstallerPath();
    if (tDownloadURL == "Platform not supported.") {
    
        // use the MacIntel
        tDownloadURL = sPluginDirectoryLocation + sMacIntelInstallerFile;
        var continuedl = confirm("We've detected that you we do not support your machine at this time but, you can click OK below and try anyways. Clicking Cancel will exit the download.");
        
        if (continuedl == true) {
            window.location = tDownloadURL;
        }
                    
    } else {
        window.location = tDownloadURL;
    }
}

//if user doesn't have the plugin, refresh to see if they installed it yet
function AutomaticReload() {
	navigator.plugins.refresh();
	if (DetectUnityWebPlayer()){
		window.location.reload();
    }
	setTimeout('AutomaticReload()', 500);
}

//Check if user has accepted terms and conditions
function termsCheck(){
	if(document.getElementById("terms").checked == true){
		document.location = GetInstallerPath();	
	}else{
		alert("Hey, you need to agree to the Terms and Conditions");	
	}	
}

// replaces innerHTML of div with the Plugin Object
function Embed_Unity(unity_file,height,width,divName){
	if(!width){
	    width="100%";
	}
	if(!height){
	    height="100%";
	}
	if(unity_file){
		if (DetectUnityWebPlayer()) {
		    var hangoutPlayer = "";
			//hangoutPlayer += '<div style="width:' + width + 'px;height:' + height + 'px">';
			hangoutPlayer += '<object id="UnityObject" classid="clsid:444785F1-DE89-4295-863A-D46C3A781394" width="'+width+'" height="'+height+'" codebase="http://webplayer.unity3d.com/download_webplayer_beta/UnityWebPlayer.cab#version=2,0,0,0"> \n';
			hangoutPlayer += '  <param name="src" value="'+unity_file+'" /> \n';
			hangoutPlayer += '  <param name="AutoUpdateUrl" value="'+sPluginDirectoryLocation+'UnityAutoUpdate" /> \n';
			hangoutPlayer += '  <param name="logoimage" value="/images/hangout_logo.png" /> \n';					
			hangoutPlayer += '  <param name="progressbarimage" value="/images/bar.png" /> \n';	
			hangoutPlayer += '  <param name="disableContextMenu" value="true" /> \n';					
			hangoutPlayer += '  <embed AutoUpdateUrl="'+sPluginDirectoryLocation+'UnityAutoUpdate" id="UnityEmbed" src="'+unity_file+'" width="'+width+'" height="'+height+'" disableContextMenu="true" type="application/vnd.unity" logoimage="/images/hangout_logo.png" progressbarimage="/images/bar.png" pluginspage="" /> \n';
			hangoutPlayer += '</object>';
			//hangoutPlayer += '</div>';
			
			document.getElementById(divName).innerHTML = hangoutPlayer; 
			return true;
		}
		else {
			AutomaticReload();
			return false;
		}	
	}
}
