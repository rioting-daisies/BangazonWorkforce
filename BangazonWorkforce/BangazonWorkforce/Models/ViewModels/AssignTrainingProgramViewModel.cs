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

        // Employee property represents the employee that the user wants to assign a training program to.
        public Employee Employee { get; set; }

        // The selected value property represents the training program id of the TP the user is assiging to the employee
        public int SelectedValue { get; set; }

        // The TrainingProgramsSelectItems represents the upcome training programs that the user has not already been assigned to. It is used to populate the dropdown
        public List<SelectListItem> TrainingProgramsSelectItems { get; set; }

        // Constructor method for the ViewModel. This method accepts two parameters: The employee id and the connection string to establish a connection to the DB.
        public AssignTrainingProgramViewModel(int id, string connectionString)
        {
            _connectionString = connectionString;

            // Set the TPSelectListItems to the result of GetAvailableTPs(by Emp id) and convert the List<TP> to a List<SelectListItem>

            TrainingProgramsSelectItems = GetAvailableTrainingPrograms(id)
                                                .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
                                                .ToList();

            // label the dropdown with the first option
            TrainingProgramsSelectItems.Insert(0, new SelectListItem("Choose a Training Program", "0"));

        }

        // The GetAvailableTrainingPrograms method is used to gather all the upcoming training programs that the employee is not currently assigned to and training programs that are full are filtered out. It accepts one parameter: the Employee Id. Returns a List<TP>
        private List<TrainingProgram> GetAvailableTrainingPrograms(int id)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT t.Id, t.Name, COUNT(et.EmployeeId)
                                            FROM TrainingProgram t
                                             LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = t.Id
                                            WHERE StartDate > GETDATE()
                                            GROUP BY t.Id, t.Name, t.MaxAttendees
                                            HAVING COUNT(et.EmployeeId) < t.MaxAttendees
                                            EXCEPT
                                            SELECT t.Id,
                                            t.Name,
                                            t.MaxAttendees
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
