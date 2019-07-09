using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class DepartmentEmployeesViewModel
    {
        public List<Employee> employees { get; set; }
        public Department department { get; set; }

        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public DepartmentEmployeesViewModel(int id, string connectionString)
        {
            _connectionString = connectionString;
            GetDepartmentById(id);
            GetDepartmentEmployees();

        }

        private Department GetDepartmentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Budget FROM Department WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    Department department = null;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        department = new Department()
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

        private void GetDepartmentEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, 
                                        e.FirstName, 
                                        e.LastName, d.Name 
                                        FROM Employee e JOIN Department d ON d.Id = e.DepartmentId
                                        ";
                 
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                        employees = new List<Employee>();
                    while (reader.Read())
                    {
                       Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            
                        };
                        employees.Add(employee);
                    }
                    reader.Close();
                }
            }
        }
    }
}
