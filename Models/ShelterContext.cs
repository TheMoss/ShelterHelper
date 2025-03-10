﻿using Microsoft.EntityFrameworkCore;

namespace ShelterHelper.Models
{
    public class ShelterContext : DbContext
    {
        public DbSet<Animal> AnimalsDb { get; set; }
        public DbSet<Species> SpeciesDb { get; set; }
        public DbSet<Assignment> AssignmentsDb { get; set; }
        public DbSet<Employee> EmployeesDb { get; set; }
        public DbSet<EmployeeAssignment> EmployeesAssignmentsDb { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        }

        public ShelterContext(DbContextOptions<ShelterContext> options) : base(options)
        {
        }
    }
}