var metrics_token = "4f9f9628d150560f38c3aa668db13f4b";
var metrics_funnel_id = "13_18_Play_Name_funnel_test_10_21";
var config_server_ip = "64.106.173.25";

function gc() {
	var name="campaign";
	name = name.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
	var regexS = "[\\?&]"+name+"=([^&#]*)";
	var regex = new RegExp(regexS);
	var results = regex.exec(window.location.href);
	if (results == null)
		return "";
	else
		return results[1]+" ";
}

var campaign = gc();

try { mpmetrics.init(metrics_token);} catch(err) {}