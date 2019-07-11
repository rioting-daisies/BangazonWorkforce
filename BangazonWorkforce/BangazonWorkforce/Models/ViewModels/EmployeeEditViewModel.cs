using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        //the connection string property will be used in the EmployeeEditViewModel constructor method as a parameter.This will allow us to keep the default connectionStrings reference within the Employee controller.

        private string _connectionString;

        // Employee property holds all the nested employee properties which is needed for the the details view for Employees.
        public Employee Employee { get; set; }

        // Department property holds all the nested department properties which is needed for the the details view for Employees. This will allow us to render all the information about the Department the employee works in.
        public List<SelectListItem> Departments { get; set; }

        // Computer property holds all the nested computer properties which is needed for the the details view for Employees.This is necessary for rendering all the information of the computer that is currently assigned to the employee.
        public List<SelectListItem> Computers { get; set; }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public EmployeeEditViewModel(int id, string connectionString)
        {
            _connectionString = connectionString;
            Departments = GetAllDepartments().Select(d => new SelectListItem(d.Name, d.Id.ToString())).ToList();
            Computers = GetAvailableComputers().Select(c => new SelectListItem(c.Make, c.Id.ToString())).ToList();
        }


        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = "SELECT Id, Name, Budget FROM Department";
                    SqlDataReader reader=cmd.ExecuteReader();

                    List<Department> depts = new List<Department>();
                    while (reader.Read())
                    {
                        depts.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

                        });
                    }

                    reader.Close();

                    return depts;

                    


                }
            }

        }

        private List<Computer> GetAvailableComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT c.Id, c.Make, c.Manufacturer, c.PurchaseDate, c.DecomissionDate
                                        FROM Computer c
                                        LEFT JOIN ComputerEmployee ce ON c.id = ce.ComputerId
                                        AND ce.UnassignDate IS NULL
                                        WHERE DecomissionDate IS NULL
                                        AND AssignDate IS NULL; ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> comps = new List<Computer>();
                    while (reader.Read())
                    {
                        comps.Add(new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))

                        });
                    }

                    reader.Close();

                    return comps;




                }
            }

        }

    }
}
