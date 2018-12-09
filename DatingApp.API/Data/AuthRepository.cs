using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;
        public AuthRepository(DataContext context)
        {
            this.context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if(user == null){
                return null;
            }

            if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt)){
                return null;
            }

            return user;

        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            CreatePaswordHash(password,out passwordHash,out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await context.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        void CreatePaswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt){

                using(var hmac = new System.Security.Cryptography.HMACSHA512()) {

                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                }                

        }

        bool VerifyPasswordHash(string password,byte[] passwordHash,byte[] passwordSalt){

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0;i<passwordHash.Length;i++){
                    if(passwordHash[i]!=computedHash[i]){
                        return false;
                    }
                }

                return true;
            }
        }

        public async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(u => u.Username == username);
        }
    }
}