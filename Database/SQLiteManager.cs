using Microsoft.Data.Sqlite;

namespace RestfulDemo.Database
{
    public class SQLiteManager
    {
        private static SqliteConnection _sql = null;


        public static void connect(String fileName = "database.sqlite")
        {
            string _strConnect = $"Data Source={fileName};";
            SQLiteManager._sql = new SqliteConnection(_strConnect);
        }

        public static void disconnect()
        {
            if (_sql != null) _sql.Close();
            _sql = null;
        }

        public static SqliteConnection getConnection()
        {
            if (_sql == null) connect();
            return _sql;
        }

    }
}
