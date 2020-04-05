<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Test.aspx.cs" Inherits="Test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin-left: 120px">
        <br />
        <br/>
        <asp:Label ID="Label2" runat="server" Text="Hangout User Id:" Width="150px" ></asp:Label><asp:TextBox id="hUid" runat="server" tabindex="1" width="200px" Height="22px" />
        <br />
        <br/> 
        <div style="margin-left: 200px">
            <asp:Button Text="Submit" runat="server" ID="btnSubmit" 
                CausesValidation="false"  UseSubmitBehavior="false"  style="height: 26px" 
                onclick="btnSubmit_Click"  />
       </div>
    </div>
    </form>
</body>
</html>
