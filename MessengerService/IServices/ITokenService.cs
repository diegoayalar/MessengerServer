using MessengerDomain.Entities;

namespace MessengerService.IServices
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userEmail);
        RefreshToken GenerateRefreshToken();
    }
}
