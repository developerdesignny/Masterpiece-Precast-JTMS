using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Helpers;
using JTMS.Models;
using JTMS.ViewModels.DialogsVM;
using System.Collections.ObjectModel;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace JTMS.ViewModels
{
    class CompanyViewModel : ObservableObject
    {
        private ObservableCollection<CompanyModel> _companies = new ObservableCollection<CompanyModel>();
        public ObservableCollection<CompanyModel> Companies { get => _companies; set => SetProperty(ref _companies, value); }

        public RelayCommand AddCompanyCommand { get; set; }
        public RelayCommand ReloadCommand { get; set; }
        public IRelayCommand EditCompanyCommand { get; set; }
        public IRelayCommand DeleteCompanyCommand { get; set; }

        private DataHandler.DataHandler dataHandler;

        public CompanyViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            AddCompanyCommand = new RelayCommand(addCompany);
            ReloadCommand = new RelayCommand(loadCompanyData);
            EditCompanyCommand = new RelayCommand<CompanyModel>(model => editCompanyData(model));
            DeleteCompanyCommand = new RelayCommand<CompanyModel>(model => delteCompanyData(model));
            WeakReferenceMessenger.Default.Register<DataMessageModel>(this, (r, model) => recieveViewData(model));
            loadCompanyData();
        }//

        private async void delteCompanyData(CompanyModel model)
        {
            try
            {
                await dataHandler.DeleteCompany(model.Id);
                Companies.Remove(model);
            }
            catch (ArgumentNullException err)
            {
                await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = err.Message }, "RootDialogHost");
            }
            catch (Exception err) { await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Error occured. Refresh and try agian." }, "RootDialogHost"); }
        }

        private async void editCompanyData(CompanyModel model)
        {
            try
            {
                var vm = new AddCompanyDialogViewModel();
                vm.IsEditing = true;

                vm.Company = new CompanyModel();
                vm.Company.Id = model.Id;
                vm.Company.Name = model.Name;
                vm.Company.Id = model.Id;

                var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
            }
            catch (Exception err) { await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Error occured. Refresh and try agian." }, "RootDialogHost"); }
        }

        private void loadCompanyData()
        {
            Task.Run(async () =>
            {
                var companies = await dataHandler.GetCompanys(1);
                Application.Current.Dispatcher.Invoke(() => { Companies = new(companies); });
            });
        }

        private void recieveViewData(DataMessageModel model)
        {
            if (model.Data == "Company-Reload")
            {
                dataHandler = new DataHandler.DataHandler();
                loadCompanyData();
            }

            else if (model.Data == "Settings-Updated")
            {
                dataHandler = new DataHandler.DataHandler();
                loadCompanyData();
            }
        }

        private async void addCompany()
        {
            var vm = new AddCompanyDialogViewModel();
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
        }
    }
}
