namespace AuthService.Repositories;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}