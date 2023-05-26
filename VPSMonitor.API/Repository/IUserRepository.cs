using VPSMonitor.API.Entities;

namespace VPSMonitor.API.Repository;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserByEmail(string email);
    Task<User> ValidateUserLoginModel(UserLoginModel model);
}