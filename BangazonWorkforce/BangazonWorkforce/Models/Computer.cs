// Author: Chris Morgan
// The purpose of the Computer Model is to represent a Computer object from the database

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Computer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a purchase date for this computer.")]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Decommission Date", AutoGenerateField =false)]
        [DataType(DataType.Date)]
        public DateTime? DecomissionDate { get; set; }

        [Required(ErrorMessage = "You must provide the Make/Model of this computer.")]
        public string Make { get; set; }

        [Required(ErrorMessage = "You must provide the Manufacturer of this computer.")]
        public string Manufacturer { get; set; }

    }
}
