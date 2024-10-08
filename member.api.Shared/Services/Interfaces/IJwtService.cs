using System.Security.Claims;
using member.api.Shared.Models;
using member.api.Shared.ResponseMessage;

namespace member.api.Shared.Services
{
    public interface IJwtService
    {
        ResponseMessage<JwtToken> CheckUserToken(string token);
        string GenToken(UserJWTPayload req);
        JwtToken CheckExpireToken(string token);
        bool CheckLicenseToken(string token);
        string GenerateToken(int lifeTime, string iss, Claim[] claims);
        string GenerateLoginToken(string iss, string userName, string systemName);
        string GenerateAccessToken(string userName, string systemName, string role, string permission, string page);
        string GenerateRefreshToken(string systemId, string userUuId);
        string GenerateResetToken(string iss, string userName, string systemName, string secretStr);
    }
}