using CommunityToolkit.Mvvm.ComponentModel;

namespace JTMS.ViewModels.DialogsVM
{
    class ListenOnScanViewModel : ObservableObject
    {
        private string _scanMode = string.Empty;
        public string ScanMode
        {
            get => _scanMode; set
            {
                SetProperty(ref _scanMode, value);
                ScanModeUpdate?.Invoke(value);
            }
        }

        public Action<string> ScanModeUpdate;

        public ListenOnScanViewModel()
        {

        }

    }
}
