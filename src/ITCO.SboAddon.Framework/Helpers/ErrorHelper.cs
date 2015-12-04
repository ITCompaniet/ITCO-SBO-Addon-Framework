using SAPbouiCOM;
using System;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class ErrorHelper
    {
        public static void HandleErrorWithException(int returnCode, string errorDescription)
        {
            if (returnCode != 0)
            {
                int errCode;
                string errMsg;
                SboApp.Company.GetLastError(out errCode, out errMsg);
                SboApp.Application.StatusBar.SetText(string.Format("{0}: {1} {2}", errorDescription, errCode, errMsg), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                throw new Exception(string.Format("{0}: {1} {2}", errorDescription, errCode, errMsg));
            }
        }

        public static bool HandleErrorWithMessageBox(int returnCode, string errorDescription = "Error")
        {
            if (returnCode != 0)
            {
                int errCode;
                string errMsg;
                SboApp.Company.GetLastError(out errCode, out errMsg);
                var errorMessage = string.Format("{0}: {1} {2}", errorDescription, errCode, errMsg);
                SboApp.Application.StatusBar.SetText(errorMessage, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                SboApp.Application.MessageBox(errorMessage);
                return false;
            }
            return true;
        }
    }
}
