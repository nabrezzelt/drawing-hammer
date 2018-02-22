using DrawingHammerPacketLibrary;
using HelperLibrary.Database;
using System.Collections.Generic;

namespace DrawingHammerServer
{
    internal class WordManager
    {
        private static readonly MySQLDatabaseManager DbManager = MySQLDatabaseManager.GetInstance();

        public static List<Word> GetWord(IList<Word> pickedWords, int limit = 3)
        {
            string query = "SELECT id, word " +
                           "FROM words  " +
                          $"{BuildWhere(pickedWords)} " +
                           "ORDER BY RAND() " +
                          $"LIMIT {limit}";

            var reader = DbManager.Select(query);            

            var words = new List<Word>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var word = reader.GetString(1);

                words.Add(new Word(id, word));
            }            

            reader.Close();

            return words;
        }

        private static string BuildWhere(IList<Word> words)
        {
            var wheres = new List<string>();            

            foreach (var word in words)
            {
                wheres.Add($"id != {word.Id}");
            }

            if (wheres.Count > 0)
                return "WHERE " + string.Join(" AND ", wheres);

            return string.Empty;
        }
    }
}
