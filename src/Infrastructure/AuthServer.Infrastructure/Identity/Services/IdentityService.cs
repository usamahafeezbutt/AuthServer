using AuthServer.Application.Common.Interfaces.Identity;
using AuthServer.Application.Services.Account.Dto;
using AuthServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthServer.Application.Common.Extensions;
using AuthServer.Application.Common.Models.Responses;
using AutoMapper;
namespace AuthServer.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        => await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        public async Task<IdentityResult> AssignRoleAsync(ApplicationUser user, string role)
        => await _userManager.AddToRoleAsync(user, role);

        public async Task<IdentityResult> RemoveRoleAsync(ApplicationUser user, string role)
        => await _userManager.RemoveFromRoleAsync(user, role);

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        => await _userManager.CheckPasswordAsync(user, password);

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser applicationUser, string password)
        => await _userManager.CreateAsync(applicationUser, password);

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser applicationUser)
        => await _userManager.CreateAsync(applicationUser);

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        => await _userManager.FindByEmailAsync(email) ?? null!;


        public async Task<ApplicationUser> FindByIdAsync(string id)
        => await _userManager.FindByIdAsync(id) ?? null!;

        public async Task<List<string>> GetUserRoles(ApplicationUser ApplicationUser)
        =>  (await _userManager.GetRolesAsync(ApplicationUser)).ToList();

        public async Task<PagedListResponse<List<AccountDetailDto>>> GetAllUsersAsync(GetPagedAccountDetailDto getPagedAccountDetailDto)
        {
            var users = await _userManager
                            .Users
                            .PageBy(getPagedAccountDetailDto.PageNumber, getPagedAccountDetailDto.PageSize)
                            .ToListAsync();

            var accounts = _mapper.Map<List<AccountDetailDto>>(users);

            foreach (var user in users)
            {
                var account = accounts.FirstOrDefault(x => x.Id == user.Id);
                account!.Role = (await _userManager.GetRolesAsync(user))?.FirstOrDefault()!;
            }

            return new PagedListResponse<List<AccountDetailDto>>(true, "Users retrieved successfully", accounts, await _userManager.Users.CountAsync());
        }

        public async Task<ApplicationUser> FindByUserNameAsync(string userName)
        => await _userManager.FindByNameAsync(userName) ?? null!;

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser ApplicationUser)
        => await _userManager.UpdateAsync(ApplicationUser) ?? null!;

        public async Task<IdentityResult> AddNewPassword(
            ApplicationUser applicationUser,
            string password)
        {
            var removePasswordResult = await _userManager.RemovePasswordAsync(applicationUser);

            if (!removePasswordResult.Succeeded)
                return removePasswordResult;

            var addPassword = await _userManager.AddPasswordAsync(applicationUser, password);

            return addPassword;
        }
    }
}
