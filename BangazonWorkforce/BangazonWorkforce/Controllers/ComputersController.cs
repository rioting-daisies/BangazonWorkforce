/*Author: Brian Jobe
* This controller allows the user to view all computers in the database, view details of each computer,
* create a new computer, and delete a computer if
it has not been used */

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
    public class ComputersController : Controller
    {

        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
        // GET: Computers
        public ActionResult Index(string searchString)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                         SELECT c.Id, c.Make, c.Manufacturer, c.PurchaseDate,
                                                ce.AssignDate, ce.UnassignDate, e.Id as EmployeeId,
                                                e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor
                                                FROM Computer c
                                                LEFT JOIN ComputerEmployee ce ON c.id = ce.ComputerId
                                                LEFT JOIN Employee e ON e.Id = ce.EmployeeId";
                    SqlDataReader reader = cmd.ExecuteReader();


                    Dictionary<int, ComputerIndexViewModel> viewModelHash = new Dictionary<int, ComputerIndexViewModel>();

                    while (reader.Read())
                    {
                        int computerId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!viewModelHash.ContainsKey(computerId))
                        {

                            viewModelHash[computerId] = new ComputerIndexViewModel()
                            {
                                Computer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                                }
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("AssignDate")) && reader.IsDBNull(reader.GetOrdinal("UnassignDate")))
                            {
                                viewModelHash[computerId].Employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                                };
                            }  
                        }
                    }
                    reader.Close();

                    List<ComputerIndexViewModel> viewModels = viewModelHash.Values.ToList();

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        viewModels = viewModels.Where(s => s.Computer.Make.Contains(searchString) || s.Computer.Manufacturer.Contains(searchString)).ToList();
                    }

                    return View(viewModels);
                }
            }
        }
        //This method pulls in the details for individual computers. It uses a join table with ComputerEmployee to check whether the computer has ever been assigned. 
        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                      SELECT c.Id, c.Make, c.Manufacturer, 
                                            c.PurchaseDate, c.DecomissionDate, 
                                            ce.ComputerId, ce.AssignDate
                                        FROM Computer c 
                                        LEFT JOIN ComputerEmployee ce on c.Id = ce.ComputerId 
                                        WHERE c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;
                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            NeverBeenAssigned = true
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            computer.NeverBeenAssigned = false;
                        }

                    }
                    reader.Close();
                    return View(computer);
                }
            }
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            ComputerIndexViewModel computerIndexViewModel = new ComputerIndexViewModel(_config.GetConnectionString("DefaultConnection"));

            return View(computerIndexViewModel);
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ComputerIndexViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Computer 
                                            (Make, Manufacturer, PurchaseDate) 
                                            VALUES 
                                            (@Make, @Manufacturer, @PurchaseDate)
                                            DECLARE @newId INT
                                            SELECT @newId = @@IDENTITY
                                            INSERT INTO ComputerEmployee
                                            (ComputerId, EmployeeId, AssignDate)
                                            VALUES
                                            (@newId, @EmployeeId, @AssignDate)";
                        cmd.Parameters.Add(new SqlParameter("@Make", viewModel.Computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@Manufacturer", viewModel.Computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", viewModel.Computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@EmployeeId", viewModel.EmployeeId));
                        cmd.Parameters.Add(new SqlParameter("@AssignDate", DateTime.Now));
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

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                Computer computer = GetComputerById(id);
                return View(computer);
            }
            catch
            {
                return NotFound();
            }
        }
        // This method uses a Left Join and a Computer Id IS NULL statement to only allow computers that have not been assigned to be deleted. 
        // POST: Computers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = @"DELETE Computer
                                            FROM Computer c
                                            LEFT JOIN ComputerEmployee ce 
                                            ON c.Id = ce.ComputerId 
                                            WHERE ce.ComputerId IS NULL 
                                            AND c.Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                        conn.Close();

                        return RedirectToAction(nameof(Index));
                    }
                }

            }
            catch
            {
                return View();
            }
        }

        // The private GetComputerById method is used to get an computer instance from the database by the computer id. It accepts one parameter: the computer id. 
        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                            Id,
                                            Make,
                                            Manufacturer,
                                            PurchaseDate
                                        FROM Computer
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    Computer computer = null;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                        };
                    }

                    reader.Close();
                    return computer;
                }
            }
        }
    }
}