// Author: Chris Morgan
// The purpose of the EmployeeDetailsViewModel is to hold the required properties that will be passed down to the Employee Details view. The view needs to contain the department and the computer that belongs to the employee as well as any training programs they have attended or will attend.

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }

        public Department Department { get; set; }

        public Computer Computer { get; set; }

        public List<SelectListItem> TrainingPrograms { get; set; }

    }
}
