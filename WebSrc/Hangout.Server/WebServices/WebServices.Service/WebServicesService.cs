using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Configuration;

namespace Hangout.Server.WebServices
{
    partial class WebServicesService : ServiceBase
    {
        private static PaymentItemConnect paymentItemConnect = null;
        private static ServiceConnect serviceConnect = null;
        //private static EventLog webServicesLog = new EventLog();

        public WebServicesService()
        {
            InitializeComponent();

			this.ServiceName = ConfigurationManager.AppSettings.Get("ServiceName");

            //if (!EventLog.SourceExists("Hangout"))
            //{
                //EventLog.CreateEventSource("Hangout", "Hangout");
            //}

            //webServicesLog.Source = "Hangout";
            //webServicesLog.Log = "Hangout";
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //webServicesLog.WriteEntry("OnStart Entry", EventLogEntryType.Information);

                serviceConnect = new ServiceConnect();
                if (serviceConnect.Listen())
                {
                     //webServicesLog.WriteEntry("Listener Started", EventLogEntryType.Information );
                }
                else
                {
                    throw (new Exception("Listener failed to Start"));
                }

      
                if (serviceConnect.RegisterWellKnownServiceType())
                {
                    //webServicesLog.WriteEntry("RegisterWellKnownServiceType Service", EventLogEntryType.Information );
                }

                paymentItemConnect = new PaymentItemConnect();

                if (paymentItemConnect.RegisterWellKnownServiceType())
                {
                    //webServicesLog.WriteEntry("RegisterWellKnownServiceType PaymentItem", EventLogEntryType.Information );
                }

                //webServicesLog.WriteEntry("OnStart Completed", EventLogEntryType.Information);
            }

            catch (Exception ex)
            {
                //webServicesLog.WriteEntry(String.Format("Error OnStart: {0}", ex.Message), EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            //webServicesLog.WriteEntry("OnStop Completed", EventLogEntryType.Information);
        }
    }
}
