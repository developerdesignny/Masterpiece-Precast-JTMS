using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Helpers;
using JTMS.Models;
using System.Collections.ObjectModel;

namespace JTMS.ViewModels.DialogsVM
{
    public class ViewMoldViewModel : ObservableObject
    {
        private ObservableCollection<MoldDetailsModel> _molds = new ObservableCollection<MoldDetailsModel>();
        public ObservableCollection<MoldDetailsModel> Molds { get => _molds; set => SetProperty(ref _molds, value); }

        private MoldModel _currentMold;
        public MoldModel CurrentMold { get => _currentMold; set => SetProperty(ref _currentMold, value); }

        private DataHandler.DataHandler dataHandler;
        public IRelayCommand DeleteMoldCommand { get; set; }
        public RelayCommand AddSubMoldCommand { get; set; }

        public ViewMoldViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            DeleteMoldCommand = new RelayCommand<MoldDetailsModel>(model => delteSubMoldData(model));
            AddSubMoldCommand = new RelayCommand(addMoldCommand);
            WeakReferenceMessenger.Default.Register<DataMessageModel>(this, (r, model) => recieveViewData(model));
        }

        private async void addMoldCommand()
        {
            var vm = new AddMoldDialogViewModel();
            vm.IsSubMold = true;
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "SubRootDialogHost");
            if (result is bool bol && bol == true)
            {
                await dataHandler.AddSubMolds(CurrentMold, vm.Mold.MoldAmount);
                WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
            }//
        }

        private async void delteSubMoldData(MoldDetailsModel? model)
        {
            try
            {
                await dataHandler.DeleteSubMold(model, CurrentMold.ProjectId);
                Molds.Remove(model);
                WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
            }
            catch (Exception err) { }
        }

        public async void initMoldList()
        {
            var mold = dataHandler.context.Molds.FirstOrDefault(obj => obj.Id == CurrentMold.Id);
            if (mold != null)
            {
                var data = await dataHandler.GetSubMolds(CurrentMold);
                Molds = new(data);
                if (Molds.Count == 0 && CurrentMold.MoldAmount > 0)
                {
                    await dataHandler.AddSubMolds(CurrentMold);
                    data = await dataHandler.GetSubMolds(CurrentMold);
                    Molds = new(data);
                }
            }//
        }//

        private void recieveViewData(DataMessageModel model)
        {
            if (model.Data == "Mold-Reload")
            {
                dataHandler = new DataHandler.DataHandler();
                initMoldList();
            }

            else if (model.Data == "Settings-Updated")
            {
                dataHandler = new DataHandler.DataHandler();
                initMoldList();
            }
        }//
    }
}
