using JTMS.ViewModels;
using System.Windows;

namespace JTMS.Dialogs
{
    /// <summary>
    /// Interaction logic for ViewPallets.xaml
    /// </summary>
    public partial class ViewPallets : Window
    {
        public ViewPallets(ViewPalletsViewModel viewPalletsViewModel)
        {
            InitializeComponent();
            DataContext = viewPalletsViewModel;
        }
    }
}
