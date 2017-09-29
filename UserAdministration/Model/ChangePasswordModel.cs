namespace UserAdministration.Model
{
    /// <summary>
    /// Model with information about password changing.
    /// </summary>
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
