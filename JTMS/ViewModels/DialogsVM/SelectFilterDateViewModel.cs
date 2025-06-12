using CommunityToolkit.Mvvm.ComponentModel;

namespace JTMS.ViewModels.DialogsVM
{
    public class SelectFilterDateViewModel : ObservableObject
    {
        private DateTime _date = DateTime.Now;
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }
    }
}
