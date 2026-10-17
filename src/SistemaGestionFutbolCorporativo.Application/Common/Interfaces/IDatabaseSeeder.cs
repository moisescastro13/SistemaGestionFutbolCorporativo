namespace SistemaGestionFutbolCorporativo.Application.Common;


public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
public interface IDatabaseSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}