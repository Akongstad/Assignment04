using System;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests : IDisposable
    {
        private readonly KanbanContext context;
        private readonly TagRepository repo;
        public TagRepositoryTests()
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
            this.repo = new TagRepository(context);
        }
        [Fact]
        public void Create_tag_creates_newTag()
        {
            var NewTag = new TagCreateDTO{Name = "New tag"};
            (var response, var id) = repo.Create(NewTag);
            Assert.Equal(Response.Created, response);
            Assert.Equal(context.Tags.Count(), id);
        }
        [Fact]
        public void Create_existing_returns_conflict()
        {
            var DesignTag = new TagCreateDTO {Name = "Design"};
            (var Response, var id) = repo.Create(DesignTag);
            Assert.Equal(Response.Conflict, Response);
        }
        [Fact]
        public void Read_id_returns_designTask()
        {
            var id = 1;
            var expected = new TagDTO(id,"Design");
            Assert.Equal(expected, repo.Read(id));
        }
        [Fact]
        public void ReadAll_returns_all_tags()
        {
            var tags = repo.ReadAll();
            Assert.Collection(tags, 
           t => {
               var expected = new TagDTO(1, "Design");
               Assert.Equal(expected,t);
           },
           t=> {
               var expected = new TagDTO(2, "Discussion");
               Assert.Equal(expected,t);
           },
           t=> {
               var expected = new TagDTO(3, "Implementation");
               Assert.Equal(expected,t);
           },
           t=> {
               var expected = new TagDTO(4, "Removal");
               Assert.Equal(expected,t);
           }
            );
        }

        [Fact]
        public void Update_updates_tag_name()
        {
            var UpdateRemovedTag = new TagUpdateDTO{Id = 4, Name = "Removal tag updated"};
            var response = repo.Update(UpdateRemovedTag);
            Assert.Equal(Response.Updated, response);
            Assert.Equal(UpdateRemovedTag.Name, repo.Read(UpdateRemovedTag.Id).Name);
        }
        [Fact]
        public void Update_invalid_id_returns_notFound()
        {
            var UpdateTag = new TagUpdateDTO{Id = 9999, Name = "Removal tag updated"};
            var response = repo.Update(UpdateTag);
            Assert.Equal(Response.NotFound, response);
        }
        [Fact]
        public void Delete_tag_without_force_returns_conflict()
        {
            var response = repo.Delete(2, false);
            Assert.Equal(Response.Conflict, response);
        }
        [Fact]
        public void Delete_tag_with_force_returns_deleted_and_deletes_tag()
        {
            context.ChangeTracker.Clear();
            var id = 2;
            Assert.Equal(Response.Deleted, repo.Delete(id, true));
            Assert.Null(context.Tags.Find(id));
        }
        [Fact]
        public void Delete_invalid_id_returns_notFound()
        {
            var response = repo.Delete(9999, true);
            Assert.Equal(Response.NotFound, response);
        }


        public void Dispose()
        {
            context.Dispose();
        }
    }
}
