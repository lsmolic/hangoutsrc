<%@ Page Language="C#" MasterPageFile="~/AdminMasterPage.master" AutoEventWireup="true" CodeFile="RemoveUser.aspx.cs" Inherits="Manage_RemoveUser" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">
    <div class="centeredAndSlim"><h1><blink>Remove Users (USE WITH CAUTION!!!)</blink></h1></div>
    <div class="centeredAndBlocking">
        <div id="RemoveUser">
            <table style="border:0;">
                <tr>
                    <td align="right" style="text-align:right; width:200px;">HangoutAccountId:</td>
                    <td align="left"  style="text-align:left; width:400px;"><asp:TextBox ID="AccountId" runat="Server" Width="250px"></asp:TextBox></td> 
                </tr>
                <tr>
                    <td align="right" style="text-align:right; width:200px;">List of HangoutAccountId:</td>
                    <td align="left"  style="text-align:left; width:400px;"><asp:TextBox ID="AccountIdCSVList" runat="Server" Width="250px"></asp:TextBox> (1231,234242,14141)</td> 
                </tr>
                <tr>
                    <td align="right" style="text-align:right; width:200px;">FbAccountId:</td>
                    <td align="left"  style="text-align:left; width:400px;"><asp:TextBox ID="FbAccountId" runat="Server" Width="250px"></asp:TextBox></td> 
                </tr>
                <tr>
                    <td align="right" style="text-align:right; width:200px;">NickName:</td>
                    <td align="left"  style="text-align:left; width:400px;"><asp:TextBox ID="NickName" runat="Server" Width="250px"></asp:TextBox></td> 
                </tr>
                <tr>
                    <td align="right" style="text-align:right; width:200px;"></td>
                    <td align="left"  style="text-align:left; width:400px;">
                        <asp:Button ID="SubmitButton" Text="Delete Matching Users" runat="Server" 
                            onclick="SubmitButton_Click" /></td>  
                </tr>
            </table> 
            <br /><br />
            <asp:TextBox ID="ServerResponse" Wrap="false"  TextMode="MultiLine" runat="server" Enabled="true" Width="700px" Height="300px" ></asp:TextBox>
        </div>
    </div>
</asp:Content>

