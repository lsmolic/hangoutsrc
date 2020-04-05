<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:fb="http://www.facebook.com/2008/fbml">
<script>
    //Variables defined in .net because < % this doesn't work in the <head>
    var GoogleAnalyticsToken = "UA-4968776-4";
    var GoogleAnalyticsDomainName = ".hangout.net";
</script>
<head id="ctl00_Head1"><title>
	Hangout.net ~ Terms and conditions
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
		<form name="aspnetForm" method="post" action="Terms.aspx" id="aspnetForm" style="margin:0px 0px 0px 0px;" AUTOCOMPLETE="off">
<div>
</div>

			<div id="topBanner">
        	    <img alt="Banner" height="51px" width="770px" src="/backup/images/banners/banner_top_beta.png?jsVersionId=2" />
		    </div>
			<div class="topMenu">
				<div class="topMenuItems">
					
					<div class="topMenuItem">
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
				    
	<div class="page_title_div">
		<div class="page_title">
			Terms and Conditions<br />
		</div>
	</div>
	<div class="page_content_div">
		<div class="terms">

			<p>
				TERMS OF SERVICE (TOS): MEMBER AGREEMENT<br />
				ACCEPTANCE OF TERMS</p>
			<p>
				THE TERMS AND CONDITIONS SET FORTH BELOW (THE &quot;AGREEMENT&quot;) GOVERN YOUR USE OF THIS WEBSITE (THE &quot;SITE&quot;) OF ENTERPRISE VISTA SYSTEMS (&quot;COMPANY&quot;), AND THE SERVICES AVAILABLE ON THIS SITE (THE &quot;SERVICES&quot;) AND ARE LEGALLY BINDING ON YOU BY CLICKING THE &quot;I ACCEPT&quot; BUTTON BELOW. IF YOU DO NOT AGREE WITH ANY OF THESE TERMS, YOU ARE NOT PERMITTED TO ACCESS OR USE THIS SITE AND/OR THE SERVICES OR ANY INFORMATION CONTAINED ON THE SITE. YOUR USE OF THIS SITE AND/OR THE SERVICES ON THIS SITE SHALL BE DEEMED TO BE YOUR AGREEMENT TO ABIDE BY EACH OF THE TERMS SET FORTH BELOW AND YOUR REPRESENTATION THAT YOU POSSESS THE AUTHORITY AND CAPACITY TO BIND YOURSELF AND, IF APPLICABLE, YOUR EMPLOYER IN CONTRACT. YOU AGREE THAT COMPANY MAY MAKE CHANGES TO THE SERVICES OFFERED ON THIS SITE, AT ANY TIME WITHOUT NOTICE, AND CAN REVISE THESE TERMS AT ANY TIME. COMPANY WILL NOTIFY YOU OF SUCH REVISIONS BY POSTING AN UPDATED VERSION OF THESE TERMS ON THE SITE. YOU ARE RESPONSIBLE FOR REGULARLY REVIEWING THESE TERMS. YOUR CONTINUED USE OF THE SITE AND/OR THE SERVICES SHALL CONSTITUTE YOUR CONSENT TO SUCH CHANGES.</p>

			<p>
				This Agreement is a legal document that details your rights and obligations as a member entitled to a member account and to access and use the Services. You cannot become a member until you have accepted the terms of this Agreement. The Agreement provides important information about your membership and the Services, so you should take the time to read and understand it. If you have questions about this Agreement, or about your rights and responsibilities as a member, please contact us at <a href="mailto:help@hangout.net" target="_blank">help@hangout.net</a>. You should also take the time to review the Company’s Privacy Policy that reflects Company's current privacy policies and that are a part of this Agreement and to which you are subject. The Internet and online world is changing rapidly and as technology and Company's business continue to evolve, these policies may have to be updated or revised. Since the Privacy Policy and may change, you should check for the most current version at <a href="Privacy.aspx" target="_blank">http://hangout.net</a>.</p>
			<p>
				DESCRIPTION OF SERVICES<br />
				This Agreement, along with the Privacy Policy is your entire agreement with Company and governs your use of the Services. To be a member and receive Services you must be at least 13 years old. If you are not at least 13 years old, you may not register with, access or use this Site. If you are between the ages of 13 and 17, you may register with, access and use this Site only with the approval and supervision of your parent or legal guardian, who must read, acknowledge and accept this Agreement and the Privacy Policy. Parents or legal guardians of minors accessing and using this Site acknowledge and agree that if their child registers with the Site, he or she will provide personal information during registration, and further understand that their child will participate in online chat with other people and post and receive information from Site message boards. By accepting this Agreement, you consent to your child’s registration with, access of and participation on the Site. Further, you may only become a member if you live in a country or other political division that permits membership. This Agreement will be void and without effect, and you will not be entitled to any Services, if you do not satisfy these age and jurisdiction requirements.</p>

			<p>
				Upon completing the registration process, you will select a password and Company will set up an account that is uniquely associated with your membership. All of your access to Services will be through that account, except as otherwise set forth in this Agreement. The Services may include access to Company's online environment (&quot;hangout.net Room&quot;). In the hangout.net Room, you will be able to interact with other members and online constructs that may include people, objects, or sub-environments (&quot;hangout.net Objects&quot;); your interactions with hangout.net Objects will be through a hangout.net Object that you create (an &quot;Avatar&quot;).</p>
			<p>
				As a member you are responsible for all activity on your account. Violations or warnings accrued by your account can lead to termination of your account and termination of any other accounts associated with your account, including without limitation, through billing method or IP address (&quot;Associated Accounts&quot;). Because you are responsible for all use of your account, you must supervise the use of your account by others. It is important that you not reveal your password to others. You agree not to reveal your password to others and you agree to indemnify and hold Company harmless for any improper or illegal use of your account. This includes illegal or improper use by someone to whom you have given permission to use your account. Your account may be terminated if you let someone use it inappropriately. If your account or membership is terminated for any reason, Company is under no obligation to offer the Services or another membership to you or any person with an Associated Account in the future.</p>

			<p>
				PRIVACY POLICY<br />
				Registration data and certain other account information about you is subject to our Privacy Policy. For more information, see our full Privacy Policy <a href="Privacy.aspx" target="_blank">here</a>.</p>
			<p>
				CHARGES, BILLING, THE FREE TRIAL AND FREE MEMBERSHIP<br />
				You are responsible for and agree to pay all charges incurred, including applicable taxes and purchases made, by you or anyone you allow to use your account. There may be extra charges to access certain premium content, Services or areas of the hangout.net Room. Company will provide notice of any extra charge before you incur any charges. You are responsible for any premium charges incurred using your account and these charges may apply even during the free trial and free membership. Billing information will be held as confidential in accordance with the terms set forth in the Privacy Policy.</p>

			<p>
				Payment by Credit Card<br />
				You must use a credit card or an approved payment method to pay for your subscription. When you provide credit card information or other payment method information to Company, you represent and warrant that you are the authorized user of the credit card that is used to pay the subscription or other charges or fees. Unless you provide us prior to the expiry of your subscription with a written notice of cancellation, your subscription will automatically renew for the same period of time. The automatic renewal will not apply if the recurring charges for your membership program have changed since the time of subscription. You agree to promptly notify Company of any changes to your credit card account number, its expiration date and/or your billing address, and you agree to promptly notify Company if your credit card expires or is canceled for any reason.</p>
			<p>
				Charges to Your Credit Card<br />
				Your subscription fees are payable in advance and are not refundable in whole or in part except as provided below in the Discontinuation of Service Section. Company reserves the right to change our fees or billing methods at any time. If your membership program includes recurring monthly fees, Company will provide you with notice of a change to such recurring monthly fees. You will be notified via email at least thirty (30) days in advance. Your continued use of the Services for thirty (30) days after notification of the changes to the recurring monthly fees means that you accept said changes. If any change is unacceptable to you, you may cancel your membership or a particular subscription at any time, but Company will not refund any fees that may have accrued to your account before cancellation of your membership or subscription, and we will not prorate fees for any subscription. If your use of the Service is subject to use or sales tax, then Company may also charge you for any such taxes, in addition to the subscription or other fees.</p>
			<p>

				Free Periods to Subscriptions<br />
				We offer a period of free access to the Services. You are always responsible for any Internet service provider, telephone, and other connection fees that you may incur when using the Services, even when we offer a free period to a subscription.</p>
			<p>
				ONLINE CONDUCT AND CONTENT</p>
			<p>
				Content<br />
				By content, we mean the text, software, communications, images, sounds and other information provided online. Most content in the hangout.net Room is provided by Company, our members, our affiliates, or independent content providers under license. You may not post or upload content to the Site that is offensive, obscene, pornographic, defamatory or libelous. Company reserves the right to pre-screen and monitor content available in the hangout.net Room. However, Company is not obligated to pre-screen or monitor the content and does not assume any responsibility or liability for content that is provided by others. Company does reserve the right to remove content that, in Company's sole discretion is inappropriate or does not comply with the Agreement, or that was originally accepted, but subsequently deemed inappropriate for any reason, but Company is not responsible for any failure or delay in removing such material. Company is not responsible for content available on the Internet, although we reserve the right to block access to any Internet area containing illegal or other harmful content or otherwise being used for purposes that are unlawful or injurious to Company or its members.</p>

			<p>
				Intentional Disruption of Services<br />
				You must respect the rights of others to enjoy safe and unimpeded access to the Services. You may not transmit or provide external links to any content containing a virus time bomb, Trojan horse, spyware, malware, robot, scraper, web crawler, spider or corrupted data, or use the Services or other Internet services in a manner that adversely affects the availability of Company's resources to other members.</p>
			<p>
				Inaccurate Representation of Identity<br />
				You may not impersonate another person (including celebrities), indicate that you are a Company employee or a representative of hangout.net or Enterprise Vista Systems, or attempt to mislead users by indicating that you represent hangout.net or Enterprise Vista Systems or any of Company's partners or affiliates. Do not disclose your password to anyone who purports to be an employee or representative of hangout.net or Enterprise Vista Systems or any of Company’s partners or affiliates.</p>
			<p>

				Selling of Accounts, and Content<br />
				You may not sell member accounts to anyone for purposes of making a profit. This includes specially priced Beta accounts. Company reserves the right to take action (which may include suspension or termination) on any accounts found to be violating this prohibition.</p>
			<p>
				Illegal Behavior<br />
				You will comply will all applicable laws, regulations, and ordinances (including without limitation privacy protection laws such as the federal Child Online Privacy Protection Act of 1998) as a condition of your membership. Company may terminate your account and membership upon receiving information involving your violation of any law, regulation or ordinance, and will cooperate with law enforcement agencies on such matters. Company may monitor the hangout.net Room to enforce this Agreement and the Privacy Policy, to detect or verify illegal activity, and to cooperate with law enforcement agencies.</p>
			<p>
				E-mail, Advertisements, Use of Member Information<br />

				Your membership allows you to send and receive e-mail to and from other members and users of the Internet. You may not use the Services to send unsolicited bulk e-mail, junk e-mail, chain letters or &quot;spam&quot;. You may not harvest or collect information, including screen names, about other members or users of the hangout.net Room. The use of any information learned through the Services or while in the hangout.net Room is limited to the express purposes set forth in this Agreement for the hangout.net Room; all other uses, including, without limitation, sending unsolicited bulk e-mail, are strictly prohibited.</p>
			<p>
				Any violation of these provisions will subject your account and membership to immediate termination and further legal action. If you have received unsolicited bulk e-mail and want to report it, use the Forward button on the e-mail screen and send the e-mail to <a href="mailto:help@hangout.net" target="_blank">help@hangout.net</a>; however, Company cannot guarantee that it can prevent your receipt of all such e-mail. Company also reserves the right to take any and all legal and technical remedies to prevent unsolicited bulk e-mail from entering, utilizing or remaining within the hangout.net Room.</p>
			<p>
				Trades<br />

				Trades are transactions between members involving any items or virtual, fictional currency that are purposely given to another member (by mutual agreement between the two involved members). Company does not take responsibility for trade between members. However, when scams or fraudulent behavior occurs, Company may research and take appropriate action. Trade scams are not permitted in The hangout.net Room. Members involved with scamming or fraudulent trades may have their accounts suspended temporarily or permanently at the sole discretion of Company.</p>
			<p>
				Proprietary Rights<br />
				The content available through the Services or in the hangout.net Room is owned by Company or its licensors, and is protected by copyrights, trademarks, and other intellectual property rights (Copyright ©2007, Enterprise Vista Systems, unless otherwise specified). All rights not expressly granted herein are reserved. Any content that you upload while using the Service must be authorized; this means you must have the legal right to copy, distribute and publicly display the content and you represent and warrant that you have such rights. You must not copy, transmit, modify, distribute, show in public or in private or create any derivative works from any of the content you find in the hangout.net Room, unless you have the legal right or permission of Enterprise Vista Systems and/or its licensors to do so. Making unauthorized copies of any content can lead to the termination of your account and may subject you to further legal action beyond the termination of your membership by Company or other content owners. You agree to indemnify and hold harmless Company and its subsidiaries, affiliates, related companies, employees, officers, directors and agents from any claims made by third parties relating to: (i) content you provide to the Site or (ii) your use of any content.</p>
			<p>
				Copyright Agents<br />
				Company respects the intellectual property of others, and we ask that you do the same. In addition to other state and federal laws, Company complies with the notice and takedown requirements of the Digital Millennium Copyright Act (the “DMCA”). The DMCA provides a process for a copyright owner to give notification to an online service provider concerning alleged copyright infringement. When a valid DMCA notification is received, the service provider responds under this process by taking down the offending content. On taking down content under the DMCA, we will take reasonable steps to contact the owner of the removed content so that a counter-notification may be filed. On receiving a valid counter-notification, we generally restore the content in question, unless we receive notice from the notification provider that a legal action has been filed seeking a court order to restrain the alleged infringer from engaging in the infringing activity.</p>

			<p>
				If you believe that your work has been copied on this Site in a way that constitutes copyright infringement and appears on this Site, please provide our copyright agent with the following information:</p>
			<ul>
				<li>an electronic or physical signature of the person authorized to act on behalf of the owner of the copyright interest;</li>
				<li>a description of the copyrighted work that you claim has been infringed;</li>
				<li>a description of where the material that you claim is infringing is located on the Site;</li>
				<li>your address, telephone number, and email address;</li>

				<li>a statement by you that you have a good faith belief that the disputed use is not authorized by the copyright owner, its agent, or the law;</li>
				<li>a statement by you, made under penalty of perjury, that the above information in your notice is accurate and that you are the copyright owner or authorized to act on the copyright owner's behalf.</li>
			</ul>
			<p>
				Our Copyright Agent for notice of claims of copyright infringement on this Site can be reached as follows:<br />
				By mail:<br />
				Enterprise Vista Systems<br />

				92 Hayden Avenue<br />
				Lexington, MA 02421<br />
				Attn: Copyright Agent</p>
			<p>
				Please note that the DMCA provides that you may be liable for damages (including costs and attorneys’ fees) if you falsely claim that an item is infringing your copyrights. We recommend contacting an attorney if you are unsure whether an item is protected by copyright laws.</p>
			<p>
				To file a counter notification please provide our copyright agent with the following information:</p>

			<ul>
				<li>an electronic or physical signature of the person authorized to act on behalf of the owner of the content that was removed;</li>
				<li>a list of the content that was removed by the hangout.net administrators, and the location at which the content appeared before it was removed; </li>
				<li>your address, telephone number, email address; </li>
				<li>a statement by you, made under penalty of perjury, that you have a good faith belief that the content identified above was removed or disabled as a result of a mistake or misidentification of the content to be removed or disabled.</li>
			</ul>
			<p>

				Submissions<br />
				Company is pleased to hear from its members and welcomes your comments regarding the Services and the hangout.net Room. However, Company does not accept or consider creative ideas, suggestions or materials other than those it has specifically requested. It is the intent of this policy to avoid any misunderstandings when projects developed by Company's professional staff might seem to others to be similar to their own creative work. Accordingly, we must ask that you do not send us any original creative materials such as game ideas or original artwork. While we do value your feedback on our Services and products, we request that you be specific in your comments on those Services and products, and not submit any creative ideas, suggestions or materials. In the event that you do provide us with any creative ideas, suggestions or materials, you agree to and hereby do grant Company a nonexclusive, perpetual, fully paid-up, sublicensable, transferable, worldwide license to use, copy, modify, create derivative works of, sublicense, rent, lease, distribute and otherwise exploit such creative ideas, suggestions and materials.</p>
			<p>
				COMPANY SOFTWARE LICENSES<br />
				Company provides you with a limited nonexclusive, nonsublicensable license to use software you receive or access as part of the Services (Software), subject to your compliance with this Agreement, the Privacy Policy and any additional conditions made known to you at the time of download of particular software. You shall not copy, distribute, sell, lease, lend, sub-license, or charge others to use or access, the Software, or reverse engineer, disassemble or attempt to discover the structure or underlying ideas of any Software. Company may occasionally provide upgrades to improve the Services and the hangout.net Room, Company employs virus-screening technology to assist in the protection of our network and our members, and Software may include routines designed to prevent the spread of viruses, or improper use of the Software (which routines may disable the Software); you agree not to attempt to circumvent any functions or routines of the Software. You understand that Company's introduction of various technologies may not be consistent across all platforms and that the performance and some features offered by Company may vary depending on your computer and other equipment.</p>
			<p>
				You acknowledge that the Site has a functionality of virtual, fictional currency referred to as hangout.net money, which constitutes a limited license to use a feature of the Site, as permitted by us. We may charge fees for the right to use such hangout.net money or may distribute such hangout.net money without charge, in our sole discretion. This hangout.net money represents a limited license right governed solely by these Terms of Service, and is not redeemable for any sum of money or monetary value from Enterprise Vista Systems at any time. You agree that Enterprise Vista Systems has the absolute right to manage, regulate, control, modify and/or eliminate such hangout.net money as it sees fit, and that Enterprise Vista Systems will have no liability to you based on the exercise of such right.</p>

			<p>
				DISCLAIMERS AND LIMITATIONS</p>
			<p>
				Warranty Disclaimers<br />
				YOU EXPRESSLY AGREE THAT THE USE OF COMPANY'S SOFTWARE, THE SERVICES, THE HANGOUT.NET ROOM AND THE INTERNET IS AT YOUR SOLE RISK. COMPANY'S SOFTWARE, THE HANGOUT.NET ROOM, AND ALL OTHER PRODUCTS, SERVICES AND TECHNOLOGY AND ACCESS (TO THE INTERNET OR OTHERWISE) ARE PROVIDED BY COMPANY AND ITS SUPPLIERS &quot;AS IS&quot; AND &quot;AS AVAILABLE&quot; FOR YOUR USE, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED. COMPANY HEREBY DISCLAIMS ALL WARRANTIES, INCLUDING, BUT NOT LIMITED TO, WARRANTIES OF MERCHANTABILITY, TITLE, FITNESS FOR A PARTICULAR PURPOSE, NON-INFRINGEMENT, ACCURACY, ACCESSIBILITY OF THE SERVICES, THAT THE USE OF THE SERVICE WILL BE SECURE, TIMELY, UNINTERRUPTED OR ERROR FREE, AND CAPACITY OF THE HANGOUT.NET ROOM. SOME STATES DO NOT ALLOW LIMITATIONS ON HOW LONG AN IMPLIED WARRANTY LASTS, SO THE ABOVE LIMITATIONS MAY NOT APPLY TO YOU. COMPANY DOES NOT ENDORSE, WARRANT OR GUARANTEE ANY PRODUCT OR SERVICE OFFERED THROUGH COMPANY OR THE hangout.net ROOM AND WILL NOT BE A PARTY TO OR IN ANY WAY BE RESPONSIBLE FOR MONITORING ANY TRANSACTION BETWEEN YOU AND THIRD-PARTY PROVIDERS OF PRODUCTS OR SERVICES.</p>

			<p>
				Liability and Remedy Limitations<br />
				COMPANY'S ENTIRE LIABILITY AND YOUR EXCLUSIVE REMEDY WITH RESPECT TO THE USE OF ANY SOFTWARE PROVIDED OR USED BY COMPANY SHALL BE THE REPLACEMENT OF SUCH SOFTWARE FOUND TO BE DEFECTIVE. YOUR SOLE AND EXCLUSIVE REMEDY FOR ANY OTHER DISPUTE WITH COMPANY AND YOUR SOLE AND EXCLUSIVE ALTERNATIVE REMEDY IF ANY REMEDY HEREUNDER FAILS OF ITS ESSENTIAL PURPOSE IS THE CANCELLATION OF YOUR ACCOUNT AS DETAILED BELOW IN SECTION 9 AND A REFUND OF AMOUNTS PAID TO COMPANY IN THE NINETY DAY PERIOD PRIOR TO SUCH CANCELLATION. NOTWITHSTANDING ANYTHING ELSE IN THIS AGREEMENT OR OTHERWISE, COMPANY WILL NOT BE LIABLE WITH RESPECT TO ANY SUBJECT MATTER OF THIS AGREEMENT UNDER ANY CONTRACT, NEGLIGENCE, STRICT LIABILITY OR OTHER LEGAL OR EQUITABLE THEORY (I) FOR ANY AMOUNTS IN EXCESS, IN THE AGGREGATE, OF THE FEES PAID TO COMPANY HEREUNDER DURING THE NINETY DAY PERIOD PRIOR TO THE DATE THE CAUSE OF ACTION AROSE OR (II) FOR ANY INCIDENTAL OR CONSEQUENTIAL DAMAGES OR LOST DATA OR (III) FOR COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR SERVICES. SOME STATES OR JURISDICTIONS DO NOT ALLOW THE EXCLUSION OR THE LIMITATION OF LIABILITY FOR CONSEQUENTIAL OR INCIDENTAL DAMAGES; IN SUCH STATES OR JURISDICTIONS, COMPANY'S LIABILITY SHALL BE LIMITED TO THE EXTENT PERMITTED BY LAW.</p>
			<p>
				Company MAKES NO GUARANTY OF CONFIDENTIALITY OR PRIVACY OF ANY COMMUNICATION OR INFORMATION TRANSMITTED USING THE SERVICES OR THE HANGOUT.NET ROOM OR ANY WEB-SITE LINKED TO THEM. Company will not be liable for the privacy of e-mail addresses, registration and identification information, disk space, communications, confidential or trade-secret information, or any other content stored on Company's equipment, transmitted over networks accessed by the site, or otherwise connected with your use of the Services.</p>
			<p>
				INDEMNIFICATION<br />

				You agree to defend, indemnify and hold harmless Company and its affiliated subsidiaries, employees, contractors, officers, directors, telecommunications providers and content providers from all liabilities, claims and expenses, including attorneys’ fees, that arise from a breach of this Agreement or are made by third parties related to your use of Services or the hangout.net Room or the Internet, or in connection with your transmission of any content using the Services or hangout.net Room. Company reserves the right to assume the exclusive defense and control of any claim otherwise subject to indemnification by you.</p>
			<p>
				TERMINATION, CANCELLATION, AND INTERRUPTION OF SERVICE</p>
			<p>
				Termination<br />
				Either you or Company may terminate or cancel your membership at any time. You understand and agree that the cancellation of your membership is your sole right and remedy with respect to any dispute with Company, except for refunds expressly provided for in the Cancellation and Discontinuation of Service sections below. This includes, but is not limited to, any dispute related to, or arising out of: (1) any term of this Agreement or Company's enforcement or application of this Agreement; (2) any policy or practice of Company including its Privacy Policy, operation of the hangout.net Room, or Company's enforcement or application of its policies; (3) the content available through the Services or hangout.net Room or any change in content so provided; (4) your ability to access and/or use Services or the hangout.net Room; or (5) the amount or type of fees, surcharges, applicable taxes, billing methods, or any change to the fees, applicable taxes, surcharges or billing methods.</p>
			<p>

				Cancellation<br />
				You can cancel your membership by contacting us at <a href="mailto:membership@hangout.net" target="_blank">membership@hangout.net</a>. Cancellation will take effect within 72 hours of receipt of your request, and Company will send you written confirmation via email. If you cancel near the end of your billing period and are inadvertently charged for the next month's fee contact Company at <a href="mailto:billing@hangout.net" target="_blank">billing@hangout.net</a>. Company reserves the right to collect fees, surcharges or costs incurred before you cancel your membership. In addition, you are responsible for any charges incurred to third-party vendors or content providers prior to your cancellation.</p>
			<p>
				In the event that your account is terminated or canceled, no refund, including any membership fees, will be granted, except for refunds expressly provided for in the Discontinuation of Service section below; no online time or other credits (e.g., points in an online game or any hangout.net Objects or hangout.net money purchased, won, or earned) will be credited to you, nor can they be converted to cash or other form of reimbursement. You may not allow former members whose memberships have been terminated to use your accounts. Any delinquent or unpaid accounts or unresolved issues relating to former membership must be resolved before Company will permit you to have a new membership. All provisions of this Agreement that by their nature should survive termination of this Agreement do survive its termination, including, but not limited to, provisions on ownership, proprietary rights, warranty disclaimers and liability and remedy limitations.</p>
			<p>

				Interruption of Service<br />
				Company reserves the right to interrupt the Service with or without prior notice for any reason or no reason. You agree that Company will not be liable for any interruption of the Service, delay or failure to perform and that Company is not obligated to refund monies for example, subscription fees or hangout.net money purchases.</p>
			<p>
				Discontinuation of Service<br />
				In the event Company permanently discontinues Service or terminates this Agreement other than for a breach of this Agreement by you or other cause, Company may, upon your request, refund your previous thirty days' purchases of subscription fees. Said refund is limited to purchases made in the thirty days prior to the permanent discontinuation of the Service and only applies to purchases you made directly from Company, not via any third-party, other members or any other source. Refunds must be requested within fifteen days of the discontinuation and will be paid within thirty days of receiving the request. Requests should be made by sending e-mail to <a href="mailto:help@hangout.net" target="_blank">help@hangout.net</a>.</p>
			<p>

				NON-DISCLOSURE<br />
				You shall keep confidential and not disclose to any third party or use (except as part of using the Services and the hangout.net Room) any non-public information obtained from Company or as part of your use of the Services and the hangout.net Room (Confidential Information). This restriction will not apply to information that you can document is publicly available, or becomes publicly available, through no act or omission of yours. Due to the unique nature of Confidential Information, you agree there can be no adequate remedy at law for breach of this Section and that such breach would cause irreparable harm to Company; therefore, Company shall be entitled to seek immediate injunctive relief, without an obligation to post a bond in addition to whatever remedies it might have at law or under this Agreement. This restriction shall remain in effect even after the termination of your membership until all Confidential Information becomes publicly available.</p>
			<p>
				LAW AND LEGAL NOTICES<br />
				This Agreement, in conjunction with the Privacy Policy, represents your entire agreement with Company. You agree that this Agreement is not intended to confer and does not confer any rights or remedies upon any person other than the parties to this Agreement. You also understand and agree that the Company's policies, including, but not limited to its Privacy Policy for the hangout.net Room, including Company's enforcement of these policies, is not intended to confer, and does not confer, any rights or remedies upon any person. If any part of this Agreement is held invalid or unenforceable, that portion shall be construed in a manner consistent with applicable law to reflect, as nearly as possible, the original intentions of the parties, and the remaining portions shall remain in full force and effect.</p>
			<p>
				CHOICE OF LAW AND FORUM SELECTION<br />

				The laws of the Commonwealth of Massachusetts, excluding its conflicts-of-law rules, govern this Agreement and your membership. As noted above, member conduct may be subject to other local, state, national, and international laws. You expressly agree that exclusive jurisdiction for any claim or dispute with Company or relating in any way to your membership or your use of Services or the hangout.net Room resides in the courts of Massachusetts and you further agree and expressly consent to the exercise of personal jurisdiction in the courts of Massachusetts in connection with any such dispute including any claim involving Company or its affiliates, subsidiaries, employees, contractors, officers, directors, telecommunication providers and content providers.</p>
			<p>
				You agree to abide by U.S. and other applicable export control laws and not to transfer, by electronic transmission or otherwise, any content or software subject to restrictions under such laws to a national destination prohibited under such laws, without first obtaining, and then complying with, any requisite government authorization. You further agree not to upload to Company or the hangout.net Room any data or software or content that cannot be exported without prior written government authorization, including, but not limited to, certain types of encryption software. This assurance and commitment shall survive termination of this Agreement.</p>
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
