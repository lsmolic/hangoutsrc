<%@ Page Language="C#" MasterPageFile="~/Payments.master" AutoEventWireup="true" CodeFile="Zong.aspx.cs" Inherits="Zong" Title="Zong Payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" Runat="Server">
     <iframe id="frameZong" height="500px" width="700px" scrolling="auto" runat="server"></iframe>
       <div>
        <table width="135" border="0" cellpadding="2" cellspacing="0" title="Click to Verify - This site chose VeriSign SSL for secure e-commerce and confidential communications.">
            <tr>
                <td width="135" align="center" valign="top"><script type="text/javascript" src="https://seal.verisign.com/getseal?host_name=secure.hangout.net&amp;size=M&amp;use_flash=YES&amp;use_transparent=YES&amp;lang=en"></script><br />
                <a href="http://www.verisign.com/ssl-certificate/" target="_blank"  style="color:#000000; text-decoration:none; font:bold 7px verdana,sans-serif; letter-spacing:.5px; text-align:center; margin:0px; padding:0px;">ABOUT SSL CERTIFICATES</a></td>
            </tr>
        </table>
       </div>
</asp:Content>

