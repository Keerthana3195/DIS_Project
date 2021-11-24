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
        List<string> countries = new List<string>();
        List<string> city = new List<string>();
        List<string> recallDate = new List<string>();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=Food_Drug;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        }

        public IActionResult Index()
        {
            FetchFoodData();
            AllData obj = new AllData{ 
                FoodList = food,
                Countries = countries.Distinct().ToList(),
                City = city.Distinct().ToList(),
                RecallInitDate = recallDate.Distinct().ToList()
            };
            return View(obj);
        }
        private void FetchFoodData()
        {
            try
            {
                var count = 0;
                connection.Open();
                command.Connection = connection;
                //command.CommandText = "SELECT TOP(1000)[city],[state],[country],[classification],[voluntary_mandated],[distribution_pattern],[reason_for_recall],[recall_initiation_date],[Product] FROM [Food_Drug].[dbo].[Food$]"; ;
                command.CommandText = "SELECT TOP(1000)[Product],[recall_initiation_date],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM [Food_Drug].[dbo].[Food$]";
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    //Debug.WriteLine(DateTime.ParseExact(dr["recall_initiation_date"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat));
                    //DateTime date = DateTime.ParseExact(dr["recall_initiation_date"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
                    food.Add(new Food() { Product = dr["Product"].ToString()
                        ,Recall = dr["recall_initiation_date"].ToString().Substring(0, 4)
                        ,Classification = dr["classification"].ToString(),
                        Reason = dr["reason_for_recall"].ToString(),
                        Mandate_Recall = dr["voluntary_mandated"].ToString(),
                        Country = dr["country"].ToString(),
                        City = dr["city"].ToString(),
                        State = dr["state"].ToString(),
                        Distribution = dr["distribution_pattern"].ToString()
                    });
                    countries.Add(dr["country"].ToString());
                    city.Add(dr["city"].ToString());
                    recallDate.Add(dr["recall_initiation_date"].ToString().Substring(0, 4));


                }
                Debug.WriteLine(count);
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
    }
}
