// Author: Chris Morgan
// The purpose of the AssignTrainingViewModel is to hold a list of training programs that the employee can be assigned to.

using Microsoft.AspNetCore.Mvc;
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

        public Employee Employee { get; set; }

        public int SelectedValue { get; set; }

        public List<SelectListItem> TrainingProgramsSelectItems { get; set; }

        public AssignTrainingProgramViewModel() { }
        public AssignTrainingProgramViewModel(int id, string connectionString)
        {
            _connectionString = connectionString;

            TrainingProgramsSelectItems = GetAvailableTrainingPrograms(id)
                                                .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
                                                .ToList();

            TrainingProgramsSelectItems.Insert(0, new SelectListItem("Choose a Training Program", "0"));

        }

        private List<TrainingProgram> GetAvailableTrainingPrograms(int id)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT Id, Name
                                            FROM TrainingProgram
                                            WHERE StartDate > GETDATE()
                                            EXCEPT
                                            SELECT t.Id,
                                            t.Name
                                            FROM TrainingProgram t
                                            LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = t.Id
                                            LEFT JOIN Employee e ON e.Id = et.EmployeeId
                                            WHERE t.StartDate > GETDATE()
                                            AND e.Id = @id";

                    

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        
                            TrainingProgram trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            trainingPrograms.Add(trainingProgram);

                    }

                    reader.Close();

                    return trainingPrograms;
                }
            }
        }
    }
}
