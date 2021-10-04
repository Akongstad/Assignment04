using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class TaskRepository
    {
        private readonly SqlConnection _connection;

        public TaskRepository(SqlConnection connection)
        {
            _connection = connection;
        }
        IEnumerable<TaskDTO> All(){ //Should/used to be IReadOnlyCollection<TaskDTO>
            var cmdText = @"SELECT t.Id, t.Title, t.Description, t.AssignedToId, t.Tags, t.State
                            FROM Tasks AS t
                            LEFT JOIN Users AS u ON t.AssignedToId = u.ID
                            ORDER BY t.Title";

            using var command = new SqlCommand(cmdText, _connection);

            OpenConnection();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new TaskDTO
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    AssignedToId = reader.GetInt32("AssignedToId"),
                    //Tags = reader.Get("Tags"),//?????
                    //State = reader.Get("State") //getString? GetEnum?
                };
            }

            CloseConnection();
        }
        int Create(TaskDTO task){
            var cmdText = @"INSERT Task (Title, Description, AssignedToId, Tags, State)
                            VALUES (@Title, @Description, @AssignedToId, @Tags, @State);
                            SELECT SCOPE_IDENTITY()";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@AssignedToId", task.AssignedToId);
            command.Parameters.AddWithValue("@Tags", task.Tags);
            command.Parameters.AddWithValue("@State", task.State);

            OpenConnection();

            var id = command.ExecuteScalar();

            CloseConnection();

            return (int)id;
        }
        void Delete(int taskId){
            var cmdText = @"DELETE Task WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", taskId);

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
        }
        //TaskDetailsDTO FindById(int id); TO DO!
        void Update(TaskDTO task){
            var cmdText = @"UPDATE Tasks SET
                            Title = @Title,
                            Description = @Description,
                            AssignedToId = @AssignedToId,
                            Tags = @Tags
                            State = @State
                            WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", task.Id);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@AssignedToId", task.AssignedToId);
            command.Parameters.AddWithValue("@Tags", task.Tags);
            command.Parameters.AddWithValue("@State", task.State);

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
        }
         
         private void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
