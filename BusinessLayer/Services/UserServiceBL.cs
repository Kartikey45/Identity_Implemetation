using BusinessLayer.Interfaces;
using CommonLayer.Models;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserServiceBL : IUserServiceBL
    {
        private IUserService userService;

        public UserServiceBL(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            try
            {
                var data = await userService.LoginUserAsync(model);
                return data;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                var data = await userService.RegisterUserAsync(model);
                return data;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
