using System.Linq.Expressions;
using GenericRepository.Core.Common;

namespace GenericRepository.Tests.Infrastructure.Entities;

public record PkTeamMember(int TeamId, int MemberId) : IEntityCompositePrimaryKey<TeamMember>
{
    public Expression<Func<TeamMember, bool>> GetFilterById()
    {
        return x => x.TeamId == TeamId && x.MemberId == MemberId;
    }
}