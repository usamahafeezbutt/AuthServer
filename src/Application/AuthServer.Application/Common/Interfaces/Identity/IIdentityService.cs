using AuthServer.Application.Common.Models.Responses;
using AuthServer.Application.Services.Account.Dto;
using AuthServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Application.Common.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<IdentityResult> CreateUserAsync(ApplicationUser applicationUser, string password);

        Task<IdentityResult> AssignRoleAsync(ApplicationUser applicationUser, string role);

        Task<ApplicationUser> FindByEmailAsync(string email);

        Task<ApplicationUser> FindByIdAsync(string id);

        Task<PagedListResponse<List<AccountDetailDto>>> GetAllUsersAsync(GetPagedAccountDetailDto getPagedAccountDetailDto);

        Task<ApplicationUser> FindByUserNameAsync(string userName);

        Task<IdentityResult> UpdateUserAsync(ApplicationUser ApplicationUser);

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

        Task<IdentityResult> ChangePasswordAsync(
            ApplicationUser user,
            string currentPassword,
            string newPassword);
      
        Task<bool> DeleteUserAsync(string userId);

        Task<IdentityResult> AddNewPassword(
            ApplicationUser applicationUser,
            string password);

        Task<IdentityResult> CreateUserAsync(ApplicationUser applicationUser);

        Task<IdentityResult> RemoveRoleAsync(ApplicationUser user, string role);

        Task<List<string>> GetUserRoles(ApplicationUser ApplicationUser);
    }
}