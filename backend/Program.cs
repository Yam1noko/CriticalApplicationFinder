using backend.Data;
using backend.Mapping;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InternalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("InternalDb")));

builder.Services.AddDbContext<ExternalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ExternalDb")));

builder.Services.AddScoped<IRequestRepository, EFRequestRepository>();
builder.Services.AddScoped<INotificationRepository, EFNotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

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
