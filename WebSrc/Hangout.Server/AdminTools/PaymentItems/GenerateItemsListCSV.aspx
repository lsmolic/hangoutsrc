<%@ Page Language="C#" MasterPageFile="~/AdminMasterPage.master" AutoEventWireup="true" CodeFile="GenerateItemsListCSV.aspx.cs" Inherits="PaymentItems_GenerateItemsListCSV" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">
    <asp:Button ID="GenerateFileButton" runat="Server" 
        onclick="GenerateFileButton_Click" Text="Generate CSV File" />
</asp:Content>

