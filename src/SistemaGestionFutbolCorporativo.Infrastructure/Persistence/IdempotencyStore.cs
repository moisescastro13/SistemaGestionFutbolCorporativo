using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;

namespace SistemaGestionFutbolCorporativo.Infrastructure.Persistence;
public class IdempotencyStore(ApplicationDbContext dbContext) : IIdempotencyStore
{
    public async Task<IdempotencyRecord?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<IdempotencyRecord>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Key == key, cancellationToken);
    }

    public async Task<bool> TryCreateAsync(string key, string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var record = new IdempotencyRecord
            {
                Key = key,
                Path = path,
                IsCompleted = false
            };

            await dbContext.Set<IdempotencyRecord>().AddAsync(record, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    } 

    public async Task MarkCompletedAsync(string key, int statusCode, string? responseBody, CancellationToken cancellationToken = default)
    {
        var record = await dbContext.Set<IdempotencyRecord>().FindAsync([key], cancellationToken);
        if (record is not null)
        {
            record.StatusCode = statusCode;
            record.ResponseBody = responseBody;
            record.IsCompleted = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}