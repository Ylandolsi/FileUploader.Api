using FileUploader.Api.Database;
using FileUploader.Api.Extensions;
using FileUploader.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<RefreshService>();
builder.Services.AddScoped<AuthService>();


builder.Services.AddAuthorization();
builder.AddAuth();


builder.Services.AddCorsPolicy();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}


app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();


app.Run();
