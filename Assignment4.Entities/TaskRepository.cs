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

        public IReadOnlyCollection<TaskDTO> ReadAll() =>
            context.Tasks
                    .Select(t => new TaskDTO(t.Id, t.Title, t.User.Name, t.Tags.Select(n => n.Name).ToList().AsReadOnly(), t.State))
                    .ToList().AsReadOnly();

        /* public IReadOnlyCollection<TaskDTO> ReadAll(){
           var tasks = new List<TaskDTO>();
           foreach (var item in context.Tasks)
           {
                tasks.Add(new TaskDTO{
                   Id = item.Id,
                   Title = item.Title,
                   AssignedToName = item.User.Name,
                   Tags =   item.Tags.Select(n => n.Name).ToList(),
                   State = item.State
               }
               );
           }
            return tasks;
        } */
       
        public (Response Response, int TaskId) Create(TaskCreateDTO task){
            var newTask = new Task{
                   Title = task.Title,
                   Description = task.Description,
                   UserID = task.AssignedToId,
                   Tags = GetTags(task.Tags).ToList(),
                   State = State.New,
                   Created = DateTime.UtcNow,
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
                    task.UserID = null;
                    task.Tags = null;
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
        
        public (Response, TaskDetailsDTO) Read(int id) {
        Task task = context.Tasks.FirstOrDefault(t => t.Id == id);
        if(task == null){
            return (Response.NotFound, null);
        }
        TaskDetailsDTO taskDetails = new TaskDetailsDTO(
            task.Id,
            task.Title,
            task.Description,
            task.Created,
            task.User.Name,
            task.Tags.Select(n => n.Name).ToList(),
            task.State,
            task.StateUpdated
            );
            return (Response.Found, taskDetails);
        }
       
        public Response Update(TaskUpdateDTO task){ 
            Task oldTask = context.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if(oldTask == null){
                return Response.NotFound;
            }
             if(context.Users.FirstOrDefault(u => u.Id == task.AssignedToId)==null){
                return Response.BadRequest;
            }
            oldTask.Title = task.Title;
            oldTask.Description = task.Description;
            oldTask.UserID = task.AssignedToId;
            oldTask.Tags = task.Tags.Select(n => new Tag{Name = n}).ToList();
            if (oldTask.State != task.State){
                oldTask.StateUpdated = DateTime.UtcNow;
            }
            oldTask.State = task.State;
            context.Update(oldTask);
            context.SaveChanges();
            return Response.Updated;
        }
        public IReadOnlyCollection<TaskDTO> ReadAllRemoved() => 
            ReadAll()
            .Where(r => r.State == State.Removed)
            .ToList().AsReadOnly();
        

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag) => 
            ReadAll()
            .Where(t => t.Tags.Contains(tag))
            .ToList().AsReadOnly();
        

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId){
            var tasks = from t in context.Tasks
            where t.UserID == userId
            select new TaskDTO(t.Id, t.Title, t.User.Name, t.Tags.Select(n => n.Name).ToList(), t.State);
            return tasks.ToList().AsReadOnly();
        }
        
        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state) => 
            ReadAll()
            .Where(r => r.State == state)
            .ToList().AsReadOnly();
        
         private IEnumerable<Tag> GetTags(IEnumerable<string> tags)
        {
            var existing = context.Tags.Where(t => tags.Contains(t.Name)).ToDictionary(t => t.Name);

            foreach (var tag in tags)
            {
                yield return existing.TryGetValue(tag, out var t) ? t : new Tag { Name = tag };
            }    
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }
}