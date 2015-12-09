using SAPbouiCOM;
using System;
using System.Diagnostics;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class ErrorHelper
    {
        public static SboError GetLastErrorMessage()
        {
            int errCode;
            string errMsg;
            SboApp.Company.GetLastError(out errCode, out errMsg);
            return new SboError(errCode, errMsg);
        }

        public static void HandleErrorWithException(int returnCode, string errorDescription)
        {
            if (returnCode != 0)
            {
                var error = GetLastErrorMessage();
                throw new Exception(string.Format("{0}: {1} {2}", errorDescription, error.Code, error.Message));
            }
        }

        public static bool HandleErrorWithMessageBox(int returnCode, string errorDescription = "Error")
        {
            if (returnCode != 0)
            {
                var error = GetLastErrorMessage();
                var errorMessage = string.Format("{0}: {1} {2}", errorDescription, error.Code, error.Message);
                SboApp.Application.StatusBar.SetText(errorMessage, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                SboApp.Application.MessageBox(errorMessage);
                return false;
            }
            return true;
        }
    }

    [DebuggerDisplay("{Code}: {Message}")]
    public class SboError
    {
        public SboError(int errorCode, string errorMessage)
        {
            Code = errorCode;
            Message = errorMessage;
        }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
