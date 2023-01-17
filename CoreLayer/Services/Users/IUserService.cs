using CoreLayer.ViewModels.Auth;
using DataLayer.Entities.Users;

namespace CoreLayer.Services.Users
{
    public interface IUserService
    {
        Task<bool> IsUserExist(string userName);
        Task<bool> IsUserExist(long userId);
        Task<bool> RegisterUser(RegisterViewModel registerModel);
        Task<User?> LoginUser(LoginViewModel loginModel);
    }
}