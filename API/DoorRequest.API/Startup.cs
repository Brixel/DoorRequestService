using System.Linq;
using AspNetCore.Totp;
using AspNetCore.Totp.Interface;
using DoorRequest.API.Config;
using DoorRequest.API.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
            services.AddControllers();
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

            var hasLDAPConfiguration = Configuration.GetSection("Authentication:LDAPConnectionOptions").GetChildren().Any();

            if (hasLDAPConfiguration)
            {
                services.AddIdentityServer(options =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseSuccessEvents = true; ;
                    })
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(InMemoryInitConfig.GetIdentityResources())
                    .AddInMemoryApiResources(InMemoryInitConfig.GetApiResources())
                    .AddInMemoryClients(InMemoryInitConfig.GetClients(identityConfiguration.AllowedOrigins))
                    .AddInMemoryApiScopes(InMemoryInitConfig.GetApiScopes())
                    .AddProfileService<FullNameProfileService>()
                    .AddResourceOwnerValidator<LDAPResourceOwnerPasswordValidator>();
            }
            else
            {
                services.AddIdentityServer(options =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                        ;
                    })
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(InMemoryInitConfig.GetIdentityResources())
                    .AddInMemoryApiResources(InMemoryInitConfig.GetApiResources())
                    .AddInMemoryClients(InMemoryInitConfig.GetClients(identityConfiguration.AllowedOrigins))
                    .AddInMemoryApiScopes(InMemoryInitConfig.GetApiScopes())
                    .AddProfileService<FullNameProfileService>()
                    .AddResourceOwnerValidator<FileBasedResourceOwnerPasswordValidator>();
            }
            


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
                    doorConfiguration.ClientId, 
                    doorConfiguration.Server,
                    doorConfiguration.Topic,
                    doorConfiguration.Port,
                    doorConfiguration.UseSSL,
                    doorConfiguration.Username,
                    doorConfiguration.Password
                    ));
            services.AddScoped<IDoorRequestService, DoorRequestService>();
            services.Configure<AccountKeyConfiguration>(Configuration.GetSection("AccountKeyConfiguration"));
            services.AddScoped<IAccountKeyService, AccountKeyService>();
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseSerilogRequestLogging();
            // TODO Uncomment once connection to LDAP is over SSL
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseIdentityServer();

            app.UseCors("CorsPolicy");
            

            app.UseEndpoints(routes =>
            {
                routes.MapControllers();
            });
        }
    }
}
