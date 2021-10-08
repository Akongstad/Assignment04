namespace Assignment4.Entities
{
    public class TagRepository
    {
        private readonly KanbanContext context;

        public TagRepository(KanbanContext context)
        {
            this.context = context;
        }
    }
}
