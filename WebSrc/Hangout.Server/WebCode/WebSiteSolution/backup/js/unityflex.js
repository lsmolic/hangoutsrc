// get our flash movie object      
function MessageToUnity(what){
	var unity=GetUnity();
	if(!unity){
	    PutUnityOnPage();
	} else {
		try {
			unity.SendMessage("Communication", "InboundMessage", what);
		} catch(Error){
		
		}
	}
}
function ControlMessage(what){
	try {
		var flashMovie = document.getElementById("flexchat");
		flashMovie.ControlMessage(what);
	} catch(Error){
	
	}
}
function BroadcastMessage(what){
	try {
		var flashMovie = document.getElementById("flexchat");
		//debugLog("<span>Sending:"+ what +"<br />flashMovie: "+ flashMovie +"</span>");
		flashMovie.BroadcastMessage(what);
	} catch(Error){
	    //debugLog("<span style='color:#FF0000'>Error sending:"+ what +"<br />flashMovie: "+ flashMovie +"</span>");
	    //alert("error broadcasting message: " + Error );
	}
}		
//this function is called by he flex app to show the mic config.
function ShowMicConfig(){
	document.location='/Settings.aspx?roomId=' + getURLParam('roomId');
}
function FlexConnectError(why){

}
function FlexDisconnect(why){
	var answer = confirm("You were disconnected from the Hangout server. \n Click OK to reconnect. \n Click Cancel to give up. ");
	if (answer){
		document.location=document.location;
	}
	else{
		document.location="/Contact.aspx"
	}
}
function getURLParam(strParamName){
  var strReturn = "";
  var strHref = window.location.href;
  if ( strHref.indexOf("?") > -1 ){
    var strQueryString = strHref.substr(strHref.indexOf("?")).toLowerCase();
    var aQueryString = strQueryString.split("&");
    for ( var iParam = 0; iParam < aQueryString.length; iParam++ ){
      if (aQueryString[iParam].indexOf(strParamName.toLowerCase() + "=") > -1 ){
        var aParam = aQueryString[iParam].split("=");
        strReturn = aParam[1];
        break;
      }
    }
  }
  return unescape(strReturn);
}