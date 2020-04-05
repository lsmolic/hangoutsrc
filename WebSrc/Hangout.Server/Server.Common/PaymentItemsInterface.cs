using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server
{
    public interface IPaymentItemInterface
    {
       string ProcessMessage(string xmlMessage);
       XmlDocument ProcessMessage(PaymentCommand command);
       string CallBackMessage(string xmlMessage);
       XmlDocument AdminProcessMessage(PaymentCommand command);
    }
}