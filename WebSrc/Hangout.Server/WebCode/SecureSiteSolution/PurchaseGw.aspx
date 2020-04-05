<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PurchaseGw.aspx.cs" Inherits="purchasegw"  EnableViewState="false"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
 <script type="text/javascript">
    function fncSubmit() 
    {  
        var theForm = document.getElementById("form1");
        var sValue = document.getElementById("sValue").value;
        theForm.action = "purchase.aspx?s=" + sValue;
        theForm.submit();
    }
    
</script>
</head>
<body onload="fncSubmit();" >  
    <form id="form1" method="post" action="purchase.aspx" >
        <input type="hidden" runat="server"  id="pValueId" value="" />
        <input type="hidden" runat="server"  id="sValue" value="" />
    </form>
</body>
</html>
