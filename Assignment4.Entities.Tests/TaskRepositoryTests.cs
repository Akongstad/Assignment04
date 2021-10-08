using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly KanbanContext context;
        private readonly TaskRepository repo;
        public TaskRepositoryTests()
        {
            using var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            using var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();
            
            this.context = context;
            this.repo = new TaskRepository(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
