using System.Windows;

namespace JTMS.Dialogs
{
    /// <summary>
    /// Interaction logic for ProjectNotesWin.xaml
    /// </summary>
    public partial class ProjectNotesWin : Window
    {
        public bool Issaved = false;
        public ProjectNotesWin()
        {
            InitializeComponent();
        }
        private void saveNote_Click(object sender, RoutedEventArgs e)
        {
            Issaved = true;
            Close();
        }
    }
}
