using System.Linq;
using FastMember;

namespace Common.Tools;

public static class BulkCopyUtil
{
    /// <summary>
    /// A Dapper like interface that uses SqlBulkCopy and FastMember ObjectReader to allow insertion of large numbers of objects into a given db table.
    /// Like Dapper the library only supports object properties (no public data members)
    /// </summary>
    /// <param name="value">An IDbConnection that is a SqlConnection, IDbConnection interface is used to provide interoperability with Dapper code.</param>
    /// <param name="objects">An IList of objects to be inserted</param>
    /// <param name="tableName">Name of the table to insert to, recommended to prefix it with schema.</param>
    /// <param name="batchSize">Size of the batches used to insert data, defaults to 1000.</param>
    /// <param name="excludeFields">If any member properties are to be excluded from the insert operation, provide a list of member names in this parameter</param>
    /// <typeparam name="T">Type of object to be inserted, has to be a class, type is inspected for public properties</typeparam>
    public static void BulkInsert<T>(this IDbConnection value, IList<T> objects, string tableName, int batchSize = 1000, IList<string> excludeFields = null) where T : class
    {
        if (value is not SqlConnection connection) return;

        var fields = typeof(T).GetProperties().Select(x => x.Name).ToArray();
        if (excludeFields != null)
        {
            fields = fields.Except(excludeFields).ToArray();
        }

        using var bulkCopy = new SqlBulkCopy(connection);
        bulkCopy.DestinationTableName = tableName;
        bulkCopy.BatchSize = batchSize;

        using var reader = ObjectReader.Create(objects, fields);
        bulkCopy.WriteToServer(reader);
    }
}