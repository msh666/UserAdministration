using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;
using UserAdministration.Helpers;
using UserAdministration.Model;
using UserAdministration.View;

namespace UserAdministration.ViewModel
{
    /// <summary>
    /// ViewModel for UsersList.xaml
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        private AccountUser _selectedUser;
        private readonly string _currentUser;
        private string _token;
        private string _errorMessage;
        private ObservableCollection<AccountUser> _users;
        private readonly RestClient _client = new RestClient(Properties.Settings.Default.BaseUrl);

        /// <summary>
        /// Command that trigger on "Add" button click and open new window for user registration 
        /// </summary>
        public RelayCommand OpenNewUserCommand { get; set; }
        /// <summary>
        /// Command that trigger on "Save" button click and start updating user information process 
        /// </summary>
        public RelayCommand UpdateUserCommand { get; set; }
        /// <summary>
        /// Command that trigger on "Delete" button click and start user deleting process 
        /// </summary>
        public RelayCommand DeleteUserCommand { get; set; }
        /// <summary>
        /// Command that trigger on "Change password" button click and Open new window for password changing 
        /// </summary>
        public RelayCommand ChangePassCommand { get; set; }

        public AccountUser SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
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

        public ObservableCollection<AccountUser> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged("Users");
            }
        }

        public UserViewModel()
        {
            Connect();
            GetUsersList();
            OpenNewUserCommand = new RelayCommand(OpenNewUserWindow);
            UpdateUserCommand = new RelayCommand(UpdateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            ChangePassCommand = new RelayCommand(ChangePass);
        }

        public UserViewModel(string token, string currentUser)
        {
            _token = token;
            _currentUser = currentUser;
            GetUsersList();
            OpenNewUserCommand = new RelayCommand(OpenNewUserWindow);
            UpdateUserCommand = new RelayCommand(UpdateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            ChangePassCommand = new RelayCommand(ChangePass);
        }

        /// <summary>
        /// Method for application testing. Use defauld info for login process
        /// </summary>
        public void Connect()
        {
            var request = new RestRequest("/Token", Method.POST);
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", "msh3000@yandex.ru");
            request.AddParameter("password", "password");

            var response = _client.Execute(request);
            var content = response.Content;
            var jsonContent = JObject.Parse(content);
            _token = jsonContent.First.First.ToString();
        }

        /// <summary>
        /// Method that take all users from server
        /// </summary>
        public void GetUsersList()
        {
            var request = new RestRequest("/api/Account/GetUsers", Method.GET);
            request.AddHeader("Authorization", "Bearer " + _token);
            var response = _client.Execute(request);
            var content = response.Content;

            Users = new ObservableCollection<AccountUser>();
            dynamic stuff = JArray.Parse(content);
            foreach (var user in stuff)
            {
                Users.Add(new AccountUser()
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName
                });
            }
        }

        /// <summary>
        /// Method thet open AddNewUser.xaml window
        /// </summary>
        /// <param name="parameter"></param>
        public void OpenNewUserWindow(object parameter)
        {
            var addNew = new AddNewUser { DataContext = new AddUserViewModel(_token) };
            addNew.ShowDialog();
            GetUsersList();
        }

        /// <summary>
        /// Method that update information about user
        /// </summary>
        /// <param name="parameter"></param>
        public void UpdateUser(object parameter)
        {
            if(!IsSaveVerified()) return;
            var request = new RestRequest("/api/Account/Update/" + SelectedUser.Id, Method.PUT);
            request.AddHeader("Authorization", "Bearer " + _token);
            request.AddObject(SelectedUser);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                var jsonContent = JObject.Parse(response.Content);
                ErrorMessage = jsonContent.First.First.ToString();
            }
            else
            {
                ErrorMessage = "";
            }
        }

        /// <summary>
        /// Method that delete user from system
        /// </summary>
        /// <param name="parameter"></param>
        public void DeleteUser(object parameter)
        {
            if (SelectedUser == null)
            {
                ErrorMessage = "Please select User!";
            }
            else
            {
                var request = new RestRequest("/api/Account/DeleteUser/" + SelectedUser.Id, Method.DELETE);
                request.AddHeader("Authorization", "Bearer " + _token);
                var response = _client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var jsonContent = JObject.Parse(response.Content);
                    ErrorMessage = jsonContent.First.First.ToString();
                }
                else
                {
                    ErrorMessage = "";
                    if (_currentUser == SelectedUser.UserName)
                    {
                        LogOut();
                    }
                    else
                    {
                        GetUsersList();
                    }
                }
            }
        }

        /// <summary>
        /// Method that open ChangePassword.xaml window
        /// </summary>
        /// <param name="parameter"></param>
        public void ChangePass(object parameter)
        {
            var changePass = new ChangePassword { DataContext = new ChangePasswordViewModel(_token) };
            changePass.ShowDialog();
        }

        /// <summary>
        /// Method that check if all information for saving are correct
        /// </summary>
        /// <returns>Return true if everything OK and false if smth wrong</returns>
        public bool IsSaveVerified()
        {
            if (SelectedUser == null)
            {
                ErrorMessage = "Please select User!";
                return false;
            }
            var userNameCount = Users.Count(x => SelectedUser.UserName.Contains(x.UserName));
            if (userNameCount > 1)
            {
                ErrorMessage = "This User Name already exist!";
                return false;
            }
            var emailCount = Users.Count(x => SelectedUser.Email.Contains(x.Email));
            if (emailCount > 1)
            {
                ErrorMessage = "This Email already exist";
                return false;
            }
            if (SelectedUser.PhoneNumber.Length < 15)
            {
                ErrorMessage = "Please fill phone number field";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Log out from system and move to LogIn window
        /// </summary>
        public void LogOut()
        {
            _token = "";
            var addNew = new LogIn();
            addNew.Show();
            CloseWindow();
        }
    }
}
