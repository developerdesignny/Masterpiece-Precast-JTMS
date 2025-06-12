using JTMS.ViewModels.DialogsVM;
using System.Windows;

namespace JTMS.Dialogs
{
    /// <summary>
    /// Interaction logic for ViewProjectReport.xaml
    /// </summary>
    public partial class ViewProjectReport : Window
    {
        public ViewProjectReport(ProjectReportViewModel projectReportViewModel)
        {
            InitializeComponent();
            DataContext = projectReportViewModel;
        }
    }
}
