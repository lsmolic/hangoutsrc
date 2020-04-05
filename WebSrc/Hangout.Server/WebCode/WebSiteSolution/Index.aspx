<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:fb="http://www.facebook.com/2008/fbml">
<script>
    //Variables defined in .net because < % this doesn't work in the <head>
    var GoogleAnalyticsToken = "UA-4968776-4";
    var GoogleAnalyticsDomainName = ".hangout.net";
</script>
<head id="ctl00_Head1"><title>
	Hangout.net ~ Home
</title><link href="/backup/App_Themes/hangout_main.css?jsVersionId=&lt;% = HangoutBasePage.JSVersionId %>" rel="stylesheet" type="text/css" />
	<script src="/backup/js/sniffer.js?jsVersionId=2" type="text/javascript"></script>
	<script language="javascript" type="text/javascript">
	    if (is_fx)
	    {
			document.write('<link href="/backup/App_Themes/hangout_mozilla.css?jsVersionId=2" rel="stylesheet" type="text/css" />');
	    } 
	</script>

	<script type="text/javascript">
	    //this is an awesome debugger for messages being sent to flex 
	    //or any other javascript based problem (thanks GONZO!!!)
        var newWin;
        function debugLog(message){
            if(!newWin || !newWin.document){
                newWin = window.open("","test","width=300,height=300,scrollbars=1,resizable=1");
            }
            newWin.document.write(message);
            newWin.document.write("<hr>");
            //newWin.focus();
        }
        
        //detect browser resolution
        var browserWidth;
        var browserHeight;
        function SetBrowserDimensions() {
            if( typeof( window.innerWidth ) == 'number' ) {
                //Non-IE
                browserWidth = window.innerWidth;
                browserHeight = window.innerHeight;
            } else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {
                //IE 6+ in 'standards compliant mode'
                browserWidth = document.documentElement.clientWidth;
                browserHeight = document.documentElement.clientHeight;
            } else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {
                //IE 4 compatible
                browserWidth = document.body.clientWidth;
                browserHeight = document.body.clientHeight;
            }
            //debugLog("unadultered width/height: " + browserWidth + "/" + browserHeight);
            
            browserHeight = browserHeight - 118;
            //if(browserHeight > 650){
            //    browserHeight = 650;
            //}
            if(browserHeight < 300){
                browserHeight = 300;
            }
            
            browserWidth = browserWidth;
            if(browserWidth < 770){
                browserWidth = 770;
            }
        }
        
        function ResizeHangoutRoomDiv(){
            if (document.getElementById("hasplugin") == null)
            {
                SetBrowserDimensions();
                try{
                    document.getElementById("hangoutroom").style.width = browserWidth+"px";
                    document.getElementById("hangoutroom").style.height = browserHeight+"px"; 
                }
                catch(Error){
                //debugLog("error setting div / object height and width: " + Error);
                }
            }
        }
    </script>

    <script src="http://static.ak.connect.facebook.com/js/api_lib/v0.4/FeatureLoader.js.php" type="text/javascript"></script>
	
	<script src="/backup/js/swfobject.js?v=2.1&jsVersionId=2" type="text/javascript"></script>

	<script src="/backup/js/load_hangout_plugin.js?jsVersionId=2" type="text/javascript"></script>

	<script src="/backup/js/unityflex.js?jsVersionId=2" type="text/javascript"></script>

	<script src="/backup/js/unityDetectActiveX.vbs?jsVersionId=2" type="text/vbscript"></script>

	
	<script src='http://www.google-analytics.com/ga.js' type='text/javascript'></script>
	<script type="text/javascript">
	
        var pageTracker = _gat._getTracker(GoogleAnalyticsToken);
        pageTracker._setDomainName(GoogleAnalyticsDomainName);
	    
	    function LogGoogleAnalyticsEvent( eventCategory, eventName ){
	        //alert(eventCategory + ", " + eventName);
            if( pageTracker != null ){ 
    	        pageTracker._trackPageview(eventCategory + "/" + eventName);
            }
        }
	</script>
	
</head>
<body onload="ResizeHangoutRoomDiv();" onresize="ResizeHangoutRoomDiv();">
	<div class="pageBody">
		<form name="aspnetForm" method="post" action="index.aspx" id="aspnetForm" style="margin:0px 0px 0px 0px;" AUTOCOMPLETE="off">
<div>
<input type="hidden" name="__VIEWSTATE" id="__VIEWSTATE" value="/wEPDwULLTEwMDUyNjYzMjhkZIQnXvsAAeHh2ZfDDaidpNMIGxes" />
</div>

			<div id="topBanner">
        	    <img alt="Banner" height="51px" width="770px" src="/backup/images/banners/banner_top_beta.png?jsVersionId=2" />
		    </div>
			<div class="topMenu">
				<div class="topMenuItems">
					
					<div class="topMenuItemSelected">
						<a href="Index.aspx">
							Home
						</a>

					</div>
					

					
					<div class="topMenuItem">
						<a href="http://blog.hangout.net">
							Blog
						</a>
					</div>
					
				</div>
				
			</div>
			<div class="constrainPage" id="constrainPage">

			    <div class="content">
				    
    <div class="page_content_div">
        <div style="width:100%; clear:both; margin-top:20px;">
            <img src="/backup/images/WebPageFront.jpg" width="750px" height="400px" />
        </div>
        <div style="width:100%;">
           <a style="float:right;margin-right:65px;" href="http://www.techcrunch50.com/2008/conference/presenter.php?presenter=47">
            <img style="border: none; margin-top: 10px;" src="/backup/images/home/techcrunch50_finalist.png" alt="TechCrunch 50 Finalist" />
           </a>

        </div>
    </div>	

			    </div>
			</div>
			
			<!-- this is used for content that stretches 100% width of he screen -->
			<div class="fullContent">
				
			</div>
			
			<div class="footer">
				<div class="footerMenuItems">

					<div class="footerMenuItem">
						<a href="/Index.aspx">home</a></div>

					<div class="footerMenuItem">

						<a href="/Privacy.aspx">privacy</a></div>
					<div class="footerMenuItem">
						<a href="/Terms.aspx">terms</a></div>
                    <div class="hiddenFlash">
			            
		            </div>
				    <div class="footerCopyRight">
					    <span class="footerLogo">hangout.net</span>&nbsp;&nbsp;&copy; 2009 Hangout Industries, Inc.
			        </div>

				</div>
			</div>
		</form>
	</div>
</body>
<script type="text/javascript">
	    if(pageTracker != null){
	        pageTracker._initData();
            pageTracker._trackPageview();
        }
    </script>
</html>
