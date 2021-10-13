using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository
    {
        private readonly KanbanContext context;

        public TagRepository(KanbanContext context)
        {
            this.context = context;
        }

        public (Response Response, int TagId) Create(TagCreateDTO tag)
        {
            var oldTag = context.Tags.FirstOrDefault(t => t.Name == tag.Name);
            if(oldTag != null){
                return (Response.Conflict, 0);
            } 
            var newTag = new Tag{Name = tag.Name};
            context.Tags.Add(newTag);
            context.SaveChanges();
            return (Response.Created, newTag.Id);
        }

        public Response Delete(int tagId, bool force)
        {
            if(!force) return Response.Conflict;
            var tagEntity = context.Tags.FirstOrDefault(t => t.Id == tagId);
            if(tagEntity == null) return Response.NotFound;
            
            context.Tags.Remove(tagEntity);
            context.SaveChanges();
            return Response.Deleted;
            
        }

        public TagDTO Read(int tagId)
         {
            var tag = context.Tags.FirstOrDefault(t => t.Id == tagId);
            if(tag == null){
                return null;
            }
            return new TagDTO(tag.Id,tag.Name);
        } 
        public IReadOnlyCollection<TagDTO> ReadAll() => 
        context.Tags.Select(t => new TagDTO(t.Id, t.Name))
                    .ToList()
                    .AsReadOnly();

        public Response Update(TagUpdateDTO tag)
        {
            var updateTag = context.Tags.FirstOrDefault(t => t.Id == tag.Id);
            if(updateTag == null){
                return Response.NotFound;
            }
            updateTag.Name = tag.Name;
            context.Tags.Update(updateTag);
            context.SaveChanges();
            return Response.Updated;
        }
    }
}
