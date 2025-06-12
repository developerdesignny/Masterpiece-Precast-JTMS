using JTMS.ViewModels;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;

namespace JTMS
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            InitializeComponent();
            Loaded += ShellWindow_Loaded;
        }

        private void ShellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ShellViewModel;
            vm.homeViewModel.ShowIcon += showNotification;
            vm.homeViewModel.loadProjects();
        }

        public void showNotification(int notificationCount)
        {
            var tbi = new TaskbarItemInfo();
            if (notificationCount > 0)
            {
                var dg = new DrawingGroup();
                var dc = dg.Open();
                dc.DrawEllipse(Brushes.Orange, new Pen(Brushes.Orange, 1), new Point(notificationCount <= 9 ? 8 : 20, 15), 22, 22);
                dc.DrawText(new FormattedText($"{notificationCount}", System.Threading.Thread.CurrentThread.CurrentUICulture, System.Windows.FlowDirection.LeftToRight,
                    new Typeface("Arial"), 25, Brushes.White), new Point(3, 0));
                dc.Close();
                var geometryImage = new DrawingImage(dg);
                geometryImage.Freeze();

                tbi.Overlay = geometryImage;
            }
            this.TaskbarItemInfo = tbi;
        }
    }
}
