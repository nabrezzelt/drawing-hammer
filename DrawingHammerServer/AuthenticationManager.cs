﻿using HelperLibrary.Cryptography;

namespace DrawingHammerServer
{
    public class AuthenticationManager
    {        
        public (bool Result, User User) IsValid(string username, string password)
        {
            User user = UserManager.GetUser(username);

            if (user == null)
                return (false, null);

            string salt = UserManager.GetUserSalt(username);

            string passwordHash = HashManager.HashSha256(password + salt);

            return passwordHash == user.PasswordHash ? (true, user) : (false, null);
        }        
    }
}
