using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWeb.Web.ViewModels.Applications;

namespace TestWeb.Services.Data
{
    public interface IApplicationsService
    {
        Task CreateAsync(CreateApplicationInputModel inputModel);

        Task<IEnumerable<GetAllViewModel>> GetAll();

        Task UpdateAsync(UpdateViewModel inputModel);

        Task DeleteAsync(int id);

        Task<UpdateViewModel> GetByIdForUpdate(int id);
    }
}
