using DataLayer.Context;

namespace CoreLayer.Services.Users;

public class UserService : BaseService, IUserService
{
    public UserService(EChatApplicationContext context) : base(context)
    {
    }
}