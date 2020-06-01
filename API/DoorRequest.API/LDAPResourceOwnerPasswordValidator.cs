using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace DoorRequest.API
{
    public class LDAPResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private const string ATTR_COMMON_NAME = "cn";
        private const string ATTR_PASSWORD = "userPassword";
        private const string ATTR_OBJECTCLASS = "objectclass";
        private readonly IConfiguration _configuration;
        private readonly ILogger<LDAPResourceOwnerPasswordValidator> _logger;

        public LDAPResourceOwnerPasswordValidator(IConfiguration configuration, ILogger<LDAPResourceOwnerPasswordValidator> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var ldapConfig = _configuration.GetSection("Authentication:LDAPConnectionOptions").Get<LDAPConnectionOptions>();
            var groupName = ldapConfig.LDAPGroupName;
            using (DirectoryEntry de = new DirectoryEntry($"LDAP://{ldapConfig.Url}/{ldapConfig.BaseDN}",
                ldapConfig.BindDn, ldapConfig.BindCredentials, AuthenticationTypes.None))
            {
                try
                {
                    var formattedUserFilter = string.Format(ldapConfig.SearchUserFilter, context.UserName);
                    var formattedGroupFilter = string.Format(ldapConfig.SearchGroupFilter, context.UserName);

                    DirectorySearcher searcher = new DirectorySearcher(de)
                    {
                        PageSize = int.MaxValue,
                        Filter = formattedUserFilter
                    };
                    var result = searcher.FindOne();
                    LDAPUser ldapUser = null;
                    if (result == null)
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                        return Task.CompletedTask;
                    }

                    var commonName = result.Properties[ATTR_COMMON_NAME][0].ToString();
                    var bytes = result.Properties[ATTR_PASSWORD][0] as byte[];
                    var byteString = Encoding.UTF8.GetString(bytes);
                    ldapUser = new LDAPUser()
                    {
                        UserName = commonName,
                        Password = byteString
                    };
                    ldapUser.Validate(context.Password);

                    DirectorySearcher groupSearcher = new DirectorySearcher(de)
                    {
                        PageSize = int.MaxValue,
                        Filter = formattedGroupFilter
                    };

                    var groups = new List<string>();
                    var groupResults = groupSearcher.FindAll();
                    for (int i = 0; i < groupResults.Count; i++)
                    {
                        var group = groupResults[i];
                        groups.Add(group.Properties[ATTR_COMMON_NAME][0].ToString());
                    }

                    if (!groups.Contains(groupName))
                    {

                        _logger.LogInformation($"LDAPUser {context.UserName} not in expected group '{groupName}'");
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                    }
                    else
                    {
                        var claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.Subject, context.UserName)
                    };

                        context.Result = new GrantValidationResult(subject: context.UserName,
                            OidcConstants.AuthenticationMethods.Password, claims);
                    }

                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                    return Task.FromResult(false);
                }
            }
        }

    }

    public class FileUser
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public static FileUser Create(string username, string password)
        {
            return new FileUser()
            {
                Username = username,
                Password = password
            };
        }

        public void Validate(string password)
        {
            if (password != Password)
            {
                throw new Exception("Passwords don't match");
            }
        }
    }
}