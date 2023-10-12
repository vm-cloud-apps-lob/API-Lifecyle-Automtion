using Autofac;
using Autofac.Extensions.DependencyInjection;
using PA.QuoteSubmission.Core;
using PA.QuoteSubmission.Infrastructure;
using PA.QuoteSubmission.Web;
using MediatR.Pipeline;
using MediatR;
using System.Reflection;
using PA.QuoteSubmission.Infrastructure.Model;
using Dapr.Workflow;
using PA.QuoteSubmission.Infrastructure.Workflow;
using PA.QuoteSubmission.Infrastructure.Activities;

var builder = WebApplication.CreateBuilder(args);

//Add Autofac service provider factory
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// Add services to the container.
builder.Services.AddDaprClient();
//Add automapper dependency
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var appConfigUrl = builder.Configuration.GetValue<string>("AppConfigURL");
builder.Configuration.AddAzureAppConfiguration(appConfigUrl);

builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<SubmissionWorkflow>();

    options.RegisterActivity<SaveEventStoreActivity>();
});



builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    //loading Mediatr Dependencies for all assemblies in the project
    var mediatrOpenTypes = new[]
        {
                typeof(IRequestHandler<,>),
                typeof(IRequestExceptionHandler<,,>),
                typeof(IRequestExceptionAction<,>),
                typeof(INotificationHandler<>),
                typeof(IStreamRequestHandler<,>)
        };
    var assemblies = new List<Assembly>();
    assemblies.Add(typeof(CoreContainerModule).GetTypeInfo().Assembly);
    assemblies.Add(typeof(InfraContainerModule).GetTypeInfo().Assembly);
    assemblies.Add(typeof(ApiContainerModule).GetTypeInfo().Assembly);
    foreach (var mediatrOpenType in mediatrOpenTypes)
    {
        containerBuilder
            .RegisterAssemblyTypes(assemblies.ToArray<Assembly>())
            .AsClosedTypesOf(mediatrOpenType)
            .AsImplementedInterfaces();
    }
    //load config properties
    var submissionCacheOptions = builder.Configuration.GetSection("SubmissionAPI:EventStoreOptions").Get<CacheOptions>();
    var refDataOptions = builder.Configuration.GetSection("SubmissionAPI:RefDataOptions").GetSection("RefDataOptions");
    //Register the dependencies defined in modules (core, infra & web/api)
    containerBuilder.RegisterModule(new CoreContainerModule());
    containerBuilder.RegisterModule(new InfraContainerModule(submissionCacheOptions,refDataOptions));
    containerBuilder.RegisterModule(new ApiContainerModule());
    /*
    * below snippet shows on how to pass configuration section from appsettings to the Module
    * To customize the Dependency objects using Load method in the SampleContainerModule class.
   var config = builder.Configuration.GetSection("configSection");
   containerBuilder.RegisterModule(new SampleContainerModule(config));
   */

    //Add more custom container modules if needed.
});

builder.Services.AddControllers();

builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();


app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.MapDefaultControllerRoute();
app.MapRazorPages();


app.Run();
