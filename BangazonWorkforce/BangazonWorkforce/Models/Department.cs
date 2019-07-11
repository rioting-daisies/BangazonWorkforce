//Author Clifton Matuszewski
//Department class to hold its properties and also determine the fields that are required for departments to be created﻿

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
   public class Department
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "You must provide a name for this department.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must provide a budget for this department.")]
        [Range(0, int.MaxValue, ErrorMessage = "A budget cannot be less than zero.")]

        public int Budget { get; set; }

        
        public int EmployeeCount { get; set; }

    }
}
