using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        public readonly DataContext _context;
        private readonly IConfiguration _config;
        public AuthRepository(DataContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
            if(user == null) {
                response.Message = "User not found!";
                response.Success = false;
            }else if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
                response.Success = false;
                response.Message = "Incorrect password";
            } else {
                response.Data = CreateToken(user);
            }
            return response;

        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if(await UserExists(user.Username)) {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }
            
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower())){
                return true;
            }
            return false;
        }    
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt){
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++) {
                    if(computedHash[i] != passwordHash[i]) {
                        return false;
                    }
                }
            }
            return true;
        }

        private string CreateToken(User user) {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)
            );
            SigningCredentials creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}