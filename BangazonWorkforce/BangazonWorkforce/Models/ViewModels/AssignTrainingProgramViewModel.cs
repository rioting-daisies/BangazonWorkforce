// Author: Chris Morgan
// The purpose of the AssignTrainingViewModel is to hold a list of training programs that the employee can be assigned to.

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class AssignTrainingProgramViewModel
    {
        private string _connectionString;

        //When the constructor is used, the connection string argument will be set as the _connectionString property. Then the Connection property is obtained by creating a new SqlConnection with the _connectionString property. This establishes a connection to the BangazonAPI database.
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        List<SelectListItem> TrainingProgramsSelectItems { get; set; }

        public AssignTrainingProgramViewModel(int id, string connectionString)
        {
            _connectionString = connectionString;


        }
    }
}
