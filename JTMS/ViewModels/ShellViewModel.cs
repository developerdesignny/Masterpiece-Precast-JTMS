using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Helpers;
using JTMS.Models;

namespace JTMS.ViewModels
{
    class ShellViewModel : ObservableObject
    {
        private object currentViewModel = null;
        public object CurrentViewModel { get => currentViewModel; set { SetProperty(ref currentViewModel, value); } }

        private bool _isCompanyChecked = false;
        private bool _isProjectChecked = false;

        public bool IsCompanyChecked { get => _isCompanyChecked; set => SetProperty(ref _isCompanyChecked, value); }
        public bool IsProjectChecked { get => _isProjectChecked; set => SetProperty(ref _isProjectChecked, value); }

        private CompanyViewModel companyViewModel = new CompanyViewModel();
        private ProjectShellViewModel projectShellViewModel = new ProjectShellViewModel();
        private SettingsViewModel settingsViewModel = new SettingsViewModel();
        public HomeViewModel homeViewModel = new HomeViewModel();

        public IRelayCommand ChangeViewCommand { get; set; }

        public ShellViewModel()
        {
            ChangeViewCommand = new RelayCommand<string>(tag => changeView(tag));
            CurrentViewModel = homeViewModel;
            WeakReferenceMessenger.Default.Register<DataMessageModel>(this, (r, model) => recieveNavData(model));
            settingsViewModel.SDataRecieved += projectShellViewModel.serialDataR;
        }//

        private void recieveNavData(DataMessageModel model)
        {
            if (model.Data == "Nav-Company")
            {
                CurrentViewModel = companyViewModel;
                IsCompanyChecked = true;
            }
            else if (model.Data == "Nav-Project")
            {
                CurrentViewModel = projectShellViewModel;
                IsProjectChecked = true;
            }
        }

        public void changeView(string tag)
        {
            if (tag == "Home")
                CurrentViewModel = homeViewModel;
            else if (tag == "Project")
                CurrentViewModel = projectShellViewModel;
            else if (tag == "Company")
                CurrentViewModel = companyViewModel;
            else if (tag == "Settings")
                CurrentViewModel = settingsViewModel;
        }//

    }
}
