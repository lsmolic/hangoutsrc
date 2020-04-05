using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Hangout.Shared;

public partial class CreditCard : PurchaseBasePage 
{
    protected override void Page_Load(object sender, EventArgs e)
    {    
        string[] cardTypes;

        SetPageExpireNow(); 

        base.Page_Load(sender, e);
        if (!Page.IsPostBack)
        {
            OfferId = (string)Context.Items["offerId"];

            string[] offerText = GetOfferDescription(OfferId);

            lPackage.Text = String.Format(PaymentResources.GetStringFromResourceFile("resCCPackageSelected"), offerText[2], offerText[1]);

            string contextCardTypes = (string)Context.Items["creditCardTypes"];

            if (contextCardTypes != null)
            {
                cardTypes = contextCardTypes.Split(',');
            }
            else
            {
                cardTypes = (string[])ViewState["cardTypes"];
            }

            ViewState.Add("cardTypes", cardTypes);

            populateCreditCardInfoList(cardTypes);
            tbEmail.Text = PaymentItemEmailAddressFromViewState;
            lConfirmMsg.Text = String.Format(PaymentResources.GetStringFromResourceFile("resCCConfirmPurchase"), offerText[2], offerText[1]);
        }
    }

    protected void populateCreditCardInfoList(string[] cardTypes)
    {
        PaymentItemUserId = PaymentItemUserIdFromViewState;

        ddCountry.Items.Add("US");

        if (cardTypes != null)
        {
           foreach (string cardType in cardTypes)
            {
                string cardTypeTemp = cardType.Replace("_", " ");
                ddCardType.Items.Add(cardTypeTemp);
            }
        }

        for (int month = 1; month < 13; month ++)
        {
           ddExpireMonth.Items.Add(month.ToString("D2"));
        }

        for (int count = 0; count < 10; count++)
        {
            DateTime expireDate = DateTime.Now.AddYears(count);
            ddExpireYear.Items.Add(expireDate.Year.ToString());
        }

        AddStates(ddState);
    }

	protected void backButton_click(object sender, EventArgs e)
	{
        Context.Items["offerId"] = OfferIdFromViewState;
		ServerTransfer("purchase.aspx");

	}
    protected void submit_Click(object sender, EventArgs e)
    {
		if (Page.IsValid)
		{
			CreditCardErrorsDiv.Visible = false;
			SetConfirmBox();
		}
		else
		{
			CreditCardErrorsDiv.Visible = true;
		}
    }

    protected void btnCancelConfirm_Click(object sender, EventArgs e)
    {
        SetResetConfirmBox(false);
    }


    protected void submitConfirm_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            SetResetConfirmBox(false);
            ProcessCreditCardPayment();
        }
    }

    private void ProcessCreditCardPayment()
    {
        string offerId = OfferIdFromViewState;
        string piUserId = PaymentItemUserIdFromViewState;
        string hangoutUserId = HangoutUserIdFromViewState;
        string sessionGuid = SessionGuidFromViewState;

        CreditCardInfo creditCardInfo = new CreditCardInfo();
        creditCardInfo.FirstName = tbFirstName.Text.Trim();
        creditCardInfo.LastName = tbLastName.Text.Trim();
        creditCardInfo.Address = tbAddress.Text.Trim();
        creditCardInfo.City = tbCity.Text.Trim();
        creditCardInfo.StateProvince = ddState.SelectedValue;
        creditCardInfo.ZipCode = tbZipCode.Text;
        creditCardInfo.CountryCode = ddCountry.SelectedValue;

        creditCardInfo.CreditCardnumber = tbCreditCardNumber.Text.Replace(" ", "").Replace("-", "");
        creditCardInfo.CreditCardtype = ddCardType.SelectedValue.ToUpper().Replace(" ", "_");
        creditCardInfo.SecurityCode = tbSecurityCode.Text;
        creditCardInfo.ExpireDate = String.Format("{0}{1}", ddExpireMonth.SelectedValue, ddExpireYear.SelectedValue);

        string transactionId = Guid.NewGuid().ToString();

        string emailaddress = tbEmail.Text.Trim();

        if (PaymentItemEmailAddressFromViewState.Trim().ToLower() != emailaddress.ToLower())
        {
            UpdateEmailAddress(piUserId, emailaddress);
        }

        string resultMessage = PurchaseGameCurrencyCreditCard(transactionId, piUserId, hangoutUserId, sessionGuid, offerId, creditCardInfo, emailaddress);

        if (resultMessage == "OK")
        {
            Context.Items["transactionId"] = transactionId;
            Context.Items["offerId"] = offerId;
            Context.Items.Add("creditCardInfo", creditCardInfo);
            ServerTransfer("CreditCardComplete.aspx");
        }
        else
        {
            ValidationError.AddNewError(resultMessage);
			CreditCardErrorsDiv.Visible = true;
        }
    }


    protected void RequiredFieldPreRender(object sender, EventArgs e)
    {
        /*if (!IsExistingValidatorErrorMessage("Required"))
        {
            IValidator validator = (IValidator)sender;
            if (validator.IsValid == false)
            {
                validator.ErrorMessage = "Required";
            }
        }*/
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Context.Items["offerId"] = OfferIdFromViewState;
        ServerTransfer("purchase.aspx");
    }
    
    protected void validateZipCode(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = true;
    }
    
    protected void validatePhoneNumber(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = true;
    }
    
    protected void validateCreditCard(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = true;
    }
    
    protected void validateCardType(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = true;
    }

    protected void validateSecurityCode(object sender, ServerValidateEventArgs e)
    {
        Validation validate = new Validation();
   
        e.IsValid = validate.ValidateSecurityCode(tbSecurityCode.Text);

        if (e.IsValid == false)
        {
            ValidationError.AddNewError(PaymentResources.GetStringFromResourceFile("resCCSecurityCodeError"));
        }
    }

    protected void validateExpireDate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = true;
    }

    protected void validateEmail(object sender, ServerValidateEventArgs e)
    {
        Validation validate = new Validation();
        string confirmEmail = "";

        e.IsValid = false;

        if (!String.IsNullOrEmpty(tbEmail.Text))
        {
            confirmEmail = tbEmail.Text.Trim().ToLower();
        }

        if (!String.IsNullOrEmpty(tbEmail.Text))
        {
            if (!validate.ValidateEmailAddress(tbEmail.Text))
            {
                ValidationError.AddNewError(PaymentResources.GetStringFromResourceFile("resCCEmailAddressNotValid"));
            }
            else
            {
                if (confirmEmail != tbEmail.Text.Trim().ToLower())
                {
                    ValidationError.AddNewError(PaymentResources.GetStringFromResourceFile("resCCEmailAddressConfirmDoesNotMatch"));
                }
                else
                {
                    e.IsValid = true;
                }
            }
        }
    }

    public bool IsExistingValidatorErrorMessage(string errorMessage)
    {
        bool exists = false;
        foreach (IValidator currentValidator in Page.Validators)
        {
            if (currentValidator.IsValid == false)
            {
                if (currentValidator.ErrorMessage == errorMessage)
                {
                    exists = true;
                    break;
                }
            }
        }
        return exists;
    }

    private void SetConfirmBox()
    {
        string offerId = OfferIdFromViewState;
        string piUserId = PaymentItemUserIdFromViewState;
        string hangoutUserId = HangoutUserIdFromViewState;
        string sessionGuid = SessionGuidFromViewState;
        SetResetConfirmBox(true);
    }

    private void SetResetConfirmBox(bool confirmOn)
    {
        System.Drawing.Color backColor = System.Drawing.Color.White;
        if (confirmOn)
        {
            backColor = System.Drawing.Color.DarkGray;
        }

        divConfirmPopUpBackDrop.Visible = confirmOn;
        divConfirmPopUp.Visible = confirmOn;

        tbFirstName.ReadOnly = confirmOn;
        tbFirstName.BackColor = backColor;

        tbLastName.ReadOnly = confirmOn;
        tbLastName.BackColor = backColor;

        tbAddress.ReadOnly = confirmOn;
        tbAddress.BackColor = backColor;

        tbCity.ReadOnly = confirmOn;
        tbCity.BackColor = backColor;

        ddState.Enabled = !confirmOn;

        tbZipCode.ReadOnly = confirmOn;
        tbZipCode.BackColor = backColor;

        ddCountry.Enabled = !confirmOn;

        tbEmail.ReadOnly = confirmOn;
        tbEmail.BackColor = backColor;

        tbCreditCardNumber.ReadOnly = confirmOn;
        tbCreditCardNumber.BackColor = backColor;

        ddCardType.Enabled = !confirmOn;

        tbSecurityCode.ReadOnly = confirmOn;
        tbSecurityCode.BackColor = backColor;

        ddExpireMonth.Enabled = !confirmOn;
        ddExpireYear.Enabled = !confirmOn;

        btnCancel.Visible = !confirmOn;
        btnSubmit.Visible = !confirmOn;
    }
                

    private void AddStates(DropDownList ddState)
    {
        CreateListItem(ddState, "", "");
        CreateListItem(ddState, "AL", "Alabama");
        CreateListItem(ddState, "AK", "Alaska");
        CreateListItem(ddState, "AZ", "Arizona");
        CreateListItem(ddState, "AR", "Arkansas");
        CreateListItem(ddState, "CA", "California");
        CreateListItem(ddState, "CO", "Colorado");
        CreateListItem(ddState, "CT", "Connecticut");
        CreateListItem(ddState, "DE", "Delaware");
        CreateListItem(ddState, "DC", "District Of Columbia");
        CreateListItem(ddState, "FL", "Florida");
        CreateListItem(ddState, "GA", "Georgia");
        CreateListItem(ddState, "HI", "Hawaii");
        CreateListItem(ddState, "ID", "Idaho");
        CreateListItem(ddState, "IL", "Illinois");
        CreateListItem(ddState, "IN", "Indiana");
        CreateListItem(ddState, "IA", "Iowa");
        CreateListItem(ddState, "KS", "Kansas");
        CreateListItem(ddState, "KY", "Kentucky");
        CreateListItem(ddState, "LA", "Louisiana");
        CreateListItem(ddState, "ME", "Maine");
        CreateListItem(ddState, "MD", "Maryland");
        CreateListItem(ddState, "MA", "Massachusetts");
        CreateListItem(ddState, "MI", "Michigan");
        CreateListItem(ddState, "MN", "Minnesota");
        CreateListItem(ddState, "MS", "Mississippi");
        CreateListItem(ddState, "MO", "Missouri");
        CreateListItem(ddState, "MT", "Montana");
        CreateListItem(ddState, "NE", "Nebraska");
        CreateListItem(ddState, "NV", "Nevada");
        CreateListItem(ddState, "NH", "New Hampshire");
        CreateListItem(ddState, "NJ", "New Jersey");
        CreateListItem(ddState, "NM", "New Mexico");
        CreateListItem(ddState, "NY", "New York");
        CreateListItem(ddState, "NC", "North Carolina");
        CreateListItem(ddState, "ND", "North Dakota");
        CreateListItem(ddState, "OH", "Ohio");
        CreateListItem(ddState, "OK", "Oklahoma");
        CreateListItem(ddState, "OR", "Oregon");
        CreateListItem(ddState, "PA", "Pennsylvania");
        CreateListItem(ddState, "RI", "Rhode Island");
        CreateListItem(ddState, "SC", "South Carolina");
        CreateListItem(ddState, "SD", "South Dakota");
        CreateListItem(ddState, "TN", "Tennessee");
        CreateListItem(ddState, "TX", "Texas");
        CreateListItem(ddState, "UT", "Utah");
        CreateListItem(ddState, "VT", "Vermont");
        CreateListItem(ddState, "VA", "Virginia");
        CreateListItem(ddState, "WA", "Washington");
        CreateListItem(ddState, "WV", "West Virginia");
        CreateListItem(ddState, "WI", "Wisconsin");
        CreateListItem(ddState, "WY", "Wyoming");
    }

    private void CreateListItem(DropDownList dropDown, string text, string value)
    {
        ListItem li = new ListItem(text, value);
        dropDown.Items.Add(li);
    }

}


