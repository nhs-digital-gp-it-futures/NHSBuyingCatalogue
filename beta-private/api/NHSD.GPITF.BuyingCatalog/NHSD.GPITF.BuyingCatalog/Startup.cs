using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHSD.GPITF.BuyingCatalog.Authentications;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace NHSD.GPITF.BuyingCatalog
{
  internal sealed class Startup
  {
    private IServiceProvider ServiceProvider { get; set; }
    private IConfiguration Configuration { get; }
    private IHostingEnvironment CurrentEnvironment { get; set; }

    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
      Configuration = configuration;

      // Environment variable:
      //    ASPNETCORE_ENVIRONMENT == Development
      CurrentEnvironment = env;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      var jsonServices = JObject.Parse(File.ReadAllText("appsettings.json"))["Services"];
      var requiredServices = JsonConvert.DeserializeObject<List<Service>>(jsonServices.ToString());

      foreach (var service in requiredServices)
      {
        services.Add(new ServiceDescriptor(
          serviceType: Type.GetType(
            service.ServiceType,
            (name) => AssemblyResolver(name),
            null,
            true),
          implementationType: Type.GetType(
            service.ImplementationType,
            (name) => AssemblyResolver(name),
            null,
            true),
          lifetime: service.Lifetime));
      }

      services.AddSingleton(sp => Configuration);
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddMvc();

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
                    return BasicAuthentication.Authenticate(
                      ServiceProvider.GetService<IContactsDatastore>(),
                      ServiceProvider.GetService<IOrganisationsDatastore>(),
                      context);
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
        options.RequireHttpsMetadata = !CurrentEnvironment.IsDevelopment();
        options.Events = new JwtBearerEvents
        {
          OnTokenValidated = async context =>
          {
            await BearerAuthentication.Authenticate(
              ServiceProvider.GetService<IUserInfoResponseDatastore>(),
              ServiceProvider.GetService<IConfiguration>(),
              ServiceProvider.GetService<IUserInfoResponseRetriever>(),
              ServiceProvider.GetService<IContactsDatastore>(),
              ServiceProvider.GetService<IOrganisationsDatastore>(),
              context);
          }
        };
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, ILoggerFactory logging)
    {
      ServiceProvider = app.ApplicationServices;

      if (CurrentEnvironment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

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

      var logConfig = Configuration.GetSection("Logging");
      logging.AddConsole(logConfig); //log levels set in your configuration
      logging.AddDebug(); //does all log levels
      logging.AddFile(logConfig.GetValue<string>("PathFormat"));
    }

    private Assembly AssemblyResolver(AssemblyName name)
    {
      var currAssyPath = Assembly.GetExecutingAssembly().Location;
      var assyPath = Path.Combine(Path.GetDirectoryName(currAssyPath), $"{name.FullName}.dll");
      var assy = Assembly.LoadFile(assyPath);
      return assy;
    }
  }
}
