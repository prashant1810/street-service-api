using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Street.Services.CrudAPI.Models;

namespace Street.Services.CrudAPI.Database
{
    public class AppDbContext: DbContext
    {
        protected readonly IConfiguration Configuration;
        public AppDbContext(IConfiguration configuration) 
        { 
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbHost = Environment.GetEnvironmentVariable("HOST");
            var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
            var userName = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

            var connectionString = string.Empty;

            if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                connectionString = Configuration.GetConnectionString("DefaultConnection");
            }
            else
            {
                connectionString = $"Host={dbHost};Database={dbName};Username={userName};Password={password}";
            }
            optionsBuilder.UseNpgsql(connectionString,
                o => o.UseNetTopologySuite());
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<StreetInfo> Streets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StreetInfo>()
            .Property(s => s.Geometry)
            .HasColumnType("geometry(LineString,4326)");

            // Seed initial data
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            modelBuilder.Entity<StreetInfo>().HasData(
            new StreetInfo
            {
                Id = 1,
                Name = "Dortmund Street",
                Geometry = geometryFactory.CreateLineString(new[]
                {
                    new Coordinate(0, 0),
                    new Coordinate(1, 1),
                    new Coordinate(2, 2)
                }),
                Capacity = 100
            },
            new StreetInfo
            {
                Id = 2,
                Name = "Berlin Street",
                Geometry = geometryFactory.CreateLineString(new[]
                {
                    new Coordinate(-1, -1),
                    new Coordinate(0, 0),
                    new Coordinate(1, -1)
                }),
                Capacity = 150
            }
        );
        }
    }
}
