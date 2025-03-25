using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.Models.DTOs;
using blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace blog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserServices _userServices;
        public UserController(UserServices userServices){
            _userServices = userServices;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody]UserDTO user) {
            bool success = await _userServices.CreateUser(user);
            //we want to return objects since this would be a api call
            if(success) return Ok(new {Success = true});
            return BadRequest(new {Success = false, Message = "Username already Exists"});
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserDTO user) {
            var success = await _userServices.Login(user);
            //we want to return objects since this would be a api call
            if(success != null) return Ok(new {Token = success});
            return Unauthorized(new{Message = "Username or Password doesn't match our system"});
        }

        [HttpGet("GetUserInfoByUsername/{username}")]
        public async Task<IActionResult> GetUserInfoByUsername(string username){
            var user = await _userServices.GetUserInfoByUsername(username);

            if(user != null) return Ok(user);
            return BadRequest(new {Message = "No User Found"});
        }

    }
}