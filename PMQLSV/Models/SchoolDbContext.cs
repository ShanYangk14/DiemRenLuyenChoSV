using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;


namespace PMQLSV.Models
{
    public class SchoolDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Grades> Grades { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Role> Roles { get; set; }

        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Class>().ToTable("classes");
            modelBuilder.Entity<Student>().ToTable("students");
            modelBuilder.Entity<Grades>().ToTable("grades");
            modelBuilder.Entity<Teacher>().ToTable("teachers");
            modelBuilder.Entity<Role>().ToTable("roles");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithOne(r => r.User)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Teacher)
                .WithOne(t => t.User)
                .HasForeignKey<Teacher>(t => t.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Classes)
                .WithOne(c => c.Teacher)
                .HasForeignKey(c => c.TeacherId);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Grades)
                .WithOne(g => g.Student)
                .HasForeignKey(g => g.StudentId);

            modelBuilder.Entity<Class>()
                .HasMany(c => c.Students)
                .WithOne(s => s.Class)
                .HasForeignKey(s => s.ClassId);
        }

    }
}
