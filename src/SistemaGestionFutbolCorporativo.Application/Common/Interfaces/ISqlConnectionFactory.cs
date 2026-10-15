namespace SistemaGestionFutbolCorporativo.Application.Common;

public interface ISqlConnectionFactory
{
    System.Data.Common.DbConnection CreateConnection();
}
