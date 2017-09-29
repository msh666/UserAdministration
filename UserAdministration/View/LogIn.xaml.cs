using System.Windows;
using UserAdministration.ViewModel;

namespace UserAdministration
{
    /// <summary>
    /// Логика взаимодействия для LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();

            DataContext = new LoginViewModel();
        }
    }
}
