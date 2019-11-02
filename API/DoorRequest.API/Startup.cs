using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using AspNetCore.Totp;
using AspNetCore.Totp.Interface;
using DoorRequest.API.Config;
using DoorRequest.API.Services;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Configuration;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DoorRequest.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var identityConfiguration = 
                Configuration.GetSection("IdentityConfiguration").Get<IdentityConfiguration>();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.RequireHttpsMetadata = false;
                    // The API resource scope issued in authorization server
                    options.ApiName = identityConfiguration.ApiName;
                    // URL of my authorization server
                    options.Authority = identityConfiguration.Authority;
                });

            // Making JWT authentication scheme the default
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseSuccessEvents = true;;
                })
                .AddDeveloperSigningCredential()
                //.AddSigningCredential(...)
                .AddInMemoryIdentityResources(InMemoryInitConfig.GetIdentityResources())
                .AddInMemoryApiResources(InMemoryInitConfig.GetApiResources())
                .AddInMemoryClients(InMemoryInitConfig.GetClients(identityConfiguration.AllowedOrigins))
                .AddProfileService<LDAPProfileService>()
                .AddResourceOwnerValidator<LDAPResourceOwnerPasswordValidator>();
                //.AddTestUsers(new List<TestUser>()
                //{
                //    new TestUser()
                //    {
                //        Username = "Testuser1",
                //        Password = "Password123",
                //        SubjectId = "Testuser1",

                //    }
                //})
                //.AddLdapUsers<OpenLdapAppUser>(ldapConfig, UserStore.InMemory);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
;
            var doorConfiguration = Configuration.GetSection("MQTTDoorConfiguration").Get<DoorConfiguration>();
            services.AddScoped<ITotpGenerator, TotpGenerator>();
            services.AddScoped<ITotpSetupGenerator, TotpSetupGenerator>();
            services.AddScoped<ITotpValidator, TotpValidator>();
            services.AddScoped<IBrixelOpenDoorClient>(x =>
                new BrixelOpenDoorClient(
                    doorConfiguration.MQTTClientId, 
                    doorConfiguration.MQTTUsername, 
                    doorConfiguration.MQTTPassword, 
                    doorConfiguration.MQTTServer,
                    doorConfiguration.MQTTTopic));
            services.AddScoped<IDoorRequestService, DoorRequestService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // TODO Uncomment once connection to LDAP is over SSL
            //app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseIdentityServer();

            app.UseCors("CorsPolicy");


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=GetAboutVersion}/{id?}");
            });
        }
    }

    public class DoorConfiguration
    {
        public string MQTTClientId { get; set; }
        public string MQTTUsername { get; set; }
        public string MQTTPassword { get; set; }
        public string MQTTServer { get; set; }
        public string MQTTTopic { get; set; }
    }


    public class IdentityConfiguration
    {
        public string Authority { get;set; }
        public string ApiName { get; set; }

        public IReadOnlyList<string> AllowedOrigins { get; set; }
    }
}
