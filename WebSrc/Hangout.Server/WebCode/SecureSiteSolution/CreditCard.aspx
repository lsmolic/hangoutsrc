<%@ Page Language="C#" MasterPageFile="~/Payments.master" AutoEventWireup="true"
    CodeFile="CreditCard.aspx.cs" Inherits="CreditCard" Title="Credit Card" %>

<asp:Content ID="headstuff" ContentPlaceHolderID="head" runat="Server">
    <link href="/CSS/creditcard.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" language="javascript">
        var flag=0; 
        function imageclicked() 
        { 
            if (flag==0) 
            { 
                flag=1; 
            } 
            else
            { 
                return false; 
            } 
        }
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="headerPlaceHolderMain" runat="Server">
    <div class="header">
    </div>
    <%--<div style="width: 120px;">
        <asp:ImageButton ID="ImageButton1" runat="Server" CssClass="backButton" ImageUrl="/Images/back_button.png"
             onmouseover="this.src='/Images/back_button_rollover.png';" onmouseout="this.src='/Images/back_button.png';" OnClick="backButton_click" />
    </div>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" runat="Server">
    <div class="container">
        <div class="mediumErrorBox" id="CreditCardErrorsDiv" runat="Server" visible="false"
            style="margin-top: 20px; margin-bottom: 20px;">
            <div class="mediumErrorBoxTop">
            </div>
            <div class="mediumErrorBoxMiddle">
                <div style="margin-left: 10px;">
                    <asp:ValidationSummary runat="server" ID="validSummary" DisplayMode="BulletList"
                        CssClass="validationsummary" HeaderText="Errors:" ForeColor="White" Style="padding-bottom: 10px;" />
                </div>
            </div>
            <div class="mediumErrorBoxBottom">
            </div>
        </div>
        <div class="mediumGreenBox" style="margin-top: 20px; margin-bottom: 20px;">
            <div class="mediumGreenBoxTop">
            </div>
            <div class="mediumGreenBoxMiddle">
                <div style="margin-left: 10px; font-weight">
                    <asp:Literal runat="server" ID="lPackage" />
                </div>
            </div>
            <div class="mediumGreenBoxBottom">
            </div>
        </div>
        <div class="mediumBrownBox">
            <div class="mediumBrownBoxTop">
                <%-- Credit Card Holder Information --%>
                <div style="float: left; margin-top: 9px; margin-left: 4px;">
                    <img width="412px" height="28px" src="/Images/enter_credit_card_holder_info.png" />
                </div>
            </div>
            <div class="clearBoth">
            </div>
            <div class="mediumBrownBoxMiddle">
                <div>
                    <div class="formItem">
                        <div class="formLabel">
                            First Name</div>
                        <asp:TextBox CssClass="formField" ID="tbFirstName" runat="server" TabIndex="1" Width="200px" />
                        <asp:RequiredFieldValidator ID="reqFirstName" ControlToValidate="tbFirstName" EnableClientScript="false"
                            OnPreRender="RequiredFieldPreRender" runat="server" ErrorMessage="Please Enter First Name"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            Last Name</div>
                        <asp:TextBox CssClass="formField" ID="tbLastName" runat="server" TabIndex="3" Width="200px" />
                        <asp:RequiredFieldValidator ID="reqLastName" ControlToValidate="tbLastName" EnableClientScript="false"
                            OnPreRender="RequiredFieldPreRender" runat="server" ErrorMessage="Please Enter Last Name"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            Address</div>
                        <asp:TextBox CssClass="formField" ID="tbAddress" runat="server" TabIndex="4" Width="200px" />
                        <asp:RequiredFieldValidator ID="reqAddressName" ControlToValidate="tbAddress" EnableClientScript="false"
                            OnPreRender="RequiredFieldPreRender" runat="server" ErrorMessage="Please Enter Address"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            City</div>
                        <asp:TextBox CssClass="formField" ID="tbCity" runat="server" TabIndex="5" Width="200px" />
                        <asp:RequiredFieldValidator ID="reqCityName" ControlToValidate="tbCity" EnableClientScript="false"
                            OnPreRender="RequiredFieldPreRender" runat="server" ErrorMessage="Please Enter City"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            State</div>
                        <asp:DropDownList CssClass="formField" Height="22px" ID="ddState" runat="server"
                            TabIndex="6" Width="200px" />
                        <asp:RequiredFieldValidator ID="reqStateName" ControlToValidate="ddState" EnableClientScript="false"
                            OnPreRender="RequiredFieldPreRender" runat="server" ErrorMessage="Please Enter State"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            Zip Code</div>
                        <asp:TextBox CssClass="formField" ID="tbZipCode" runat="server" TabIndex="7" Width="200px" />
                        <asp:RequiredFieldValidator ID="reqZipCode" ControlToValidate="tbZipCode" EnableClientScript="false"
                            OnPreRender="RequiredFieldPreRender" runat="server" ErrorMessage="Please Enter Zip Code"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="validZipCode" ControlToValidate="tbZipCode" EnableClientScript="false"
                            runat="server" OnServerValidate="validateZipCode" Display="Dynamic">*</asp:CustomValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            Country</div>
                        <asp:DropDownList CssClass="formField" Height="22px" ID="ddCountry" runat="server"
                            TabIndex="8" Width="200px" />
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            Email</div>
                        <asp:TextBox CssClass="formField" ID="tbEmail" runat="server" TabIndex="9" Width="200px" />
                        <asp:RequiredFieldValidator ID="reqEmail" ControlToValidate="tbEmail" ErrorMessage="Please Enter Email Address"
                            EnableClientScript="false" OnPreRender="RequiredFieldPreRender" runat="server"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="validEmail" ControlToValidate="tbEmail" EnableClientScript="false"
                            runat="server" OnServerValidate="validateEmail" Display="Dynamic">*</asp:CustomValidator>
                    </div>
                </div>
                <%-- Credit Card Information --%>
                <div style="">
                    <div style="">
                        <img width="424px" height="78px" src="/Images/enter_credit_card_info.png" />
                    </div>
                </div>
                <div style="margin-left: 0px">
                    <div class="formItem">
                        <div class="formLabel">
                            Card Number</div>
                        <asp:TextBox CssClass="formField" ID="tbCreditCardNumber" runat="server" TabIndex="10"
                            Width="200px" />
                        <asp:RequiredFieldValidator ID="reqCreditCard" ControlToValidate="tbCreditCardNumber"
                            EnableClientScript="false" runat="server" ErrorMessage="Please Enter Credit Card Number"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="validCreditCard" ControlToValidate="tbCreditCardNumber"
                            EnableClientScript="false" runat="server" OnServerValidate="validateCreditCard"
                            Display="Dynamic" ErrorMessage="*"></asp:CustomValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            Card Type</div>
                        <asp:DropDownList CssClass="formField" Height="22px" ID="ddCardType" runat="server"
                            TabIndex="11" Width="200px" />
                        <asp:CustomValidator ID="validCardType" ControlToValidate="ddCardType" EnableClientScript="false"
                            runat="server" OnServerValidate="validateCardType" Display="Dynamic">*</asp:CustomValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            Security Code</div>
                        <asp:TextBox CssClass="formField" ID="tbSecurityCode" runat="server" TabIndex="12"
                            Width="200px" />
                        <asp:RequiredFieldValidator ID="reqSecurityCode" ErrorMessage="Please Enter Security Code"
                            ControlToValidate="tbSecurityCode" EnableClientScript="false" runat="server"
                            Display="Dynamic">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="validSecurityCode" ControlToValidate="tbSecurityCode" EnableClientScript="false"
                            runat="server" OnServerValidate="validateSecurityCode" Display="Dynamic">*</asp:CustomValidator>
                    </div>
                    <div class="formItem">
                        <div class="formLabel">
                            Expiration Date</div>
                        <asp:DropDownList CssClass="formField" Height="22px" ID="ddExpireMonth" runat="server"
                            TabIndex="13" Width="100px" />
                        <asp:DropDownList CssClass="formField" Height="22px" ID="ddExpireYear" runat="server"
                            TabIndex="13" Width="100px" />
                    </div>
                </div>
                <div class="clearBoth">
                </div>
                <div style="margin-top: 0px;">
                    <div class="" style="float: left; width: 110px; margin-left: 25px;">
                        <asp:ImageButton Style="height: 26px; margin-left: 15px; margin-top: 11px;" ImageUrl="/Images/cancel.png"
                            onmouseover="this.src='/Images/cancel_rollover.png';" onmouseout="this.src='/Images/cancel.png';"
                            runat="server" ID="btnCancel" CausesValidation="false" OnClick="btnCancel_Click" />
                    </div>
                    <div class="" style="float: right; width: 110px; margin-right: 25px;">
                        <asp:ImageButton ImageUrl="/Images/submit.png" runat="server" ID="btnSubmit" CausesValidation="true"
                            OnClientClick="imageclicked();" UseSubmitBehavior="False" OnClick="submit_Click"
                            onmouseover="this.src='/Images/submit_rollover.png';" onmouseout="this.src='/Images/submit.png';"
                            Style="height: 26px; margin-left: 8px; margin-top: 11px;" />
                    </div>
                    <div class="clearBoth">
                    </div>
                </div>
            </div>
            <div class="clearBoth">
            </div>
            <div class="mediumBrownBoxBottom">
            </div>
            <div>
            <div class="" style="float: left; width: 265px; margin-left: 15px; margin-bottom: 15px;font-size:10px">
                            Hangout's Fashion City does not store any of your credit card information. Your purchase is encrypted using a VeriSign Secure Socket Layer (SSL) Certificate.
               </div>
                <div class="" style="float: right; width: 110px; margin-right: 25px;">

                    <table width="135" border="0" cellpadding="2" cellspacing="0" title="Click to Verify - This site chose VeriSign SSL for secure e-commerce and confidential communications.">
                        <tr>
                            <td width="135" align="center" valign="top">

                                <script type="text/javascript" src="https://seal.verisign.com/getseal?host_name=secure.hangout.net&amp;size=M&amp;use_flash=YES&amp;use_transparent=YES&amp;lang=en"></script>

                                <br />
                                <a href="http://www.verisign.com/ssl-certificate/" target="_blank" style="color: #000000;
                                    text-decoration: none; font: bold 7px verdana,sans-serif; letter-spacing: .5px;
                                    text-align: center; margin: 0px; padding: 0px;">ABOUT SSL CERTIFICATES</a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div id="divConfirmPopUpBackDrop" class="blackOverlay" runat="server" visible="false"></div>
        <div id="divConfirmPopUp" class="clearOverlay" runat="server" visible="false">
            <div class="greenPopUpBox" style="margin-top: 300px;">
                <div class="greenPopUpBoxTop">
                </div>
                <div class="clearBoth">
                </div>
                <div class="greenPopUpBoxMiddle" style="text-align: center;">
                    <asp:Literal runat="server" ID="lConfirmMsg" />
                    <div style="margin-top: 0px;">
                        <div class="" style="float: left; width: 110px; margin-left: 25px;">
                            <asp:ImageButton Style="height: 26px; margin-left: 15px; margin-top: 11px;" ImageUrl="/Images/cancel.png"
                                onmouseover="this.src='/Images/cancel_rollover.png';" onmouseout="this.src='/Images/cancel.png';"
                                runat="server" ID="ImageButton2" CausesValidation="false" OnClick="btnCancelConfirm_Click" />
                        </div>
                        <div class="" style="float: right; width: 110px; margin-right: 25px;">
                            <asp:ImageButton ImageUrl="/Images/submit.png" runat="server" ID="ImageButton3" CausesValidation="true"
                                OnClientClick="imageclicked();" UseSubmitBehavior="False" OnClick="submitConfirm_Click"
                                onmouseover="this.src='/Images/submit_rollover.png';" onmouseout="this.src='/Images/submit.png';"
                                Style="height: 26px; margin-left: 8px; margin-top: 11px;" />
                        </div>
                    </div>
                    <div class="clearBoth">
                    </div>
                </div>
                <div class="clearBoth">
                </div>
                <div class="greenPopUpBoxBottom">
                </div>
            </div>
        </div>
        
</asp:Content>
