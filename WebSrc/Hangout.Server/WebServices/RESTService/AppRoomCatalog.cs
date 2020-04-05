using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server.WebServices
{

    public class AppRoomCatalog 
    {
        private Dictionary<string, AppDomain> _catalogAppDomains = new Dictionary<string, AppDomain>();


        ~AppRoomCatalog()
        {
            UnLoadAllAppDomains();
        }

        public bool AddCatalogAppDomain(string catalogNumber, AppDomain appDomain)
        {
           bool retVal = true;
            try
            {
                if (!_catalogAppDomains.ContainsKey(catalogNumber))
                    _catalogAppDomains.Add(catalogNumber, appDomain);
                
            }

            catch 
            {
                retVal = false;
            }
            return retVal;
        }

        public AppDomain RemoveCatalogAppDomain(string catalogNumber)
        {
            AppDomain appDomain = null;

            try
            {
                appDomain = GetCatalogAppDomain(catalogNumber);
                if (appDomain != null)
                    _catalogAppDomains.Remove(catalogNumber);

            }

            catch
            {
            }
            return appDomain;
        }


        public AppDomain GetCatalogAppDomain(string catalogNumber)
        {
            AppDomain appDomain = null;

            try
            {
                _catalogAppDomains.TryGetValue(catalogNumber, out appDomain);

            }

            catch
            {
            }

            return appDomain;
        }

        public List<string> GetAllCatalogNumbers()
        {
            try
            {
                 return (new List<string>(_catalogAppDomains.Keys));
            }

            catch
            {
            }

            return null;
        }

        public void UnLoadAllAppDomains()
        {

             foreach (KeyValuePair<string, AppDomain> catalogInfo in _catalogAppDomains)
             {
                 UnLoadAppDomain(catalogInfo);
             }
             _catalogAppDomains.Clear();
        }


        public void UnLoadAppDomain(KeyValuePair<string, AppDomain> catalogInfo)
        {
            try
            {
                AppDomain.Unload(catalogInfo.Value);
            }
            catch { }
        }
    }
}
    
    

