function hideUnityContainer()
{
    origHeight = $("#unity_container").height();
    origWidth = $("#unity_container").width();
    $("#unity_container").height(1);
    $("#unity_container").width(1);
}

function showUnityContainer()
{
    $("#unity_container").height(origHeight);
    $("#unity_container").width(origWidth);
}

function completeFeedUpdate()
{
    showUnityContainer();
    GetUnity().SendMessage(feedUpdateCallback, "Callback", "");
}

function requestFashionScoreFeedUpdate(callback, args)
{
    hideUnityContainer();

    feedUpdateCallback = callback;
    
    var user_message_prompt = "Write something here!";
    var user_message = {value: "Default message"};
    var template_data = {"comment": ":" + user_message};

    FB.ensureInit(function() {
        FB.Connect.showFeedDialog("149532370799", template_data, null, null, null, 
                                  FB.RequireConnect.require, completeFeedUpdate, user_message_prompt, user_message);
    });
}

function completeCashStore()
{
    $("#cash_store_container").hide();
    showUnityContainer();
    GetUnity().SendMessage("Entry Point", "ReceiveJavascriptCallback", "HI SAMIR!");
}

function requestCashStore()
{
    hideUnityContainer();
    $("#cash_store_frame").attr("src", "fake_billing.html");
    $("#cash_store_container").show();
}    

$(document).ready(function(){
    FB_RequireFeatures(["XFBML"], function()
                       {
                           FB.init(fb_api_key, "xd_receiver.htm");
                           FB.Connect.requireSession();
                       }); 

});
