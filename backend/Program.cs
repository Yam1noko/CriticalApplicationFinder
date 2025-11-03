using backend.BackgroundServices;
using backend.Data;
using backend.Email;
using backend.Mapping;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InternalDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("InternalDb"))
);

builder.Services.AddDbContext<ExternalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ExternalDb")));

builder.Services.AddScoped<IRequestRepository, EFRequestRepository>();
builder.Services.AddScoped<INotificationRepository, EFNotificationRepository>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddSingleton(new EmailSender(
    host: "sandbox.smtp.mailtrap.io",
    port: 2525,
    from: "pavel.yavits@yandex.ru",
    username: "c07ddc1e215a42",                // <-- username из Mailtrap
    password: "ff23305765ebcb"                 // <-- password из Mailtrap
));

builder.Services.AddScoped<IExternalRequestRepository, EFExternalRequestRepository>();
builder.Services.AddScoped<IRequestService, RequestService>();

<<<<<<< Updated upstream
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
=======
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<EmailSender>();
>>>>>>> Stashed changes

builder.Services.AddSingleton(new EmailSender(
    host: "sandbox.smtp.mailtrap.io",
    port: 2525,
    from: "pavel.yavits@yandex.ru",                  
    username: "ad8970d4a31125",                // <-- username из Mailtrap
    password: "7c01f159729dd2"                 // <-- password из Mailtrap
));

builder.Services.AddHostedService<MonitoringService>();

builder.Services.AddScoped<IRuleRepository, EFRuleRepository>();

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

app.Urls.Add("http://0.0.0.0:5000");

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();
app.Run();
