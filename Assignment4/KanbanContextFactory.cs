using System.IO;
using Assignment4.Core;
using Assignment4;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class KanbanContextFactory : IDesignTimeDbContextFactory<KanbanContext>
{
    public KanbanContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<Program>()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("Kanban");

        var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>()
            .UseSqlServer(connectionString);

        return new KanbanContext(optionsBuilder.Options);
    }
      public static void seed(KanbanContext context){

            context.Database.ExecuteSqlRaw("DELETE dbo.Tags");
            context.Database.ExecuteSqlRaw("DELETE dbo.TagTask");
            context.Database.ExecuteSqlRaw("DELETE dbo.Tasks");
            context.Database.ExecuteSqlRaw("DELETE dbo.Users");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tags', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tasks', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Users', RESEED, 0)");

            var DesignTag = new Tag {Name = "Design"};
            var DiscussionTag = new Tag {Name = "Discussion"};
            var ImplementationTag = new Tag{Name = "Implementation"};
            var TbdTag = new Tag{Name = "Tbd"};


            var debugTask  = new Task {Title = "Debug", State = Assignment4.Core.State.New, Tags = new[]{ImplementationTag}};
            var implementTask = new Task {Title = "Implement code",State = Assignment4.Core.State.Active, Tags = new[]{ImplementationTag}};
            var coffeeTask = new Task {Title = "Get Coffee",State = Assignment4.Core.State.Resolved, Tags = new[]{TbdTag, DiscussionTag}};
            var mcdTask = new Task {Title = "Get Mc Donalds",State = Assignment4.Core.State.New, Tags = new[]{TbdTag, DiscussionTag}};
            var designTask = new Task {Title = "Design new stuff", State = Assignment4.Core.State.New, Tags = new[]{DesignTag, DiscussionTag}};

            context.Users.Add(new User {Name = "Bruhn", Email = "pebn@itu.dk", Tasks = new[]{debugTask, mcdTask}});
            context.Users.Add(new User {Name = "Von Barner", Email = "beal@itu.dk", Tasks = new[]{coffeeTask, implementTask}});
            context.Users.Add(new User {Name = "Kong", Email = "kong@itu.dk", Tasks = new[]{designTask}});     
            context.SaveChanges();
        } 
    }
