using CommunityToolkit.Mvvm.ComponentModel;

namespace JTMS.ViewModels.DialogsVM
{
    public class ScanProcessSelectorViewModel : ObservableObject
    {
        private string _scanMode = "Process 1 (Mold Ready)";
        public string ScanMode { get => _scanMode; set => SetProperty(ref _scanMode, value); }

        public ScanProcessSelectorViewModel()
        {

        }
    }
}
