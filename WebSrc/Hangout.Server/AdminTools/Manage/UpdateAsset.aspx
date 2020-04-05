<%@ Page Language="C#" MasterPageFile="~/AdminMasterPage.master" AutoEventWireup="true" CodeFile="UpdateAsset.aspx.cs" ValidateRequest="false" Inherits="Manage_UpdateAsset" Title="UpdateAsset" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">
    
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="main" Runat="Server">
    <div class="centeredAndSlim"><h1><blink>Update Asset!</blink></h1></div>
    <div class="centeredAndBlocking">
        <a href="ViewAssets.aspx">View All Assets</a><br /><br />
        <div id="AddAsset">
            
            <label><span class="labelText">Asset Type:</span><asp:DropDownList ID="AssetTypeDropDown" runat="server" ></asp:DropDownList></label>

            
            <!-- All Assets -->
            <br /><label><span class="labelText">Asset SubType:</span><asp:DropDownList ID="AssetSubTypeDropDown" Enabled="true" runat="server" Width="150px" AutoPostBack="true" ></asp:DropDownList></label>
            <br /><label><span class="labelText">Asset Id:</span><asp:TextBox ID="AssetIdRangeStartId" Enabled="false" runat="server" Width="150px" ></asp:TextBox></label>
            
            <br /><label><span class="labelText">Asset Name:</span><asp:TextBox ID="AssetName" Enabled="true" runat="server" Width="150px" ></asp:TextBox></label>
            
            <br /><label><span class="labelText">File Name:</span><asp:TextBox ID="CurrentFileName" Enabled="false" runat="server" Width="150px" ></asp:TextBox></label>
            <br /><label><span class="labelText">Asset Upload:</span><asp:FileUpload ID="AssetFileData" runat="server" />(for changing asset file only)</label>
            <br /><label><span class="labelText">AssetData(XML):</span><asp:TextBox ID="AssetData" Wrap="false" CssClass="scrollingTextBox" TextMode="MultiLine" runat="server" Enabled="true" Width="700px" Height="300px" ></asp:TextBox></label>
            <br /><asp:Button ID="UpdateAsset" runat="server" Text="UpdateAsset" OnClick="UpdateAssetButton_Clicked" />
            
            <br /><br />
            <asp:TextBox ID="ServerResponse" Wrap="false"  TextMode="MultiLine" runat="server" Enabled="true" Width="700px" Height="300px" ></asp:TextBox>
        </div>
    </div>
</asp:Content>

