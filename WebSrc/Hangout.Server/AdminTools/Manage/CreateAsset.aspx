<%@ Page Language="C#" MasterPageFile="~/AdminMasterPage.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CreateAsset.aspx.cs" Inherits="Manage_UploadNewAsset" Title="Upload New Asset"  %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">
    <link href="/CSS/UploadNewAsset.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="main" Runat="Server">
    <div class="centeredAndSlim"><h1><blink>Create Asset!</blink></h1></div>
    <div class="centeredAndBlocking">
        <a href="ViewAssets.aspx">View All Assets</a><br /><br />
        <div id="AddAsset">
            
            <label><span class="labelText">Asset Type:</span><asp:DropDownList ID="AssetTypeDropDown" runat="server" AutoPostBack="true"></asp:DropDownList></label>

            <!-- All Assets -->
            <br /><label><span class="labelText">Asset SubType:</span><asp:DropDownList ID="AssetSubTypeDropDown" Enabled="true" runat="server" Width="150px" AutoPostBack="true" ></asp:DropDownList></label>
            <br /><label><span class="labelText">Asset Name:</span><asp:TextBox ID="AssetName" Enabled="true" runat="server" Width="150px" ></asp:TextBox></label>

            <!-- Texture, Mesh -->
            <br /><label><span class="labelText">Asset Upload:</span><asp:FileUpload ID="AssetFileData" runat="server" /></label>
            <br /><label><span class="labelText">AssetData(XML):</span><asp:TextBox ID="AssetData" Wrap="true"  TextMode="MultiLine" runat="server" Enabled="true" Width="700px" Height="300px" ></asp:TextBox></label>
            <br /><asp:Button ID="SubmitAsset" runat="server" Text="Submit Asset" OnClick="SubmitAssetButton_Clicked" />
            
            <br /><br />
            <asp:TextBox ID="ServerResponse" Wrap="false"  TextMode="MultiLine" runat="server" Enabled="true" Width="700px" Height="300px" ></asp:TextBox>
        </div>
    </div>
</asp:Content>

