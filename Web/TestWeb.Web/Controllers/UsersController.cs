using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestWeb.Common;
using TestWeb.Services.Data;
using TestWeb.Web.ViewModels;
using TestWeb.Web.ViewModels.Users;

namespace TestWeb.Web.Controllers
{
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View();
            }

            await this.usersService.CreateAsync(inputModel);

            return this.Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<AllUsersViewModel> allUsers = await this.usersService.GetAll();

            return this.View(allUsers);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            UpdateUserInputModel inputModel = await this.usersService.GetByIdForUpdate(id);

            return this.View(inputModel);
        }

        public async Task<IActionResult> Update(UpdateUserInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View();
            }

            await this.usersService.UpdateAsync(inputModel);

            return this.Redirect("/Users/GetAll");
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.usersService.DeleteAsync(id);

            return this.Redirect("/Users/GetAll");
        }
    }
}
