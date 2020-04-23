using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ReportDesigner.Services
{
    public class MyReportStorageWebExtension : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
    {
        readonly string reportDirectory;
        const string FileExtension = ".repx";
        public MyReportStorageWebExtension(string reportDirectory)
        {
            this.reportDirectory = reportDirectory;
        }

        public override bool CanSetData(string url)
        {
            return true;
        }

        public override bool IsValidUrl(string url)
        {
            return true;
        }

        public override byte[] GetData(string url)
        {
            try
            {
                using (var reportFile = File.Open(Path.Combine(reportDirectory, url + FileExtension), FileMode.Open))
                using (var memoryStream = new MemoryStream())
                {
                    reportFile.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception)
            {
                throw new FaultException(new FaultReason(string.Format("Cannot not find the report '{0}'.", url)), new FaultCode("Server"), "GetData");
            }
        }

        public override Dictionary<string, string> GetUrls()
        {
            return Directory.GetFiles(reportDirectory, "*" + FileExtension)
                                    .Select(Path.GetFileNameWithoutExtension)
                                    .ToDictionary<string, string>(x => x);
        }

        public override void SetData(XtraReport report, string url)
        {
            using (var reportFile = File.Open(Path.Combine(reportDirectory, url + FileExtension), FileMode.OpenOrCreate))
            {
                report.SaveLayoutToXml(reportFile);
            }
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            SetData(report, defaultUrl);
            return defaultUrl;
        }
    }
}
