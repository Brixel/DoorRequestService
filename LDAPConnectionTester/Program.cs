using System;
using Novell.Directory.Ldap;

namespace DoorRequest.LDAPConnectionTester
{
    class Program
    {
        private const string ATTR_COMMON_NAME = "cn";
        private const string ATTR_PASSWORD = "userPassword";
        private const string ATTR_OBJECTCLASS = "objectclass";
        static void Main(string[] args)
        {
            string ldapServer = "10.0.0.13";
            string bindDN = "cn=admin,dc=contoso,dc=com";
            string bindDNPassword = "P@ss1W0Rd!";

            var username = "";
            var password = "";
            Console.WriteLine("Attempting to connect");
            try
            {
                using (var connection = new LdapConnection())
                {
                    connection.Connect(ldapServer, 389);
                    //connection.Connect(new Uri($"LDAP://{ldapServer}:389/dc=contoso,dc=com"));
                    //connection.Bind(Native.LdapAuthMechanism.SIMPLE, bindDN, bindDNPassword);
                    connection.Bind(bindDN, bindDNPassword);

                    var userResults = 
                        connection.Search(
                            "dc=contoso,dc=com", 
                            LdapConnection.ScopeSub,
                            string.Format("(&(objectclass=posixAccount)(uid={0}))", "Berend"),
                            new[] { ATTR_PASSWORD, ATTR_COMMON_NAME, ATTR_OBJECTCLASS }, 
                            false);

                    while (userResults.HasMore())
                    {
                        var nextEntry = userResults.Next();
                        var cn = nextEntry.GetAttribute(ATTR_COMMON_NAME);
                        var userpassword = nextEntry.GetAttribute(ATTR_PASSWORD);
                        Console.WriteLine(cn);
                    }
                }
                
                //connection.Bind();
                //Console.WriteLine(dirctoryEntry.Name);
                //object nativeObject = dirctoryEntry.NativeObject;
                //Rest of the logic
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Handle error
            }

            Console.ReadLine();
        }
    }
}
