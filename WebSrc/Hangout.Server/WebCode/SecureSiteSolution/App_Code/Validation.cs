using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

/// <summary>
/// Summary description for Validation
/// </summary>
public class Validation
{
    public bool ValidateName(string value)
    {
        string pattern = @"^[A-Z][a-zA-Z]*$";

        return (Regex.Match(value, pattern).Success);
    }

    public bool ValidateCreditCardValue(string value)
    {
        // for Visa, MC, Discover and American Express. It also allows for dashes between sets of numbers
        string pattern = @"^((4\d{3})|(5[1-5]\d{2})|(6011))-?\d{4}-?\d{4}-?\d{4}|3[4,7][\d\s-]{15}$";

        return (Regex.Match(value, pattern).Success);
    }

    public bool ValidateSecurityCode(string value)
    {
        string pattern = @"^[0-9]{3,4}$";
        return (Regex.Match(value, pattern).Success);
    }

    public bool ValidateUSPostalCode(string value)
    {
        string pattern = @"^\d{5}\p{Punct}?\s?(?:\d{4})?$";

        return (Regex.Match(value, pattern).Success);
    }

    public bool ValidateCreditCardDate(string value)
    {
        string pattern = @"((0[1-9])|(1[0-2]))\/((2009)|(20[1-2][0-9]))";

        return (Regex.Match(value, pattern).Success);

    }

    public bool ValidateUSPhoneNumber(string value)
    {
        string pattern = @"^(1\s*[-\/\.]?)?(\((\d{3})\)|(\d{3}))\s*[-\/\.]?\s*(\d{3})\s*[-\/\.]?\s*(\d{4})\s*(([xX]|[eE][xX][tT])\.?\s*(\d+))*$";

        return (Regex.Match(value, pattern).Success);
    }

    public bool ValidateEmailAddress(string value)
    {
        StringBuilder pattern = new StringBuilder(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@");
        pattern.Append(@"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.");
        pattern.Append(@"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?	[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|");
        pattern.Append(@"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$");

        return (Regex.Match(value, pattern.ToString()).Success);
    }

}

public class ValidationError : IValidator
{
    private ValidationError(string message)
    {
        ErrorMessage = message;
        IsValid = false;
    }

    public string ErrorMessage { get; set; }
    public bool IsValid { get; set; }

    public void Validate()
    {
        // no action required
    }

    public static void AddNewError(string message)
    {
        Page currentPage = HttpContext.Current.Handler as Page;
        currentPage.Validators.Add(new ValidationError(message));
    }
}