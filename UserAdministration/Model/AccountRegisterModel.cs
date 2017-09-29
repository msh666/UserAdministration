namespace UserAdministration.Model
{
    /// <summary>
    /// Model with iformation for user registration
    /// </summary>
    public class AccountRegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
    }
}
