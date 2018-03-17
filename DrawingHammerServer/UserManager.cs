using DrawingHammerServer.Exceptions;
using HelperLibrary.Cryptography;
using HelperLibrary.Database;
using System.Collections.Generic;

namespace DrawingHammerServer
{
    internal class UserManager
    {
        public const int MaxUsernameLength = 64;
        public const int MinUsernameLength = 6;
        private static readonly MySqlDatabaseManager DbManager = MySqlDatabaseManager.GetInstance();
        
        public static User GetUser(int userId)
        {
            string query = "SELECT username, password " +
                           "FROM users " +
                          $"WHERE userID = {userId}";

            var reader = DbManager.Select(query);

            reader.Read();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            var username = reader.GetString(0);
            var passwordHash = reader.GetString(1);            

            reader.Close();

            return new User(userId, username, passwordHash);
        }

        public static User GetUser(string username)
        {
            string query = "SELECT id, password " +
                           "FROM users " +
                           "WHERE username = @username";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@username", username.ToLower());
            var reader = DbManager.ExecutePreparedSelect();

            reader.Read();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }
                
            var id = reader.GetInt32(0);
            var passwordHash = reader.GetString(1);            

            reader.Close();

            return new User(id, username, passwordHash);
        }

        public static string GetUserSalt(string username)
        {
            const string query = "SELECT salt " +
                                 "FROM user_salt " +
                                 "JOIN users " +
                                 "ON users.id = user_salt.userID " +
                                 "WHERE users.username = @username";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@username", username.ToLower());
            var reader = DbManager.ExecutePreparedSelect();

            reader.Read();

            if (!reader.HasRows)
            {
                reader.Close();
                return "";
            }

            var salt = reader.GetString(0);

            reader.Close();

            return salt;

        }

        public static string GetUserSalt(int userId)
        {
            string query = "SELECT salt " +
                           "FROM user_salt " +
                          $"WHERE userID = {userId}";
            var reader = DbManager.Select(query);

            reader.Read();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            string salt = reader.GetString(0);

            reader.Close();

            return salt;
        }

        public static int CreateUser(string username, string password)
        {
            if (GetUser(username) != null)
                throw new UserAlreadyExitsException();

            if(username.Length < MinUsernameLength)
                throw new UsernameTooShortException();

            if (username.Length > MaxUsernameLength)
                throw new UsernameTooLongException();

            string salt = HashManager.GenerateSecureRandomToken();
            string passwordHash = HashManager.HashSha256(password + salt);

            string query = "INSERT INTO users (username, password) VALUES " +
                           "(@username, @password)";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@username", username.ToLower());
            DbManager.BindValue("@password", passwordHash);
            DbManager.ExecutePreparedInsertUpdateDelete();

            int userId = DbManager.GetLastId();

            query = "INSERT INTO user_salt (userID, salt) VALUES " +
                   $"({userId}, '{salt}'";
            DbManager.InsertUpdateDelete(query);

            return userId;
        }

        public static void ResetPassword(int userId, string password)
        {
            string salt = GetUserSalt(userId);
            string passwordHash = HashManager.HashSha256(password + salt);

            string query = "UPDATE users " +
                          $"SET password = '{passwordHash}' " +
                          $"WHERE id = {userId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void DeleteUser(int userId)
        {
            string query = $"DELETE FROM user_salt WHERE userID = {userId}";
            DbManager.InsertUpdateDelete(query);

            query = $"DELETE FROM users WHERE id = {userId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static List<User> GetUsers()
        {
            List<User> users = new List<User>();

            const string query = "SELECT * " +
                                 "FROM users";
            var reader = DbManager.Select(query);

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var username = reader.GetString(1);
                var passwordHash = reader.GetString(2);                            

                users.Add(new User(id, username, passwordHash));
            }

            reader.Close();

            return users;
        }

        public static List<User> GetUsers(string usernameFilter)
        {
            List<User> users = new List<User>();

            const string query = "SELECT * " +
                                 "FROM users " +
                                 "WHERE username LIKE @filter";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@filter", "%" + usernameFilter + "%");
            var reader = DbManager.ExecutePreparedSelect();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var username = reader.GetString(1);
                var passwordHash = reader.GetString(2);                
                
                users.Add(new User(id, username, passwordHash));
            }

            reader.Close();

            return users;
        }

        public static void ChangePassword(int userId, string newPassword)
        {
            string salt = GetUserSalt(userId);
            string passwordHash = HashManager.HashSha256(newPassword + salt);

            const string query = "UPDATE users " +
                                 "SET password = @password " +
                                 "WHERE id = @id";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@password", passwordHash);
            DbManager.BindValue("@id", userId);
            DbManager.ExecutePreparedInsertUpdateDelete();
        }
    }
}