using System;
using System.Collections.Generic;
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
    /// View model for ChangePassword.xaml
    /// </summary>
    public class ChangePasswordViewModel : BaseViewModel
    {
        private readonly string _token;
        private string _errorMessage;

        /// <summary>
        /// Command that trigger on "Change" button click and start Password changing process 
        /// </summary>
        public RelayCommand ChangeCommand { get; set; }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }
        public ChangePasswordViewModel(string token)
        {
            _token = token;
            ChangeCommand = new RelayCommand(ChangePassword);
        }

        /// <summary>
        /// Logic for password changing. If it's finished successfull then password will be changed. Or you will see exception message.
        /// </summary>
        /// <param name="parameter">Old, new and confirmation passwords come as a parameter from view</param>
        public void ChangePassword(object parameter)
        {
            var newPassword = PasswordConvert(parameter);
            if (!IsVerified(newPassword)) return;
            var client = new RestClient(Properties.Settings.Default.BaseUrl);
            var request = new RestRequest("/api/Account/ChangePassword", Method.POST);
            request.AddObject(newPassword);
            request.AddHeader("Authorization", "Bearer " + _token);
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

        /// <summary>
        /// Method to convert passwords from object to ChangePasswordModel
        /// </summary>
        /// <param name="parameter">Passwords come as a parameter</param>
        /// <returns>Model with filled passwords</returns>
        public ChangePasswordModel PasswordConvert(object parameter)
        {
            var passwords = (object[])parameter;
            var oldPass = passwords[0] as PasswordBox;
            var newPass = passwords[1] as PasswordBox;
            var confPass = passwords[2] as PasswordBox;
            return new ChangePasswordModel {OldPassword = oldPass.Password, NewPassword = newPass.Password, ConfirmPassword = confPass.Password};
        }

        /// <summary>
        /// Method for verification onput data
        /// </summary>
        /// <param name="passwords">ChangePasswordModel object came as password parameter</param>
        /// <returns>Is everything correct or not</returns>
        public bool IsVerified(ChangePasswordModel passwords)
        {
            if (passwords.GetType().GetProperties()
                .Where(pi => pi.GetValue(passwords) is string)
                .Select(pi => (string)pi.GetValue(passwords))
                .Any(string.IsNullOrEmpty))
            {
                ErrorMessage = "Please, fill in all fields!";
                return false;
            }
            if (passwords.NewPassword != passwords.ConfirmPassword)
            {
                ErrorMessage = "Passwords does not match";
                return false;
            }
            return true;
        }
    }
}
