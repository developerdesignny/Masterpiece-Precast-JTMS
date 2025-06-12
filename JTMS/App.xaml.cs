using OfficeOpenXml;
using System.Windows;

namespace JTMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXpfd3VUR2RfUEB3X0c=");
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }//

    }
}
