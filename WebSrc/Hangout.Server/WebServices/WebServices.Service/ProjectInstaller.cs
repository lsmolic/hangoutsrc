﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Configuration;


namespace WebServices.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

		protected string GetConfigurationValue(string key)
		{
			Assembly service = Assembly.GetAssembly(typeof(ProjectInstaller));
			Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
			if (config.AppSettings.Settings[key] != null)
			{
				return config.AppSettings.Settings[key].Value;
			}
			else
			{
				throw new IndexOutOfRangeException
					("Settings collection does not contain the requested key: " + key);
			}
		}
    }
}
