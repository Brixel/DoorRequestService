using System;
using System.Security.Cryptography;
using DoorRequest.API.Config;
using Microsoft.Extensions.Options;

namespace DoorRequest.API.Services
{
    public class AccountKeyService : IAccountKeyService
    {
        private readonly string _systemAccountKey;

        public AccountKeyService(IOptions<AccountKeyConfiguration> accountKeyOptions)
        {
            var accountKeyConfiguration = accountKeyOptions.Value;
            if (accountKeyConfiguration == null)
            {
                throw new ArgumentNullException(nameof(accountKeyConfiguration));
            }

            _systemAccountKey = accountKeyConfiguration.SystemAccountKey;
        }
        public string GetAccountKey(string userName)
        {
            return _systemAccountKey + userName;
        }
    }
}