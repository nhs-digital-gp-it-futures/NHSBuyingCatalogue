using Gif.Service.Authentications;
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Filters;
using Gif.Service.Interfaces;
using Gif.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSD.GPITF.BuyingCatalog;
using NLog;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Net;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace Gif.Service
{
  /// <summary>
  /// Startup
  /// </summary>
  public class Startup
  {
    private IServiceProvider ServiceProvider { get; set; }
    private IConfiguration Configuration { get; set; }
    private IHostingEnvironment CurrentEnvironment { get; }

    public Startup(IHostingEnvironment env)
    {
      CurrentEnvironment = env;

      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddUserSecrets<Program>();

      Configuration = builder.Build();

      // database connection string for nLog
      GlobalDiagnosticsContext.Set("LOG_CONNECTIONSTRING", Settings.LOG_CONNECTIONSTRING(Configuration));

      DumpSettings();
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
      #region dependency injection
      services.AddSingleton(sp => Configuration);
      services.AddSingleton<IBasicAuthentication, BasicAuthentication>();
      services.AddSingleton<IRepository, Repository>();
      services.AddSingleton<ICapabilityDatastore, CapabilitiesService>();
      services.AddSingleton<ICapabilitiesImplementedDatastore, CapabilitiesImplementedService>();
      services.AddSingleton<ICapabilitiesImplementedEvidenceDatastore, CapabilitiesImplementedEvidenceService>();
      services.AddSingleton<ICapabilitiesImplementedReviewsDatastore, CapabilitiesImplementedReviewsService>();
      services.AddSingleton<ICapabilityStandardDatastore, CapabilityStandardService>();
      services.AddSingleton<IContactsDatastore, ContactsService>();
      services.AddSingleton<IFrameworksDatastore, FrameworksService>();
      services.AddSingleton<ILinkManagerDatastore, LinkManagerService>();
      services.AddSingleton<IOrganisationsDatastore, OrganisationsService>();
      services.AddSingleton<ISolutionsDatastore, SolutionsService>();
      services.AddSingleton<ISolutionsExDatastore, SolutionExService>();
      services.AddSingleton<IStandardsDatastore, StandardsService>();
      services.AddSingleton<IStandardsApplicableDatastore, StandardsApplicableService>();
      services.AddSingleton<IStandardsApplicableEvidenceDatastore, StandardsApplicableEvidenceService>();
      services.AddSingleton<IStandardsApplicableReviewsDatastore, StandardsApplicableReviewsService>();
      services.AddSingleton<ITechnicalContactsDatastore, TechnicalContactService>();
      #endregion

      // Add framework services.
      services
        .AddMvc()
        // Add controllers as services so they'll be resolved.
        .AddControllersAsServices()
        .AddJsonOptions(opts =>
        {
          opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
          opts.SerializerSettings.Converters.Add(new StringEnumConverter
          {
            CamelCaseText = false
          });
        });


      if (CurrentEnvironment.IsDevelopment())
      {
        services
          .AddSwaggerGen(options =>
          {
            options.SwaggerDoc("v1.0",
              new Info
              {
                Title = "Buying Catalog GIF API",
                Version = "v1.0",
                Description = "Buying Catalog GIF API (ASP.NET Core 2.0)"
              });

            options.CustomSchemaIds(type => type.FriendlyId(true));

            // Set the comments path for the Swagger JSON and UI.
            var xmlPath = Path.Combine(AppContext.BaseDirectory, "NHSD.GPITF.BuyingCatalog.xml");
            options.IncludeXmlComments(xmlPath);
            options.DescribeAllEnumsAsStrings();

            // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
            // Use [ValidateModelState] on Actions to actually validate it in C# as well!
            options.OperationFilter<GeneratePathParamsValidationFilter>();
          });
      }

      services.AddIdentityServer()
        // TODO : Depending on hosting this may need replacing with a cert (also probably better practice to handle instance restarts)
        // https://stackoverflow.com/questions/41572900/addtemporarysigningcredential-vs-addsigningcredential-in-identityserver4
        .AddDeveloperSigningCredential()
        .AddInMemoryApiResources(AuthConfig.GetApiResources())
        .AddInMemoryClients(AuthConfig.GetClients(Configuration));

      services.AddMvcCore()
        .AddAuthorization()
        .AddJsonFormatters();

      services
        .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
        .AddBasicAuthentication(
          options =>
          {
            options.Realm = "GIFBuyingCatalogue";
            options.Events = new BasicAuthenticationEvents
            {
              OnValidatePrincipal = context =>
              {
                var auth = ServiceProvider.GetService<IBasicAuthentication>();
                return auth.Authenticate(context);
              }
            };
          });

      services.AddAuthentication("Bearer")
        .AddIdentityServerAuthentication(options =>
        {
          options.Authority = Settings.GIF_AUTHORITY_URI(Configuration);
          options.RequireHttpsMetadata = false;

          options.ApiName = "GIFBuyingCatalogue";
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      ServiceProvider = app.ApplicationServices;

      if (CurrentEnvironment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseExceptionHandler(options =>
      {
        options.Run(async context =>
        {
          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
          context.Response.ContentType = "text/html";
          var ex = context.Features.Get<IExceptionHandlerFeature>();
          if (ex != null)
          {
            var logger = ServiceProvider.GetService<ILogger<Startup>>();
            var err = $"Error: {ex.Error.Message}{Environment.NewLine}{ex.Error.StackTrace }";
            logger.LogError(err);
            if (env.IsDevelopment())
            {
              await context.Response.WriteAsync(err).ConfigureAwait(false);
            }
          }
        });
      });

      app.UseIdentityServer();
      app.UseAuthentication();

      if (env.IsDevelopment())
      {
        app.UseSwagger();

        app.UseSwaggerUI(opts =>
        {
          opts.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Buying Catalog GIF API");

          opts.DocExpansion(DocExpansion.None);
        });
      }

      app.UseStaticFiles();
      app.UseMvc();
    }

    private void DumpSettings()
    {
      Console.WriteLine("Settings:");
      Console.WriteLine($"  GIF:");
      Console.WriteLine($"    GIF_CRM_URL                 : {Settings.GIF_CRM_URL(Configuration)}");
      Console.WriteLine($"    GIF_AUTHORITY_URI           : {Settings.GIF_AUTHORITY_URI(Configuration)}");
      Console.WriteLine($"    GIF_CRM_AUTHORITY           : {Settings.GIF_CRM_AUTHORITY(Configuration)}");
      Console.WriteLine($"    GIF_AZURE_CLIENT_ID         : {Settings.GIF_AZURE_CLIENT_ID(Configuration)}");
      Console.WriteLine($"    GIF_ENCRYPTED_CLIENT_SECRET : {Settings.GIF_ENCRYPTED_CLIENT_SECRET(Configuration)}");

      Console.WriteLine($"  LOG:");
      Console.WriteLine($"    LOG_CONNECTIONSTRING : {Settings.LOG_CONNECTIONSTRING(Configuration)}");
      Console.WriteLine($"    LOG_CRM              : {Settings.LOG_CRM(Configuration)}");

      Console.WriteLine($"  CRM:");
      Console.WriteLine($"    CRM_CLIENTID            : {Settings.CRM_CLIENTID(Configuration)}");
      Console.WriteLine($"    CRM_CLIENTSECRET        : {Settings.CRM_CLIENTSECRET(Configuration)}");
    }
  }
}
