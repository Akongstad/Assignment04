using System;
using System.Collections.Generic;
using System.Linq;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment4.Entities.Tests
{
    public class UserRepositoryTests : IDisposable
    {   
        private readonly KanbanContext context;
        private readonly UserRepository repo;
        public UserRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            var DiscussionTag = new Tag {Name = "Discussion"}; 
            var DesignTag = new Tag {Name = "Design"};
            var ImplementationTag = new Tag{Name = "Implementation"};
            var RemovedTag = new Tag{Name = "Removal"};
    
            var designTask = new Task {Title = "Design new stuff", State = Assignment4.Core.State.New, Tags = new[]{DesignTag, DiscussionTag}, StateUpdated = System.DateTime.UtcNow, Created = System.DateTime.UtcNow};
            var implementTask = new Task {Title = "Implement code",State = Assignment4.Core.State.Active, Tags = new[]{ImplementationTag}, StateUpdated = System.DateTime.UtcNow, Created = System.DateTime.UtcNow};
            var removedTask = new Task {Title = "Is soon removed",State = Assignment4.Core.State.Removed, Tags = new[]{RemovedTag}, StateUpdated = System.DateTime.UtcNow, Created = System.DateTime.UtcNow};
            context.Users.AddRange(
                new User {Name = "Kong", Email = "kong@itu.dk", Tasks = new[]{designTask}},
                new User {Name = "Bruhn", Email = "pebn@itu.dk", Tasks = new[]{implementTask}},
                new User {Name = "Barner", Email ="beal@itu.dk", Tasks = new[]{removedTask}}
                ); 
            
            context.SaveChanges();
            this.context = context;
            this.repo = new UserRepository(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public void Create_user_creates_user()
        {
        var User = new UserCreateDTO{Name = "TestUser", Email = "TestUser@itu.dk"};
            (var response, var id) = repo.Create(User);
            Assert.Equal(Response.Created, response);
            Assert.Equal(context.Users.Count(), id);
        }

        [Fact]
        public void Create_existing_user_returns_conflict()
        {
        var User = new UserCreateDTO{Name = "Bruhn", Email = "pebn@itu.dk"};
            (var response, var id) = repo.Create(User);
            Assert.Equal(Response.Conflict, response);
        }
        
        [Fact]
        public void Read_id_returns_user()
        {
        var id = 1;
            var expected = new UserDTO(id,"Kong", "kong@itu.dk");
            Assert.Equal(expected, repo.Read(id));
        }

        [Fact]
        public void ReadAll_returns_all_Users()
        {
            var users = repo.ReadAll();
            Assert.Collection(users, 
            u => {
               var expected = new UserDTO(1, "Kong", "kong@itu.dk");
               Assert.Equal(expected,u);
           },
           u => {
               var expected = new UserDTO(2, "Bruhn", "pebn@itu.dk");
               Assert.Equal(expected,u);
           },
           u => {
               var expected = new UserDTO(3, "Barner", "beal@itu.dk");
               Assert.Equal(expected,u);
            }
         );
        }

        [Fact]
        public void Delete_existing_user_with_force_returns_deleted()
        {   
            var id = 1;
            Assert.Equal(Response.Deleted, repo.Delete(id, true));
            Assert.Null(repo.Read(id));
        }

         [Fact]
        public void Delete_existing_user_without_force_returns_conflict()
        {
            Assert.Equal(Response.Conflict, repo.Delete(1, false));
        }

         [Fact]
        public void Delete_nonexisting_user_with_force_returns_notFound()
        {
            Assert.Equal(Response.NotFound, repo.Delete(0, true));
        }

         [Fact]
        public void Update_user_returns_updated()
        {
            var UserUpdateResponse = repo.Update(new UserUpdateDTO{Id = 1, Name =  "Donkey Kong",  Email ="NewEmail@itu.dk"});
            var u = repo.Read(1);
            Assert.Equal(1, u.Id);
            Assert.Equal("Donkey Kong", u.Name);
            Assert.Equal("NewEmail@itu.dk", u.Email);
            Assert.Equal(Response.Updated, UserUpdateResponse);
        }
         [Fact]
        public void Update_non_existing_user_returns_NotFound()
        {
           var UserUpdateResponse = repo.Update(new UserUpdateDTO{Id = 69, Name =  "Donkey Kong",  Email ="NewEmail@itu.dk"});
           Assert.Equal(Response.NotFound, UserUpdateResponse);
        }
    }
}
