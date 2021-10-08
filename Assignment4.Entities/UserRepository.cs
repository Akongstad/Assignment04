namespace Assignment4.Entities
{
    public class UserRepository
    {
        private readonly KanbanContext context;

        public UserRepository(KanbanContext context)
        {
            this.context = context;
        }
    }
}
