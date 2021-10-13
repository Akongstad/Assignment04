using System;
using System.Collections.Generic;
using System.Linq;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class UserRepository : IUserRepository
    {
        private readonly KanbanContext context;

        public UserRepository(KanbanContext context) =>  this.context = context;
        

        public (Response Response, int UserId) Create(UserCreateDTO user)
        {
             if(context.Users.Where(u => u.Email.Equals(user.Email)).FirstOrDefault() != null)
            {
                return (Response.Conflict, 0);
            }
            var newUser = new User{
                   Name = user.Name,
                   Email = user.Email
               };
            context.Users.Add(newUser);
            context.SaveChanges();
            return (Response.Created, newUser.Id);
        }

        public Response Delete(int userId, bool force)
        {
            if(force == false) return Response.Conflict;
            var userEntity = context.Users.Find(userId);

            if(userEntity == null) 
            {
                return Response.NotFound;
            } 
            context.Users.Remove(userEntity);
            context.SaveChanges();
            return Response.Deleted;
            
        }

        public UserDTO Read(int userId)
        {
            User user = context.Users.FirstOrDefault(u => u.Id == userId);
            if(user == null){
                return null;
            }
            var userDTO = new UserDTO(
                user.Id,
                user.Name,
                user.Email
            );
            return(userDTO);
        }

        public IReadOnlyCollection<UserDTO> ReadAll() =>
            context.Users
                    .Select(u => new UserDTO(u.Id, u.Name, u.Email))
                    .ToList()
                    .AsReadOnly();
        

        public Response Update(UserUpdateDTO user)
        {
            User updateUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
            if(updateUser == null){
                return Response.NotFound;
            }

            updateUser.Name = user.Name;
            updateUser.Email = user.Email;

            context.Users.Update(updateUser);
            context.SaveChanges();
            return Response.Updated;
        }
    }
}
