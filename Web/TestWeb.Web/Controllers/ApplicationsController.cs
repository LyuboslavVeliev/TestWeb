using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestWeb.Services.Data;
using TestWeb.Web.ViewModels.Applications;

namespace TestWeb.Web.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly IApplicationsService applicationsService;

        public ApplicationsController(IApplicationsService applicationsService)
        {
            this.applicationsService = applicationsService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateApplicationInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View();
            }

            await this.applicationsService.CreateAsync(inputModel);

            return this.Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<GetAllViewModel> allUsers = await this.applicationsService.GetAll();

            return this.View(allUsers);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            UpdateViewModel inputModel = await this.applicationsService.GetByIdForUpdate(id);

            return this.View(inputModel);
        }

        public async Task<IActionResult> Update(UpdateViewModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View();
            }

            await this.applicationsService.UpdateAsync(inputModel);

            return this.Redirect("/Applications/GetAll");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await this.applicationsService.DeleteAsync(id);

            return this.Redirect("/Applications/GetAll");
        }
    }
}
