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
        List<SelectListItem> foodCitiesDropdown = new List<SelectListItem>();
        List<SelectListItem> drugCitiesDropdown = new List<SelectListItem>();
        List<SelectListItem> recallDate = new List<SelectListItem>();
        List<SelectListItem> cityItems = new List<SelectListItem>();
        List<SelectListItem> dateItems = new List<SelectListItem>();
        List<Food> food = new List<Food>();
        List<Drug> drugs = new List<Drug>();
        //List<Drug> allDrugs = new List<Drug>();
        List<Food> food2 = new List<Food>();
        List<string> countries = new List<string>();
        List<string> city = new List<string>();
        
        string[] FoodCities = new string[]
        {
        "Alexandria",
        "Bayamon",
        "Faribault",
        "Gardena",
        "Grand Rapids",
        "Indianapolis",
        "Irvine",
        "Kansas City",
        "Kent",
        "Manitowoc",
        "Medford",
        "Medley",
        "Mukilteo",
        "Plymouth",
        "Portales",
        "Provo",
        "Purchase",
        "Raleigh",
        "Rancho Dominguez",
        "Swedesboro",
        "Tucker",
        "Vernon",
        "West Valley City",
        "Woodward"
        };

        string[] DrugCities = new string[]
        {
        "Baltimore",
        "Bridgewater",
        "Burbank",
        "Deerfield",
        "Dublin",
        "East Windsor",
        "Elizabeth",
        "Gaithersburg",
        "Lake Forest",
        "Mesa",
        "New Paltz",
        "New York",
        "North Wales",
        "Princeton",
        "Waltham",
        "Whitehouse Station"
        };
        string[] recallYears = new string[]
        {
            "2010",
            "2011",
            "2012"
        };
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //[HttpPost]
        //public ActionResult sendDataToDb(AllData f1)
        //{
            
        //    Food obj = new Food();
        //    obj.Product = f1.Foodobj.Product;
        //    obj.Reason = f1.Foodobj.Reason;
        //    obj.Recall = f1.Foodobj.Recall;
        //    obj.State = f1.Foodobj.State;
        //    obj.Classification = f1.Foodobj.Classification;
        //    obj.City = f1.Foodobj.City;
        //    obj.Country = f1.Foodobj.Country;
        //    obj.Distribution = f1.Foodobj.Distribution;
        //    obj.State = f1.Foodobj.State;

        //    command.CommandText = $"INSERT INTO[Food].[dbo].[Sheet1$] VALUES ('{obj.City}','{obj.State}','{obj.Country}','{obj.Classification}','{obj.Mandate_Recall}','{obj.Distribution}','{obj.Reason}','{obj.Recall}','{obj.Product}','','','')";
        //    dr = command.ExecuteReader();

        //    //connection.Close();
        //    //FetchFoodData();
        //    AllData obj3 = new AllData
        //    {
        //        FoodList = food,
        //        Countries = countries.Distinct().ToList(),
        //        City = city.Distinct().ToList(),
        //        RecallInitDate = recallDate.Distinct().ToList()
        //    };
        //    return View(obj3);
            
        //}
        public void getFoodDataAsPerSelection(string selectedCity)
        {
            connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=FoodDrugDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection.Open();
            command.Connection = connection;
            command.CommandText = $"SELECT TOP(1000)[Product],[recall_initiation_date],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM[FoodDrugDb].[dbo].[Food$] Where city like '{selectedCity}'";
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
                
            }
            connection.Close();  
        }

        [HttpPost]
        public ActionResult getSelectedFood()
        {
            var selectedCity = Request.Form["City"].ToString(); //this will get selected value
            getFoodDataAsPerSelection(selectedCity);
            FetchInitialDrugData();
            AllData d1 = new AllData
            {
                DrugList = drugs,
                FoodList = food,
            };
            fetchDropDown();
            return View(d1);
        }

        [HttpPost]
        public ActionResult getSelectedDrug()
        {
            var selectedCity = Request.Form["CityD"].ToString(); //this will get selected value
            
            getDrugDataAsPerSelection(selectedCity);
            FetchInitialFoodData();
            AllData obj1 = new AllData
            {
                DrugList = drugs,
                FoodList = food,
            };
            fetchDropDown();
            return View(obj1);

        }

        public void getDrugDataAsPerSelection(string cityDrug)
        {
            connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=FoodDrugDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection.Open();
            command.Connection = connection;
            command.CommandText = $"SELECT TOP(1000)[city],[state],[country],[classification],[voluntary_mandated],[distribution_pattern],[product_description],[reason_for_recall],[recall_initiation_date] FROM[FoodDrugDb].[dbo].[Drug$] Where city like '{cityDrug}'";
            dr = command.ExecuteReader();
            while (dr.Read())
            {
                drugs.Add(new Drug()
                {
                    City = dr["city"].ToString(),
                    State = dr["state"].ToString(),
                    Country = dr["country"].ToString(),
                    Classification = dr["classification"].ToString(),
                    Voluntary_Mandate = dr["voluntary_mandated"].ToString(),
                    Distribution = dr["distribution_pattern"].ToString(),
                    ProductDescription = dr["product_description"].ToString(),
                    Reason = dr["reason_for_recall"].ToString(),
                    RecallInitDate = dr["recall_initiation_date"].ToString()
                });
                 
            }
            connection.Close();
            
        }
        public IActionResult Index()
        {
            FetchInitialDrugData();
            FetchInitialFoodData();
            AllData obj = new AllData{ 
                DrugList = drugs,
                FoodList = food,
            };
            return View(obj);
        }
        private void FetchInitialFoodData()
        {
            try
            {
                connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=FoodDrugDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT TOP(1000)[Product],[recall_initiation_date],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM [FoodDrugDb].[dbo].[Food$]";
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    food.Add(new Food() { 
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
                }
                fetchDropDown();
                connection.Close();  
            }
            catch(Exception)
            {
                throw;
            }
        }
        private void FetchInitialDrugData()
        {
            try
            {
                connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=FoodDrugDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT TOP(1000)[city],[state],[country],[classification],[voluntary_mandated],[distribution_pattern],[product_description],[reason_for_recall],[recall_initiation_date] FROM[FoodDrugDb].[dbo].[Drug$]";
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    drugs.Add(new Drug()
                    {
                        ProductDescription = dr["product_description"].ToString(),
                        RecallInitDate = dr["recall_initiation_date"].ToString().Substring(0, 4)
                        ,
                        Classification = dr["classification"].ToString()
                        ,
                        Reason = dr["reason_for_recall"].ToString()
                        ,
                        Voluntary_Mandate = dr["voluntary_mandated"].ToString()
                        ,
                        Country = dr["country"].ToString()
                        ,
                        City = dr["city"].ToString()
                        ,
                        State = dr["state"].ToString()
                        ,
                        Distribution = dr["distribution_pattern"].ToString()
                    });

                }
                connection.Close();

            }
            catch (Exception)
            {
                throw;
            }
        }
        private void fetchDropDown()
        {
            foreach (var i in FoodCities)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = i,
                    Value = i,
                    Selected = false
                };
                foodCitiesDropdown.Add(item);

            }
            ViewBag.foodCitiesDropdown = foodCitiesDropdown;

            foreach (var i in DrugCities)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = i,
                    Value = i,
                    Selected = false
                };
                drugCitiesDropdown.Add(item);
            }

            ViewBag.drugCitiesDropdown = drugCitiesDropdown;

            foreach (var i in recallYears)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = i,
                    Value = i,
                    Selected = false
                };
                recallDate.Add(item);
            }
            ViewBag.dateList = recallDate;
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
