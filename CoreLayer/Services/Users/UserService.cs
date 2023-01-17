using CoreLayer.Utilities.Security;
using CoreLayer.ViewModels.Auth;
using DataLayer.Context;
using DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CoreLayer.Services.Users
{
    public class UserService : BaseService, IUserService
    {
        public UserService(EChatApplicationContext context) : base(context)
        {
        }

        public async Task<bool> IsUserExist(string userName)
        {
            return await Table<User>().AnyAsync(u => u.UserName == userName.ToLower());
        }

        public async Task<bool> IsUserExist(long userId)
        {
            return await Table<User>().AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> RegisterUser(RegisterViewModel registerModel)
        {
            if (await IsUserExist(registerModel.UserName))
                return false;

            if (registerModel.Password != registerModel.RePassword)
                return false;

            var password = registerModel.Password.EncodePasswordMd5();
            var user = new User()
            {
                AvatarName = "Default.jpg",
                Password = password,
                UserName = registerModel.UserName.ToLower()
            };
            Insert(user);
            await Save();
            return true;
        }

        public async Task<User?> LoginUser(LoginViewModel loginModel)
        {
            var user = await Table<User>().SingleOrDefaultAsync(x => x.UserName.Equals(loginModel.UserName.ToLower()));

            if (user == null) return null;

            var password = loginModel.Password.EncodePasswordMd5();

            return password == user.Password ? user : null;
        }
    }
}