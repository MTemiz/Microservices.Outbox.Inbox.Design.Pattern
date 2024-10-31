using Dapper;
using System.Data.SqlClient;
using System.Data;

namespace Order.Outbox.Table.Publisher.Service;

public class OrderOutboxSingletonDatabase
{
    private static IDbConnection _dbConnection;

    private static bool _dataReaderState = true;

    public static IDbConnection DbConnection
    {
        get
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            return _dbConnection;
        }
    }

    public static bool DataReaderState
    {
        get { return _dataReaderState; }
    }

    static OrderOutboxSingletonDatabase()
    {
        _dbConnection =
            new SqlConnection(
                "Server=localhost,1433; Database=OrderDb; User Id=sa; Password=Strong@Passw0rd;TrustServerCertificate=True");
    }

    public static async Task<IEnumerable<T>> QueryAsync<T>(string sql)
    {
        return await _dbConnection.QueryAsync<T>(sql);
    }

    public static async Task<int> ExecuteAsync(string sql)
    {
        return await _dbConnection.ExecuteAsync(sql);
    }

    public static void SetDataReaderReady()
    {
        _dataReaderState = true;
    }

    public static void SetDataReaderBusy()
    {
        _dataReaderState = false;
    }
}