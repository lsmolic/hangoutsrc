<%@ Page Language="C#" MasterPageFile="~/AdminMasterPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="ViewAssets.aspx.cs" Inherits="Manage_ViewAssets" Title="View Assets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">
<div class="centeredAndSlim"><h1><blink>View Assets!</blink></h1></div>
<div class="centeredAndBlocking">
    <a href="CreateAsset.aspx">Create Asset</a><br /><br />
    <label>
        <span class="labelText">Asset Type:</span>
        <asp:DropDownList ID="AssetTypeDropDown" runat="server" OnSelectedIndexChanged="AssetType_OnSelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    </label>
        <br />
    <label>
        <span class="labelText">Asset SubType:</span>
        <asp:DropDownList ID="AssetSubTypeDropDown" runat="server" OnSelectedIndexChanged="AssetSubType_OnSelectedIndexChanged" AutoPostBack="true"  Width="150px" ></asp:DropDownList>
    </label>
    <asp:GridView ID="AssetGridView" runat="server" BackColor="White" BorderColor="#DEDFDE"
            BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" AutoGenerateColumns="false" >
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <a href='UpdateAsset.aspx?assetId=<%#XPath("@AssetId") %>'>[Edit]</a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <a href='DeleteAsset.aspx?assetId=<%#XPath("@AssetId") %>'>[Delete]</a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:templatefield headertext="AssetId">
                <itemtemplate><%#XPath("@AssetId") %></itemtemplate>
            </asp:templatefield>
            <asp:templatefield headertext="AssetType">
                <itemtemplate><%#XPath("@AssetType")%></itemtemplate>
            </asp:templatefield>
            <asp:templatefield headertext="AssetSubType">
                <itemtemplate><%#XPath("@AssetSubType")%></itemtemplate>
            </asp:templatefield>
            <asp:templatefield headertext="AssetName">
                <itemtemplate><%#XPath("@AssetName")%></itemtemplate>
            </asp:templatefield>
            <asp:templatefield headertext="FileName">
                <itemtemplate><%#XPath("@FileName")%></itemtemplate>
            </asp:templatefield>
        </Columns>
    </asp:GridView>    
</div>

</asp:Content>

