/*global Hangout, document, navigator */

Hangout.createModule("Hangout.unityDispatcher");

Hangout.unityDispatcher.unityToJSCalls = {};

// Fetch the correct Unity object from the DOM depending on which browser we're running.
Hangout.unityDispatcher.getUnityObject = function ()
{
    if (navigator.appVersion.indexOf("MSIE") != -1 && 
	navigator.appVersion.toLowerCase().indexOf("win") != -1)
    {
	return document.getElementById("UnityObject");
    }
    else if (navigator.appVersion.toLowerCase().indexOf("safari") != -1)
    {
	return document.getElementById("UnityObject");
    }
    else
    {
	return document.getElementById("UnityEmbed");
    }
};

// Send a message to a specific Unity GameObject method.
Hangout.unityDispatcher.sendMessage = function (destObj, destMethod, message)
{
    if((typeof message === 'string') || (typeof message === 'number'))
    {
	this.getUnityObject().SendMessage(destObj, destMethod, message);
    }
    else
    {
	throw new Error("sendMessage received a non-number non-string message!");
    }
};

/*
exposeFunctionToUnity: Call this to expose your JS function to the JSDispatcher in Unity.

name: The name assigned to the new function.
isSynchronous: Will the result be computed and returned immediately?
func: Function to be exposed.

If isSynchronous is true, we expect func to return its result immediately and will call
    Unity back with the return value as soon as the function exits.

If isSynchronous is false, func must receive a callback function as its first argument.
    The callback function passes its first argument as the result in a callback to Unity.
    func's implementor is responsible for making sure this callback function is invoked.
    If the function is not invoked, Unity will receive no result.
*/
Hangout.unityDispatcher.exposeFunctionToUnity = function(name, isSynchronous, func)
{
    if (name in this.unityToJSCalls)
    {
	throw new Error("Function name " + name + " already exists in unityToJSCalls!");
    }

    if (isSynchronous)
    {
	this.unityToJSCalls[name] = function (callbackObject, callbackMethod)
	{
	    var newArgs = [];
	    for(var i=2; i<arguments.length; i++)
	    {
		newArgs.push(arguments[i]);
	    }
	    Hangout.unityDispatcher.sendMessage(callbackObject, 
						callbackMethod, 
						String(func.apply(null,newArgs)));
	};
    }
    else
    {
	this.unityToJSCalls[name] = function (callbackObject, callbackMethod)
	{
	    var cbFunc = function(res)
	    {
		Hangout.unityDispatcher.sendMessage(callbackObject, 
						    callbackMethod, 
						    String(res));
	    };
	    var newArgs = [];
	    newArgs.push(cbFunc);
	    for(var i=2; i<arguments.length; i++)
	    {
		newArgs.push(arguments[i]);
	    }
	    func.apply(null, newArgs);
	};
    }
};

// Let Unity grab the list of functions in the interface we're providing it!
Hangout.unityDispatcher.exposeFunctionToUnity("getExposedFunctions", true, function ()
{
    var funcList = "";
    for (var funcName in Hangout.unityDispatcher.unityToJSCalls)
    {
	if (Hangout.unityDispatcher.unityToJSCalls.hasOwnProperty(funcName))
	{
	    funcList += funcName + ",";
	}
    }

    if(funcList.length > 0)
    {
	return funcList.slice(0, funcList.length - 1);
    }
    else
    {
	return "";
    }
});
