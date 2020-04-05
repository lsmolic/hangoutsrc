<%@ Page Language="C#" MasterPageFile="~/AdminMasterPage.master" AutoEventWireup="true" CodeFile="UploadMetricsLogFile.aspx.cs" Inherits="Metrics_UploadMetricLogFile" Title="Upload Metrics Log Files Manually" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">
    <h2>UPLOAD METRICS LOG FILE MANUALLY</h2>
    <h5>FORMAT: Category|Event|SubEvent|Data|AccountId|TimeStamp||Category|Event|SubEvent|Data|AccountId|TimeStamp (double delimits entries)</h5>
    <asp:FileUpload ID="LogFile" runat="Server" />
    <asp:Button ID="SubmitButton" Text="Submit Logs" runat="Server" OnClick="SubmitButton_onClick" />
</asp:Content>

