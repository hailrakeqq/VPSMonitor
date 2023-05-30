using Microsoft.EntityFrameworkCore;
using VPSMonitor.API;
using VPSMonitor.API.Authentication;
using VPSMonitor.API.Properties;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddCurrentUser();
builder.Services.AddTokenService();
builder.Services.AddLoginResponse();
builder.Services.AddUserService();
builder.Services.AddSSHService();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "V1",
        Title = "VPSMonitor API",
        Description = "API for manage and get info about VPS"
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();