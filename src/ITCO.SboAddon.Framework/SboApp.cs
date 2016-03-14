using ITCO.SboAddon.Framework.Helpers;
using System;
using System.Configuration;

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
        public static void Connect(string connectionString = null)
        {
            if (connectionString == null)
            {
                connectionString = Environment.GetCommandLineArgs().Length > 1 ? 
                    Convert.ToString(Environment.GetCommandLineArgs().GetValue(1)) : 
                    "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
            }

            var sboGuiApi = new SAPbouiCOM.SboGuiApi();
            _diCompany = new SAPbobsCOM.Company();

            try
            {
                sboGuiApi.Connect(connectionString);
                _application = sboGuiApi.GetApplication();

                var contextCookie = _diCompany.GetContextCookie();
                var diCompanyConnectionString = _application.Company.GetConnectionContext(contextCookie);

                var responseCode = _diCompany.SetSboLoginContext(diCompanyConnectionString);
                ErrorHelper.HandleErrorWithException(responseCode, "DI API Could not Set Sbo Login Context");

                var connectResponse = _diCompany.Connect();
                ErrorHelper.HandleErrorWithException(connectResponse, "DI API Could not connect");

                var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                _application.StatusBar.SetText($"{assemblyName} connected to SBO", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                SetAppEvents();
            }
            catch (Exception ex)
            {
                _application?.StatusBar.SetText(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Connect only DI Api
        /// </summary>
        /// <param name="serverName">SQL Server Name</param>
        /// <param name="serverType">Server type</param>
        /// <param name="companyDb"></param>
        /// <param name="dbUsername"></param>
        /// <param name="dbPassword"></param>
        /// <param name="username">SBO Username</param>
        /// <param name="password">SBO Password</param>
        /// <param name="licenceService">Licence Server</param>
        public static void DiConnect(string serverName, SAPbobsCOM.BoDataServerTypes serverType, string companyDb,
            string dbUsername = null, string dbPassword = null, string username = null, string password = null, string licenceService = null)
        {
            _diCompany = new SAPbobsCOM.Company
            {
                Server = serverName,
                DbServerType = serverType,
                CompanyDB = companyDb
            };


            if (licenceService != null)
                _diCompany.LicenseServer = licenceService;

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
        /// <summary>
        /// Connect only DI Api from app.config
        /// </summary>
        /// <example>
        /// Sbo:ServerName
        /// Sbo:ServerType (BoDataServerTypes eg. MSSQL2012)
        /// Sbo:CompanyDb
        /// Sbo:DbUsername
        /// Sbo:DbPassword
        /// Sbo:Username
        /// Sbo:Password
        /// Sbo:LicenceService
        /// </example>
        public static void DiConnectFromAppConfig()
        {
            var serverName = ConfigurationManager.AppSettings["Sbo:ServerName"];

            SAPbobsCOM.BoDataServerTypes serverType;
            Enum.TryParse("dst_" + ConfigurationManager.AppSettings["Sbo:ServerType"], out serverType);

            var companyDb = ConfigurationManager.AppSettings["Sbo:CompanyDb"];
            var dbUsername = ConfigurationManager.AppSettings["Sbo:DbUsername"];
            var dbPassword = ConfigurationManager.AppSettings["Sbo:DbPassword"];
            var username = ConfigurationManager.AppSettings["Sbo:Username"];
            var password = ConfigurationManager.AppSettings["Sbo:Password"];
            var licenceServer = ConfigurationManager.AppSettings["Sbo:LicenceServer"];

            DiConnect(serverName, serverType, companyDb, dbUsername, dbPassword, username, password, licenceServer);
        }

        /// <summary>
        /// SBO UI Application Object
        /// </summary>
        public static SAPbouiCOM.Application Application
        {
            get
            {
                if (!ApplicationConnected)
                    Connect();

                return _application;
            }
        }

        /// <summary>
        /// SBO DI Company Object
        /// </summary>
        public static SAPbobsCOM.Company Company
        {
            get
            {
                if (!DiConnected)
                    Connect();

                return _diCompany;
            }
        }

        /// <summary>
        /// Check if SBO UI API is Connected
        /// </summary>
        public static bool ApplicationConnected => _application != null && DiConnected;

        /// <summary>
        /// Check if SBO DI API is Connected
        /// </summary>
        public static bool DiConnected => _diCompany != null && _diCompany.Connected;

        /// <summary>
        /// Set Default App Events
        /// </summary>
        protected static void SetAppEvents()
        {
            _application.AppEvent += Application_AppEvent;
        }

        private static void Application_AppEvent(SAPbouiCOM.BoAppEventTypes eventType)
        {
            switch (eventType)
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