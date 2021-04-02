using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Services.Infrastructure
{
    public class SqliteDatabaseAccessor : IDatabaseAccessor
    {
        public DataSet Query(string query)
        {
            using (var connection = new SqliteConnection("Data Source=Data/MyCourse.db"))
            { 
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var dataSet = new DataSet();
                        dataSet.EnforceConstraints = false;//TODO : da togliere o aggiornare pacchetto
                        var dataTable = new DataTable();
                        dataSet.Tables.Add(dataTable);

                        dataTable.Load(reader);

                        return dataSet;
                    }
                }
            }

            throw new NotImplementedException();
        }
    }
}
