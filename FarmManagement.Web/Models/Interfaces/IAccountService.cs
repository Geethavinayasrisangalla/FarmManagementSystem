using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Interfaces;

public interface IAccountService
{
    Task<AppUser?> AuthenticateAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterViewModel vm);
    Task<bool> EmailExistsAsync(string email);
}
