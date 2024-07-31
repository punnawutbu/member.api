using System.Data;

namespace member.api.Shared.Repositories
{
    public interface IBaseRepository
    {
        IDbConnection GetDbConnection();
    }
}