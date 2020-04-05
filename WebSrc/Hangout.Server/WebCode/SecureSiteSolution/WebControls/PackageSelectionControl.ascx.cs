using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

public partial class WebControls_PackageSelectionControl : System.Web.UI.UserControl
{
    private string mPaymentType = "";
    private string mGroupPackage = "";
    private string mSelectedOffer = "";
    private string mSelectedIndex = "";
    private string mSelectedUrl = "";
    private string mDefaultSelectedOfferId = "";

    
    public string PaymentType
    {
        set { mPaymentType = value; }
    }   

    public string GroupPackage
    {
        set { mGroupPackage = value; }
    }

    public string SelectedItem
    {
        get  {return mSelectedOffer; }
    }

    public string SelectedIndex
    {
        get { return mSelectedIndex; }
    }

    public string SelectedUrl
    {
        get { return mSelectedUrl; }
    }
               
    protected void Page_Load(object sender, EventArgs e)
    {
        mSelectedOffer = "";

        if (Page.IsPostBack)
        {
            RadioButton rbSelected = (RadioButton)FindRecursiveCheckedRadioButtonControl(rptPackage);
            if (rbSelected != null)
            {
                mSelectedOffer = GetSelectedHiddenFieldValue(rbSelected, "hSelect");
                mSelectedIndex = GetSelectedHiddenFieldValue(rbSelected, "hIndex");
                mSelectedUrl = GetSelectedUrl((ArrayList)ViewState["urlArrayList"], mSelectedIndex);
            }
        }

        //imgType.AlternateText = mPaymentType;
        //imgType.ImageUrl = "";

        if (mPaymentType == "CASH")
        {
            //packageMsg1.Text = PaymentResources.GetStringFromResourceFile("resGetCashLine1");  //"GET CASH";
            //packageMsg2.Text = PaymentResources.GetStringFromResourceFile("resGetCashLine2");  //"Get Cash to buy special items!";
        }
        else
        {
            //packageMsg1.Text = PaymentResources.GetStringFromResourceFile("resGetCoinLine1");  //"GET COIN";
            //packageMsg2.Text = PaymentResources.GetStringFromResourceFile("resGetCoinLine2");  //"Need more coins? Buy some now!";
        }
    }

    private string GetSelectedHiddenFieldValue(RadioButton rbSelected, string fieldName)
    {
        string selectedValue = "";

        HiddenField field = (HiddenField)rbSelected.Parent.FindControl(fieldName);

        if (field != null)
        {
            selectedValue = field.Value;
        }

        return selectedValue;
    }

    private string GetSelectedUrl(ArrayList urlArrayList, string selectedIndex)
    { 
        string selectedUrl = "";

        if (urlArrayList.Count > 0)
        {
            foreach (KeyValuePair<int, string> kvp in urlArrayList)
            {
                if (kvp.Key.ToString() == selectedIndex)
                {
                    selectedUrl = kvp.Value;
                    break;
                }
            }
        }

        return selectedUrl;
    }

    protected void rptPackage_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        string packageName = "rptPackage.*" +  mGroupPackage;
        string script = "SetUniqueRadioButton('" + packageName + "', this)";
        RadioButton rb = (RadioButton)e.Item.FindControl("rbSelect");
        rb.GroupName = mGroupPackage;
        rb.Attributes.Add("onclick", script);

        if  (String.IsNullOrEmpty(mDefaultSelectedOfferId))
        {
            if (FindBestValueImage(e.Item.Controls))
            {
                rb.Checked = true;
            }
        }
        else
        {
            if (FindOfferId(mDefaultSelectedOfferId, e.Item.Controls))
            {
                rb.Checked = true;
            }
        }
    }

    public void PopulatePaymentOffersRadioButtons(XmlDocument xmlDoc, string defaultSelectedOfferId)
    {
        mDefaultSelectedOfferId = defaultSelectedOfferId;
        XmlNodeList nodes = xmlDoc.SelectNodes("/Response/offers/Offer");

        DataTable dtOffers = ConvertXmlNodeListToDataTable("Offers", nodes);

        if (dtOffers.Rows.Count > 0)
        {
            DataView viewOffers = dtOffers.DefaultView;
            viewOffers.Sort = "sortValue desc, sortValueMoney desc";

            rptPackage.DataSource = viewOffers;
            rptPackage.DataBind();

        }
        else
        {                       //No Offers available
            headerMsg.Text = PaymentResources.GetStringFromResourceFile("resDefaultOffers");
            headerMsg.Visible = true;
        }
    }


    private RadioButton FindRecursiveCheckedRadioButtonControl(Control control)
    {
        RadioButton rbChecked = null;

        foreach (Control ctrl in control.Controls)
        {
            if (ctrl is RadioButton)
            {
                RadioButton rb = (RadioButton)ctrl;
                if (rb.Checked)
                {
                    rbChecked = rb;
                    break;
                }
            }
            else
            {
                if (ctrl.Controls.Count > 0)
                {
                    rbChecked = FindRecursiveCheckedRadioButtonControl(ctrl);
                    if (rbChecked != null)
                    {
                        break;
                    }
                }
            }
        }
        return rbChecked;
    }

    private bool FindOfferId(string offerId, ControlCollection controls)
    {
        bool offerFound = false;

        foreach (Control ctrl in controls)
        {
            if (ctrl is HiddenField)
            {
                HiddenField hidden = (HiddenField)ctrl;
                if (hidden.Value == offerId)
                {
                    offerFound = true;
                    break;
                }
            }
        }
        return offerFound;
    }


    private bool FindBestValueImage(ControlCollection controls)
    {
        bool bestValue = false;

        foreach (Control ctrl in controls)
        {
            if (ctrl is Image)
            {
                Image img = (Image)ctrl;
                if (img.ID == "imgBestValue")
                {
                    if (img.Visible == true)
                    {
                        bestValue = true;
                        break;
                    }
                }
            }
        }
        return bestValue;
    }


    protected bool ShowBestValueImage(string showImage)
    {
        bool showImageBestValue = false;
        if (showImage.ToLower().Trim() == "true")
        {
            showImageBestValue = true;
        }
        return showImageBestValue;
    }

      
    private DataTable ConvertXmlNodeListToDataTable(string tableName, XmlNodeList nodeList)
    {
        ArrayList urlArrayList = new ArrayList();
        DataTable dt = new DataTable(tableName);

        if ((nodeList != null) && (nodeList.Count > 0))
        {

            foreach (XmlAttribute attribute in nodeList[0].Attributes)
            {
                DataColumn dc = new DataColumn(attribute.Name, System.Type.GetType("System.String"));
                dt.Columns.Add(dc);
            }

            DataColumn dcNew = new DataColumn("index", System.Type.GetType("System.UInt16"));
            dt.Columns.Add(dcNew);

            dcNew = new DataColumn("sortValue", System.Type.GetType("System.Decimal"));
            dt.Columns.Add(dcNew);

            dcNew = new DataColumn("sortValueMoney", System.Type.GetType("System.Decimal"));
            dt.Columns.Add(dcNew);

            dcNew = new DataColumn("bestValue", System.Type.GetType("System.String"));
            dt.Columns.Add(dcNew);

            dcNew = new DataColumn("imageUrl", System.Type.GetType("System.String"));
            dt.Columns.Add(dcNew);

            dcNew = new DataColumn("imageAltText", System.Type.GetType("System.String"));
            dt.Columns.Add(dcNew);

            decimal bestValue = FindBestValue(nodeList);

            for (int rowCount = 0; rowCount < nodeList.Count; rowCount++)
            {
                DataRow dr = dt.NewRow();
                XmlNode node = nodeList.Item(rowCount);
                for (int colCount = 0; colCount < dt.Columns.Count - 6; colCount++)
                {
                    dr[colCount] = node.Attributes[colCount].InnerText;
                }

                dr["index"] = rowCount;

                if (node.SelectSingleNode("iframeURL") != null)
                {
                    string urlValue = node.SelectSingleNode("iframeURL").InnerText;
                    KeyValuePair<int, string> kvp = new KeyValuePair<int, string>(rowCount, urlValue);
                    urlArrayList.Add(kvp);
                }

                decimal currentOfferValue = CalculateOffer(dr["vMoney"].ToString(), dr["usd"].ToString());
                dr["SortValue"] = currentOfferValue;
                dr["SortValueMoney"] = ConvertToDecimal(dr["vMoney"].ToString());
                if (currentOfferValue == bestValue)
                {
                    bestValue = bestValue + 1;   // only one best value
                    dr["bestValue"] = true;
                }
                else
                {
                    dr["bestValue"] = false;
                }

                if (mPaymentType == "COIN")
                {
                    dr["imageUrl"] = "";
                    dr["imageAltText"] = "COIN";
                }
                else
                {
					dr["imageUrl"] = "/Images/cash_icon.png";
                    dr["imageAltText"] = "CASH";
                }

                dt.Rows.Add(dr);
            }
        }
        ViewState["urlArrayList"] = urlArrayList;
        return dt;
    }

    private decimal FindBestValue(XmlNodeList nodeList)
    {
        decimal bestValue = 0;

        foreach (XmlNode node in nodeList)
        {
            string vMoney = node.Attributes["vMoney"].InnerText;
            string usd = node.Attributes["usd"].InnerText;
            decimal calcValue = CalculateOffer(vMoney, usd);

            if (calcValue > bestValue)
            {
                bestValue = calcValue;
            }
        }
        return bestValue;
    }

    private decimal CalculateOffer(string vMoney, string usd)
    {
        decimal value = 0;

        decimal tempMoney = 0;
        decimal tempUsd = 0;
        if (decimal.TryParse(vMoney, out tempMoney))
        {
            if (decimal.TryParse(usd, out tempUsd))
            {
                if ((tempMoney > 0) && (tempUsd > 0))
                {
                    value = tempMoney /  tempUsd;
                }
            }
        }

        return value;
    }


    private decimal ConvertToDecimal(string value)
    {        
        decimal decimalValue = 0;
        decimal tempValue = 0;

        if (decimal.TryParse(value, out tempValue))
        {
            decimalValue = tempValue;
        }

        return decimalValue;
    }
}

