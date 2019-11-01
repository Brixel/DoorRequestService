using System;
using System.DirectoryServices;

namespace DoorRequest.LDAPConnectionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string ldapServer = "LDAP://localhost:389/dc=contoso,dc=com";
            string userName = "uid=TestUserA,ou=people,dc=contoso,dc=com";
            string password = "TestUserA";

            Console.WriteLine("Attempting to connect");
            var dirctoryEntry = new DirectoryEntry(ldapServer);

            try
            {
                Console.WriteLine("Who am I?");
                Console.WriteLine(dirctoryEntry.Name);
                object nativeObject = dirctoryEntry.NativeObject;
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
