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
        [ActionName("Details")]
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
        // The AssignTrainingProgramForm is responsible for gathering the data needed for rendering the page that allows the user to assign an employee to a training program. It accepts one parameter, the employee id. This method creates an instance of the AssignTrainingProgramViewModel which contains the properties that are needed for the form.
        // GET: Employees/AssignTrainingProgram
        [HttpGet("/Employees/AssignTrainingProgram/{id?}"), ActionName("AssignTrainingProgramForm")]
        public ActionResult AssignTrainingProgramForm(int id)
        {

            AssignTrainingProgramViewModel viewModel = new AssignTrainingProgramViewModel(id, _config.GetConnectionString("DefaultConnection"));

            viewModel.Employee = GetEmployeeById(id);

            return View(viewModel);
        }

        // The AssignTrainingProgram is responsible for updating the EmployeeTraining table in the database which represents assigning an employee to a training program. It accepts two parameters: The employeeId from the route [/Employees/AssignTrainingProgram/{id}], and the SelectedValue which represents the training program Id that is attatched to the training program that the user is assigning the employee to.
        public ActionResult AssignTrainingProgram([FromRoute]int id, [FromForm] int SelectedValue)
        {
            // If the user doesn't select an option, the program won't break and the user will still be returned to details
            if(SelectedValue != 0)
            {

                using(SqlConnection conn = Connection)
                {
                    conn.Open();

                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (@EmployeeId, @TrainingProgramId)";
                        cmd.Parameters.Add(new SqlParameter("@EmployeeId", id));
                        cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", SelectedValue));

                        cmd.ExecuteNonQuery();

                    }
                }
            }
            
            return RedirectToAction("Details", "Employees", new { id = id });
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel(id, _config.GetConnectionString("DefaultConnection"));

           Employee employee = GetEmployeeById(id);

            employeeEditViewModel.Employee = employee;

            return View(employeeEditViewModel);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel viewmodel)
        {
           
                string sql = @"UPDATE Employee 
                               SET FirstName = @FirstName, 
                                   LastName = @LastName, 
                                   DepartmentId = @DepartmentId
                               WHERE Id = @Id;";
                if (viewmodel.ComputerId != 0 && viewmodel.ComputerId != null)
                {
                    sql = $@"{sql}
                            UPDATE ComputerEmployee
                            SET UnassignDate = @AssignDate
                            WHERE EmployeeId = @Id AND UnassignDate IS NULL
                            INSERT INTO ComputerEmployee
                            (ComputerId, EmployeeId, AssignDate)
                            VALUES
                            (@ComputerId, @Id, @AssignDate)";
                }


                // TODO: Add update logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;

                        cmd.Parameters.Add(new SqlParameter("@FirstName", viewmodel.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", viewmodel.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@DepartmentId", viewmodel.Employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        if (viewmodel.ComputerId != 0 && viewmodel.ComputerId != null)
                        {
                            cmd.Parameters.Add(new SqlParameter("@ComputerId", viewmodel.ComputerId));
                            cmd.Parameters.Add(new SqlParameter("@AssignDate", DateTime.Now));

                        }

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));


                    }
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