namespace TestWeb.Services.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using TestWeb.Data;
    using TestWeb.Data.Models;
    using TestWeb.Web.ViewModels;
    using TestWeb.Web.ViewModels.Users;

    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext dbcontext;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public UsersService(ApplicationDbContext dbContext,
            UserManager<ApplicationUser> _userManager,
            IUserStore<ApplicationUser> _userStore)
        {
            this.dbcontext = dbContext;
            this._userManager = _userManager;
            this._userStore = _userStore;
        }

        public async Task CreateAsync(CreateUserInputModel inputModel)
        {
            var user = Activator.CreateInstance<ApplicationUser>();

            await _userStore.SetUserNameAsync(user, inputModel.UserName, CancellationToken.None);
            await _userManager.SetEmailAsync(user, inputModel.Email);

            user.Id = Guid.NewGuid().ToString();
            user.UserName = inputModel.UserName;
            user.Email = inputModel.Email;

            var result = await _userManager.CreateAsync(user, inputModel.Password);

            if (inputModel.Role != "Administrator" && inputModel.Role != "Tech" && inputModel.Role != "Customer")
            {
                throw new InvalidOperationException("Invalid role!");
            }

            await this._userManager.AddToRoleAsync(user, inputModel.Role);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("User creation failed!");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var user = await this._userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException("This user doesn't exist!");
            }

            var userRole = await this.dbcontext.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);

            this.dbcontext.UserRoles.Remove(userRole);

            await this.dbcontext.SaveChangesAsync();

            await this._userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<AllUsersViewModel>> GetAll()
        {
            return await this.dbcontext.Users.OrderBy(x => x.UserName).Select(x => new AllUsersViewModel
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                Role = this.dbcontext.Roles.FirstOrDefault(r => r.Id == x.Roles.Single().RoleId).Name,
            }).ToListAsync();
        }

        public async Task<UpdateUserInputModel> GetByIdForUpdate(string id)
        {
            var user = await this._userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException("This user doesn't exist!");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            return new UpdateUserInputModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = userRoles.FirstOrDefault(),
            };
        }

        public async Task UpdateAsync(UpdateUserInputModel inputModel)
        {
            var user = await this._userManager.FindByIdAsync(inputModel.Id);

            if (user == null)
            {
                throw new InvalidOperationException("This user doesn't exist!");
            }

            user.UserName = inputModel.UserName;
            user.Email = inputModel.Email;

            if (!(inputModel.Role == "Administrator" || inputModel.Role == "Tech" || inputModel.Role == "Customer"))
            {
                throw new InvalidOperationException("Invalid role!");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var userRole = userRoles.FirstOrDefault();

            await this._userManager.RemoveFromRoleAsync(user, userRole);
            await this._userManager.AddToRoleAsync(user, inputModel.Role);

            await this._userManager.ChangePasswordAsync(user, inputModel.CurrentPassword, inputModel.NewPassword);
        }
    }
}
