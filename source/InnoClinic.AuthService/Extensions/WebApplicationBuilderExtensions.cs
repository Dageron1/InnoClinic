using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InnoClinic.AuthService.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
    {
        var settingsSection = builder.Configuration.GetSection("ApiSettings");
        if (!settingsSection.Exists())
        {
            throw new InvalidOperationException("ApiSettings section is missing in the configuration.");
        }

        var secret = GetRequiredConfigurationValue(settingsSection, "JwtOptions:Secret");
        var issuer = GetRequiredConfigurationValue(settingsSection, "JwtOptions:Issuer");
        var audience = GetRequiredConfigurationValue(settingsSection, "JwtOptions:Audience");

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

    private static string GetRequiredConfigurationValue(IConfigurationSection section, string key)
    {
        var value = section.GetValue<string>(key);
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"{key} is missing or empty in the configuration.");
        }
        return value;
    }
}
