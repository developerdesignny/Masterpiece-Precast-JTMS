using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Dialogs;
using JTMS.Helpers;
using JTMS.Models;
using MaterialDesignThemes.Wpf;

namespace JTMS.ViewModels.DialogsVM
{
    class AddMoldDialogViewModel : ObservableObject
    {
        private MoldModel _mold = new MoldModel();
        public MoldModel Mold { get => _mold; set => SetProperty(ref _mold, value); }

        public ProjectModel CurrentProject = new ProjectModel();

        private bool _isEditing = false;
        public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

        private bool _isSubMold = false;
        public bool IsSubMold { get => _isSubMold; set => SetProperty(ref _isSubMold, value); }


        public RelayCommand SaveCommand { get; set; }
        private DataHandler.DataHandler dataHandler;

        public AddMoldDialogViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            SaveCommand = new RelayCommand(saveCMD);
        } //

        private async void saveCMD()
        {
            if (IsSubMold == true)
                DialogHost.CloseDialogCommand.Execute(true, null);
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(Mold.MoldSize) && !string.IsNullOrEmpty(Mold.MoldCode))
                    {
                        if (IsEditing)
                        {
                            var mold = dataHandler.context.Molds.FirstOrDefault(obj => obj.ProjectId == CurrentProject.Id && obj.MoldCode == Mold.MoldCode && obj.Id != Mold.Id);
                            if (mold != null)
                                new MessageWin("error", "mold code already exists");
                            else
                            {
                                await dataHandler.EditMold(Mold);
                                WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
                                WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Nav-Mold" });
                                DialogHost.CloseDialogCommand.Execute(null, null);
                            }
                        }
                        else
                        {
                            var mold = dataHandler.context.Molds.FirstOrDefault(obj => obj.ProjectId == CurrentProject.Id && obj.MoldCode == Mold.MoldCode);
                            if (mold != null)
                                new MessageWin("error", "mold code already exists");
                            else
                            {
                                Mold.ProjectId = CurrentProject.Id;
                                await dataHandler.AddMold(Mold);
                                WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
                                WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Nav-Mold" });
                                DialogHost.CloseDialogCommand.Execute(null, null);
                            }
                        }
                    }
                    else
                        new MessageWin("error", "Mold size and mold code required");
                }
                catch (Exception)
                {
                    new MessageWin("error", "Make sure you are connected to the database.");
                }
            }//
        }//

    }
}
