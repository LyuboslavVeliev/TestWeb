namespace TestWeb.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TestWeb.Web.ViewModels;
    using TestWeb.Web.ViewModels.Users;

    public interface IUsersService
    {
        Task CreateAsync(CreateUserInputModel inputModel);

        Task UpdateAsync(UpdateUserInputModel inputModel);

        Task DeleteAsync(string id);

        Task<IEnumerable<AllUsersViewModel>> GetAll();

        Task<UpdateUserInputModel> GetByIdForUpdate(string id);
    }
}
