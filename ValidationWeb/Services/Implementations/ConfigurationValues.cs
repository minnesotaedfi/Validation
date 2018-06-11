using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    internal class ConfigurationValues : IConfigurationValues
    {
        private static bool _useFakeViewModelData;
        
        static ConfigurationValues()
        {
            _useFakeViewModelData = ConfigurationManager.AppSettings["UseFakeViewModelData"] == "true";
        }

        /// <summary>
        /// The FakeViewModelData value should be either "false" of omitted in production. 
        /// Only if the value is present and is exactly "true" will fake, hardcoded ViewModel data be used for the UI.
        /// </summary>
        public bool UseFakeViewModelData
        {
            get
            {
                return _useFakeViewModelData;
            }
        }
    }
}