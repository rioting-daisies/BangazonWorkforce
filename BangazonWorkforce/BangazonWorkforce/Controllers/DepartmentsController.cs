﻿//Author Clifton Matuszewski

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace BangazonWorkforce.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // The GET Method for getting all of the departments is used to display all of the departments Names budgets as well as a count of the total employees in the department
        // GET: Department
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.Name AS DepartmentName, d.Budget AS Budget,
                                        COUNT(e.Id) AS EmployeeId
                                        FROM Department d 
                                        LEFT JOIN Employee e on d.Id = e.DepartmentId
                                        GROUP BY d.Name, d.Budget, d.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal(("Id"))),
                            Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeId"))
                        };
                        departments.Add(department);
                    }
                    reader.Close();

                    return View(departments);
                }
            }

        }
        //
        // GET: Department/Details/5
        public ActionResult Details(int id)
           {
            DepartmentEmployeesViewModel viewModel = new DepartmentEmployeesViewModel(id, _config.GetConnectionString("DefaultConnection"));

            Department department = GetDepartmentById(id);

            viewModel.department = department;

            return View(viewModel);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Department department)
        {
            try
            {
                // TODO: Add insert logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Department (Name, Budget) VALUES (@Name, @Budget)";
                        cmd.Parameters.Add(new SqlParameter("@Name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@Budget", department.Budget));
                        await cmd.ExecuteNonQueryAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }

                
            }
            catch
            {
                return View();
            }
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Departments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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

    }
}