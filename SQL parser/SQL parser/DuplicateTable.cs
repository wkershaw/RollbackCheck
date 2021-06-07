using System;
using System.Data.SqlClient;

namespace S2C
{
    class DuplicateTable
    {
        private static string DuplicateQuery(string table, string newTableName)
        {
            return $@"SELECT * INTO {newTableName} FROM {table};";
        }


        public static string Duplicate(string table, SqlConnection connection)
        {
            string newTableName = table + "_ROLLBACK_TEST_BACKUP_" + DateTime.Now.ToShortTimeString().Replace(":", "");
            string query = DuplicateQuery(table, newTableName);

            Logger.PrintInfo($"Duplicating table: '{table}' as '{newTableName}'");

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }

            return newTableName;
        }

    }
}
