// Author: Chris Morgan
// The purpose of the TrainingProgram Model is to represent a TrainingProgram object from the database

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You must provide a name for this training program.")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must provide a start date for this training program.")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must provide a end date for this training program.")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "You must provide the max attendee number for this training program.")]
        [Display(Name = "Max Attendees")]
        public int MaxAttendees { get; set; }
    }
}
