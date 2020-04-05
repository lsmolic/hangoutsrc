<%@ Page Language="C#" MasterPageFile="~/Payments.master" AutoEventWireup="true" CodeFile="Complete.aspx.cs" Inherits="Complete" Title="Complete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <div class="header"></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentPlaceHolderMain" Runat="Server">
<div class="container">
    <div class="mediumBrownBox">
    <div class="mediumBrownBoxTop">  
        <div style="float:left;"><asp:Literal ID="lblComplete" runat="server"><img width="266px" height="36px" src="/Images/transaction_complete.png" /></asp:Literal></div>
    </div>
    <div class="clearBoth"></div>
    <div class="mediumBrownBoxMiddle">
        <div style="margin-left:auto; margin-right:auto; text-align:center; width:415px; "><asp:Literal ID="messageId" runat="server"/></div>
        <%--<div style="float:left; margin-left:20px; width:184px; height:157px;"><asp:ImageButton runat="Server" style="border:0; margin-top:60px; " width="182px" height="36px" ImageUrl="/Images/return_to_fashion_city_btn.png" 
            onmouseover="this.src='/Images/return_to_fashion_city_rollover.png';" onmouseout="this.src='/Images/return_to_fashion_city_btn.png';" OnClick="return_button_onclick" />
        </div>--%>
        <div style="float:right; margin-right:15px; width:194px;"><img style="border:0; margin-top:10px;" width="192px" height="157px" src="/Images/emo_girl_thanks.png" /></div>
        <div class="clearBoth"></div>
    </div>
    <div class="clearBoth"></div>
    
</div>
</asp:Content>

