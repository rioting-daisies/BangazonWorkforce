// Author: Brian Jobe
// The purpose of the EmployeeCreateViewModel is to hold the required properties that will be passed down to the Create new employee view. The view needs to contain the list of departments.


using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeCreateViewModel
    {

        // List of Departments as Select List Items that the employee can be assigned to, populates the drop down on the create employee form
        public List<SelectListItem> Departments { get; set; }

        // Employee property used to display the employee properties on the form
        public Employee Employee { get; set; }

        // Connection methods / used to establish connection to BangazonAPI
        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        // Overloaded method accepting no parameters used in the POST. It grabs all the information off the form to build this employee
        public EmployeeCreateViewModel() { }

        // Overloaded method accepting one parameter: the connection string. This constructor method sets the Departments property by calling GetAllDepartments and converting the List of Depts => List of Select list items
        public EmployeeCreateViewModel(string connectionString)
        {
            _connectionString = connectionString;

            Departments = GetAllDepartments()
                .Select(d => new SelectListItem
                {
                    Text = d.Name,
                    Value = d.Id.ToString()
                })
                .ToList();

            Departments
                .Insert(0, new SelectListItem
                {
                    Text = "Choose department...",
                    Value = "0"
                });

        }

        // Gets all departments in the database
        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }

                    reader.Close();

                    return departments;
                }
            }
        }
    }
}
