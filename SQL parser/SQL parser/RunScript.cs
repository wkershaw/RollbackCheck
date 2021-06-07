using System.Data.SqlClient;
using System.IO;

namespace S2C
{
    class RunScript
    {
        public static void Run(string script, SqlConnection connection)
        {
            string sql = File.ReadAllText(script);

            Logger.PrintInfo($"Running {script}");

            if (sql.Length > 0)
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }            
            }
        }
    }
}
