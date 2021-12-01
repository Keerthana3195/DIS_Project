using DIS_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DIS_Project.Controllers
{
    public class HomeController : Controller
    {
        SqlCommand command = new SqlCommand();
        SqlConnection connection = new SqlConnection();
        SqlDataReader dr;
        AllData obj2 = new AllData();
        List<SelectListItem> countryItems = new List<SelectListItem>();
        List<SelectListItem> cityItems = new List<SelectListItem>();
        List<SelectListItem> dateItems = new List<SelectListItem>();
        List<Food> food = new List<Food>();
        List<Food> food2 = new List<Food>();
        List<string> countries = new List<string>();
        List<string> city = new List<string>();
        List<string> recallDate = new List<string>();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=Food_Drug;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection.Open();
            command.Connection = connection;
            
        }
        public AllData myFunction(string selectedCity)
        {
            command.CommandText = $"SELECT TOP(1000)[Product],[recall_initiation_date],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM[Food_Drug].[dbo].[Food$] Where city like '{selectedCity}'";
            //command.CommandText = $"SELECT TOP(1000)[Product],[recall_initiation_date],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM[Food_Drug].[dbo].[Food$] Where city like 'Mukilteo'" ;
            dr = command.ExecuteReader();
            while (dr.Read())
            {
                food.Add(new Food()
                {
                    Product = dr["Product"].ToString()
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
            AllData obj2 = new AllData
            {
                FoodList = food,
                Countries = countries.Distinct().ToList(),
                City = city.Distinct().ToList(),
                RecallInitDate = recallDate.Distinct().ToList()
            };
            fetchDropDown(countries, city, recallDate);
            return obj2;
        }
        [HttpPost]
        public ActionResult getSelectedValue()
        {
            var selectedCountry = Request.Form["Country"].ToString(); //this will get selected value
            var selectedCity = Request.Form["City"].ToString(); //this will get selected value
            var selectFromDate = Request.Form["RecallFromInit"].ToString();
            var selectToDate = Request.Form["RecallToInit"].ToString();
            AllData d1 = myFunction(selectedCity);
            
            return View(d1);
            
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
                command.CommandText = "SELECT TOP(1000)[Product],[recall_initiation_date],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM [Food_Drug].[dbo].[Food$]";
                dr = command.ExecuteReader();
                while (dr.Read())
                {
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
                fetchDropDown(countries, city, recallDate);
                connection.Close();
            }
            catch(Exception)
            {
                throw;
            }
        }
        private void fetchDropDown(List<string> countries, List<string> city, List<string> recallDate)
        {
            foreach (var i in countries)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = i,
                    Value = i,
                    Selected = false
                };
                countryItems.Add(item);

            }
            ViewBag.countryList = countryItems.Distinct().ToList();
            foreach (var i in city)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = i,
                    Value = i,
                    Selected = false
                };
                cityItems.Add(item);
            }

            ViewBag.cityList = cityItems.Distinct().ToList();
            foreach (var i in recallDate)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = i,
                    Value = i,
                    Selected = false
                };
                dateItems.Add(item);
            }
            ViewBag.dateList = dateItems.Distinct().ToList();
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
