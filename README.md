# ReservationHub API

ASP.NET Core 9 ile geliştirilmiş otel rezervasyon yönetim sistemi.

## Özellikler
- Clean Architecture (Domain, Application, Infrastructure, API)
- JWT tabanlı kimlik doğrulama
- Entity Framework Core + MSSQL (Code-First)
- Redis Cache ile sorgu optimizasyonu
- Repository Pattern + Generic Repository
- BCrypt ile şifre güvenliği
- Swagger / OpenAPI dokümantasyonu

## Teknolojiler
- .NET 9 / ASP.NET Core
- Entity Framework Core 9
- MSSQL (SQL Server)
- Redis (StackExchange.Redis)
- JWT Bearer Authentication
- FluentValidation
- BCrypt.Net
- Swagger (Swashbuckle)

## Kurulum

1. Repoyu klonla
git clone https://github.com/kullaniciadin/ReservationHub.git

2. appsettings.json içindeki bağlantı bilgilerini düzenle

3. Migration uygula
Update-Database

4. Projeyi çalıştır
dotnet run --project ReservationHub.API

5. Swagger: https://localhost:{port}/swagger
