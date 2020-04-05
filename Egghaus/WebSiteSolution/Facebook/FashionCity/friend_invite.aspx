<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:fb="http://www.facebook.com/2008/fbml">
<head>
<title></title>

<script type="text/javascript" src="/JS/mixpanel.js"></script>
<script type="text/javascript" src="/JS/hangout_base.js"></script>
<script type="text/javascript" src="/JS/config.js.aspx"></script>
<script type="text/javascript" src="/JS/jquery.js"></script>

<script type="text/javascript">

$(document).ready(function()
{
    FB_RequireFeatures(["XFBML"], function()
    {
        FB.init(Hangout.config.fb_api_key, "xd_receiver.htm");
        FB.Connect.requireSession();
    });
});

</script>
</head>

<body>

<div id="FB_HiddenContainer"  style="position:absolute; top:-10000px; width:0px; height:0px;" ></div>
<script src="http://static.ak.connect.facebook.com/js/api_lib/v0.4/FeatureLoader.js.php" type="text/javascript"></script>

<h1>Welcome to Amazing Fake Friend invite!</h1>

<div id="friend_invite_container">
  <fb:serverfbml style="width: 750px;">
    <script type="text/fbml">
      <fb:fbml>
	<fb:request-form action="<%= WebConfig.WebHostBaseURL %>/Facebook/FashionCity/closemyframe.html" 
			 method="GET" 
			 invite="true" 
			 type="Fashion City" 
			 content="Test invite!  <fb:req-choice url='<%= WebConfig.FacebookBaseURL %>' label='Register' />">
	  <fb:multi-friend-selector showborder="false" actiontext="Invite your Facebook Friends to use Fashion City" <!--condensed="true"--> />
	  <fb:request-form-submit />
	</fb:request-form>
      </fb:fbml>
    </script>
  </fb:serverfbml>
</div>

</body>
</html>
