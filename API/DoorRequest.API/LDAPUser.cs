namespace DoorRequest.API
{
    public class LDAPUser
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public void Validate(string contextPassword)
        {
            OpenLDAPPasswordHelpers.Compare(contextPassword, Password);
        }
    }
}