using System.Threading.Tasks;
using member.api.Shared.Models;

namespace member.api.Shared.Repositories
{
    public interface IMemberRepository
    {
        Task<string> GetPassword(string userName);
        Task<RegisterActivate> CreateUser(Register dto);
        Task<User> FindUser(string userName);
    }
}