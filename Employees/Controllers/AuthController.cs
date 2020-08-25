using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Employees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserServiceBL service;

        public AuthController(IUserServiceBL service)
        {
            this.service = service;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> RegisterAsync([FromBody]RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await service.RegisterUserAsync(model);

                    if (result.IsSuccess)
                    {
                        return Ok(new
                        {
                            success = true,
                            Message = "Registration Successfull"
                        });
                    }
                    else
                    {
                        return Conflict(new { success = false, Message = "registration failed" });
                    }
                }
                else
                {
                    return UnprocessableEntity(new { success = false, Message = "Invalid input"});
                }
            }
            catch(Exception e)
            {
                return BadRequest(new { success = false, Message = e.Message });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> LoginAsync([FromBody]LoginViewModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var result = await service.LoginUserAsync(model);

                    if(result.IsSuccess)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
                else
                {
                    return UnprocessableEntity(new { success = false, message = "Invalid Input"});
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { success = false, Message = ex.Message });
            }
        }
    }
} 
