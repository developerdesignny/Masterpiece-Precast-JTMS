using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Helpers;
using JTMS.Models;
using JTMS.ViewModels.DialogsVM;
using System.Collections.ObjectModel;
using System.Windows;

namespace JTMS.ViewModels
{
    class HomeViewModel : ObservableObject
    {
        private ObservableCollection<ProjectModel> _projects = new ObservableCollection<ProjectModel>();
        public ObservableCollection<ProjectModel> Projects { get => _projects; set => SetProperty(ref _projects, value); }

        private ObservableCollection<ProjectNotifications> _projectNotifications = new ObservableCollection<ProjectNotifications>();
        public ObservableCollection<ProjectNotifications> ProjectNotifications { get => _projectNotifications; set => SetProperty(ref _projectNotifications, value); }

        public RelayCommand AddCompanyCommand { get; set; }
        public RelayCommand AddProjectCommand { get; set; }
        public IRelayCommand RemoveNotificationCommand { get; set; }
        public IRelayCommand CheckNotificationCommand { get; set; }

        public Action<int> ShowIcon;

        private DataHandler.DataHandler dataHandler = new DataHandler.DataHandler();

        public HomeViewModel()
        {
            AddProjectCommand = new RelayCommand(addProject);
            AddCompanyCommand = new RelayCommand(addCompany);
            RemoveNotificationCommand = new RelayCommand<ProjectNotifications>(removeNotification);
            CheckNotificationCommand = new RelayCommand<ProjectNotifications>(checkNotification);
            WeakReferenceMessenger.Default.Register<DataMessageModel>(this, (r, model) => recieveViewData(model));
        }//

        private void checkNotification(ProjectNotifications notifications)
        {

        }

        private void removeNotification(ProjectNotifications notifications)
        {
            try
            {
                ProjectNotifications.Remove(notifications);
                var notification = dataHandler.context.Notifications.FirstOrDefault(obj => obj.Id == notifications.Id);
                if (notification != null)
                {
                    dataHandler.context.Notifications.Remove(notification);
                    dataHandler.context.SaveChanges();
                }
                ShowIcon?.Invoke(ProjectNotifications.Count);
            }
            catch (Exception err) { }
        }

        private void recieveViewData(DataMessageModel model)
        {
            if (model.Data == "Project-Reload" || model.Data == "Mold-Reload")
            {
                dataHandler = new DataHandler.DataHandler();
                loadProjects();
            }

            else if (model.Data == "Settings-Updated")
            {
                dataHandler = new DataHandler.DataHandler();
                loadProjects();
            }
        }

        public void loadProjects()
        {
            try
            {
                var projects = dataHandler.context.Projects.Where(obj => obj.PercentageCleared < 100).OrderByDescending(obj => obj.ProjectUpdateDate).Take(15);
                Projects = new ObservableCollection<ProjectModel>(projects);

                var projectN = dataHandler.context.Notifications;
                ProjectNotifications = new ObservableCollection<ProjectNotifications>(projectN);
                Application.Current.Dispatcher.Invoke(() => { ShowIcon?.Invoke(ProjectNotifications.Count); });
            }
            catch (Exception err) { }
        }

        private async void addCompany()
        {
            var vm = new AddCompanyDialogViewModel();
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
            if (result is bool bol && bol == true) { }
        }

        private async void addProject()
        {
            var vm = new AddProjectDialogViewModel();
            vm.initCompanyList();
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
            if (result is bool bol && bol == true) { }
        }
    }
}
