using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Authentications;
using NHSD.GPITF.BuyingCatalog.OperationFilters;
using NLog;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace NHSD.GPITF.BuyingCatalog
{
  internal sealed class Startup
  {
    private IServiceProvider ServiceProvider { get; set; }
    private IConfigurationRoot Configuration { get; }
    private IHostingEnvironment CurrentEnvironment { get; }
    private IContainer ApplicationContainer { get; set; }

    public Startup(IHostingEnvironment env)
    {
      // Environment variable:
      //    ASPNETCORE_ENVIRONMENT == Development
      CurrentEnvironment = env;

      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables()
          .AddUserSecrets<Program>();

      DumpEnvironment();

      Configuration = builder.Build();

      // database connection string for nLog
      GlobalDiagnosticsContext.Set("LOG_CONNECTIONSTRING", Environment.GetEnvironmentVariable("LOG_CONNECTIONSTRING") ?? Configuration["Log:ConnectionString"]);
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      // Add controllers as services so they'll be resolved.
      services.AddMvc().AddControllersAsServices();

      services.Configure<FormOptions>(x =>
      {
        x.ValueLengthLimit = int.MaxValue;
        x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
      });

      if (CurrentEnvironment.IsDevelopment())
      {
        // Register the Swagger generator, defining one or more Swagger documents
        services.AddSwaggerGen(options =>
        {
          options.SwaggerDoc("v1",
            new Info
            {
              Title = "catalogue-api",
              Version = "1.0.0-private-beta",
              Description = "NHS Digital GP IT Futures Buying Catalog API"
            });
          options.SwaggerDoc("porcelain",
            new Info
            {
              Title = "catalogue-api",
              Version = "porcelain",
              Description = "NHS Digital GP IT Futures Buying Catalog API"
            });

          options.DocInclusionPredicate((docName, apiDesc) =>
          {
            var controllerActionDescriptor = apiDesc.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
              return false;
            }

            var versions = controllerActionDescriptor.MethodInfo.DeclaringType
                .GetCustomAttributes(true)
                .OfType<ApiVersionAttribute>()
                .SelectMany(attr => attr.Versions);
            var tags = controllerActionDescriptor.MethodInfo.DeclaringType
                .GetCustomAttributes(true)
                .OfType<ApiTagAttribute>();

            return versions.Any(
              v => $"v{v.ToString()}" == docName) ||
              tags.Any(tag => tag.Tag == docName);
          });

          options.AddSecurityDefinition("oauth2", new OAuth2Scheme
          {
            Type = "oauth2",
            Flow = "accessCode"
          });
          options.AddSecurityDefinition("basic", new BasicAuthScheme());

          options.OperationFilter<AssignSecurityRequirements>();

          // Set the comments path for the Swagger JSON and UI.
          var xmlPath = Path.Combine(AppContext.BaseDirectory, "NHSD.GPITF.BuyingCatalog.xml");
          options.IncludeXmlComments(xmlPath);
          options.DescribeAllEnumsAsStrings();
          options.OperationFilter<ExamplesOperationFilter>();
        });

        services
          .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
          .AddBasicAuthentication(
            options =>
            {
              options.Realm = "NHSD.GPITF.BuyingCatalog";
              options.Events = new BasicAuthenticationEvents
              {
                OnValidatePrincipal = context =>
                {
                  var auth = ServiceProvider.GetService<IBasicAuthentication>();
                  return auth.Authenticate(context);
                }
              };
            });
      }

      services
        .AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          options.Authority = Environment.GetEnvironmentVariable("OIDC_ISSUER_URL") ?? Configuration["Jwt:Authority"];
          options.Audience = Environment.GetEnvironmentVariable("OIDC_AUDIENCE") ?? Configuration["Jwt:Audience"];
          options.RequireHttpsMetadata = !CurrentEnvironment.IsDevelopment();
          options.Events = new JwtBearerEvents
          {
            OnTokenValidated = async context =>
            {
              var auth = ServiceProvider.GetService<IBearerAuthentication>();
              await auth.Authenticate(context);
            }
          };
        });

      // Create the container builder.
      var builder = new ContainerBuilder();

      // Register dependencies, populate the services from
      // the collection, and build the container.
      //
      // Note that Populate is basically a foreach to add things
      // into Autofac that are in the collection. If you register
      // things in Autofac BEFORE Populate then the stuff in the
      // ServiceCollection can override those things; if you register
      // AFTER Populate those registrations can override things
      // in the ServiceCollection. Mix and match as needed.
      builder.Populate(services);

      // load all assemblies in same directory and register classes with interfaces
      // Note that we have to explicitly add this (executing) assembly
      var exeAssy = Assembly.GetExecutingAssembly();
      var exeAssyPath = exeAssy.Location;
      var exeAssyDir = Path.GetDirectoryName(exeAssyPath);
      var assyPaths = Directory.EnumerateFiles(exeAssyDir, "NHSD.*.dll");
      var assys = assyPaths.Select(filePath => Assembly.LoadFile(filePath)).ToList();
      assys.Add(exeAssy);
      builder
        .RegisterAssemblyTypes(assys.ToArray())
        .PublicOnly()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder.Register(cc => Configuration).As<IConfiguration>();

      ApplicationContainer = builder.Build();

      // Create the IServiceProvider based on the container.
      return new AutofacServiceProvider(ApplicationContainer);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, ILoggerFactory logging)
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
              if (CurrentEnvironment.IsDevelopment())
              {
                await context.Response.WriteAsync(err).ConfigureAwait(false);
              }
            }
          });
        }
      );

      app.UseAuthentication();

      if (CurrentEnvironment.IsDevelopment())
      {
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(opts =>
        {
          opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Buying Catalog API V1");
          opts.SwaggerEndpoint("/swagger/porcelain/swagger.json", "Buying Catalog API V1 Porcelain");

          opts.DocExpansion(DocExpansion.None);
        });
      }

      app.UseStaticFiles();
      app.UseMvc();

      if (CurrentEnvironment.IsDevelopment())
      {
        var logConfig = Configuration.GetSection("Logging");
        logging.AddConsole(logConfig); //log levels set in your configuration
        logging.AddDebug(); //does all log levels
        logging.AddFile(logConfig.GetValue<string>("PathFormat"));
      }
    }

    private static void DumpEnvironment()
    {
      Console.WriteLine("Environment:");
      Console.WriteLine($"  CRM:");
      Console.WriteLine($"    CRM_APIURI          : {Environment.GetEnvironmentVariable("CRM_APIURI")}");
      Console.WriteLine($"    CRM_ACCESSTOKENURI  : {Environment.GetEnvironmentVariable("CRM_ACCESSTOKENURI")}");
      Console.WriteLine($"    CRM_CLIENTID        : {Environment.GetEnvironmentVariable("CRM_CLIENTID")}");
      Console.WriteLine($"    CRM_CLIENTSECRET    : {Environment.GetEnvironmentVariable("CRM_CLIENTSECRET")}");

      Console.WriteLine($"  DATASTORE:");
      Console.WriteLine($"    DATASTORE_CONNECTIONTYPE    : {Environment.GetEnvironmentVariable("DATASTORE_CONNECTIONTYPE")}");
      Console.WriteLine($"    DATASTORE_CONNECTIONSTRING  : {Environment.GetEnvironmentVariable("DATASTORE_CONNECTIONSTRING")}");

      Console.WriteLine($"  LOG:");
      Console.WriteLine($"    LOG_CONNECTIONSTRING : {Environment.GetEnvironmentVariable("LOG_CONNECTIONSTRING")}");

      Console.WriteLine($"  OIDC:");
      Console.WriteLine($"    OIDC_USERINFO_URL : {Environment.GetEnvironmentVariable("OIDC_USERINFO_URL")}");
      Console.WriteLine($"    OIDC_ISSUER_URL   : {Environment.GetEnvironmentVariable("OIDC_ISSUER_URL")}");
      Console.WriteLine($"    OIDC_AUDIENCE     : {Environment.GetEnvironmentVariable("OIDC_AUDIENCE")}");

      Console.WriteLine($"  SHAREPOINT:");
      Console.WriteLine($"    SHAREPOINT_BASEURL                  : {Environment.GetEnvironmentVariable("SHAREPOINT_BASEURL")}");
      Console.WriteLine($"    SHAREPOINT_ORGANISATIONSRELATIVEURL : {Environment.GetEnvironmentVariable("SHAREPOINT_ORGANISATIONSRELATIVEURL")}");
      Console.WriteLine($"    SHAREPOINT_LOGIN                    : {Environment.GetEnvironmentVariable("SHAREPOINT_LOGIN")}");
      Console.WriteLine($"    SHAREPOINT_PASSWORD                 : {Environment.GetEnvironmentVariable("SHAREPOINT_PASSWORD")}");

      Console.WriteLine($"  CRM:");
      Console.WriteLine($"    CRM_APIURI : {Environment.GetEnvironmentVariable("CRM_APIURI")}");
      Console.WriteLine($"    CRM_ACCESSTOKENURI : {Environment.GetEnvironmentVariable("CRM_ACCESSTOKENURI")}");
      Console.WriteLine($"    CRM_CLIENTID : {Environment.GetEnvironmentVariable("CRM_CLIENTID")}");
      Console.WriteLine($"    CRM_CLIENTSECRET : {Environment.GetEnvironmentVariable("CRM_CLIENTSECRET")}");
    }
  }
}
