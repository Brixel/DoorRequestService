namespace DoorRequest.API
{
    public class LDAPConnectionOptions
    {
        public string Url { get; set; }

        public int Port { get; set; }

        public string BaseDN { get; set; }
        public string BindDn { get; set; }

        public string BindCredentials { get; set; }

        public string LDAPGroupName { get; set; }

        public string SearchUserFilter { get; set; }
        public string SearchGroupFilter { get; set; }
    }
}