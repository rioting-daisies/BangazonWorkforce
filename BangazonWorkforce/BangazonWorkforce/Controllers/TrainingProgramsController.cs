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

        // This makes a post to the TrainingProgram table in the BangazonAPI database
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

        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainingPrograms/Edit/5
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

        // GET: TrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingPrograms/Delete/5
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