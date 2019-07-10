// Author: Brian Jobe, Chris Morgan, Josh Hibray

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
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
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
        // GET: Employees
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT e.Id,
                e.FirstName,
                e.LastName,
                de.Name,
                e.IsSuperVisor
                FROM Employee e
             JOIN Department de on e.DepartmentId = de.Id
            ";


                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentName = reader.GetString(reader.GetOrdinal("Name")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        };

                        employees.Add(employee);
                    }

                    reader.Close();

                    return View(employees);
                }
            }
        }

        // The Details method of the EmployeeController is used to GET all of the necessary data from the database and pass it down to the Employee Details view, which is the Employee/Details.cshtml file. The method accepts one parameter: The employee id, which is used within the EmployeeDetailsViewModel constructor method and the GetEmployeeById method. First, we create a new instance of the view model and pass in the employee id and the connection string. Then, we set the Employee property of the view model by using GetEmployeeId. We return the newly created view model to the View().

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {

                EmployeeDetailsViewModel viewModel = new EmployeeDetailsViewModel(id, _config.GetConnectionString("DefaultConnection"));

                Employee employee = GetEmployeeById(id);

                viewModel.Employee = employee;

                return View(viewModel);

        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            EmployeeCreateViewModel employeeCreateViewModel = new EmployeeCreateViewModel(_config.GetConnectionString("DefaultConnection"));

            return View(employeeCreateViewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmployeeCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee
                                               (FirstName, LastName, IsSuperVisor, DepartmentId)
                                                VALUES
                                                (@firstName, @lastName, @isSuperVisor, @departmentId)";
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", model.Employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", model.Employee.DepartmentId));
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

        // GET: Employees/AssignTrainingProgram
        [HttpGet("/Employees/AssignTrainingProgram/{id?}"), ActionName("AssignTrainingProgramForm")]
        public ActionResult AssignTrainingProgramForm(int id)
        {

            AssignTrainingProgramViewModel viewModel = new AssignTrainingProgramViewModel(id, _config.GetConnectionString("DefaultConnection"));

            viewModel.Employee = GetEmployeeById(id);

            return View(viewModel);
        }

        public ActionResult AssignTrainingProgram(int id, [FromForm] int SelectedValue)
        {
            //AssignTrainingProgramViewModel viewModel = new AssignTrainingProgramViewModel(id, _config.GetConnectionString("DefaultConnection"));

            using(SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (@EmployeeId, @TrainingProgramId)";
                    cmd.Parameters.Add(new SqlParameter("@EmployeeId", id));
                    cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", SelectedValue));

                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employees/Edit/5
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

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
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

        // The private GetEmployeeById method is used to get an employee instance from the database by the employee id. It accepts one parameter: the employee id. It returns an Employee type object. This method will be used throughout the controller.
        private Employee GetEmployeeById(int id)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                            Id,
                                            FirstName,
                                            LastName,
                                            DepartmentId,
                                            IsSuperVisor
                                        FROM Employee
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    Employee employee = null;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if(reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        };
                    }

                    reader.Close();
                    return employee;
                }
            }
        }
    }
}