using backend.BackgroundServices;
using backend.Controllers;
using backend.Data;
using backend.Email;
using backend.Mapping;
using backend.Options;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<MonitoringOptions>(builder.Configuration.GetSection("Monitoring"));
builder.Services.Configure<RequestOptions>(builder.Configuration.GetSection("Request"));
builder.Services.Configure<NotificationOptions>(builder.Configuration.GetSection("Notification"));



builder.Services.AddDbContext<InternalDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("InternalDb"))
);

builder.Services.AddDbContext<ExternalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ExternalDb")));

builder.Services.AddScoped<IRequestRepository, EFRequestRepository>();
builder.Services.AddScoped<INotificationRepository, EFNotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});


builder.Services.AddScoped<IExternalRequestRepository, EFExternalRequestRepository>();
builder.Services.AddScoped<IRequestService, RequestService>();


builder.Services.AddSingleton<EmailSender>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<EmailOptions>>().Value;
    return new EmailSender(
        host: settings.Host,
        port: settings.Port,
        from: settings.From,
        username: settings.Username,
        password: settings.Password);
});

builder.Services.AddHostedService<MonitoringService>();

builder.Services.AddScoped<IRuleRepository, EFRuleRepository>();


builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<RuleEngine>();

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


using (var scope = app.Services.CreateScope())
{
    var internalDb = scope.ServiceProvider.GetRequiredService<InternalDbContext>();
    internalDb.Database.EnsureCreated();

    var externalDb = scope.ServiceProvider.GetRequiredService<ExternalDbContext>();
    externalDb.Database.EnsureCreated();
}

var url = builder.Configuration.GetValue<string>("Host:Url") ?? "http://0.0.0.0:5000";
app.Urls.Add(url);

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();
app.Run();
