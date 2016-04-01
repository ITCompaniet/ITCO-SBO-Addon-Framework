using SAPbouiCOM;
using System;
using System.Diagnostics;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class ErrorHelper
    {
        /// <summary>
        /// Get Last Error Message from SBO DI API
        /// </summary>
        /// <returns></returns>
        public static SboError GetLastErrorMessage()
        {
            int errCode;
            string errMsg;
            SboApp.Company.GetLastError(out errCode, out errMsg);
            return new SboError(errCode, errMsg);
        }

        /// <summary>
        /// Handle Return Code
        /// Throws Exception if Return Code != 0
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="errorDescription"></param>
        public static void HandleErrorWithException(int returnCode, string errorDescription)
        {
            if (returnCode == 0)
                return;

            var error = GetLastErrorMessage();
            throw new Exception($"{errorDescription}: {error.Code} {error.Message}");
        }

        public static bool HandleErrorWithMessageBox(int returnCode, string errorDescription = "Error")
        {
            if (returnCode == 0)
                return true;

            var error = GetLastErrorMessage();
            var errorMessage = $"{errorDescription}: {error.Code} {error.Message}";
            SboApp.Application.StatusBar.SetText(errorMessage, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            SboApp.Application.MessageBox(errorMessage);
            return false;
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
