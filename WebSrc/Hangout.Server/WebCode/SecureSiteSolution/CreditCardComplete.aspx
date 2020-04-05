<%@ Page Language="C#" MasterPageFile="~/Payments.master" AutoEventWireup="true"
    CodeFile="CreditCardComplete.aspx.cs" Inherits="CreditCardComplete" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headerPlaceHolderMain" runat="Server">
    <div class="header"></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentPlaceHolderMain" runat="Server">
    <div class="container">
        <div class="brownBox">
            <div class="brownBoxTop">
                <div style="float: left;">
                    <asp:Literal ID="lblComplete" runat="server"><img width="266px" height="36px" src="/Images/transaction_complete.png" /></asp:Literal>
                </div>
            </div>
            <div class="clearBoth">
            </div>
            <div class="brownBoxMiddle">
                <div style="margin-right: auto; text-align: center; width: 415px;">
                <div style="width: 580px; margin-right: auto; margin-left: auto;">
                    <%--<div style="float: left; margin-left: 20px; width: 184px; height: 157px;">
                        <asp:ImageButton ID="ImageButton1" runat="Server" Style="border: 0; margin-top: 60px;"
                            Width="182px" Height="36px" ImageUrl="/Images/return_to_fashion_city_btn.png"
                            onmouseover="this.src='/Images/return_to_fashion_city_rollover.png';" onmouseout="this.src='/Images/return_to_fashion_city_btn.png';" />
                    </div>--%>
                        <div style="float: left; margin-left: 100px;margin-right: 50px; margin-top: 50px;width: 200px;font-weight: bold; text-align: left;">
                           <div class="formLabel"><asp:Literal ID="lThankYou" runat="server" /> </div>
                            <div style="margin-right: 50px; margin-top: 30px; float: right;">
                                <asp:ImageButton runat="server" ImageUrl="/Images/print_reciept.png" Text="Print" OnClientClick="window.print();" ID="tbPrint" />
                            </div>
                        </div>
                    <div style="float: right; margin-right: 15px; width: 194px;">
                        <img style="border: 0; margin-top: 10px;" width="192px" height="157px" src="/Images/emo_girl_thanks.png" />
                    </div>
                </div>
                <div class="clearBoth">
                </div>
            </div>
            <div class="clearBoth">
            </div>
            <div class="brownBoxMiddle">
                <div style="margin-left: 100px; font-weight: bold;">
                    <div class="formItem">
                         <div class="formLabel"><asp:label ID="lblPackage" runat="server" /> <asp:Literal  ID="lPackage" runat="server" /></div>
                         <br />
                         <div class="formLabel"><asp:label runat="server" ID="lblName"></asp:label> <asp:Literal ID="lName" runat="server"  /></div>
                         <br />
                         <div class="formLabel"><asp:label runat="server" ID="lblCardType"></asp:label> <asp:Literal ID="lCardType" runat="server"  /></div>
                         <br />
                         <div class="formLabel"><asp:label runat="server" ID="lblCreditCard"></asp:label> <asp:Literal ID="lCreditCard" runat="server" /></div>
                        <br /> 
                        <div class="formLabel"><asp:label ID="lblExpireDate" runat="server" /> <asp:Literal ID="lExpireDate" runat="server" /></div>
                        <br /> 
                        <div class="formLabel"><asp:label ID="lblEmail" runat="server" /> <asp:Literal ID="lEmail" runat="server"  /></div>
                        <br />
                        <div class="formLabel"><asp:label runat="server" ID="lblTransaction"></asp:label> <asp:Literal ID="lTransaction" runat="server" /></div>
                        <br />
                        <span style="font-size:12px"> <asp:Literal ID="lSupport" runat="server"  /> <asp:HyperLink ID="hlSupport" runat="server" /></span>  
                        <div style="margin-bottom:20px"></div>
                </div>
            </div>
            <div class="clearBoth">
            </div>
            <div class="brownBoxBottom">
            </div>
        </div>
       </div>
      </div>
        <table width="135" border="0" cellpadding="2" cellspacing="0" title="Click to Verify - This site chose VeriSign SSL for secure e-commerce and confidential communications.">
            <tr>
                <td width="135" align="center" valign="top"><script type="text/javascript" src="https://seal.verisign.com/getseal?host_name=secure.hangout.net&amp;size=M&amp;use_flash=YES&amp;use_transparent=YES&amp;lang=en"></script><br />
                <a href="http://www.verisign.com/ssl-certificate/" target="_blank"  style="color:#000000; text-decoration:none; font:bold 7px verdana,sans-serif; letter-spacing:.5px; text-align:center; margin:0px; padding:0px;">ABOUT SSL CERTIFICATES</a></td>
            </tr>
        </table>
    </div>
</asp:Content>
