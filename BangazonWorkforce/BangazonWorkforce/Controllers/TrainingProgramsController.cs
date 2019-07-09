// Author: Chris Morgan
// The purpose of the TrainingProgramsController is to hold all of the methods that deal with the TrainingProgram database actions / CRUD functionality within the application

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
    public class TrainingProgramsController : Controller
    {
        // Establish connection properties to connect to the BangazonAPI
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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

        // The index() method is a GetAllTrainingDepartments method. It only returns training departments that haven't started yet. The result is passed into the Index view for Employees
        // GET: TrainingPrograms
        public ActionResult Index()
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
                                        WHERE t.StartDate > GetDate()";

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

                    return View(trainingPrograms);

                }
            }
           
        }

        // Gets details of a training program the user selects on the index view. This method accepts one parameter: the training program id.
        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            TrainingProgram trainingProgram = GetTrainingProgramById(id);

            if(trainingProgram == null)
            {
                return NotFound();
            }
            else
            {
                TrainingProgramDetailsViewModel viewModel = new TrainingProgramDetailsViewModel(id, _config.GetConnectionString("DefaultConnection"));

                viewModel.TrainingProgram = trainingProgram;

                return View(viewModel);
            }

        }

        // This is the initial get for the create functionality and builds the form
        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // This makes a post to the TrainingProgram table in the BangazonAPI database. This method takes one parameter: A TrainingProgram object that is built with the form inputs
        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram trainingProgram)
        {
            try
            {
                // TODO: Add insert logic here

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO TrainingProgram
                ( Name, StartDate, EndDate, MaxAttendees )
                VALUES
                ( @Name, @StartDate, @EndDate, @MaxAttendees)";
                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }


            }
            catch
            {
                return View();
            }
        }

        // Initial Edit(get) method, this gets the training program the user wants to edit and passes it down to the edit view. This method accepts one parameter: the trainingprogram id
        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            TrainingProgram trainingProgram = GetTrainingProgramById(id);

            return View(trainingProgram);
        }

        // The UPDATE functionality for editing training programs. This method accepts two parameters: The updated TrainingProgram id, and the TrainingProgram built with the form inputs
        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram trainingProgram)
        {
            try
            {
                using(SqlConnection conn = Connection)
                {
                    conn.Open();

                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram
                                            SET Name = @Name, StartDate = @StartDate, EndDate = @EndDate, MaxAttendees = @MaxAttendees
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));

                    }
                }

            }
            catch
            {
                return View();
            }
        }
        // This gets the training program by Id and then passes it down to the delete view.
        // GET: TrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            TrainingProgram trainingProgram = GetTrainingProgramById(id);

            return View(trainingProgram);
        }

        // This is the functionality of the delete, this method makes sure to delete all the items in the EmployeeTraining join table first, then deletes the training program.
        // POST: TrainingPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, IFormCollection collection)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM EmployeeTraining WHERE TrainingProgramId = @id; 
                                            DELETE FROM TrainingProgram WHERE Id=@id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // This method will be used to get a certain training program by Id. This private method is used in Details, Edit (get), and Delete(get). This method accepts one parameter: the training program id
        private TrainingProgram GetTrainingProgramById(int id)
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
                                        WHERE t.StartDate > GetDate()
                                        AND t.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    TrainingProgram trainingProgram = null;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };  
                    }

                    reader.Close();

                    return trainingProgram;

                }
            }
        }
    }
}