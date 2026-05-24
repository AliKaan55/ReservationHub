using Microsoft.AspNetCore.Mvc;
using ReservationHub.Application.DTOs;
using ReservationHub.Application.Interfaces;
using ReservationHub.API.Middleware;
using ReservationHub.Domain.Interfaces;
using ReservationHub.Domain.Entities;

namespace ReservationHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtService _jwtService;
    private readonly IRepository<User> _userRepository;

    public AuthController(
        IAuthService authService,
        JwtService jwtService,
        IRepository<User> userRepository)
    {
        _authService = authService;
        _jwtService = jwtService;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            await _authService.RegisterAsync(
                dto.FirstName, dto.LastName, dto.Email, dto.Password);
            return Ok(new { message = "Kayıt başarılı." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var userIdStr = await _authService.LoginAsync(dto.Email, dto.Password);
            var userId = int.Parse(userIdStr);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null) return Unauthorized();

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}