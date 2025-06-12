using JTMS.ViewModels;
using System.Windows.Controls;

namespace JTMS.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var vm = DataContext as SettingsViewModel;
            var ports = vm.GetAvailablePorts();
            var combo = (ComboBox)sender;

            if (combo.Name == "cPort1")
            {
                combo.SelectedValue = vm.Port1;
                ports.Remove(vm.Port2); ports.Remove(vm.Port3); ports.Remove(vm.Port4_2);
                ports.Remove(vm.Port4); ports.Remove(vm.Port5); ports.Remove(vm.Port6);
            }

            if (combo.Name == "cPort2")
            {
                combo.SelectedValue = vm.Port2;
                ports.Remove(vm.Port1); ports.Remove(vm.Port3); ports.Remove(vm.Port4_2);
                ports.Remove(vm.Port4); ports.Remove(vm.Port5); ports.Remove(vm.Port6);
            }

            if (combo.Name == "cPort3")
            {
                combo.SelectedValue = vm.Port3;
                ports.Remove(vm.Port2); ports.Remove(vm.Port1); ports.Remove(vm.Port4_2);
                ports.Remove(vm.Port4); ports.Remove(vm.Port5); ports.Remove(vm.Port6);
            }

            if (combo.Name == "cPort4")
            {
                combo.SelectedValue = vm.Port4;
                ports.Remove(vm.Port2); ports.Remove(vm.Port3); ports.Remove(vm.Port4_2);
                ports.Remove(vm.Port1); ports.Remove(vm.Port5); ports.Remove(vm.Port6);
            }

            if (combo.Name == "cPort4_2")
            {
                combo.SelectedValue = vm.Port4_2;
                ports.Remove(vm.Port2); ports.Remove(vm.Port3); ports.Remove(vm.Port4);
                ports.Remove(vm.Port1); ports.Remove(vm.Port5); ports.Remove(vm.Port6);
            }

            if (combo.Name == "cPort5")
            {
                combo.SelectedValue = vm.Port5;
                ports.Remove(vm.Port2); ports.Remove(vm.Port3); ports.Remove(vm.Port4_2);
                ports.Remove(vm.Port4); ports.Remove(vm.Port1); ports.Remove(vm.Port6);
            }

            if (combo.Name == "cPort6")
            {
                combo.SelectedValue = vm.Port6;
                ports.Remove(vm.Port2); ports.Remove(vm.Port3); ports.Remove(vm.Port4_2);
                ports.Remove(vm.Port4); ports.Remove(vm.Port5); ports.Remove(vm.Port1);
            }

            combo.ItemsSource = ports;
        }//
    }
}
