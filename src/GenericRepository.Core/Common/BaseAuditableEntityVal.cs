﻿using GenericRepository.Core.Common.Auditable.Create;
using GenericRepository.Core.Common.Auditable.Update;

namespace GenericRepository.Core.Common;

public abstract class BaseAuditableEntityVal<TPrimaryKey> : BaseAuditableEntityVal<TPrimaryKey, TPrimaryKey>
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>;

public abstract class BaseAuditableEntityVal<TPrimaryKey, TUserPrimaryKey> : BaseEntity<TPrimaryKey>,
    IAuditableCreated<TUserPrimaryKey>, IAuditableModifiedAt, IAuditableModifiedByVal<TUserPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
    where TUserPrimaryKey : struct
{
    public virtual DateTime CreatedAtUtc { get; set; }
    public virtual TUserPrimaryKey CreatedByUserId { get; set; } = default!;

    public virtual DateTime? ModifiedAtUtc { get; set; }
    public virtual TUserPrimaryKey? ModifiedByUserId { get; set; } = default!;
}