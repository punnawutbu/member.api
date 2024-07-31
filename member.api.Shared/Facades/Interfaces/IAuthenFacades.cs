using System.Threading.Tasks;
using member.api.Shared.Models;
using member.api.Shared.ResponseMessage;

namespace member.api.Shared.Facades
{
    public interface IAuthenFacades
    {
        Task<LoginResponse> Login(LoginRequest req, string systemName, string role = "user");
        Task<ResponseMessage<RegisterActivate>> Register(Register dto);
    }
}