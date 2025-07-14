using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NPS = Carlytics.Properties.Settings;

namespace Carlytics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Functions
        private string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null) return string.Empty;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            name.Focus();
        }

        private void onClickCancel(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void onClickLogin(object sender, RoutedEventArgs e)
        {
            if(name.Text == NPS.Default.Name && ConvertToUnsecureString(pass.SecurePassword) == NPS.Default.Password)
            {
                ProgramWindow programWindow = new ProgramWindow();
                programWindow.Show();
                this.Close();
            } else 
                MessageBox.Show("Incorrect credentials", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                onClickLogin(sender, e);
                e.Handled = true;
            }
        }
    }
}