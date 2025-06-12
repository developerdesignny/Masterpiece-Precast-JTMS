using CommunityToolkit.Mvvm.ComponentModel;
using JTMS.Models;
using System.Collections.ObjectModel;

namespace JTMS.ViewModels.DialogsVM
{
    public class ProjectReportViewModel : ObservableObject
    {
        private ObservableCollection<ProgressReportModel> _projectReports = new ObservableCollection<ProgressReportModel>();
        public ObservableCollection<ProgressReportModel> ProjectReports { get => _projectReports; set => SetProperty(ref _projectReports, value); }

        private string _label = string.Empty;
        public string Label { get => _label; set => SetProperty(ref _label, value); }

        public ProjectReportViewModel()
        {

        }
    }
}
