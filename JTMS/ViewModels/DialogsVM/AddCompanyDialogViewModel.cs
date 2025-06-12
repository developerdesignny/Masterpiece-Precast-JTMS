using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Dialogs;
using JTMS.Helpers;
using JTMS.Models;
using MaterialDesignThemes.Wpf;

namespace JTMS.ViewModels.DialogsVM
{
    public class AddCompanyDialogViewModel : ObservableObject
    {
        private CompanyModel _company = new CompanyModel();
        public CompanyModel Company { get => _company; set => SetProperty(ref _company, value); }

        public bool isSaved = false;
        public bool IsEditing = false;

        public RelayCommand SaveCommand { get; set; }
        private DataHandler.DataHandler dataHandler;

        public AddCompanyDialogViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            SaveCommand = new RelayCommand(saveCMD);
        }

        private async void saveCMD()
        {
            try
            {
                if (!string.IsNullOrEmpty(Company.Name))
                {
                    if (IsEditing)
                        await dataHandler.EditCompany(Company);
                    else
                        await dataHandler.AddCompany(Company);
                    WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Company-Reload" });
                    WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Nav-Company" });
                    DialogHost.CloseDialogCommand.Execute(null, null);
                }
                else
                    new MessageWin("error", "company name required");
            }
            catch (Exception)
            {
                new MessageWin("error", "Make sure you are connected to the database.");
            }
        }//
    }
}
