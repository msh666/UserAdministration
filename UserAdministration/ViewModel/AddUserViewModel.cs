using System.Linq;
using System.Net;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using RestSharp;
using UserAdministration.Helpers;
using UserAdministration.Model;

namespace UserAdministration.ViewModel
{
    /// <summary>
    /// ViewModel for AddNewUser.xaml
    /// </summary>
    public class AddUserViewModel : BaseViewModel
    {
        private readonly string _token;
        private string _errorMessage;
        private AccountRegisterModel _newUser = new AccountRegisterModel();

        /// <summary>
        /// Command that trigger on "Add" button click and start User Registration process
        /// </summary>
        public RelayCommand AddNewUserCommand { get; set; }

        public AccountRegisterModel NewUser
        {
            get { return _newUser; }
            set
            {
                _newUser = value;
                OnPropertyChanged("NewUser");
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

        public AddUserViewModel(string token)
        {
            _token = token;
            AddNewUserCommand = new RelayCommand(AddNewUser);
        }

        /// <summary>
        /// Process of User registration
        /// </summary>
        /// <param name="parameter">Password and confirmation income as a parameter</param>
        public void AddNewUser(object parameter)
        {
            PasswordConvert(parameter);
            if (!IsVerified()) return;
            if (NewUser.PhoneNumber.Length < 15)
            {
                ErrorMessage = "Please fill phone number field";
            }
            else
            {
                var client = new RestClient(Properties.Settings.Default.BaseUrl);
                var request = new RestRequest("/api/Account/Register", Method.POST);
                request.AddHeader("Authorization", "Bearer " + _token);
                request.AddObject(NewUser);
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var jsonContent = JObject.Parse(response.Content);
                    ErrorMessage = jsonContent["ModelState"].First.First.First.ToString();
                }
                else
                {
                    CloseWindow();
                }
            }
        }

        /// <summary>
        /// Method to convert passwords from object to ChangePasswordModel
        /// </summary>
        /// <param name="parameter">Passwords come as a parameter</param>
        public void PasswordConvert(object parameter)
        {
            var passwords = (object[])parameter;
            var pass = passwords[0] as PasswordBox;
            var confPass = passwords[1] as PasswordBox;
            NewUser.Password = pass.Password;
            NewUser.ConfirmPassword = confPass.Password;
        }

        /// <summary>
        /// Method for verification onput data
        /// </summary>
        /// <returns>Is everything correct or no</returns>
        public bool IsVerified()
        {
            if (NewUser.GetType().GetProperties()
                .Where(pi => pi.GetValue(NewUser) is string)
                .Select(pi => (string)pi.GetValue(NewUser))
                .Any(string.IsNullOrEmpty))
            {
                ErrorMessage = "Please, fill in all fields!";
                return false;
            }
            if (NewUser.Password != NewUser.ConfirmPassword)
            {
                ErrorMessage = "Passwords does not match";
                return false;
            }
            return true;
        }
    }
}
