namespace DDJ.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string token);
}
