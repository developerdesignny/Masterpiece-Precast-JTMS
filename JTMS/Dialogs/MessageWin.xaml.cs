using System.Windows;

namespace JTMS.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageWin.xaml
    /// </summary>
    public partial class MessageWin : Window
    {
        public MessageWin(string title, string mess, int width = 400, int height = 130)
        {
            InitializeComponent();
            PTitle.Text = title;
            PText.Text = mess;
            this.Width = width;
            this.Height = height;
            ShowDialog();
        }

        private void Closebtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
