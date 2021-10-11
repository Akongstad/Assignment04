using System.Collections.Generic;
using Assignment4.Core;

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
            throw new System.NotImplementedException();
        }

        public Response Delete(int tagId, bool force = false)
        {
            throw new System.NotImplementedException();
        }

        public TagDTO Read(int tagId)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public Response Update(TagUpdateDTO tag)
        {
            throw new System.NotImplementedException();
        }
    }
}
