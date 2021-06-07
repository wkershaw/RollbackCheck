using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace S2C
{
    class Program
    {
        // Specify all tables to be check after rollback
        static List<string> tables = new List<string>{};

        static List<string> backupTables = new List<string>();
        static readonly string connString = "Data Source=localhost;Database=S2C;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False";    
        static readonly string preSchemaCompare = Path.Combine("..\\..\\..\\", "PreSchemaCompare.sql");
        static readonly string preSchemaCompareRollback = Path.Combine("..\\..\\..\\", "PreSchemaCompareRollback.sql");

        /**
         * Entry point for Rollback Check
         * comprises of 4 steps:
         * Duplicate all tables specified in <see cref="tables"/>
         * Run PreSchemaCompare and PreSchemaCompareRollback scripts stored in this project
         * Compare all tables specified in <see cref="tables"/> with the backups made, checking for schema and data changes
         * Delete the backup table if the comparison passes, otherwise the backup tables are kept for manual rollback
         */
        static void Main(string[] args)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    foreach (string table in tables)
                    {
                        string backupTable = DuplicateTable.Duplicate(table, connection);
                        backupTables.Add(backupTable);
                    }

                    RunScript.Run(preSchemaCompare, connection);
                    RunScript.Run(preSchemaCompareRollback, connection);

                    for (int i = 0; i < tables.Count; i++)
                    {
                        // Change the last argument to true to print out any rows differences between the tables
                        if (TestRollback.Test(tables[i], backupTables[i], connection, false))
                        {
                            DropTable.Drop(backupTables[i], connection);
                            Logger.PrintInfo($"'{backupTables[i]}' has been dropped\n");
                        }
                        else
                        {
                            Logger.PrintError($"'{backupTables[i]}' has not been dropped\n");
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
