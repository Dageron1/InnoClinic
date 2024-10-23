using InnoClinic.AuthService.Data;
using InnoClinic.AuthService.Extensions;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace InnoClinic.AuthService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration);
        builder.AddAppAuthentication();

        var app = builder.Build();

        var supportedCultures = new[] { new CultureInfo("en") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        //app.UseMiddleware<ExceptionMiddleware>();
        app.MapControllers();

        app.UseExceptionHandler(o => { });
        await ApplyMigrationAsync();
        await app.RunAsync();

        async Task ApplyMigrationAsync()
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

                await dbInitializer.InitializeAsync();
            }
        }
    }
}
