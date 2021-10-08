using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using Assignment4.Core;
using System.Linq;
using System;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext context;

        public TaskRepository(KanbanContext context) => this.context = context;
        public IReadOnlyCollection<TaskDTO> All(){
           var tasks = new List<TaskDTO>();
           foreach (var item in context.Tasks)
           {
                tasks.Add(new TaskDTO{
                   Id = item.Id,
                   Title = item.Title,
                   Description = item.Description,
                   AssignedToId = item.UserID,
                   Tags =   item.Tags.Select(n => n.Name).ToList().AsReadOnly(),
                   State = item.State
               }
               );
           }
            return tasks;
        }
        
        public (Response, int id) Create(TaskDTO task){
            var newTask = new Task{
                   Title = task.Title,
                   Description = task.Description,
                   UserID = task.AssignedToId,
                   Tags = task.Tags.Select(n => new Tag{Name = n}).ToList().AsReadOnly(),
                   State = State.New,
                   StateUpdated = DateTime.UtcNow
               };
            context.Tasks.Add(newTask);
            context.SaveChanges();
            return (Response.Created, newTask.Id);
        }

        public Response Delete(int taskId)
        {
            Task task = context.Tasks.FirstOrDefault(t => t.Id == taskId);
            Response response;
            if(task == null){
                return Response.NotFound;
            }
            switch (task.State){
                case State.New:
                    context.Remove(task);
                    response = Response.Deleted;
                    break;

                case State.Active:
                    task.State = State.Removed;
                    response = Response.Deleted;
                    break;

                case State.Resolved:
                case State.Closed:
                case State.Removed:
                    response = Response.Conflict;
                    break;
                default:
                    response = Response.BadRequest;
                break;
            }
            context.SaveChanges();
            return response;
        }
        
        public (Response, TaskDetailsDTO) FindById(int id) {
        Task task = context.Tasks.FirstOrDefault(t => t.Id == id);
        if(task == null){
            return (Response.NotFound, null);
        }
        TaskDetailsDTO taskDetails = new TaskDetailsDTO{
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            AssignedToId = task.UserID,
            AssignedToEmail = task.User.Email,
            AssignedToName = task.User.Name,
            Tags = task.Tags.Select(n => n.Name).ToList().AsReadOnly(),
            State = task.State
        };
            return (Response.Found, taskDetails);
        }
       
        public Response Update(TaskDTO task){ //TODO. Need to update all attributes. 
            Task oldTask = context.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if(oldTask == null){
            return Response.NotFound;
        }
            if (oldTask.State != task.State){
                oldTask.StateUpdated = DateTime.UtcNow;
                }
            
            context.Update(oldTask);
            context.SaveChanges();
            return Response.Updated;
 
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}
