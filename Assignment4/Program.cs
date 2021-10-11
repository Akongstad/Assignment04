using System;
using System.IO;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Assignment4
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configuration = LoadConfiguration();
            var connectionString = configuration.GetConnectionString("Kanban");
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(connectionString);
            using var context = new KanbanContext(optionsBuilder.Options);
            KanbanContextFactory.seed(context);
            var taskRepo = new TaskRepository(context);
            Console.WriteLine(taskRepo.Create(new Core.TaskDTO {Title = "Debuggings", State = Assignment4.Core.State.New, Tags = new[]{"Do the debug"}}));
            foreach (var item in taskRepo.All()){
                Console.WriteLine(item);
            }
            Console.WriteLine(taskRepo.FindById(1));
        }
        static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>();

            return builder.Build();
        }
    }
}
