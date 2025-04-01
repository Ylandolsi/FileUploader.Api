namespace FileUploader.Api.Extensions;
using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


public  static class ServiceCollectionExtensions
{
    internal static void AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter your JWT token in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };

            o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    []
                }
            };

            o.AddSecurityRequirement(securityRequirement);
        });


    }
    
    internal static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy",
                builder =>
                {
                    builder.WithOrigins(
                            "http://localhost:5072",      
                            "http://localhost:5173")  
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

        });

    }

    internal static void AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });
    }

    internal static void ConfigureCloudinary ( this WebApplicationBuilder builder){
        Account cloudinaryAccount;
        try{
            var cloudName = builder.Configuration["Cloudinary:CloudName"];
            var apiKey = builder.Configuration["Cloudinary:ApiKey"];
            var apiSecret = builder.Configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new Exception("Missing required Cloudinary configuration values");
            }

            cloudinaryAccount = new Account(
                cloud: cloudName,
                apiKey: apiKey,
                apiSecret: apiSecret
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse Cloudinary URL: {ex.Message}");
        }
        var cloudinary = new Cloudinary(cloudinaryAccount);
        builder.Services.AddSingleton(cloudinary);

    }
}