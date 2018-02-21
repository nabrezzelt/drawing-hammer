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
        private static readonly MySQLDatabaseManager DbManager = MySQLDatabaseManager.GetInstance();
        
        public static User GetUser(int id)
        {
            string query = "SELECT username, password, isBanned " +
                           "FROM users " +
                           "WHERE id = " + id;

            var reader = DbManager.Select(query);

            reader.Read();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            var username = reader.GetString(0);
            var passwordHash = reader.GetString(1);
            var isSuspended = reader.GetBoolean(2);

            reader.Close();

            return new User(id, username, passwordHash, isSuspended);
        }

        public static User GetUser(string username)
        {
            string query = "SELECT id, password, isBanned " +
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
            var isSuspended = reader.GetBoolean(2);

            reader.Close();

            return new User(id, username, passwordHash, isSuspended);
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

        public static string GetUserSalt(int id)
        {
            string query = "SELECT salt " +
                           "FROM user_salt " +
                           "WHERE userID = " + id;
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

            int userID = DbManager.GetLastID();

            query = "INSERT INTO user_salt (userID, salt) VALUES " +
                    "(" + userID + ", '" + salt + "')";
            DbManager.InsertUpdateDelete(query);

            return userID;
        }

        public static void ResetPassword(int userID, string password)
        {
            string salt = GetUserSalt(userID);
            string passwordHash = HashManager.HashSha256(password + salt);

            string query = "UPDATE users " +
                           "SET password = '" + passwordHash + "' " +
                           "WHERE id = " + userID;
            DbManager.InsertUpdateDelete(query);
        }

        public static void DeleteUser(int userID)
        {
            string query = "DELETE FROM users WHERE id = " + userID;
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
                var isSuspended = reader.GetBoolean(3);                

                users.Add(new User(id, username, passwordHash, isSuspended));
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
                var isSuspended = reader.GetBoolean(3);
                
                users.Add(new User(id, username, passwordHash, isSuspended));
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