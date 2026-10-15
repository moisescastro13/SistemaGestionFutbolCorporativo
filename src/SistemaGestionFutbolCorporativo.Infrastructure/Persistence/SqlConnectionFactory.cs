using System.Data.Common;
using Microsoft.Extensions.Configuration;
using SistemaGestionFutbolCorporativo.Application.Common;
using Npgsql; 

namespace SistemaGestionFutbolCorporativo.Infrastructure.Persistence;

public class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    private readonly string _connectionString = 
        configuration.GetConnectionString("Database") 
        ?? throw new InvalidOperationException("ConnectionString is empty.");

    public DbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}