using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWeb.Data.Models;

namespace TestWeb.Web.ViewModels.Applications
{
    public class UpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        public string Status { get; set; }

        public string Picture { get; set; }

        public DateTime? VisitFromTech { get; set; }

        public string TechnicianId { get; set; }

        public ApplicationUser Technician { get; set; }
    }
}
