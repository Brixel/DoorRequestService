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
        private const string COMMON_NAME = "cn";
        private readonly IConfiguration _configuration;

        public LDAPResourceOwnerPasswordValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var ldapConfig = _configuration.GetSection("LDAPConnection").Get<LDAPConnection>();

            LdapConnection lc = new LdapConnection();
            try
            {
                lc.Connect(ldapConfig.Url, ldapConfig.Port);
                lc.Bind(ldapConfig.BindDn, context.Password);

                string searchFilter = $"(&(cn=*)(memberUid={context.UserName}))";

                var baseDN = "dc=contoso,dc=com";
                var userFilter = $"(&(objectclass=posixAccount)(uid={context.UserName}))";
                var userSearch = lc.Search(baseDN, LdapConnection.SCOPE_SUB, userFilter, null, false);
                var userIds = new List<string>();
                while (userSearch.HasMore())
                {
                    var nextEntry = userSearch.Next();
                    var cn = nextEntry.getAttribute(COMMON_NAME);
                    if (cn != null)
                    {
                        userIds.Add(cn.StringValue);
                    }
                }

                if (userIds.Count == 0)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_username_or_password");
                    return Task.FromResult(false);
                }

                if (userIds.Distinct().Count() > 1)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_username_or_password");
                    return Task.FromResult(false);
                }

                var userId = userIds.Single();

                var search = lc.Search(baseDN, LdapConnection.SCOPE_SUB, searchFilter, null, false);
                var groups = new List<string>();
                while (search.HasMore())
                {
                    var nextEntry = search.Next();
                    var cn = nextEntry.getAttribute(COMMON_NAME);
                    if (cn != null)
                    {
                        groups.Add(cn.StringValue);
                    }
                }

                var claims = new List<Claim>()
                {
                    new Claim(JwtClaimTypes.Subject, userId)
                };

                context.Result = new GrantValidationResult(subject: userId, 
                    OidcConstants.AuthenticationMethods.Password, claims);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                //context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return Task.FromResult(false);
            }
        }
    }

    public class LDAPConnection
    {
        public string Url { get; set; }

        public int Port { get; set; }

        public string BindDn { get; set; }

    }
}