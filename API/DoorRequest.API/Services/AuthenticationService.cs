using System.Security.Cryptography;

namespace DoorRequest.API.Services
{
    public static class AuthenticationService
    {
        private static readonly string UniqueKey = "UniqueKey";

        public static string GetAccountKey(string userName)
        {
            return UniqueKey + userName;
        }
    }
}