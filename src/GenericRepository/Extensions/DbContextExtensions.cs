using GenericRepository.Core.Common;
using GenericRepository.Core.Common.Auditable.Create;
using GenericRepository.Core.Common.Auditable.SoftDelete;
using GenericRepository.Core.Common.Auditable.Update;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GenericRepository.Extensions;

/// <summary>
///     Uses to handle timezones for <see cref="DateTime" /> for Entity Framework.
/// </summary>
public static class DbContextExtensions
{
    private const string IsUtcAnnotation = "HasKind";
    private const int StringIdPropertyLengthConstants = 64;

    /// <summary>
    ///     Sets the <see cref="DateTimeKind" /> for this property.
    /// </summary>
    /// <param name="builder">The property builder.</param>
    /// <param name="kind">The kind of this <see cref="DateTime" />.</param>
    /// <typeparam name="TProperty">The type of property.</typeparam>
    /// <returns>The property builder passed to <paramref name="builder" />.</returns>
    public static PropertyBuilder<TProperty> HasKind<TProperty>(this PropertyBuilder<TProperty> builder, DateTimeKind kind)
        where TProperty : IEquatable<DateTime>
    {
        return builder.HasAnnotation(IsUtcAnnotation, kind);
    }

    /// <summary>
    ///     Specifies a default <see cref="DateTimeKind" /> for all <see cref="DateTime" /> properties.
    ///     Make sure this is called after configuring all your entities.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    /// <param name="dateTimeKind">The default kind for all <see cref="DateTime" /> in the database.</param>
    public static void SetDefaultDateTimeKind(this ModelBuilder builder, DateTimeKind dateTimeKind)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        foreach (var property in entityType.GetProperties())
            if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
            {
                var kind = (DateTimeKind?)property.FindAnnotation(IsUtcAnnotation)?.Value ?? dateTimeKind;
                property.SetValueConverter(new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, kind)));
            }
    }

    /// <summary>
    ///     Ignores all composite primary keys implementations from model builder. This prevents these models from being treated as entities.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public static void IgnoreCompositePrimaryKeys(this ModelBuilder modelBuilder)
    {
        var compositeKeyBaseInterface = typeof(IEntityCompositePrimaryKey<>);

        var typesToIgnore = modelBuilder.Model.GetEntityTypes()
            .Select(t => t.ClrType)
            .Where(t => compositeKeyBaseInterface.IsAssignableFromGenericType(t))
            .ToList();

        foreach (var type in typesToIgnore)
            modelBuilder.Ignore(type);
    }

    /// <summary>
    ///     Applies default value to current date and time of the database server to all entities that implement
    ///     <see cref="IAuditableCreatedAt" /> or <see cref="IAuditableModifiedAt" />.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="defaultDateSql"></param>
    public static void DefaultValueForAutoAuditDates(this ModelBuilder modelBuilder, string defaultDateSql)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            if (typeof(IAuditableCreatedAt).IsAssignableFrom(entityType.ClrType))
            {
                var property = entityType.FindProperty(nameof(IAuditableCreatedAt.CreatedAtUtc));
                property?.SetDefaultValueSql(defaultDateSql);
            }
    }

    public static void NullableValueForAutoAuditUserIds(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IAuditableModifiedByVal<>).IsAssignableFromGenericType(entityType.ClrType))
            {
                var property = entityType.FindProperty(nameof(IAuditableModifiedByVal<int>.ModifiedByUserId));
                if (property != null) property.IsNullable = true;
            }

            if (typeof(IAuditableModifiedByRef<>).IsAssignableFromGenericType(entityType.ClrType))
            {
                var property = entityType.FindProperty(nameof(IAuditableModifiedByRef<object>.ModifiedByUserId));
                if (property != null) property.IsNullable = true;
            }

            if (typeof(IAuditableDeletedBy<>).IsAssignableFromGenericType(entityType.ClrType))
            {
                var property = entityType.FindProperty(nameof(IAuditableDeletedBy<object>.DeletedByUserId));
                if (property != null) property.IsNullable = true;
            }
        }
    }

    public static void DefaultValueForAutoAuditUserIds(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IAuditableCreatedBy<>).IsAssignableFromGenericType(entityType.ClrType))
                entityType.SetPropertyMaxLengthIfNotSet(nameof(IAuditableCreatedBy<object>.CreatedByUserId),
                    StringIdPropertyLengthConstants);

            if (typeof(IAuditableModifiedByVal<>).IsAssignableFromGenericType(entityType.ClrType))
                entityType.SetPropertyMaxLengthIfNotSet(nameof(IAuditableModifiedByVal<int>.ModifiedByUserId),
                    StringIdPropertyLengthConstants);

            if (typeof(IAuditableModifiedByRef<>).IsAssignableFromGenericType(entityType.ClrType))
                entityType.SetPropertyMaxLengthIfNotSet(nameof(IAuditableModifiedByRef<object>.ModifiedByUserId),
                    StringIdPropertyLengthConstants);

            if (typeof(IAuditableDeletedBy<>).IsAssignableFromGenericType(entityType.ClrType))
                entityType.SetPropertyMaxLengthIfNotSet(nameof(IAuditableDeletedBy<object>.DeletedByUserId),
                    StringIdPropertyLengthConstants);
        }
    }

    private static void SetPropertyMaxLengthIfNotSet(this IMutableTypeBase entityType, string propertyName, int maxLength)
    {
        var createdByUserId = entityType.FindProperty(propertyName);
        if (createdByUserId?.GetMaxLength() == null) createdByUserId?.SetMaxLength(maxLength);
    }
}