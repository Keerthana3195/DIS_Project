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
        List<SelectListItem> country = new List<SelectListItem>();
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
            "Select",
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
            "Select",
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
            "Select",
            "2012",
            "2013",
            "2016",
            "2017",
            "2018",
            "2019",
            "2020",
            "2021",
        };

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        

        [HttpPost]
        public ActionResult getSelectedFood()
        {
            var selectedCity = Request.Form["City"].ToString(); //this will get selected value
            var recallYear = Request.Form["RecallFromInit"].ToString();

            getFoodDataAsPerSelection(selectedCity, recallYear);
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
            var recallYear = Request.Form["RecallFromInitD"].ToString();
            getDrugDataAsPerSelection(selectedCity, recallYear);
            FetchInitialFoodData();
            AllData obj1 = new AllData
            {
                DrugList = drugs,
                FoodList = food,
            };
            fetchDropDown();
            return View(obj1);

        }
        public void getFoodDataAsPerSelection(string selectedCity, string recallYear)
        {
            connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=FoodDrugDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection.Open();
            command.Connection = connection;
            string initialQuery = $"SELECT TOP(1000)[Product],[recall_year],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM[FoodDrugDb].[dbo].[Fu3$]";
            command.CommandText = initialQuery;
            if (recallYear == "Select" && selectedCity != "Select")
            {
                command.CommandText = initialQuery + $" Where city like '{selectedCity}'";
            }
            if (selectedCity == "Select" && recallYear != "Select")
            {
                command.CommandText = initialQuery + $" Where recall_year like '{recallYear}'";
            }
            if (selectedCity != "Select" && recallYear != "Select")
            {
                command.CommandText = initialQuery + $" Where city like '{selectedCity}' and recall_year like '{recallYear}'";
            }
            dr = command.ExecuteReader();
            while (dr.Read())
            {
                food.Add(new Food()
                {
                    Product = dr["Product"].ToString()
                    ,
                    Recall = dr["recall_year"].ToString().Substring(0, 4)
                    ,
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

        public void getDrugDataAsPerSelection(string cityDrug, string recallYear)
        {
            connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=FoodDrugDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection.Open();
            command.Connection = connection;
            string initialQuery = $"SELECT TOP(1000)[city],[state],[country],[classification],[voluntary_mandated],[distribution_pattern],[product_description],[reason_for_recall],[recall_initiation_date] FROM[FoodDrugDb].[dbo].[Drug$]";
            command.CommandText = initialQuery;
            if (recallYear == "Select" && cityDrug != "Select")
            {
                command.CommandText = initialQuery + $" Where city like '{cityDrug}'";
            }
            if (cityDrug == "Select" && recallYear != "Select")
            {
                command.CommandText = initialQuery + $" Where substring([recall_initiation_date],1,4) like '{recallYear}'";
            }
            if (cityDrug != "Select" && recallYear != "Select")
            {
                command.CommandText = initialQuery + $" Where city like '{cityDrug}' and substring([recall_initiation_date],1,4) like '{recallYear}'";
            }
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
                command.CommandText = "SELECT TOP(1000)[Product],[recall_year],[classification],[reason_for_recall],[voluntary_mandated],[country],[city],[state],[distribution_pattern] FROM [FoodDrugDb].[dbo].[Fu3$]";
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    food.Add(new Food() { 
                        Product = dr["Product"].ToString()
                        ,Recall = dr["recall_year"].ToString().Substring(0, 4)
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

            SelectListItem countryItem = new SelectListItem()
            {
                Text = "United States",
                Value = "United States",
                Selected = true
            };
            country.Add(countryItem);
            ViewBag.countryList = country;


        }
        public ActionResult Delete(Drug data)
        {
            connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=FoodDrugDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection.Open();
            command.Connection = connection;
            command.CommandText = $"DELETE FROM[FoodDrugDb].[dbo].[Drug$] Where UUID like '{data.UUID}'";
            dr = command.ExecuteReader();
            connection.Close();
            FetchInitialFoodData();
            FetchInitialDrugData();
            AllData objDel = new AllData
            {
                DrugList = drugs,
                FoodList = food,
            };
            return View(objDel);  
        }
        [HttpPost]
        public ActionResult sendDataToDb(AllData f1)
        {
            connection.ConnectionString = "Data Source=DESKTOP-HQCSK8E;Initial Catalog=FoodDrugDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection.Open();
            command.Connection = connection;
            Food obj = new Food();
            obj.Product = f1.Foodobj.Product;
            obj.Reason = f1.Foodobj.Reason;
            obj.Recall = f1.Foodobj.Recall;
            obj.State = f1.Foodobj.State;
            obj.Classification = f1.Foodobj.Classification;
            obj.City = f1.Foodobj.City;
            obj.Country = f1.Foodobj.Country;
            obj.Distribution = f1.Foodobj.Distribution;
            obj.State = f1.Foodobj.State;
            obj.Mandate_Recall = f1.Foodobj.Mandate_Recall;

            command.CommandText = $"INSERT INTO[FoodDrugDb].[dbo].[Fu3$] VALUES ('{obj.City}','{obj.State}','{obj.Country}','{obj.Classification}','{obj.Mandate_Recall}','{obj.Distribution}','{obj.Reason}','NULL','{obj.Product}','{obj.Recall}','NULL','NULL')";
            dr = command.ExecuteReader();
            connection.Close();
            FetchInitialFoodData();
            FetchInitialDrugData();
            AllData objCreate = new AllData
            {
                DrugList = drugs,
                FoodList = food,
            };
            return View(objCreate);
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
