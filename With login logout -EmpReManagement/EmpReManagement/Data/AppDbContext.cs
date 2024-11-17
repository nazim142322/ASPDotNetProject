using EmpReManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpReManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        //DbSet property
        public DbSet<Department>Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the one-to-many relationship between Department and Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Departments)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId) // specify the foreign key in Employee
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade delete
        }
    }
}
