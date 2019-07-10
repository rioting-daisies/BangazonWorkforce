﻿// Author: Chris Morgan
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

        }

        public void AssignExercise()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (@EmployeeId, @TrainingProgramId)";
                    cmd.Parameters.Add(new SqlParameter("@EmployeeId", Employee.Id));
                    cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", SelectedValue));

                    cmd.ExecuteNonQuery();

                    
                    
                }
            }
        }

        private List<TrainingProgram> GetAvailableTrainingPrograms(int id)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                            t.Id,
                                            t.Name,
                                            t.StartDate,
                                            t.EndDate,
                                            t.MaxAttendees
                                        FROM TrainingProgram t
                                        JOIN EmployeeTraining et ON et.TrainingProgramId = t.Id
                                        JOIN Employee e ON e.Id = et.EmployeeId
                                        WHERE t.StartDate > GetDate()
                                        AND et.EmployeeId != @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
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