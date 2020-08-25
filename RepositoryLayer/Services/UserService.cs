using CommonLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class UserService : IUserService
    {
        private UserManager<IdentityUser> userManager;
        private IConfiguration configuration;

        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return new UserManagerResponse
                    {
                        Message = "There is no user with that Email Address",
                        IsSuccess = false
                    };
                }

                var result = await userManager.CheckPasswordAsync(user, model.Password);

                if (!result)
                {
                    return new UserManagerResponse
                    {
                        Message = "Invalid Passsword",
                        IsSuccess = false
                    };
                }

                var claims = new[]
                {
                    new Claim("Email", model.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));

                var token = new JwtSecurityToken(
                    issuer : configuration["jwt:Issuer"],
                    audience : configuration["jwt:Audience"],
                    claims : claims,
                    expires : DateTime.Now.AddDays(1),
                    signingCredentials : new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                return new UserManagerResponse
                {
                    Message = tokenAsString,
                    IsSuccess = true,
                    ExpireDate = token.ValidTo
                };
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
                if (model == null)
                {
                    throw new NullReferenceException("Register model is null");
                }

                if (model.Password != model.ConfirmPassword)
                {
                    return new UserManagerResponse
                    {
                        Message = "Doesn't match the password",
                        IsSuccess = false
                    };
                }

                var identityUser = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await userManager.CreateAsync(identityUser, model.Password);

                if (result.Succeeded)
                {
                    return new UserManagerResponse
                    {
                        Message = "User created successfully",
                        IsSuccess = true
                    };
                }

                return new UserManagerResponse
                {
                    Message = "User did not create",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
