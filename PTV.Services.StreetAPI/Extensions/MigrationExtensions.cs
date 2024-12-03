using Microsoft.EntityFrameworkCore;
using Street.Services.CrudAPI.Database;

namespace Street.Services.CrudAPI.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var _db = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    serviceScope.ServiceProvider.GetService<AppDbContext>().Database.Migrate();
                }
            }
        }
    }
}
