using DrawingHammerServer.Exceptions;
using HelperLibrary.Cryptography;
using HelperLibrary.Database;

namespace DrawingHammerServer
{
    internal class UserManager
    {
        public const int MaxUsernameLength = 64;
        private static readonly MySQLDatabaseManager DBManager = MySQLDatabaseManager.GetInstance();
        
        public static User GetUser(int id)
        {
            string query = "SELECT username, password, isBanned " +
                           "FROM users " +
                           "WHERE id = " + id;

            var reader = DBManager.Select(query);

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
            DBManager.PrepareQuery(query);
            DBManager.BindValue("@username", username.ToLower());
            var reader = DBManager.ExecutePreparedSelect();

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
            DBManager.PrepareQuery(query);
            DBManager.BindValue("@username", username.ToLower());
            var reader = DBManager.ExecutePreparedSelect();

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
            var reader = DBManager.Select(query);

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

            if (username.Length > MaxUsernameLength)
                throw new UsernameTooLongException();

            string salt = HashManager.GenerateSecureRandomToken();
            string passwordHash = HashManager.HashSha256(password + salt);

            string query = "INSERT INTO users (username, password) VALUES (@username, @password)";
            DBManager.PrepareQuery(query);
            DBManager.BindValue("@username", username.ToLower());
            DBManager.BindValue("@password", passwordHash);
            DBManager.ExecutePreparedInsertUpdateDelete();

            int userID = DBManager.GetLastID();

            query = "INSERT INTO user_salt (userID, salt) VALUES (" + userID + ", '" + salt + "')";
            DBManager.InsertUpdateDelete(query);

            return userID;
        }
    }
}