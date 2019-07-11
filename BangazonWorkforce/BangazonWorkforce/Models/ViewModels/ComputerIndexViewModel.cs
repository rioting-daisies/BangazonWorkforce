using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class ComputerIndexViewModel
    {


        public Computer Computer { get; set; }

        public Employee Employee { get; set; }

        public List<SelectListItem> Employees { get; set; }

        public int? EmployeeId { get; set; }


        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public ComputerIndexViewModel() { }

        public ComputerIndexViewModel(string connectionString)
        {
            _connectionString = connectionString;

            Employees = GetAllEmployees()
                .Select(d => new SelectListItem
                {
                    Text = d.GetEmployeeName,
                    Value = d.Id.ToString()
                })
                .ToList();

            Employees
                .Insert(0, new SelectListItem
                {
                    Text = "Choose employee...",
                    Value = "0"
                });

        }

        private List<Employee> GetAllEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, DepartmentId, IsSuperVisor FROM Employee";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }

    }
}


