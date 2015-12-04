using System;

namespace ITCO.SboAddon.Framework
{
    public class SboApp
    {
        private static SAPbouiCOM.Application _application;
        private static SAPbobsCOM.Company _diCompany;

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

                var response = _diCompany.SetSboLoginContext(diCompanyConnectionString);
                if (response != 0)
                    throw new Exception("DI API Could not Set Sbo Login Context");

                var connectResponse = _diCompany.Connect();
                if (connectResponse != 0)
                    throw new Exception("DI API Could not connect");

                _application.StatusBar.SetText("Connected to SBO", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                SetEvents();
            }
            catch (Exception ex)
            {
                _application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                throw ex;
            }
        }

        public static SAPbouiCOM.Application Application
        {
            get { return _application; }
        }

        public static SAPbobsCOM.Company Company
        {
            get { return _diCompany; }
        }

        public static void SetEvents()
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
                    Environment.Exit(0);
                    break;
            }
        }
    }
}