// Author: Chris Morgan
// The purpose of the TrainingProgramDetailsViewModel is to hold the required properties that will be passed down to the Training Program Details view. The view needs to contain the training program selectd and all the employees currently attending the program.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class TrainingProgramDetailsViewModel
    {
        //the connection string property will be used in the TrainingProgramDetailsViewModel constructor method as a parameter. This will allow us to keep the default connectionStrings reference within the TrainingProgram controller.
        private string _connectionString;

        //When the constructor is used, the connection string argument will be set as the _connectionString property. Then the Connection property is obtained by creating a new SqlConnection with the _connectionString property. This establishes a connection to the BangazonAPI database.
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        // The Employees property represents a List of employees who are currently signed up for the training program.
        public List<Employee> Employees { get; set; }

        // Training program property in order to access training program properties for the view
        public TrainingProgram TrainingProgram { get; set; }

        // Constructor method that assigns the Employees property of the viewModel and sets the _connectionString property with the connectionString parameter. The first parameter is the TrainingProgramId, which is used to call the GetEmployeesInProgram method to set the Employees property.
        public TrainingProgramDetailsViewModel(int id, string connectionString)
        {
            _connectionString = connectionString;

            Employees = GetEmployeesInProgram(id);

        }
        // The GetEmployeesInProgram private method is used to grab all the employees that are in the training program and return a List<Employee>. It accepts one parameter: the TrainingProgramId
        private List<Employee> GetEmployeesInProgram(int id)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                            e.Id,
                                            e.FirstName,
                                            e.LastName,
                                            e.DepartmentId,
                                            e.IsSuperVisor
                                            FROM Employee e
                                            JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                            WHERE et.TrainingProgramId = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    List<Employee> employees = new List<Employee>();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        };

                        employees.Add(employee);
                    }

                    reader.Close();

                    return employees;

                }
            }
        }
    }
}
