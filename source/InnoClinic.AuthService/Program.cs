using InnoClinic.AuthService.Data;
using InnoClinic.AuthService.Extensions;

namespace InnoClinic.AuthService;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration);
        builder.AddAppAuthentication();

        var app = builder.Build();

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
