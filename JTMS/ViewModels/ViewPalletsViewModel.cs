using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Helpers;
using JTMS.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace JTMS.ViewModels
{
    public class ViewPalletsViewModel : ObservableObject
    {
        private ObservableCollection<PalletModel> _pallets;
        public ObservableCollection<PalletModel> Pallets { get => _pallets; set => SetProperty(ref _pallets, value); }

        private DataHandler.DataHandler dataHandler;

        public IRelayCommand UndoPalletCommand { get; set; }
        public IRelayCommand DeletePalletCommand { get; set; }

        public ViewPalletsViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            UndoPalletCommand = new RelayCommand<PalletModel>(obj => undoPallet(obj));
            DeletePalletCommand = new RelayCommand<PalletModel>(obj => deletePallet(obj));
        }

        private async void deletePallet(PalletModel pallet)
        {
            var subMolds = pallet.MoldIds.Split(",");
            if (subMolds.Length > 0)
            {
                foreach (var m in subMolds)
                {
                    if (string.IsNullOrEmpty(m)) continue;
                    try
                    {
                        var mold = dataHandler.context.SubMolds.FirstOrDefault(obj => obj.Id.ToString() == m);
                        mold.Process4Complete = false;
                        mold.PalletID = "";
                        await dataHandler.UpdateProject(mold, true);
                    }
                    catch (Exception err) { Debug.WriteLine(err.Message); }
                }
            }
            dataHandler.context.Pallets.Remove(pallet);
            Pallets.Remove(pallet);
            dataHandler.context.SaveChanges();
            WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
        }

        private async void undoPallet(PalletModel pallet)
        {
            var pModel = dataHandler.context.Pallets.FirstOrDefault(obj => obj.Id == pallet.Id);
            pModel.IsFilled = false;
            dataHandler.context.SaveChanges();
        }
        public void LoadMoldData(Guid projectID)
        {
            Task.Run(async () =>
            {
                var data = await dataHandler.GetPallets(1, projectID);
                Application.Current.Dispatcher.Invoke(() => { Pallets = new ObservableCollection<PalletModel>(data); });
            });
        }//

    }
}
