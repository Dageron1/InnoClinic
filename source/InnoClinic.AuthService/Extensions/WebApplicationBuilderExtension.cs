﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InnoClinic.AuthService.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
    {
        var settingsSection = builder.Configuration.GetSection("ApiSettings");

        var secret = settingsSection.GetValue<string>("JwtOptions:Secret");
        var issuer = settingsSection.GetValue<string>("JwtOptions:Issuer");
        var audience = settingsSection.GetValue<string>("JwtOptions:Audience");

        var key = Encoding.ASCII.GetBytes(secret);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                ValidateAudience = true
            };
        });

        return builder;
    }
}