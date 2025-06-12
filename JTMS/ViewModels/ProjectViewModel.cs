using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Dialogs;
using JTMS.Helpers;
using JTMS.Models;
using JTMS.ViewModels.DialogsVM;
using System.Collections.ObjectModel;
using System.Windows;

namespace JTMS.ViewModels
{
    class ProjectViewModel : ObservableObject
    {
        private ObservableCollection<ProjectModel> _projects = new ObservableCollection<ProjectModel>();
        public ObservableCollection<ProjectModel> Projects { get => _projects; set => SetProperty(ref _projects, value); }

        private ObservableCollection<CompanyModel> _companies = new ObservableCollection<CompanyModel>();
        public ObservableCollection<CompanyModel> Companies { get => _companies; set => SetProperty(ref _companies, value); }

        private CompanyModel _selectedCompany = new CompanyModel();
        public CompanyModel SelectedCompany
        {
            get => _selectedCompany; set
            {
                SetProperty(ref _selectedCompany, value);
                ProjectFilter();
            }
        }

        private string _searchParam = string.Empty;
        public string SearchParam
        {
            get => _searchParam;
            set
            {
                SetProperty(ref _searchParam, value);
                ProjectFilter();
            }
        }

        public RelayCommand AddProjectCommand { get; set; }
        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand ProgressReportCommand { get; set; }
        public IRelayCommand EditProjectCommand { get; set; }
        public IRelayCommand DeleteProjectCommand { get; set; }
        public IRelayCommand OpenProjectCommand { get; set; }

        private DataHandler.DataHandler dataHandler;

        public ProjectViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            ProgressReportCommand = new RelayCommand(getProgressReport);
            AddProjectCommand = new RelayCommand(addProject);
            ReloadCommand = new RelayCommand(loadProjectData);
            EditProjectCommand = new RelayCommand<ProjectModel>(model => editProjectData(model));
            DeleteProjectCommand = new RelayCommand<ProjectModel>(model => delteProjectData(model));
            OpenProjectCommand = new RelayCommand<ProjectModel>(model => openProjectData(model));
            WeakReferenceMessenger.Default.Register<DataMessageModel>(this, (r, model) => recieveViewData(model));
            loadProjectData();
        }

        private async void getProgressReport()
        {
            var vm = new SelectFilterDateViewModel();
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
            if (result is bool bol && bol == true)
            {
                var date = DateOnly.FromDateTime(vm.Date);
                var projVM = new ProjectReportViewModel();
                projVM.Label = $"Progress Report - {date}";
                projVM.ProjectReports = new(await dataHandler.GetProjectsReportsByDate(date));
                var win = new ViewProjectReport(projVM);
                win.ShowDialog();
            }
        }

        private void openProjectData(ProjectModel model)
        {
            //var vm = new ScanProcessSelectorViewModel();
            //var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
            //if (result is bool bol && bol == true)
            //{
            WeakReferenceMessenger.Default.Send(new ProjectShellNavModel { CurrentProject = model, Data = "PShellNav-Mold" });
            //}
        }

        private async void delteProjectData(ProjectModel model)
        {
            try
            {
                await dataHandler.DeleteProject(model.Id);
                Projects.Remove(model);
            }
            catch (ArgumentNullException err)
            {
                await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = err.Message }, "RootDialogHost");
            }
            catch (Exception err) { await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Error occured. Refresh and try agian." }, "RootDialogHost"); }
        }

        private async void editProjectData(ProjectModel model)
        {
            try
            {
                var vm = new AddProjectDialogViewModel();
                vm.IsEditing = true;

                vm.Project = new ProjectModel();
                vm.Project.Id = model.Id;
                vm.Project.JobName = model.JobName;
                vm.Project.JobDate = model.JobDate;
                vm.Project.JobColor = model.JobColor;
                vm.Project.CompanyId = model.Company.Id;
                vm.Project.Company = model.Company;
                vm.Project.Address = model.Address;

                vm.initCompanyList();
                var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
            }
            catch (Exception err) { await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Error occured. Refresh and try agian." }, "RootDialogHost"); }
        }

        private async void addProject()
        {
            var vm = new AddProjectDialogViewModel();
            vm.initCompanyList();
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
        }

        private void loadProjectData()
        {
            Task.Run(async () =>
            {
                var projects = await dataHandler.GetProjects(1, string.Empty, null);
                var companies = await dataHandler.GetCompanys(1);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Projects = new(projects);
                    Companies = new ObservableCollection<CompanyModel>(companies);
                    Companies.Insert(0, new CompanyModel { Name = "ALL" });
                    SelectedCompany = Companies.FirstOrDefault();
                });
            });
        }

        private void ProjectFilter()
        {
            Task.Run(async () =>
            {
                var projects = new List<ProjectModel>();

                if (SelectedCompany != null)
                {
                    if (SelectedCompany.Name == "ALL")
                        projects = await dataHandler.GetProjects(1, SearchParam, null);
                    else
                        projects = await dataHandler.GetProjects(1, SearchParam, SelectedCompany.Id);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Projects = new(projects);
                });
            });
        }

        private void recieveViewData(DataMessageModel model)
        {
            if (model.Data == "Project-Reload")
            {
                dataHandler = new DataHandler.DataHandler();
                loadProjectData();
            }

            else if (model.Data == "Settings-Updated")
            {
                dataHandler = new DataHandler.DataHandler();
                loadProjectData();
            }
        }

    }
}
