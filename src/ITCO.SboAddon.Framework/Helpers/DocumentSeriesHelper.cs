namespace ITCO.SboAddon.Framework.Helpers
{
    using System;
    using SAPbobsCOM;

    public static class DocumentSeriesHelper
    {
        public static NextNumberResponse GetNextNumber(BoObjectTypes documentType, string documentSubType = null, int? serieId = null)
        {
            var companyService = SboApp.Company.GetCompanyService();
            var seriesService = companyService.GetBusinessService(ServiceTypes.SeriesService) as SeriesService;
            var docTypeParams = seriesService.GetDataInterface(SeriesServiceDataInterfaces.ssdiDocumentTypeParams) as DocumentTypeParams;

            try
            {
                docTypeParams.Document = ((int) documentType).ToString();
                if (documentSubType != null)
                    docTypeParams.DocumentSubType = documentSubType;

                Series serie = null;
                if (serieId.HasValue)
                {
                    var allSeries = seriesService.GetDocumentSeries(docTypeParams);
                    foreach (Series s in allSeries)
                        if (s.Series == serieId.Value)
                        {
                            serie = s;
                            break;
                        }
                    if (serie == null)
                        throw new Exception($"Could not find serie {serieId.Value}");
                }
                else
                {
                    serie = seriesService.GetDefaultSeries(docTypeParams);
                }

                return new NextNumberResponse
                {
                    NextNumber = $"{serie.Prefix}{serie.NextNumber}{serie.Suffix}",
                    Series = serie.Series
                };
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(seriesService);
            }
        }
    }

    public class NextNumberResponse
    {
        public string NextNumber { get; set; }
        public int Series { get; set; }
    }
}
