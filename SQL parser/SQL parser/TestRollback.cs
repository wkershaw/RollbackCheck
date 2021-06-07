using System;
using System.Data.SqlClient;
namespace S2C
{
    class TestRollback
    {
        public static bool Test(string table, string backupTable, SqlConnection connection, bool showDifferences)
        {
            Logger.PrintInfo($"Testing '{table}'");
            
            CompareSchemasQuery compareSchemas = new CompareSchemasQuery(table, backupTable);
            CompareDataQuery compareData = new CompareDataQuery(table, backupTable);

            if (compareSchemas.SchemasIdentical(connection))
            {
                if (compareData.DataIdentical(connection))
                {
                    Logger.PrintSuccess("Passed");
                    return true;
                }
                else
                {
                    Logger.PrintError("Data not identical");
                    if (showDifferences)
                    {
                        Console.WriteLine(compareData.DataDifferences(connection));

                    }
                    return false;
                }
            }
            else
            {
                Logger.PrintError("Schemas not identical");
                if (showDifferences)
                {
                    Console.WriteLine(compareSchemas.SchemaDifferences(connection));
                }
                return false;
            }
        }
    }
}
