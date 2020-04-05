<%@ Page Language="C#" MasterPageFile="~/Payments.master" AutoEventWireup="true"
    CodeFile="ZongOffers.aspx.cs" Inherits="Zong" Title="Zong Offers" %>

<%@ Register Src="webcontrols/PackageSelectionControl.ascx" TagName="PackageSelectionControl"
    TagPrefix="ps1" %>
    <asp:Content ID="headstuff" ContentPlaceHolderID="head" runat="Server">
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

<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" runat="Server">
    <div class="container">
        <div class="brownBox">
            <div class="brownBoxTop">
                <div style="float: left;">
                    <img width="424px" height="41px" src="/Images/or_select_one_of_these_offers_below.png" /></div>
            </div>
            <div class="clearBoth">
            </div>
            <div class="brownBoxMiddle">
                <div style="margin-left: 50px;">
                    <div style="text-align: left; margin-left: 10px;">
                        <h1>
                            <asp:Literal runat="server" ID="lblSelectPackage"> </asp:Literal></h1>
                    </div>
                    <div style="white-space: nowrap">
                        <div style="float: left; margin-right: 20px;">
                            <ps1:PackageSelectionControl runat="server" GroupPackage="package" PaymentType="CASH"
                                ID="psCash" />
                        </div>
                        <div visible="false" style="float: left; margin-left: 20px;">
                            <ps1:PackageSelectionControl Visible="false" runat="server" GroupPackage="package"
                                PaymentType="COIN" ID="psCoin" />
                        </div>
                    </div>
                </div>
                <div class="clearBoth"></div
                <div style="margin-top: 0px;">
                    <div class="" style="float: left; width: 110px; margin-left: 25px;">
                        <asp:ImageButton Style="height: 26px; margin-left: 15px; margin-top: 11px;" ImageUrl="/Images/cancel.png"
                            onmouseover="this.src='/Images/cancel_rollover.png';" onmouseout="this.src='/Images/cancel.png';"
                            runat="server" ID="ImageButton1" CausesValidation="false" OnClick="btnCancel_Click" />
                    </div>
                    <div class="" style="float: right; width: 110px; margin-right: 300px;">
                        <asp:ImageButton ImageUrl="/Images/submit.png" runat="server" ID="ImageButton2" CausesValidation="true"
                            OnClientClick="imageclicked();" UseSubmitBehavior="False" OnClick="btnZong_Click"
                            onmouseover="this.src='/Images/submit_rollover.png';" onmouseout="this.src='/Images/submit.png';"
                            Style="height: 26px; margin-left: 8px; margin-top: 11px;" />
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
            <div class="clearBoth">
            </div>
            <div class="brownBoxBottom">
            </div>
            <div>
                <table width="135" border="0" cellpadding="2" cellspacing="0" title="Click to Verify - This site chose VeriSign SSL for secure e-commerce and confidential communications.">
                    <tr>
                        <td width="135" align="center" valign="top"><script type="text/javascript" src="https://seal.verisign.com/getseal?host_name=secure.hangout.net&amp;size=M&amp;use_flash=YES&amp;use_transparent=YES&amp;lang=en"></script><br />
                        <a href="http://www.verisign.com/ssl-certificate/" target="_blank"  style="color:#000000; text-decoration:none; font:bold 7px verdana,sans-serif; letter-spacing:.5px; text-align:center; margin:0px; padding:0px;">ABOUT SSL CERTIFICATES</a></td>
                    </tr>
                </table>
             </div>
       </div>
</asp:Content>
