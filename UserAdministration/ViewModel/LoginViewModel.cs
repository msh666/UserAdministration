using System.Net;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using RestSharp;
using UserAdministration.Helpers;

namespace UserAdministration.ViewModel
{
    /// <summary>
    /// ViewModel for LogIn.xaml
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _errorMessage;

        /// <summary>
        /// Command that trigger on "LogIn" button click and start login process
        /// </summary>
        public RelayCommand LoginCommand { get; set; }
        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("UserName");
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        /// <summary>
        /// Logic for LogIn process. If success server will give token for verification
        /// </summary>
        /// <param name="parameter">password come as a parameter from UI</param>
        public void Login(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var client = new RestClient(Properties.Settings.Default.BaseUrl);
            var request = new RestRequest("/Token", Method.POST);
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", UserName);
            request.AddParameter("password", passwordBox.Password);
            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                ErrorMessage = "Имя пользователя или пароль указаны не верно";
            }
            else
            {
                var content = response.Content;
                var jsonContent = JObject.Parse(content);
                var token = jsonContent.First.First.ToString();
                var ul = new UsersList { DataContext = new UserViewModel(token, UserName) };
                ul.Show();
                CloseWindow();
            }
        }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }
    }
}
