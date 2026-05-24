namespace ReservationHub.Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(string firstName, string lastName, string email, string password);
    Task<string> LoginAsync(string email, string password);
}