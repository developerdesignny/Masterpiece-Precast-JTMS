using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Dialogs;
using JTMS.Helpers;
using JTMS.Models;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows;

namespace JTMS.ViewModels.DialogsVM
{
    public class AddProjectDialogViewModel : ObservableObject
    {
        private ProjectModel _project = new ProjectModel();
        public ProjectModel Project { get => _project; set => SetProperty(ref _project, value); }

        private ObservableCollection<CompanyModel> _companies = new ObservableCollection<CompanyModel>();
        public ObservableCollection<CompanyModel> Companies { get => _companies; set => SetProperty(ref _companies, value); }

        public bool isSaved = false;
        public bool IsEditing = false;

        public RelayCommand SaveCommand { get; set; }
        private DataHandler.DataHandler dataHandler;

        public AddProjectDialogViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            SaveCommand = new RelayCommand(saveCMD);
        }

        public void initCompanyList()
        {
            _ = Task.Run(async () =>
            {
                var companies = await dataHandler.GetCompanys(1);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Companies = new ObservableCollection<CompanyModel>(companies);
                    if (Companies.Count > 0 && Project.Company != null)
                        Project.selectedIndex = Companies.IndexOf(Companies.FirstOrDefault(obj => obj.Id == Project.Company.Id));
                });
            });
        }

        private async void saveCMD()
        {
            try
            {
                if (!string.IsNullOrEmpty(Project.JobName) || Project.selectedIndex >= 0)
                {
                    Project.Company = Companies[Project.selectedIndex];
                    if (IsEditing)
                        await dataHandler.EditProject(Project);
                    else
                        await dataHandler.AddProject(Project);

                    WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Project-Reload" });
                    WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Nav-Project" });
                    DialogHost.CloseDialogCommand.Execute(null, null);
                }
                else
                    new MessageWin("error", "Project job name and company required");
            }
            catch (Exception)
            {
                new MessageWin("error", "Make sure you are connected to the database.");
            }
        }//

    }
}
