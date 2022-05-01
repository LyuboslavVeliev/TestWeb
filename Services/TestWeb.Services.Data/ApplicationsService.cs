using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestWeb.Data;
using TestWeb.Data.Models;
using TestWeb.Data.Models.Enums;
using TestWeb.Web.ViewModels.Applications;

namespace TestWeb.Services.Data
{
    public class ApplicationsService : IApplicationsService
    {
        private readonly ApplicationDbContext dbContext;
        private ApplicationUser currentUser;

        public ApplicationsService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;

            this.currentUser = this.dbContext.Users.Where(user => user.Id == httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault();
        }

        public async Task CreateAsync(CreateApplicationInputModel inputModel)
        {
            var userRole = this.dbContext.UserRoles.FirstOrDefault(x => x.UserId == currentUser.Id);
            var role = this.dbContext.Roles.FirstOrDefault(x => x.Id == userRole.RoleId);

            var application = new Application();

            application.Name = inputModel.Name;
            application.Description = inputModel.Description;
            application.Address = inputModel.Address;
            application.Picture = String.IsNullOrEmpty(inputModel.Picture) ? String.Empty : inputModel.Picture;
            application.Status = Status.Waiting;
            application.CreatorId = this.currentUser.Id;
            application.Creator = this.currentUser;

            if (role.Name == "Administrator")
            {
                application.TechnicianId = inputModel.TechnicianId;
                application.Technician = inputModel.Technician;

                if (inputModel.VisitFromTech < DateTime.UtcNow)
                {
                    throw new InvalidOperationException("Invalid date!");
                }
                else
                {
                    application.VisitFromTech = inputModel.VisitFromTech ?? DateTime.UtcNow;
                }
            }

            await this.dbContext.Applications.AddAsync(application);
            await this.dbContext.SaveChangesAsync();

            this.currentUser.CreatedApplications.Add(application);
        }

        public async Task DeleteAsync(int id)
        {
            var userRole = this.dbContext.UserRoles.FirstOrDefault(x => x.UserId == this.currentUser.Id);
            var role = this.dbContext.Roles.FirstOrDefault(x => x.Id == userRole.RoleId);

            var application = this.dbContext.Applications.FirstOrDefault(x => x.Id == id);

            if (application == null)
            {
                throw new InvalidOperationException("Invalid application!");
            }

            if (role.Name == "Customer")
            {
                if (application.Status == Status.Waiting)
                {
                    this.dbContext.Applications.Remove(application);
                    await this.dbContext.SaveChangesAsync();
                }
            }
            else if (role.Name == "Administrator")
            {
                this.dbContext.Applications.Remove(application);
                await this.dbContext.SaveChangesAsync();
            }
            
        }

        public async Task<IEnumerable<GetAllViewModel>> GetAll()
        {
            var userRole = this.dbContext.UserRoles.FirstOrDefault(x => x.UserId == currentUser.Id);
            var role = this.dbContext.Roles.FirstOrDefault(x => x.Id == userRole.RoleId);

            if (role.Name == "Administrator")
            {
                IEnumerable<GetAllViewModel> all = this.dbContext.Applications
                .Select(x => new GetAllViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                }).ToList();

                return all;
            }
            else if (role.Name == "Administrator")
            {
                IEnumerable<GetAllViewModel> all = this.dbContext.Applications.Where(x => x.CreatorId == this.currentUser.Id)
                .Select(x => new GetAllViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                }).ToList();

                return all;
            }
            else
            {
                IEnumerable<GetAllViewModel> all = this.dbContext.Applications.Where(x => x.TechnicianId == this.currentUser.Id)
                .Select(x => new GetAllViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                }).ToList();

                return all;
            }
        }

        public async Task<UpdateViewModel> GetByIdForUpdate(int id)
        {
            var application = await this.dbContext.Applications.FirstOrDefaultAsync(x => x.Id == id);

            if (application == null)
            {
                throw new InvalidOperationException("Application not found!");
            }

            UpdateViewModel model = new UpdateViewModel
            {
                Id = id,
                Name = application.Name,
                Address = application.Address,
                Description = application.Description,
                Picture = application.Picture,
                Technician = application.Technician,
            };

            return model;
        }

        public async Task UpdateAsync(UpdateViewModel inputModel)
        {
            var userRole = this.dbContext.UserRoles.FirstOrDefault(x => x.UserId == this.currentUser.Id);
            var role = this.dbContext.Roles.FirstOrDefault(x => x.Id == userRole.RoleId);

            var application = this.dbContext.Applications.FirstOrDefault(x => x.Id == inputModel.Id);

            if (application == null)
            {
                throw new InvalidOperationException("Application not found!");
            }

            application.Name = inputModel.Name;
            application.Description = inputModel.Description;
            application.Address = inputModel.Address;
            application.Picture = String.IsNullOrEmpty(inputModel.Picture) ? String.Empty : inputModel.Picture;

            if (role.Name == "Tech")
            {
                application.Status = (Status)Enum.Parse(typeof(Status), inputModel.Status);
            }

            await this.dbContext.SaveChangesAsync();
        }
    }
}
