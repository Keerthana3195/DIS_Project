using DIS_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace DIS_Project.Controllers
{
    public class HomeController : Controller
    {
        SqlCommand command = new SqlCommand();
        SqlConnection connection = new SqlConnection();
        SqlDataReader dr;
        List<Food> food = new List<Food>();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            connection.ConnectionString = "Data Source=DESKTOP-AEPCTHQ;Initial Catalog=Food;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        }

        public IActionResult Index()
        {
            FetchFoodData();
            return View(food);
        }
        private void FetchFoodData()
        {
            try
            {
                connection.Open();
                command.Connection = connection;
                //command.CommandText = "SELECT TOP(1000)[city],[state],[country],[classification],[voluntary_mandated],[distribution_pattern],[reason_for_recall],[recall_initiation_date],[Product] FROM [Food_Drug].[dbo].[Food$]"; ;
                command.CommandText = "SELECT TOP(1000)[Product],[recall_initiation_date],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM [Food].[dbo].[Food$]";
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    food.Add(new Food() { Product = dr["Product"].ToString()
                        ,Recall = dr["recall_initiation_date"].ToString(),
                        Classification = dr["classification"].ToString(),
                        Reason = dr["reason_for_recall"].ToString(),
                        Mandate_Recall = dr["voluntary_mandated"].ToString(),
                        Country = dr["country"].ToString(),
                        City = dr["city"].ToString(),
                        State = dr["state"].ToString(),
                        Distribution = dr["distribution_pattern"].ToString()

                    });
                }
                connection.Close();
            }
            catch(Exception)
            {
                throw;
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Dashboard()
        {
            return View();
        }

    }
}
