<%@ Page Language="C#" MasterPageFile="~/Payments.master" AutoEventWireup="true"
    CodeFile="Purchase.aspx.cs" Inherits="Purchase" Title="Purchase" %>

<%@ Register Src="webcontrols/PackageSelectionControl.ascx" TagName="PackageSelectionControl"
    TagPrefix="ps1" %>
<asp:Content ID="headstuff" ContentPlaceHolderID="head" runat="Server">
    <link href="CSS/purchase.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="headerPlaceHolderMain" runat="Server">
    <div class="header">
    </div>
    <%--<div style="width: 120px;">
        <asp:ImageButton runat="Server" CssClass="backButton" ImageUrl="/Images/back_button.png"
             onmouseover="this.src='/Images/back_button_rollover.png';" onmouseout="this.src='/Images/back_button.png';" OnClick="backButton_click" />
    </div>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" runat="Server">
    <div class="container">
        <div class="brownBox" runat="server" id="divPackages">
            <div class="brownBoxTop">
                <div style="float: left;">
                    <img width="216px" height="40px" src="/Images/1_select_package.png" /></div>
                <div style="float: right;">
                    <img width="299px" height="41px" src="/Images/2_select_payment_option.png" /></div>
            </div>
            <div class="clearBoth">
            </div>
            <div class="brownBoxMiddle">
                <div class="selectPackage">
                    <div class="selectPackageTop">
                    </div>
                    <div class="selectPackageMiddle">
                        <div id="packages">
                            <div>
                                <ps1:PackageSelectionControl runat="server" GroupPackage="package" PaymentType="CASH"
                                    ID="psCash" />
                            </div>
                            <div visible="false">
                                <ps1:PackageSelectionControl Visible="false" runat="server" GroupPackage="package"
                                    PaymentType="COIN" ID="psCoin" />
                            </div>
                        </div>
                    </div>
                    <div class="clearBoth">
                    </div>
                    <div class="selectPackageBottom">
                    </div>
                </div>
                <div class="arrowContainer">
                    <img height="94px" width="75px" src="/Images/red_arrow.png" />
                </div>
                <div id="divPurchase" class="smallGreenBox" style="float: left;">
                    <div class="smallGreenBoxTop">
                    </div>
                    <div class="smallGreenBoxMiddle">
                        <asp:ImageButton CssClass="paymentTypeButton" ImageUrl="/Images/visa_button.png"
                            onmouseover="this.src='/Images/visa_button_rollover.png';" onmouseout="this.src='/Images/visa_button.png';"
                            ID="btnCreditCard" runat="server" Height="59" Width="217px" OnClick="btnCreditCard_Click" /><br />
                        <asp:ImageButton CssClass="paymentTypeButton" ImageUrl="/Images/paypal_button.png"
                            onmouseover="this.src='/Images/paypal_button_rollover.png';" onmouseout="this.src='/Images/paypal_button.png';"
                            ID="btnPayPal" runat="server" Height="59" Width="217px" OnClick="btnPayPal_Click" />
                    </div>
                    <div class="smallGreenBoxBottom">
                                            <div>
                            <table width="135" border="0" cellpadding="2" cellspacing="0" title="Click to Verify - This site chose VeriSign SSL for secure e-commerce and confidential communications.">
                                <tr>
                                    <td width="135" align="center" valign="top"><script type="text/javascript" src="https://seal.verisign.com/getseal?host_name=secure.hangout.net&amp;size=M&amp;use_flash=YES&amp;use_transparent=YES&amp;lang=en"></script><br />
                                    <a href="http://www.verisign.com/ssl-certificate/" target="_blank"  style="color:#000000; text-decoration:none; font:bold 7px verdana,sans-serif; letter-spacing:.5px; text-align:center; margin:0px; padding:0px;">ABOUT SSL CERTIFICATES</a></td>
                                </tr>
                           </table>
                        </div>
                    </div>
                </div>
                <div class="clearBoth">
                </div>
            </div>
            <div class="clearBoth">
            </div>
            <div class="brownBoxBottom">
            </div>
            <div class="clearBoth">
            </div>
            <div id="divErrorPopUp" runat="server" visible="false" style="border: thin inset #FFFFCC;
                text-align: center; position: absolute; z-index: 2; display: block; background-color: #FFFFFF;
                width: 200px; height: 100px; top: 100px; left: 100px;">
                <br />
                <asp:Literal runat="server" ID="textError"></asp:Literal>
                <br />
                <br />
                <asp:Button runat="server" ID="btnError" Text="OK" />
            </div>
        </div>
        <div style="margin-top: 30px; margin-bottom: 30px; margin-left: auto; margin-right: auto;
            width: 500px" runat="server" id="divZong">
            <div class="mediumBrownBox">
                <div class="mediumBrownBoxTop">
                    <%-- Credit Card Holder Information --%>
                    <div style="float: left; margin-top: 9px; margin-left: 4px;">
                        <img width="412px" height="28px" src="/Images/or_pay_via_your_mobile_phone.png" />
                    </div>
                </div>
                <div class="clearBoth">
                </div>
                <div class="mediumBrownBoxMiddle">
                    <div class="smallGreenBox">
                        <div class="smallGreenBoxTop"></div>
                        <div class="smallGreenBoxMiddle">
                            <asp:ImageButton ID="btnZong" ImageUrl="/Images/zong_button_normal.png" runat="server"
                                Width="217px" Height="76px" OnClick="btnZong_Click" onmouseover="this.src='/Images/zong_button_rollover.png';"
                                onmouseout="this.src='/Images/zong_button_normal.png';" />
                        </div>
                        <div class="smallGreenBoxBottom"></div>
                    </div>
                </div>
                <div class="clearBoth">
                </div>
                <div class="mediumBrownBoxBottom">
                </div>
            </div>
        </div>
        <div class="brownBox" runat="server" id="divGambit">
            <div class="brownBoxTop">
                <div style="float: left;">
                    <img width="424px" height="41px" src="/Images/or_select_one_of_these_offers_below.png" />
                </div>
            </div>
            <div class="clearBoth">
                <iframe id="frameGambit" frameborder="0" width="685" height="1750" allowtransparency="true"
                    runat="server"></iframe>
            </div>
        </div>
    </div>
</asp:Content>
