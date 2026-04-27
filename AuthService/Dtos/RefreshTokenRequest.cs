namespace AuthService.Dtos;

public record RefreshTokenRequest(string AccessToken, string RefreshToken);
