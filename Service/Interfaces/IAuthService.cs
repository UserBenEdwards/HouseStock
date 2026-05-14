namespace Service.Interfaces;

public interface IAuthService
{
    bool Authenticate(string password);
}
