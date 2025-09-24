using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;

public interface IApplicationDbContext
{

    DbSet<TodoItem> TodoItems { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}