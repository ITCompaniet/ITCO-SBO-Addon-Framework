using ITCO.SboAddon.Framework.Helpers;
using System;

namespace ITCO.SboAddon.Framework
{
    /// <summary>
    /// SBO Application Connector
    /// </summary>
    public class SboApp
    {
        private static SAPbouiCOM.Application _application;
        private static SAPbobsCOM.Company _diCompany;

        /// <summary>
        /// Set existing DI and/or UI Api Connection
        /// </summary>
        /// <param name="diCompany">SAPbobsCOM.Company</param>
        /// <param name="application">SAPbouiCOM.Application</param>
        public static void SetApiConnection(
            SAPbobsCOM.Company diCompany, SAPbouiCOM.Application application = null)
        {
            _diCompany = diCompany;
            if (application != null)
                _application = application;
        }

        /// <summary>
        /// Connect UI and DI Api
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Connect(string connectionString)
        {
            var sboGuiApi = new SAPbouiCOM.SboGuiApi();
            _diCompany = new SAPbobsCOM.Company();

            try
            {
                sboGuiApi.Connect(connectionString);
                _application = sboGuiApi.GetApplication(-1);

                var contextCookie = _diCompany.GetContextCookie();
                var diCompanyConnectionString = _application.Company.GetConnectionContext(contextCookie);

                var responseCode = _diCompany.SetSboLoginContext(diCompanyConnectionString);
                ErrorHelper.HandleErrorWithException(responseCode, "DI API Could not Set Sbo Login Context");

                var connectResponse = _diCompany.Connect();
                ErrorHelper.HandleErrorWithException(connectResponse, "DI API Could not connect");

                var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                _application.StatusBar.SetText($"{assemblyName} connected to SBO", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                SetAppEvents();
            }
            catch (Exception ex)
            {
                if (_application != null)
                    _application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                throw ex;
            }
        }

        /// <summary>
        /// Connect only DI Api
        /// </summary>
        /// <param name="serverName">SQL Server Name</param>
        /// <param name="serverType"></param>
        /// <param name="companyDb"></param>
        /// <param name="dbUsername"></param>
        /// <param name="dbPassword"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void DiConnect(string serverName, SAPbobsCOM.BoDataServerTypes serverType, string companyDb,
            string dbUsername = null, string dbPassword = null, string username = null, string password = null)
        {
            _diCompany = new SAPbobsCOM.Company();

            try
            {
                _diCompany.Server = serverName;
                _diCompany.DbServerType = serverType;
                _diCompany.CompanyDB = companyDb;

                if (username == null)
                {
                    _diCompany.UseTrusted = true;
                }
                else
                {
                    _diCompany.UseTrusted = false;
                    _diCompany.UserName = username;
                    _diCompany.Password = password;
                    _diCompany.DbUserName = dbUsername;
                    _diCompany.DbPassword = dbPassword;
                }

                var connectResponse = _diCompany.Connect();
                ErrorHelper.HandleErrorWithException(connectResponse, "DI API Could not connect");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// SBO UI Application Object
        /// </summary>
        public static SAPbouiCOM.Application Application => _application;

        /// <summary>
        /// SBO DI Company Object
        /// </summary>
        public static SAPbobsCOM.Company Company => _diCompany;

        /// <summary>
        /// Check if SBO UI API is Connected
        /// </summary>
        public static bool ApplicationConnected
        {
            get
            {
                if (_application == null)
                    return false;
                
                if (DiConnected)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Check if SBO DI API is Connected
        /// </summary>
        public static bool DiConnected
        {
            get
            {
                if (_diCompany != null && _diCompany.Connected)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Set Default App Events
        /// </summary>
        protected static void SetAppEvents()
        {
            _application.AppEvent += Application_AppEvent;
        }

        private static void Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    if (_diCompany.Connected)
                        _diCompany.Disconnect();

                    System.Windows.Forms.Application.Exit();
                    break;
            }
        }
    }
}