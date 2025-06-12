using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Dialogs;
using JTMS.Helpers;
using JTMS.Models;
using Microsoft.EntityFrameworkCore;

namespace JTMS.ViewModels
{
    class ProjectShellViewModel : ObservableObject
    {
        private object currentViewModel = null;
        public object CurrentViewModel { get => currentViewModel; set { SetProperty(ref currentViewModel, value); } }

        private DataHandler.DataHandler dataHandler;

        public ProjectShellViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            CurrentViewModel = new ProjectViewModel();
            WeakReferenceMessenger.Default.Register<DataMessageModel>(this, (r, model) => recieveNavData(model));
            WeakReferenceMessenger.Default.Register<ProjectShellNavModel>(this, (r, model) => recieveShellNavData(model));
        }

        private void recieveShellNavData(ProjectShellNavModel model)
        {
            if (model.Data == "PShellNav-Project")
                CurrentViewModel = new ProjectViewModel();

            if (model.Data == "PShellNav-Mold")
            {
                var moldViewModel = new MoldViewModel();
                moldViewModel.CurrentProject = (ProjectModel)model.CurrentProject;
                moldViewModel.LoadMoldData();
                CurrentViewModel = moldViewModel;
            }
        }

        public async void serialDataR(string data, ScanMode scanMode)
        {
            if (string.IsNullOrEmpty(data))
                return;
            try
            {

                //data = "Proj 2:000AO-2\r";
                //data = "Proj 2:PF\r";
                var projectName = data.Split(":")[0].Trim();
                var currentProject = dataHandler.context.Projects.FirstOrDefault(obj => obj.JobName.Trim() == projectName);

                if (data.Contains("PF"))
                {
                    var res = await dataHandler.UpdatePallet(currentProject.Id);
                    if (res == false)
                        new MessageWin("error", $"No Free pallet was found");
                }//
                else
                {
                    var code = data.Split(":")[1].Replace("\r", "").Replace("\n", "");
                    var sMolds = dataHandler.context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Mold.ProjectId == currentProject.Id).ToList();
                    var mold = sMolds.FirstOrDefault(obj => obj.MCode == code.Trim());

                    //if (mold.ProcessCompleteCount == mold.MoldAmount)
                    //    new MessageWin("error", $"No avaibale molds for this process.Mold amount = {mold.MoldAmount}");

                    var updated = false;

                    if (scanMode == ScanMode.Process1)
                    {
                        if (mold.Process1Complete == true) { }
                        //new MessageWin("error", "Process 1 already completed");
                        else
                        {
                            mold.Process1Complete = true;
                            await updateReport(mold, currentProject);
                            updated = true;
                        }
                    }
                    else if (scanMode == ScanMode.Process2)
                    {
                        if (mold.Process1Complete == false)
                        { }
                        else if (mold.Process2Complete == true)
                        { }
                        else
                        {
                            mold.Process2Complete = true;
                            await updateReport(mold, currentProject);
                            updated = true;
                        }
                    }
                    else if (scanMode == ScanMode.Process3)
                    {
                        if (mold.Process1Complete == false)
                        { }
                        else if (mold.Process3Complete == true)
                        { }
                        else
                        {
                            mold.Process3Complete = true;
                            await updateReport(mold, currentProject);
                            updated = true;
                        }
                    }

                    else if (scanMode == ScanMode.Process4)
                    {
                        if (mold.Process1Complete == false && mold.Process3Complete == false)
                        { }
                        else if (mold.Process4Complete == true)
                        { }
                        else
                        {
                            mold.Process4Complete = true;
                            await updateReport(mold, currentProject);
                            updated = true;
                        }
                    }

                    else if (scanMode == ScanMode.Process5)
                    {
                        var notification = new ProjectNotifications();
                        notification.NotificationT = $"{currentProject.JobName}-{code} mold broken";
                        notification.SubMoldId = mold.Id;
                        dataHandler.context.Notifications.Add(notification);
                        dataHandler.context.SaveChanges();
                        WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
                    }
                    else if (scanMode == ScanMode.Process6)
                    {
                        var notification = new ProjectNotifications();
                        notification.NotificationT = $"{currentProject.JobName}-{code} stone broken";
                        notification.SubMoldId = mold.Id;
                        dataHandler.context.Notifications.Add(notification);
                        dataHandler.context.SaveChanges();
                        WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
                    }

                    if (updated == true)
                    {
                        _ = Task.Run(async () =>
                        {
                            await dataHandler.UpdateMoldProcess(mold);
                            WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
                        });
                    }
                }//
            }
            catch (Exception err) { }
        }

        private async Task updateReport(MoldDetailsModel mold, ProjectModel currentProject)
        {
            await dataHandler.AddProgressReport(new ProgressReportModel
            {
                MoldCode = mold.MCode,
                MoldProcess = mold.CurrentProcess,
                Project = currentProject,
                ProjectId = currentProject.Id,
                ProcessDate = DateOnly.FromDateTime(DateTime.Now)
            });
        }

        private void recieveNavData(DataMessageModel model)
        {
            if (model.Data == "Nav-Project")
                CurrentViewModel = new ProjectViewModel();
        }
    }
}
