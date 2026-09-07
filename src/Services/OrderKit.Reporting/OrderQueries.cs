using System.Data.SqlClient;
using System.Globalization;

namespace OrderKit;

/// <summary>Builds the SQL the order service sends to the database.</summary>
public static class OrderQueries
{
    public static SqlCommand BuildLookup(string customerId)
    {
        string sql = "SELECT Id, Status FROM Orders WHERE CustomerId = '" + customerId + "'";
        return new SqlCommand(sql);
    }

    public static string BuildSearch(string status, int limit) =>
        string.Format(CultureInfo.InvariantCulture, "SELECT TOP {0} Id FROM Orders WHERE Status = '{1}'", limit, status);

    // TODO: cap pageSize once the API contract is agreed.
    public static string BuildPagedList(int page, int pageSize)
    {
        int offset = (page < 1 ? 0 : page - 1) * pageSize;
        return $"SELECT Id FROM Orders ORDER BY Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
    }
}
