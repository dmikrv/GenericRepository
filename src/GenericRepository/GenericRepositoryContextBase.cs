using GenericRepository.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository;

public abstract class GenericRepositoryContextBase<TContext>(DbContextOptions<TContext> options)
    : DbContext(options) where TContext : DbContext
{
    protected virtual string DefaultDateSql => "GetUtcDate()"; // T-SQL

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.IgnoreCompositePrimaryKeys();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly); // TODO: customize

        modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
        modelBuilder.DefaultValueForAutoAuditDates(DefaultDateSql);
        modelBuilder.DefaultValueForAutoAuditUserIds();
        modelBuilder.NullableValueForAutoAuditUserIds();
        modelBuilder.SetNotModifiedTenantIdColumn();
    }
}