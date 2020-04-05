<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:fb="http://www.facebook.com/2008/fbml">
<script>
    //Variables defined in .net because < % this doesn't work in the <head>
    var GoogleAnalyticsToken = "UA-4968776-4";
    var GoogleAnalyticsDomainName = ".hangout.net";
</script>
<head id="ctl00_Head1"><title>
	Hangout.net ~ Privacy policy
</title><link href="/backup/App_Themes/hangout_main.css?jsVersionId=2" rel="stylesheet" type="text/css" />
	<script src="/backup/js/sniffer.js?jsVersionId=2" type="text/javascript"></script>
	<script language="javascript" type="text/javascript">
	    if (is_fx)
	    {
			document.write('<link href="/backup/App_Themes/hangout_mozilla.css?jsVersionId=2" rel="stylesheet" type="text/css" />');
	    } 
	</script>

	<script type="text/javascript">
	    //this is an awesome debugger for messages being sent to flex 
	    //or any other javascript based problem (thanks mike!)
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
		<form name="aspnetForm" method="post" action="Privacy.aspx" id="aspnetForm" style="margin:0px 0px 0px 0px;" AUTOCOMPLETE="off">
<div>
<input type="hidden" name="__VIEWSTATE" id="__VIEWSTATE" value="/wEPDwULLTEwMDUyNjYzMjhkZI77UW83D7zNyLt0ZBplwKngj5cW" />
</div>

			<div id="topBanner">
        	    <img alt="Banner" height="51px" width="770px" src="/backup/images/banners/banner_top_beta.png?jsVersionId=2" />
		    </div>
			<div class="topMenu">
				<div class="topMenuItems">
					
					<div class="topMenuItem">
						<a href="/Index.aspx">
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
				    
	<div class="page_title_div">
		<div class="page_title">
			Privacy Policy</div>
	</div>
	<div class="page_content_div">
		<div class="privacy">
			<p>

				Protecting our members' privacy is one of our most important corporate objectives. This Privacy Policy details how Enterprise Vista Systems, Inc. (&quot;Company&quot; or &quot;We&quot; or &quot;Our&quot;) treats visitors' and members' personally identifiable information collected during their interactions with our products and services on the website <a href="http://hangout.net" target="_blank">http://hangout.net</a> (the &quot;Site&quot;). Personally Identifiable Information includes information about you that is personally identifiable, such as your name, address, email address, or phone number, and that is not otherwise publicly available.</p>
			<p>

				We at Enterprise Vista Systems are committed to protecting your privacy. We use the information we collect about you to provide you with a more personalized experience at the Site, to improve or add features and functionality, and to contact you for marketing purposes that may be of interest to you. Please read on for more details about our Privacy Policy.</p>
			<p>
				(1) Except as set forth in this Privacy Policy, we do not collect your Personally Identifiable Information or provide your Personally Identifiable Information to third parties, unless authorized by you to do so.</p>
			<p>
				In many cases we need Personally Identifiable Information to provide the personalized or enhanced service that you have requested. The amount of Personally Identifiable Information that you choose to disclose to Company is completely up to you. The only way we know something about you personally is if you provide it to us in conjunction with one of our services.</p>
			<p>
				Credit Card Information and Credit Card Security. Payment information (credit card numbers and expiration dates) is treated confidentially and will not intentionally be shared with anyone outside of Company, its affiliates and the financial institutions used to process payments. We currently use the Secure Sockets Layer (SSL) protocol to safeguard your information, including your credit card number, during online transactions. Secure Sockets Layer (SSL) is designed to encrypt information and keep the data private and confidential between your machine and Company. This technology is designed to make it safe to transmit your credit card number over the Internet. Look for two security icons, the &quot;s&quot; after &quot;http&quot; in the address line and the lock in the top menu bar and bottom status bar of your browser in Netscape (or in the bottom status bar only in Internet Explorer). For your protection, never put your credit card number or other sensitive information in unencrypted e-mail.</p>

			<p>
				When you join hangout.net, we ask you to create an account, or an account may be created for you by accepting an invitation from a friend. We ask that you complete and submit an online form with Personally Identifiable Information such as your name, email address, birth date, and gender. Providing any additional Personally Identifiable Information beyond what is required to create an account is optional. If you have indicated that you would like other members to be able to view your profile, your profile and the Personally Identifiable Information contained in your profile may be viewed by other members. We may use your Personally Identifiable Information to deliver certain services, products or information you have requested, verify your authority to enter certain password protected areas of the Site, send you notices for services that you have used or that may be of interest to you, and improve the content and general administration of the Site.</p>
			<p>
				The Site may track the total number of visitors to our Site, the number of visitors to each page of our Site, IP addresses, External Web Sites linked to, time visiting our site, and we may analyze these data for trends and statistics in the aggregate, but such information will be maintained, used and disclosed in aggregate form only and it will not contain Personally Identifiable Information. We may use such aggregate information to analyze trends, administer the Site, track users' movement, and gather broad demographic information for aggregate use. Reports of this aggregate information may be shared with outside companies, but no specific member Personally Identifiable Information will be revealed or shared.</p>
			<p>
				We also collect and use other information for internal purposes. For example, we keep records in your account history of your complaints about other members' online behavior, your contact with Company Services personnel and any reported violations of our Terms of Service or this Privacy Policy that you or someone on your account may have committed. We will sometimes use information about your geographical location to provide localized service. For example, we may use the time zone you are in to make sure you have access to certain time-sensitive features.</p>
			<p>

				We may release your Personally Identifiable Information about you and/or your account: (a) to a successor entity upon a merger, consolidation or other corporate reorganization in which we participate or to a purchaser of all or substantially all of our assets to which this Site relates. Such successor entity will be bound by the terms and conditions of this Privacy Policy; (b) to comply with valid legal process such as a search warrant, subpoena or court order, or (c) in special cases such as a physical threat to you or others.</p>
			<p>
				We provide you with the opportunity to update or correct your contact and billing information that we have on file. Just as you want to make sure that information the Company has about you is accurate, we want to keep only the most up-to-date information about your account. Therefore, whenever you believe that your contact or billing information needs updating, you can email <a href="mailto:help@hangout.net" target="_blank">help@hangout.net</a> with the updated information, or use &quot;My Profile&quot;.</p>
			<p>
				(2) We may set, access and collect Cookies</p>

			<p>
				We may use small text files called cookies to improve overall Site experience. A cookie is a piece of data stored on the user's hard drive containing information about the user. Cookies generally do not permit us to personally identify you.</p>
			<p>
				We set and access cookies on your computer for a number of purposes, including without limitation to:</p>
			<ul>
				<li>Identify you when you Login;</li>
				<li>Provide you with your Avatar content;</li>

				<li>Maintain your session state;</li>
				<li>Present the most appropriate in-room content, messaging, and promotions, based on your skill levels and activity;</li>
				<li>Analyze and report our total audience size and traffic; and</li>
				<li>Conduct research to improve the overall experience.</li>
			</ul>
			<p>
				We may contract with a third party to track and analyze non-personally identifiable usage and volume statistical information from our visitors and members to administer our Site in order to constantly improve the Site quality. The third party might use cookies to help track visitor behavior and would set cookies on our behalf. We reserve the right to publish non-personally identifiable or summary information regarding our visitors and members for promotional purposes and as a representative audience for advertisers. Please note that this is not Personally Identifiable Information, only general summaries or aggregate information of the activities of our visitors and members.</p>

			<p>
				(3) Generally, we do not read your private online communications.</p>
			<p>
				We honor the confidentiality of our members' private communications in private areas and during call conversations. We generally do not read your private communications but we reserve the right to read your private communications and we may disclose private communications as required by law, e.g.; to comply with a search warrant, subpoena, court order or other legal requirement, to protect the company's rights and property, or in some cases to resolve member disputes. Topical conversation statistics may be collected in aggregate. Of course, what you write or post in public areas (e.g., message boards) is available to all members.</p>
			<p>
				(4) We may use information about the kinds of products you buy from us to make other promotional offers to you.</p>
			<p>

				We may give our members the opportunity to buy gifts such as virtual objects, using our virtual currency and real-world currency. Like other retailers, we record information about purchases. When you buy from us online, our system automatically gathers purchase data.</p>
			<p>
				We use this information in two ways:</p>
			<p>
				(a) We review what kinds of products and services appeal most to our members as a group. This statistical information helps us improve our offerings and promotions in the same way that other companies change their catalog based on member preferences and does not consist of Personally Identifiable Information.</p>
			<p>
				(b) We use information such as the number of purchases members make and the categories of goods and services they buy to make offers to you that we believe will interest you. In addition, we use other information such as when members joined hangout.net, how often they use the service or their type of computer system to make such offers. We also use publicly available consumer data (such as zip-code based demographics) to help us decide which promotional offers to make and which promotions members see.</p>

			<p>
				By promotions we mean a wide variety of activities and merchandise, all related to your experience as a hangout.net member. For example, members interested in a specific activity (such as flipping through magazines) might receive special offers on new magazines that are not generally available to other members. Members who choose to interact with virtual versions of real-world products and services might receive offers to buy the real-world good and service at a discount.</p>
			<p>
				You may choose not to receive marketing offers from Company. For more information about your choices, please see number 5, below.</p>
			<p>
				We do not give out any Personally Identifiable Information about what you, as an individual, purchase from Company except to complete your transactions, or to comply with valid legal process such as a search warrant, subpoena or court order. We share with outside companies only aggregate or statistical information about what Company products or services our members, as a group, buy.</p>
			<p>

				(5) We give you control about how we use your Personally Identifiable Information.</p>
			<p>
				You have choices about how the information you have provided may be used by us to make special offers to you.</p>
			<p>
				You may opt out from the hangout.net mailing list at any time by emailing: <a href="mailto:optout@hangout.net" target="_blank">optout@hangout.net</a> from the email account at which you are receiving hangout.net emails.</p>
			<p>

				(6) We take extra steps to protect the safety and privacy of children.</p>
			<p>
				Young people need special safeguards and privacy protection. We urge all parents to teach their children about protecting their Personally Identifiable Information while online. Our Site and services are not intended for children under the age of 13. We do not knowingly collect Personally Identifiable Information from children under the age of 13 and we do not target our services or this Site to children under 13. Teens age 13 to 18 should obtain permission from their parents before providing any Personally Identifiable Information or registering for an account.</p>
			<p>
				(7) We consider adding images, video and other content as publishing.</p>
			<p>
				Please note that any image, video, or other content posted or shared by Users on the website or inside of rooms in hangout.net becomes published content and is not considered Personally Identifiable Information subject to this Privacy Policy. Please see our Terms of Service for more information on published content.</p>

			<p>
				(8) We cannot predict the future.</p>
			<p>
				In the event that hangout.net is acquired by or merged with a third party entity, we reserve the right, in any of these circumstances, to transfer or assign the information we have collected from our Members as part of such merger, acquisition, sale, or other change of control. In the unlikely event of our bankruptcy, insolvency, reorganization, receivership, or assignment for the benefit of creditors, or the application of laws or equitable principles affecting creditors' rights generally, we may not be able to control how your Personally Identifiable Information is treated, transferred, or used.</p>
			<p>
				(9) We use technology and security and privacy protection controls to safe guard your personally identifiable information.</p>
			<p>

				We may use state-of-the-art technology to keep your Personally Identifiable Information including your billing and account information secure. We also have put in place privacy protection control systems that are reasonably designed to ensure that your Personally Identifiable Information remain safe and private.</p>
			<p>
				(10) We will keep you updated about what we do with your Personally Identifiable Information.</p>
			<p>
				When you register for our service, you are presented with our Privacy Policy and should familiarize yourself with this and all other Company policies at that time. In addition, this Privacy Policy is easily accessible at <a href="Privacy.aspx" target="_blank">http://hangout.net</a>.</p>
			<p>

				Due to the Internet's rapidly evolving nature, we may need to update this Privacy Policy from time to time. If so, we will post our updated Privacy Policy on this Site located at <a href="Privacy.aspx" target="_blank">http://hangout.net</a> so you are always aware of what Personally Identifiable Information we may collect and how we may use such information. We encourage you to review this Privacy Policy regularly for any changes. Your continued use of this Site and/or the services on this Site will be subject to the then-current Privacy Policy.</p>
			<p>
				If you'd like to comment on or have questions about our Privacy Policy, or if you have a concern or Privacy Policy violation you wish to report, please e-mail <a href="mailto:help@hangout.net" target="_blank">help@hangout.net</a> or write to:</p>
			<p>
				Enterprise Vista Systems<br />

				92 Hayden Avenue<br />
				Lexington, MA 02421</p>
			<p>
				This Privacy Policy was last updated: May 18, 2007</p>
			<p>
				This Privacy Policy is effective as of May 18, 2007</p>
			<p>

				© Copyright 2007, Enterprise Vista Systems. All rights reserved.</p>
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
