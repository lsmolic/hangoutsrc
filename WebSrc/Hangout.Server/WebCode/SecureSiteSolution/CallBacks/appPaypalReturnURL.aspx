<%@ Page Language="C#" AutoEventWireup="true" CodeFile="appPaypalReturnURL.aspx.cs" Inherits="paypal_appPaypalReturnURL" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PaypalReturnURL</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="divResponse" runat="server"><asp:Literal ID="lblPayPalReturnUrl" runat="server"></asp:Literal></div>
    </div>
    </form>
</body>
</html>
