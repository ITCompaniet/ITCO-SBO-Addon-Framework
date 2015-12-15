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

                _application.StatusBar.SetText("Connected to SBO", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                SetEvents();
            }
            catch (Exception ex)
            {
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
        public static SAPbouiCOM.Application Application
        {
            get { return _application; }
        }

        /// <summary>
        /// SBO DI Company Object
        /// </summary>
        public static SAPbobsCOM.Company Company
        {
            get { return _diCompany; }
        }

        private static void SetEvents()
        {
            Application.AppEvent += Application_AppEvent;
        }

        private static void Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    if (Company.Connected)
                        Company.Disconnect();

                    System.Windows.Forms.Application.Exit();
                    break;
            }
        }
    }
}