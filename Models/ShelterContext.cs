using Microsoft.EntityFrameworkCore;

namespace ShelterHelper.Models
{
    public class ShelterContext : DbContext
    {
        public DbSet<Accessory> Accessory { get; set; }
        public DbSet<Animal> AnimalsDb { get; set; }
        public DbSet<Bedding> Bedding { get; set; }
        public DbSet<Diet> Diet { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<Assignment> Assignment { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeAssignment> EmployeesAssignments { get; set; }
        public DbSet<Owner> Owner { get; set; }
        public DbSet<Toy> Toy { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseNpgsql(configuration.GetConnectionString("ShelterContext")).UseLazyLoadingProxies();
        }

        public ShelterContext(DbContextOptions<ShelterContext> options) : base(options)
        {
        }
    }
}