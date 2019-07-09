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
        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }
        public Employee Employee { get; set; }

        public Department Department { get; set; }

        public Computer Computer { get; set; }

        public List<TrainingProgram> TrainingPrograms { get; set; }

        public EmployeeDetailsViewModel(int id, string connectionString)
        {
            _connectionString = connectionString;

            Department = GetEmployeeDepartment(id);

            Computer = GetEmployeeComputer(id);

            TrainingPrograms = GetEmployeeTrainingPrograms(id);
        }

        private Department GetEmployeeDepartment(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
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

        private Computer GetEmployeeComputer(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
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

        private List<TrainingProgram> GetEmployeeTrainingPrograms(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
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