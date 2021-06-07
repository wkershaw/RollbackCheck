using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2C
{
    class DropTable
    {
        public static void Drop(string table, SqlConnection connection)
        {
            string query = $@"DROP TABLE {table};";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
