var metrics_token = "6d04990628ff4d629f48bb4d2d93a866";
var metrics_funnel_id = "f3_funnel";
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