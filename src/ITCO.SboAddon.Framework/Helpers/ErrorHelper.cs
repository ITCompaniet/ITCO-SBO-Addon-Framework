using SAPbouiCOM;
using System;
using System.Diagnostics;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// ErrorHelper
    /// </summary>
    public class ErrorHelper
    {
        /// <summary>
        /// Get Last Error Message from SBO DI API
        /// </summary>
        /// <returns></returns>
        public static SboError GetLastErrorMessage(SAPbobsCOM.Company company = null)
        {
            var _company = company ?? SboApp.Company;
            int errCode;
            string errMsg;
            _company.GetLastError(out errCode, out errMsg);
            return new SboError(errCode, errMsg);
        }

        /// <summary>
        /// Handle Return Code
        /// Throws Exception if Return Code != 0
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="errorDescription"></param>
        public static void HandleErrorWithException(int returnCode, string errorDescription, SAPbobsCOM.Company company = null)
        {
            if (returnCode == 0)
                return;

            try
            {
                var error = GetLastErrorMessage(company);
                throw new Exception($"{errorDescription}: {error.Code} {error.Message}");
            }
            catch (Exception)
            {
                throw new Exception($"{errorDescription}: ReturnCode: {returnCode}");
            }
        }

        /// <summary>
        /// Handle Return Code
        /// Shows MessageBox if Return Code != 0
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="errorDescription"></param>
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

    /// <summary>
    /// SboError
    /// </summary>
    [DebuggerDisplay("{Code}: {Message}")]
    public class SboError
    {
        /// <summary>
        /// SboError
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        public SboError(int errorCode, string errorMessage)
        {
            Code = errorCode;
            Message = errorMessage;
        }
        /// <summary>
        /// ErrorCode
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// ErrorMessage
        /// </summary>
        public string Message { get; set; }
    }
}
