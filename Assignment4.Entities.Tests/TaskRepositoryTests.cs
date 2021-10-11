using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly KanbanContext context;
        private readonly TaskRepository repo;
        public TaskRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            var DiscussionTag = new Tag {Name = "Discussion"}; 
            var DesignTag = new Tag {Name = "Design"};
            var designTask = new Task {Title = "Design new stuff", State = Assignment4.Core.State.New, Tags = new[]{DesignTag, DiscussionTag}, StateUpdated = System.DateTime.UtcNow};
            context.Users.Add(new User {Name = "Kong", Email = "kong@itu.dk", Tasks = new[]{designTask}}); 
            
            context.SaveChanges();
            this.context = context;
            this.repo = new TaskRepository(context);
        }

        [Fact] public void All_Returns_collection_length_1()
        {
            var expected = 1;
            Assert.Equal(expected, repo.All().Count);
        }
        public void Dispose()
        {
            context.Dispose();
        }
        
    }
}
