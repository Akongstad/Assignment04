using System.IO;
using System;
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
            var BeerTag = new Tag{Name = "Øl"};


            var debugTask  = new Task {Title = "Debug", State = Assignment4.Core.State.New, Tags = new[]{ImplementationTag}};
            var implementTask = new Task {Title = "Implement code",State = Assignment4.Core.State.Active, Tags = new[]{ImplementationTag}, StateUpdated = System.DateTime.UtcNow, Created = System.DateTime.UtcNow};
            var coffeeTask = new Task {Title = "Get Coffee",State = Assignment4.Core.State.Resolved, Tags = new[]{TbdTag, DiscussionTag}, StateUpdated = System.DateTime.UtcNow, Created = System.DateTime.UtcNow};
            var mcdTask = new Task {Title = "Get Mc Donalds",State = Assignment4.Core.State.New, Tags = new[]{TbdTag, DiscussionTag}, StateUpdated = System.DateTime.UtcNow, Created = System.DateTime.UtcNow};
            var designTask = new Task {Title = "Design new stuff", State = Assignment4.Core.State.New, Tags = new[]{DesignTag, DiscussionTag}, StateUpdated = System.DateTime.UtcNow, Created = System.DateTime.UtcNow};
            var drinkTask = new Task {Title = "Drik øl", State = Assignment4.Core.State.Active, Tags = new[]{BeerTag}, StateUpdated = System.DateTime.UtcNow, Created = System.DateTime.UtcNow};

            context.Users.AddRange( 
                new User {Name = "Bruhn", Email = "pebn@itu.dk", Tasks = new[]{debugTask, mcdTask}}, 
                new User {Name = "Von Barner", Email = "beal@itu.dk", Tasks = new[]{coffeeTask, implementTask}},
                new User {Name = "Kong", Email = "kong@itu.dk", Tasks = new[]{designTask}},
                new User {Name = "Soma", Email = "Alsoma@gmail.com", Tasks = new[]{drinkTask}}
            );
            context.SaveChanges();
        } 
    }
