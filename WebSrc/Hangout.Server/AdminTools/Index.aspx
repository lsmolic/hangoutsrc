<%@ Page Language="C#" MasterPageFile="~/AdminMasterPage.master" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="IndexPage" Title="Site Index" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="main" Runat="Server">
    <div class="centeredAndSlim"><h1><blink>ADMIN PAGES!</blink></h1></div>
    <div class="centeredAndBlocking">
        <h4>Metrics</h4>
        <ul>
            <li><a href="/Metrics/UploadMetricsLogFile.aspx" > Upload Metrics Log File Manually </a></li>
        </ul>
        <h4>Assets / Items</h4>
        <ul>
            <li><a href="/Manage/ViewAssets.aspx" > View All Assets </a></li>
            <li><a href="/Manage/CreateAsset.aspx" > Create Asset </a></li>
            <li><a href="/PaymentItems/GenerateItemsListCSV.aspx" > Generate ItemsList CSV </a></li>
        </ul>
    </div>   
</asp:Content>

