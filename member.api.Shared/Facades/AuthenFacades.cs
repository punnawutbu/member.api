using System.Threading.Tasks;
using member.api.Shared.Constant;
using member.api.Shared.Models;
using member.api.Shared.Repositories;
using member.api.Shared.ResponseMessage;
using member.api.Shared.Services;

namespace member.api.Shared.Facades
{
    public class AuthenFacades : IAuthenFacades
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ISecurityService _securityService;
        private readonly IJwtService _jwtService;
        public AuthenFacades(IMemberRepository memberRepository, ISecurityService securityService, IJwtService jwtService)
        {
            _memberRepository = memberRepository;
            _securityService = securityService;
            _jwtService = jwtService;
        }
        public async Task<ResponseMessage<RegisterActivate>> Register(Register dto)
        {
            var resp = new ResponseMessage<RegisterActivate>
            {
                Result = null,
                Message = Message.Fail
            };

            var user = await _memberRepository.FindUser(dto.UserName);
            if (user == null)
            {
                dto.Password = _securityService.BcryptPassword(dto.Password);
                resp.Result = await _memberRepository.CreateUser(dto);
                resp.Message = Message.Success;
                return resp;
            }
            return resp;
        }
        public async Task<LoginResponse> Login(LoginRequest req, string systemName, string role = "user")
        {
            var resp = new LoginResponse();

            var hashedPasswordInDb = await _memberRepository.GetPassword(req.Username);

            if (hashedPasswordInDb == null)
            {
                resp.Message = LoginMessage.UserOrPasswordIsIncorrect;
                return resp;
            }

            var isPasswordValid = _securityService.PasswordVerify(req.Password, hashedPasswordInDb);

            if (!isPasswordValid)
            {
                resp.Message = LoginMessage.UserOrPasswordIsIncorrect;
                return resp;
            }

            // var menuConfig = await _systemConfigRepo.GetMenuConfigByRoleIdAndBranchCode(profile.RoleId, dataSource.BranchCode);


            resp.Token = _jwtService.GenerateLoginToken(systemName, req.Username, systemName);
            resp.Message = LoginMessage.LoginSuccess;

            return resp;
        }
    }
}