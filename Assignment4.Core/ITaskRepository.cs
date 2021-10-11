using System;
using System.Collections.Generic;

namespace Assignment4.Core
{
    public interface ITaskRepository : IDisposable
    {
        IReadOnlyCollection<TaskDTO> All();
        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <returns>The id of the newly created task</returns>
        (Response, int id) Create(TaskDTO task);
        Response Delete(int taskId);
        (Response, TaskDetailsDTO) FindById(int id);
        Response Update(TaskDTO task);
    }
}
