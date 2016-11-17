namespace ITCO.SboAddon.Framework.Helpers
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using SAPbobsCOM;

    /// <summary>
    /// Internal SBO messenger helper
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        /// Send internal SBO message
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="userCode"></param>
        /// <param name="docNum"></param>
        /// <param name="boObjectTypes"></param>
        public static void SendMessage(string subject, string text, string userCode,
            int? docNum = null, BoObjectTypes? boObjectTypes = null)
        {
            SendMessage(subject, text, new[] {userCode}, docNum, boObjectTypes);
        }

        /// <summary>
        /// Send internal SBO message
        /// </summary>
        public static void SendMessage(string subject, string text, IEnumerable<string> userCodes,
            int? docNum = null, BoObjectTypes? boObjectTypes = null)
        {
            var companyService = SboApp.Company.GetCompanyService();
            var messageService = companyService.GetBusinessService(ServiceTypes.MessagesService) as MessagesService;

            var message = messageService.GetDataInterface(MessagesServiceDataInterfaces.msdiMessage) as Message;
            try
            {
                foreach (var userCode in userCodes)
                {
                    message.Subject = subject;
                    message.Text = text;
                    message.RecipientCollection.Add();
                    message.RecipientCollection.Item(0).SendInternal = BoYesNoEnum.tYES;
                    message.RecipientCollection.Item(0).UserCode = userCode;

                    if (docNum.HasValue && boObjectTypes.HasValue)
                    {
                        var docEntry = docNum.Value.GetDocEntry("ORDR");
                        if (docEntry.HasValue)
                        {
                            var column1 = message.MessageDataColumns.Add();
                            column1.ColumnName = "Document";
                            column1.Link = BoYesNoEnum.tYES;

                            var line1 = column1.MessageDataLines.Add();
                            line1.Value = docNum.ToString();
                            line1.ObjectKey = docEntry.Value.ToString();
                            line1.Object = ((int) boObjectTypes.Value).ToString();
                        }
                    }
                    messageService.SendMessage(message);
                }
            }
            catch (Exception e)
            {
                SboApp.Logger.Error("SendMessage Error", e);
                throw;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(message);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(messageService);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(companyService);
            }
        }
    }
}
