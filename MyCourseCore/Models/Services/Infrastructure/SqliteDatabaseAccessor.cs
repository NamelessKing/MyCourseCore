using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourseCore.Models.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Services.Infrastructure
{
    public class SqliteDatabaseAccessor : IDatabaseAccessor
    {
        public SqliteDatabaseAccessor(ILogger<SqliteDatabaseAccessor> logger, IOptionsMonitor<ConnectionStringsOptions> connectionStringOption)
        {
            Logger = logger;
            ConnectionStringOption = connectionStringOption;
        }

        private ILogger<SqliteDatabaseAccessor> Logger { get; }
        public IOptionsMonitor<ConnectionStringsOptions> ConnectionStringOption { get; }

        public async Task<DataSet> QueryAsync(FormattableString formattableQuery)
        {
            Logger.LogInformation(formattableQuery.Format, formattableQuery.GetArguments());

            //Creiamo dei SqliteParameter a partire dalla FormattableString
            var queryArguments = formattableQuery.GetArguments();
            var sqliteParameters = new List<SqliteParameter>();
            for (var i = 0; i < queryArguments.Length; i++)
            {
                var parameter = new SqliteParameter(i.ToString(), queryArguments[i]);
                sqliteParameters.Add(parameter);
                queryArguments[i] = "@" + i;
            }
            string query = formattableQuery.ToString();

            string connectionString = ConnectionStringOption.CurrentValue.Default;
            using (var connection = new SqliteConnection(connectionString))
            { 
                await connection.OpenAsync();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddRange(sqliteParameters);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var dataSet = new DataSet
                        {
                            EnforceConstraints = false//TODO : da togliere o aggiornare pacchetto
                        };

                        do
                        {
                            var dataTable = new DataTable();
                            dataSet.Tables.Add(dataTable);
                            dataTable.Load(reader);
                        } while (!reader.IsClosed);

                        return dataSet;
                    }
                }
            }
        }
    }
}
