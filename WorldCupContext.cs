using Microsoft.EntityFrameworkCore;
using World.Cup.Simulator.Models;

namespace World.Cup.Simulator
{
    public class WorldCupContext: DbContext
    {
        public WorldCupContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("ServerConnection");
            string connectionStringUrl = Environment.GetEnvironmentVariable("MYSQL_URL");
            connectionString = string.IsNullOrEmpty(connectionStringUrl) ? connectionString : connectionStringUrl;

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        }

        public DbSet<Team> Teams { get; set; }

    }
}
