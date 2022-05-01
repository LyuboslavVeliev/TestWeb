namespace TestWeb.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TestWeb.Data.Models.Enums;

    public class Application
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Picture { get; set; }

        public Status Status { get; set; }

        public DateTime VisitFromTech { get; set; }

        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }

        public string TechnicianId { get; set; }

        public ApplicationUser Technician { get; set; }
    }
}
