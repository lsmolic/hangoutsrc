var metrics_token = "c68850b1c3b40adc247092580106b89e";
var metrics_funnel_id = "13_18_Play_Fashion_Frenzy_10_21";
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