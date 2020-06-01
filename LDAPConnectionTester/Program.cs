using System;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;

namespace DoorRequest.LDAPConnectionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string ldapServer = "";
            string bindDN = "";
            string bindDNPassword = "";

            var username = "";
            var password = "";
            Console.WriteLine("Attempting to connect");
            try
            {
                using DirectoryEntry de = new DirectoryEntry(ldapServer, bindDN, bindDNPassword, AuthenticationTypes.None);
                DirectorySearcher searcher = new DirectorySearcher(de)
                {
                    PageSize = int.MaxValue,
                    Filter = $"(&(uid={username}))"
                };
                var result = searcher.FindOne();

                if (result != null)
                {
                    var entry = result.GetDirectoryEntry();
                    Console.WriteLine(entry);
                    var bytes = result.Properties["userPassword"][0] as byte[];
                    var byteString = Encoding.UTF8.GetString(bytes);
                    Console.WriteLine(byteString);
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
