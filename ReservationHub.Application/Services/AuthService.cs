using ReservationHub.Application.Interfaces;
using ReservationHub.Domain.Entities;
using ReservationHub.Domain.Interfaces;

namespace ReservationHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;

    public AuthService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> RegisterAsync(
        string firstName, string lastName, string email, string password)
    {
        // Email daha önce kullanılmış mı?
        var existing = await _userRepository.FindAsync(u => u.Email == email);
        if (existing.Any())
            throw new Exception("Bu email adresi zaten kullanımda.");

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            // Şifreyi düz metin saklamıyoruz — hash'liyoruz
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Customer"
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return "Kayıt başarılı.";
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email);
        var user = users.FirstOrDefault()
            ?? throw new Exception("Email veya şifre hatalı.");

        // Hash'lenmiş şifreyle karşılaştır
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new Exception("Email veya şifre hatalı.");

        // Token üretimi controller'da yapılacak (JwtService inject edilerek)
        // Burada userId döndürüyoruz, controller token üretecek
        return user.Id.ToString();
    }
}