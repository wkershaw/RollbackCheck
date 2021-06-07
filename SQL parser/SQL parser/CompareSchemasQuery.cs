using System;
using System.Data.SqlClient;

namespace S2C
{
    class CompareSchemasQuery
    {
        public CompareSchemasQuery(string table1, string table2)
        {
            Table1 = table1;
            Table2 = table2;
        }
        public string Table1
        {
            get; set;
        }
        public string Table2
        {
            get; set;
        }

        private string Query
        {
            get
            {
                return $@"
                   (
	                    SELECT * from sys.dm_exec_describe_first_result_set (N'SELECT * FROM {Table1}', NULL, 0)
	                    EXCEPT
	                    SELECT * from sys.dm_exec_describe_first_result_set (N'SELECT * FROM {Table2}', NULL, 0)
                    )
                    UNION
                    (
	                    SELECT * from sys.dm_exec_describe_first_result_set (N'SELECT * FROM {Table2}', NULL, 0)
	                    EXCEPT
	                    SELECT * from sys.dm_exec_describe_first_result_set (N'SELECT * FROM {Table1}', NULL, 0)
                    );";
            }
        }

        public bool SchemasIdentical(SqlConnection connection)
        {
            bool schemaCorrect;
            using (SqlCommand command = new SqlCommand(Query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    schemaCorrect = !reader.HasRows;
                }
            }
            return schemaCorrect;
        }

        public string SchemaDifferences(SqlConnection connection)
        {
            string differences = "";
            using (SqlCommand command = new SqlCommand(Query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            differences += reader.GetValue(i).ToString() + ",";
                        }
                        differences += "\n";
                    }
                }
            }
            return differences;
        }

    }
}
