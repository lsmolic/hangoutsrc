using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Payments : System.Web.UI.MasterPage
{
    private bool headerNotVisible = false;

    protected void Page_Init(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (headerNotVisible)
        {
            headerPlaceHolderMain.Visible = false;
        }
        else
        {
            headerPlaceHolderMain.Visible = true;
        }
    }

    public bool HeaderNotVisible
    {
        get {return headerNotVisible; }
        set {headerNotVisible = value; }
    }
   
}
