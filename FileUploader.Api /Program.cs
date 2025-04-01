using CloudinaryDotNet;
using FileUploader.Api.Database;
using FileUploader.Api.Extensions;
using FileUploader.Api.Infrastructure;
using FileUploader.Api.Middleware;
using FileUploader.Api.Services;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(new UserSaveInterceptor()) );  // for creating root folder for new users


        
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<RefreshService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<FolderService>();

builder.Services.AddAuthorization();
builder.AddAuth();




builder.ConfigureCloudinary();


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

app.UseGlobalExceptionHandler();


app.UseCors("DefaultPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();


app.Run();
