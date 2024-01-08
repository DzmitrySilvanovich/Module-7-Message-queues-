using Microsoft.EntityFrameworkCore;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Services;
using Ticketing.DAL.Repositories;
using Ticketing.BAL.Configs;
using NSwag;
using log4net.Config;
using log4net;
using Ticketing.BAL.RabbitMq;
using SendGrid.Helpers.Mail;
using Ticketing.BAL.Options;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

XmlConfigurator.Configure(new FileInfo("log4net.config"));
builder.Services.AddSingleton(LogManager.GetLogger(typeof(Program)));

var configuration = builder.Configuration;
string? connection = configuration.GetConnectionString("DefaultConnection");

var keyConcurrency = Convert.ToBoolean(configuration["Concurrency:Optimistic"]);

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(options => {
    options.PostProcess = document =>
    {
        document.Info = new OpenApiInfo
        {
            Version = "v1",
            Title = "ToDo API",
            Description = "An ASP.NET Core Web API for managing Ticketing items",
            TermsOfService = "https://ticketing.com/terms",
            Contact = new OpenApiContact
            {
                Name = "Example Contact",
                Url = "https://ticketing.com/contact"
            },
            License = new OpenApiLicense
            {
                Name = "Example License",
                Url = "https://ticketing.com/license"
            }
        };
    };
});


builder.Services.RegisterMapsterConfiguration();

builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddSingleton<IEmailService, EmailService>();

if (keyConcurrency)
{
    builder.Services.AddScoped(typeof(Repository<>));
}
else
{
    builder.Services.AddScoped(typeof(PessimisticRepository<>));
}

builder.Services.AddScoped<IMessageQueue, RabbitMqService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheAdapter, MemoryCacheAdapter>();

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(35))
                .Tag("tag-all"));

    options.AddPolicy("CacheForTenSeconds", builder =>
        builder.Expire(TimeSpan.FromSeconds(10))
               .SetVaryByQuery("venues")
               .SetVaryByHeader("X-Client-Id"));

    options.AddPolicy("Expensive", builder =>
        builder.Expire(TimeSpan.FromMinutes(1))
                .Tag("tag-expensive"));
});

builder.Services.AddHostedService<RabbitMqListener>();
builder.Services.Configure<EmailSettings>
   (options => builder.Configuration.GetSection("EmailSettings").Bind(options));

builder.Services.Configure<RabbitMqSettings>
   (options => builder.Configuration.GetSection("RabbitMqSettings").Bind(options));

builder.Services.Configure<RetryPolicySettings>
   (options => builder.Configuration.GetSection("RetryPolicySettings").Bind(options));

builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration
    .GetSection("EmailSettings").GetValue<string>("ApiKey");
});

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();

    app.UseSwaggerUi3();

    app.UseReDoc(options =>
    {
        options.Path = "/redoc";
    });
}

app.Run();
public partial class Program { }