using System;
using System.Collections.Generic;
using System.Linq;
using Assignment4.Core;
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
            this.repo = new TaskRepository(context);
        }

        [Fact] public void All_Returns_collection_of_all_tasks()
        {
            var tasks = repo.ReadAll();

              Assert.Collection(tasks,
                t => {
                    var Expected = new TaskDTO(1, "Design new stuff", "Kong", new[]{"Design", "Discussion"}.ToList().AsReadOnly(), State.New);
                    Assert.Equal(Expected.Id, t.Id);
                    Assert.Equal(Expected.Title, t.Title);
                    Assert.Equal(Expected.AssignedToName, t.AssignedToName);
                    Assert.Collection(Expected.Tags, 
                        tags => Assert.Equal("Design", tags),
                        tags => Assert.Equal("Discussion", tags));
                    Assert.Equal(Expected.State, t.State);
                },
                t => {
                    var Expected = new TaskDTO(2, "Implement code", "Bruhn", new[]{"Implementation"}.ToList().AsReadOnly(), State.Active);
                    Assert.Equal(Expected.Id, t.Id);
                    Assert.Equal(Expected.Title, t.Title);
                    Assert.Equal(Expected.AssignedToName, t.AssignedToName);
                    Assert.Collection(Expected.Tags, 
                        tags => Assert.Equal("Implementation", tags));
                    Assert.Equal(Expected.State, t.State);
                },
                t => {
                    var Expected = new TaskDTO(3, "Is soon removed", "Barner", new[]{"Removal"}.ToList().AsReadOnly(), State.Removed);
                    Assert.Equal(Expected.Id, t.Id);
                    Assert.Equal(Expected.Title, t.Title);
                    Assert.Equal(Expected.AssignedToName, t.AssignedToName);
                    Assert.Collection(Expected.Tags, 
                        tags => Assert.Equal("Removal", tags));
                    Assert.Equal(Expected.State, t.State);
                }
            ); 
            
        }

        [Fact]
        public void ReadAllRemoved_returns_all_removed()
        {
            repo.Delete(3);
            var tasks = repo.ReadAllRemoved();

            Assert.Collection(tasks, 
            t => {
                var task = new TaskDTO(3, "Is soon removed", "Barner", new[]{"Removal"}.ToList().AsReadOnly(), State.Removed);
                Assert.Equal(task.Id, t.Id);
                    Assert.Equal(task.Title, t.Title);
                    Assert.Equal(task.AssignedToName, t.AssignedToName);
                    Assert.Collection(task.Tags, 
                        tags => Assert.Equal("Removal", tags));
                    Assert.Equal(task.State, t.State);
            }
            );
        }

        [Fact]
        public void ReadAllByTag_given_Removal_returns_Removaltask()
        {
            var tasks = repo.ReadAllByTag("Removal");

            Assert.Collection(tasks, 
            t => {
                var task = new TaskDTO(3, "Is soon removed", "Barner", new[]{"Removal"}.ToList().AsReadOnly(), State.Removed);
                Assert.Equal(task.Id, t.Id);
                    Assert.Equal(task.Title, t.Title);
                    Assert.Equal(task.AssignedToName, t.AssignedToName);
                    Assert.Collection(task.Tags, 
                        tags => Assert.Equal("Removal", tags));
                    Assert.Equal(task.State, t.State);
            }
            );
        }


        [Fact]
        public void Create_creates_new_Task()
        {
            var newTask = new TaskCreateDTO{Title = "Get Coffee", Description = "21312312", Tags =new[]{"Discussion"}.ToList()};
            (var response, var task) = repo.Create(newTask);
            Assert.Equal((Response.Created, 4), (response, task));
        }
        [Fact]
        public void read_Returns_Task_given_valid_Task(){
            (Response response, TaskDetailsDTO task) = repo.Read(1);  
            var expected = new TaskDetailsDTO(1, "Design new stuff", "Kong", DateTime.UtcNow,"Kong", new[]{"Design", "Discussion"}.ToList().AsReadOnly(), State.New, DateTime.UtcNow);
            Assert.Equal(expected.Id, task.Id);
            Assert.Equal(expected.Title, task.Title);
            Assert.Equal(expected.AssignedToName, task.AssignedToName);
            Assert.Collection(task.Tags, 
                        tags => Assert.Equal("Design", tags),
                        tags => Assert.Equal("Discussion", tags));
            Assert.Equal(expected.Created, task.Created, precision: TimeSpan.FromSeconds(5));
            Assert.Equal(expected.State, task.State);
            Assert.Equal(expected.StateUpdated, task.StateUpdated, precision: TimeSpan.FromSeconds(5));
            Assert.Equal(Response.Found, response);

        }
        [Fact]
        public void read_reuturns_null_notfound_Given_invalid_id()
        {
            (Response response, TaskDetailsDTO task) = repo.Read(10000);
            Assert.Equal((Response.NotFound, null), (response,task));
        }
        [Fact]
        public void Delete_removed_returns_conflict()
        {
            Assert.Equal(Response.Conflict, repo.Delete(3));
        }

        [Fact]
        public void Delete_Active_Task_Sets_State_Removed()
        {
            repo.Delete(2);
            (var response, var task) = repo.Read(2);
            Assert.Equal(State.Removed, task.State);
        }
        
         /* [Fact] //TODO get to work :)
        public void Delete_StateNew_removes_task()
        {
            (Response response, TaskDetailsDTO task) = repo.Read(1);  
            var expected = new TaskDetailsDTO(1, "Design new stuff", "Kong", DateTime.UtcNow,"Kong", new[]{"Design", "Discussion"}.ToList().AsReadOnly(), State.New, DateTime.UtcNow);
            Assert.Equal(expected.Id, task.Id);
            Assert.Equal(expected.Title, task.Title);
            Assert.Equal(expected.AssignedToName, task.AssignedToName);
            Assert.Collection(task.Tags, 
                        tags => Assert.Equal("Design", tags),
                        tags => Assert.Equal("Discussion", tags));
            Assert.Equal(expected.Created, task.Created, precision: TimeSpan.FromSeconds(5));
            Assert.Equal(expected.State, task.State);
            Assert.Equal(expected.StateUpdated, task.StateUpdated, precision: TimeSpan.FromSeconds(5));
            Assert.Equal(Response.Found, response);
            repo.Delete(1);
            Assert.Null(repo.Read(1));
        }  */

        [Fact]
        public void Update_updates_task_title()  //Cant update with the same tags. Cause Unique tag.Name attribute
        {
        (Response response, TaskDetailsDTO task) = repo.Read(1);  
            var expected = new TaskDetailsDTO(1, "Design new stuff", "Kong", DateTime.UtcNow,"Kong", new[]{"Design", "Discussion"}.ToList().AsReadOnly(), State.New, DateTime.UtcNow);
            Assert.Equal(expected.Id, task.Id);
            Assert.Equal(expected.Title, task.Title);
            Assert.Equal(expected.AssignedToName, task.AssignedToName);
            Assert.Collection(task.Tags, 
                        tags => Assert.Equal("Design", tags),
                        tags => Assert.Equal("Discussion", tags));
            Assert.Equal(expected.Created, task.Created, precision: TimeSpan.FromSeconds(5));
            Assert.Equal(expected.State, task.State);
            Assert.Equal(expected.StateUpdated, task.StateUpdated, precision: TimeSpan.FromSeconds(5));
            Assert.Equal(Response.Found, response);
            repo.Update(new TaskUpdateDTO{Id = 1, Title= "No designing here", AssignedToId = 1,Tags =  new[]{"No Design", "No Discussion"}.ToList().AsReadOnly(),State = State.New});
            (var r, var t) = repo.Read(1);
            Assert.Equal("No designing here", t.Title);
        }

        public void Dispose()
        {
            context.Dispose();
        }
        
    }
}
