// Author: Chris Morgan
// The purpose of the EmployeeDetailsViewModel is to hold the required properties that will be passed down to the Employee Details view. The view needs to contain the department and the computer that belongs to the employee as well as any training programs they have attended or will attend.

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeDetailsViewModel
    {

        //the connection string property will be used in the EmployeeDetailsViewModel constructor method as a parameter. This will allow us to keep the default connectionStrings reference within the Employee controller.
        private string _connectionString;

        //When the constructor is used, the connection string argument will be set as the _connectionString property. Then the Connection property is obtained by creating a new SqlConnection with the _connectionString property. This establishes a connection to the BangazonAPI database.
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        // Employee property holds all the nested employee properties which is needed for the the details view for Employees.
        public Employee Employee { get; set; }

        // Department property holds all the nested department properties which is needed for the the details view for Employees. This will allow us to render all the information about the Department the employee works in.
        public Department Department { get; set; }

        // Computer property holds all the nested computer properties which is needed for the the details view for Employees.This is necessary for rendering all the information of the computer that is currently assigned to the employee.
        public Computer Computer { get; set; }

        // The TrainingPrograms property is necessary to display all the information about the training programs that the employee has attended or will attend.
        public List<TrainingProgram> TrainingPrograms { get; set; }

        // Constructor method for the view model. The constructor method accepts two arguments: the id of the Employee we are getting details, and the DefaultConnection string which establishes a connection to the database and allows us to get The Employee's Department, Computer, and Training Program information from the database
        public EmployeeDetailsViewModel(int id, string connectionString)
        {
            _connectionString = connectionString;

            Department = GetEmployeeDepartment(id);

            Computer = GetEmployeeComputer(id);

            TrainingPrograms = GetEmployeeTrainingPrograms(id);
        }

        // The GetEmployeeDepartment private method is used to get information about the department that the employee is currently assigned to. It accepts one parameter: the employee Id, which is used within our sql statement in order to select the correct department. It returns a Department type object which will be used to set the Department Property of the View Model.

        private Department GetEmployeeDepartment(int id)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                            d.Id,
                                            d.Name,
                                            d.Budget
                                        FROM Department d JOIN Employee e ON e.DepartmentId = d.Id
                                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    Department department = null;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        };
                    }

                    reader.Close();
                    return department;
                }
            }
        }

        // The GetEmployeeComputer private method is used to get information about the computer that the employee is currently using. It accepts one parameter: the employee Id, which is used within our sql statement in order to select the correct computer. It returns a Computer type object which will be used to set the Computer Property of the View Model.
        private Computer GetEmployeeComputer(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // UnassignDate is NULL because this will return the computer that is currently assigned to the employee
                    // Left Join brings back the null items for us
                    cmd.CommandText = @"SELECT 
                                            c.Id,
                                            c.Make,
                                            c.Manufacturer,
                                            c.PurchaseDate,
                                            c.DecomissionDate,
                                            ce.AssignDate,
                                            ce.UnassignDate
                                        FROM Computer c 
                                        LEFT JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
                                        LEFT JOIN Employee e ON e.Id = ce.EmployeeId
                                        WHERE e.Id = @id
                                        AND ce.UnassignDate IS NULL";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    Computer computer = null;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // DecomissionDate is null, therefore we don't need to display this information as we know that the computer is currently assigned.
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                            //DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))
                        };
                    }

                    reader.Close();
                    return computer;
                }
            }
        }

        // The private method GetEmployeeTrainingPrograms is meant to select all the of the training programs that an employee has attended or will attend. It makes a GET to the TrainingProgram table and joins the EmployeeTraining join table and joins  the Employee which allows us to select only the training programs that the employee has attended. It accepts one parameter: the employee Id, which is used in the SQL statement to filter out the correct training programs. It returns a list of training programs that the employee has attended and is used to set the TrainingPrograms property.

        private List<TrainingProgram> GetEmployeeTrainingPrograms(int id)
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
                                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
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
