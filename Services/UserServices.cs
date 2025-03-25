using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using blog.Context;
using blog.Models;
using blog.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace blog.Services
{
    public class UserServices
    {
        private readonly DataContext _dataContext;
        //allow us access to our appsettings
        private readonly IConfiguration _config;
        public UserServices(DataContext dataContext, IConfiguration config){
            _dataContext = dataContext;
            _config = config;
        }
    
        //Create Our CreateUser Method
        //We have to also create a helper function to check whether or not the user already exist
        //We have to encrypt the password

        public async Task<bool> CreateUser(UserDTO newUser) {
            if(await DoesUserExist(newUser.Username)) return false;

            UserModel userToAdd = new();

            PasswordDTO encryptedPassword = HashPassword(newUser.Password);

            userToAdd.Hash = encryptedPassword.Hash;
            userToAdd.Salt = encryptedPassword.Salt;
            userToAdd.UserName = newUser.Username;

            await _dataContext.User.AddAsync(userToAdd);
            return await _dataContext.SaveChangesAsync() != 0;
        }

        private static PasswordDTO HashPassword(string password) {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(64);

            string salt = Convert.ToBase64String(saltBytes);

            string hash;

            using (var derivedBytes = new Rfc2898DeriveBytes(password, saltBytes, 310000, HashAlgorithmName.SHA256)) {
                hash = Convert.ToBase64String(derivedBytes.GetBytes(32));
            }

            return new PasswordDTO{
                Salt = salt,
                Hash = hash
            };
        }

        private async Task<bool> DoesUserExist(string username) => await _dataContext.User.SingleOrDefaultAsync(user => user.UserName == username) != null;
        
        public async Task<string> Login(UserDTO user) {
            UserModel currentUser = await GetUserByUserName(user.Username);

            if(currentUser == null) return null;

            if(!VerifyPassword(user.Password, currentUser.Salt, currentUser.Hash)) return null;

            return GenerateJWToken(new List<Claim>());
        }

        private string GenerateJWToken(List<Claim> claims){
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<UserModel> GetUserByUserName(string userName) => await _dataContext.User.SingleOrDefaultAsync(user => user.UserName == userName);
    
        private static bool VerifyPassword(string password, string salt, string hash){
            byte[] saltByte = Convert.FromBase64String(salt);

            string checkHash;

            using(var derivedBytes = new Rfc2898DeriveBytes(password, saltByte, 310000, HashAlgorithmName.SHA256)) {
                checkHash = Convert.ToBase64String(derivedBytes.GetBytes(32));
            }

            return hash == checkHash;
        }

        public async Task<UserInfoDTO> GetUserInfoByUsername(string username) {
            var currentUser = await _dataContext.User.SingleOrDefaultAsync(user=>user.UserName == username);

            UserInfoDTO user = new();

            user.Id = currentUser.Id;
            user.Username = currentUser.UserName;
            
            return user;
        }


    }
}