/*global window, navigator, DetectUnityWebPlayerActiveX, $ */

var Hangout;

if (Hangout && (typeof Hangout != "object" || Hangout.NAME))
{
    throw new Error("Hangout namespace already exists!");
}

// --------- Hangout base ---------

Hangout = {};
Hangout.globalNamespace = this;
Hangout.modules = {};

Hangout.createModule = function (name)
{
    if (!name)
    {
	throw new Error("No name specified in createModule.");
    }
    if (name.charAt(0) == '.' ||
	name.charAt(name.length-1) == '.' ||
	name.indexOf("..") != -1)
    {
	throw new Error("Illegal module name: " + name);
    }

    var parts = name.split('.');
    var container = Hangout.globalNamespace;

    for (var i=0; i<parts.length; i++)
    {
	var part = parts[i];
	if (!container[part])
	{
	    container[part] = {};
	}
	else if (typeof container[part] != "object")
	{
	    var n = parts.slice(0, i).join('.');
	    throw new Error("Attempted to register module " + n + 
			    ", which already exists as non-object!");
	}
	container = container[part];
    }

    var newModule = container;

    if (newModule.NAME)
    {
	throw new Error("Module " + name + " already exists!");
    }

    newModule.NAME = name;
    Hangout.modules[name] = newModule;
    return newModule;
};

Hangout.require = function(name)
{
    if (!(name in Hangout.modules))
    {
	throw new Error("Required module not found: " + name);
    }
};

Hangout.createModule("Hangout");


// --------- Hangout.util ---------

Hangout.createModule("Hangout.util");


// Is Unity installed?
Hangout.util.isUnityInstalled = function ()
{
    var tInstalled = false;
    if (navigator.appVersion.indexOf("MSIE") != -1 && 
	navigator.appVersion.toLowerCase().indexOf("win") != -1)
    {
	tInstalled = DetectUnityWebPlayerActiveX();
    }
    else
    {
        if (navigator.mimeTypes && navigator.mimeTypes["application/vnd.unity"])
	{
            if (navigator.mimeTypes["application/vnd.unity"].enabledPlugin && 
		navigator.plugins && 
		navigator.plugins["Unity Player"])
	    {
                tInstalled = true;
            }
        }
    }
    return tInstalled;	
};

// Is our facebook app added by this user?
Hangout.util.isFacebookAppInstalled = function ()
{
    return window.location.search.indexOf("fb_sig_added=1") != -1;
};

// Hide Unity so we can do other web stuff while keeping it running
Hangout.util.hideUnityContainer = function ()
{
    Hangout.util.unityContainerHeight = $("#unity_container").height();
    Hangout.util.unityContainerWidth = $("#unity_container").width();
    $("#unity_container").height(1);
    $("#unity_container").width(1);
};

// Show Unity again after we finish with whatever web stuff we were doing
Hangout.util.showUnityContainer = function ()
{
    $("#unity_container").height(Hangout.util.unityContainerHeight);
    $("#unity_container").width(Hangout.util.unityContainerWidth);
};

Hangout.util.getInstanceURL = function (prefix, suffix)
{
    if (suffix.slice(0,7) === "http://")
    {
	return suffix;
    }
    else if (prefix.charAt(-1) === '/')
    {
	if (suffix.charAt(0) === '/')
	{
	    return prefix + suffix.slice(1);
	}
	else
	{
	    return prefix + suffix;
	}
    }
    else
    {
	if (suffix.charAt(0) === '/')
	{
	    return prefix + suffix;
	}
	else
	{
	    return prefix + "/" + suffix;
	}
    }
};

Hangout.util.fbid = function() 
{
	Hangout.log.info("fbid");
	var query = window.location.search.substring(1); 
	var vars = query.split("&"); 
	Hangout.log.info(query);
	  for (var i=0;i<vars.length;i++) { 
	    var pair = vars[i].split("="); 
	    if (pair[0] == "fb_sig_user") { 
	      return pair[1]; 
	    } 
	  } 
	return 0;
}

Hangout.util.abtest = function()
{
	var fbid = Hangout.util.fbid();
	Hangout.log.info("FBID "+fbid);
	var test_id = fbid % 2;  
	if(test_id){
		return true;
	}
	else{
		return false;
	}
    
}

Hangout.util.browserDetect = {
	init: function () {
		this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
		this.version = this.searchVersion(navigator.userAgent)
			|| this.searchVersion(navigator.appVersion)
			|| "an unknown version";
		this.OS = this.searchString(this.dataOS) || "an unknown OS";
	},
	searchString: function (data) {
		for (var i=0;i<data.length;i++)	{
			var dataString = data[i].string;
			var dataProp = data[i].prop;
			this.versionSearchString = data[i].versionSearch || data[i].identity;
			if (dataString) {
				if (dataString.indexOf(data[i].subString) != -1)
					return data[i].identity;
			}
			else if (dataProp)
				return data[i].identity;
		}
	},
	searchVersion: function (dataString) {
		var index = dataString.indexOf(this.versionSearchString);
		if (index == -1) return;
		return parseFloat(dataString.substring(index+this.versionSearchString.length+1));
	},
	dataBrowser: [
		{
			string: navigator.userAgent,
			subString: "Chrome",
			identity: "Chrome"
		},
		{ 	string: navigator.userAgent,
			subString: "OmniWeb",
			versionSearch: "OmniWeb/",
			identity: "OmniWeb"
		},
		{
			string: navigator.vendor,
			subString: "Apple",
			identity: "Safari",
			versionSearch: "Version"
		},
		{
			prop: window.opera,
			identity: "Opera"
		},
		{
			string: navigator.vendor,
			subString: "iCab",
			identity: "iCab"
		},
		{
			string: navigator.vendor,
			subString: "KDE",
			identity: "Konqueror"
		},
		{
			string: navigator.userAgent,
			subString: "Firefox",
			identity: "Firefox"
		},
		{
			string: navigator.vendor,
			subString: "Camino",
			identity: "Camino"
		},
		{		// for newer Netscapes (6+)
			string: navigator.userAgent,
			subString: "Netscape",
			identity: "Netscape"
		},
		{
			string: navigator.userAgent,
			subString: "MSIE",
			identity: "Explorer",
			versionSearch: "MSIE"
		},
		{
			string: navigator.userAgent,
			subString: "Gecko",
			identity: "Mozilla",
			versionSearch: "rv"
		},
		{ 		// for older Netscapes (4-)
			string: navigator.userAgent,
			subString: "Mozilla",
			identity: "Netscape",
			versionSearch: "Mozilla"
		}
	],
	dataOS : [
		{
			string: navigator.platform,
			subString: "Win",
			identity: "Windows"
		},
		{
			string: navigator.platform,
			subString: "Mac",
			identity: "Mac"
		},
		{
			   string: navigator.userAgent,
			   subString: "iPhone",
			   identity: "iPhone/iPod"
	    },
		{
			string: navigator.platform,
			subString: "Linux",
			identity: "Linux"
		}
	]

};
Hangout.util.browserDetect.init();


// --------- Hangout.log ---------

Hangout.createModule("Hangout.log");

Hangout.log.options = {};

Hangout.log.log = function (level, message)
{
    var logContainer = document.getElementById("hangout_log_container");
    if (!logContainer || Hangout.log.options[level] !== "on")
    {
	return;
    }

    var zeroPad = function (num, digits)
    {
	num = num.toString();
	while (num.length < digits)
	{
	    num = "0" + num;
	}
	return num;
    };

    var d = new Date();
    message = d.getFullYear() + "/" +
        zeroPad(d.getMonth(), 2) + "/" +
	zeroPad(d.getDate(), 2) + " " +
	zeroPad(d.getHours(), 2) + ":" +
	zeroPad(d.getMinutes(), 2) + ":" +
	zeroPad(d.getSeconds(), 2) + " " +
	level + " " +
	message;

    var logEntry = document.createElement("div");
    logEntry.className = level + "_message";
    logEntry.appendChild(document.createTextNode(message));
    logContainer.appendChild(logEntry);
};

Hangout.log.debug = function (message) { Hangout.log.log("debug", message); };
Hangout.log.info = function (message) { Hangout.log.log("info", message); };
Hangout.log.warning = function (message) { Hangout.log.log("warning", message); };
Hangout.log.error = function (message) { Hangout.log.log("error", message); };
