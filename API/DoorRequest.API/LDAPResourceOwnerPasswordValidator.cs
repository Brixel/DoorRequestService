using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;

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
            _logger.LogInformation($"Searching for user '{context.UserName}' in group '{groupName}'");
            using (var connection = new LdapConnection())
            {
                connection.Connect(ldapConfig.Url, ldapConfig.Port);
                connection.Bind(ldapConfig.BindDn, ldapConfig.BindCredentials);

                var formattedUserFilter = string.Format(ldapConfig.SearchUserFilter, context.UserName);
                var formattedGroupFilter = string.Format(ldapConfig.SearchGroupFilter, context.UserName);

                var userResults = connection.Search(
                    ldapConfig.BaseDN, 
                    LdapConnection.ScopeSub,
                    formattedUserFilter,
                    new[] { ATTR_PASSWORD, ATTR_COMMON_NAME, ATTR_OBJECTCLASS }, 
                    false);

                LDAPUser ldapUser = null;
                while (userResults.HasMore())
                {
                    var nextEntry = userResults.Next();
                    var cn = nextEntry.GetAttribute(ATTR_COMMON_NAME);
                    var password = nextEntry.GetAttribute(ATTR_PASSWORD);
                    if (cn != null)
                    {
                        ldapUser = new LDAPUser()
                        {
                            UserName = cn.StringValue,
                            Password = password.StringValue
                        };
                    }
                }

                if (ldapUser == null)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                    return Task.CompletedTask;
                }
                ldapUser.Validate(context.Password);

                var groupResults = connection.Search(
                    ldapConfig.BaseDN, 
                    LdapConnection.ScopeSub, 
                    formattedGroupFilter, 
                    null, 
                    false);

                var groups = new List<string>();
                while (groupResults.HasMore())
                {
                    var nextEntry = groupResults.Next();
                    var cn = nextEntry.GetAttribute(ATTR_COMMON_NAME);
                    if (cn != null)
                    {
                        groups.Add(cn.StringValue);
                    }
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
                    _logger.LogInformation($"User '{context.UserName}' is valid and in correct group");
                    context.Result = new GrantValidationResult(subject: context.UserName,
                        OidcConstants.AuthenticationMethods.Password, claims);
                }

                return Task.CompletedTask;
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