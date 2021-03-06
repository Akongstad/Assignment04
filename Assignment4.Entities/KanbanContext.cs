using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class KanbanContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public KanbanContext(DbContextOptions<KanbanContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Task>()

                .Property(e => e.State)

                .HasConversion(new EnumToStringConverter<State>());
            modelBuilder 
                .Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();
            modelBuilder 
                .Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); 
        }
    }
}