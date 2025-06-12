using JTMS.ViewModels.DialogsVM;
using System.Windows;

namespace JTMS.Dialogs
{
    /// <summary>
    /// Interaction logic for ViewMolds.xaml
    /// </summary>
    public partial class ViewMolds : Window
    {
        public ViewMolds(ViewMoldViewModel viewMold)
        {
            InitializeComponent();
            DataContext = viewMold;
        }
    }
}
