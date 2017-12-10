using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingHammerServer
{
    internal class Word_Manager
    {
        private static readonly MySQLDatabaseManager DBManager = MySQLDatabaseManager.GetInstance();

        public static User GetWordGetUser(int id)
        {
            string query = "SELECT word FROM words ORDER BY RAND() Limit 3";

            var reader = DBManager.Select(query);

            reader.Read();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            var word = reader.GetString(0);

            reader.Close();


            return new word(word);
        }
    }
}
