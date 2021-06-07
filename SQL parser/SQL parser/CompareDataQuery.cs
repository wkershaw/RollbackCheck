using System;
using System.Data.SqlClient;
namespace S2C
{
    class CompareDataQuery
    {
        public CompareDataQuery(string table1, string table2)
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
	                    SELECT * from {Table1}
	                    EXCEPT
	                    SELECT * from {Table2}
                    )
                    UNION
                    (
	                    SELECT * from {Table2}
	                    EXCEPT
	                    SELECT * from {Table1}
                    );";
            }
        }

        public bool DataIdentical(SqlConnection connection)
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

        public string DataDifferences(SqlConnection connection)
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
