using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;

namespace DoorRequest.API
{
    public class LDAPResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private const string ATTR_COMMON_NAME = "cn";
        private const string ATTR_PASSWORD = "userPassword";
        private readonly IConfiguration _configuration;

        public LDAPResourceOwnerPasswordValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var ldapConfig = _configuration.GetSection("LDAPConnection").Get<LDAPConnection>();
            var groupName = ldapConfig.LDAPGroupName;

            LdapConnection lc = new LdapConnection();
            try
            {
                lc.Connect(ldapConfig.Url, ldapConfig.Port);
                lc.Bind(ldapConfig.BindDn, ldapConfig.BindCredentials);


                var formattedUserFilter = string.Format(ldapConfig.SearchUserFilter, context.UserName);
                var formattedGroupFilter = string.Format(ldapConfig.SearchGroupFilter, context.UserName);
                
                var userResults = lc.Search(ldapConfig.BaseDN, LdapConnection.SCOPE_SUB, 
                    formattedUserFilter, new[] {ATTR_PASSWORD, ATTR_COMMON_NAME}, false);
                LDAPUser ldapUser = null;
                while (userResults.HasMore())
                {
                    var nextEntry = userResults.Next();
                    var cn = nextEntry.getAttribute(ATTR_COMMON_NAME);
                    var password = nextEntry.getAttribute(ATTR_PASSWORD);
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


                var groupResults = lc.Search(ldapConfig.BaseDN, LdapConnection.SCOPE_SUB, formattedGroupFilter, null, false);
                var groups = new List<string>();
                while (groupResults.HasMore())
                {
                    var nextEntry = groupResults.Next();
                    var cn = nextEntry.getAttribute(ATTR_COMMON_NAME);
                    if (cn != null)
                    {
                        groups.Add(cn.StringValue);
                    }
                }


                if (!groups.Contains(groupName))
                {
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

    public class LDAPUser
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public void Validate(string contextPassword)
        {
            OpenLDAPPasswordHelpers.Compare(contextPassword, Password);
        }
    }



    public class LDAPConnection
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