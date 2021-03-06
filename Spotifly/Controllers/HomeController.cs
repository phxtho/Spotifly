﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

//SpotifyAPI-NET
using Spotifly.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Web.Enums;

//ChartJs
using ChartJSCore.Models;

namespace Spotifly.Controllers
{
    public class HomeController : Controller
    {
        private ISpotifyWebAPI _spotifyWebAPI;


        public HomeController(ISpotifyWebAPI spotifyWebAPI)
        {
            _spotifyWebAPI = spotifyWebAPI;

        }


        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                Response.Redirect("/Home/Login");
                return View("Login");
            }
            ISpotifyWebAPI api = SpotifyAuth.FetchUserEndpoint(HttpContext.Session);
            var userSpotifyProfile = api.GetPrivateProfile();

            User user = new User();
            user.Name = userSpotifyProfile.DisplayName;
            user.Email = userSpotifyProfile.Email;

            ViewData["PieChart"] = GeneratePieChart();
            ViewData["Users"] = Models.User.SelectAll();
            ViewData["Playlists"] = api.GetUserPlaylists(api.GetPrivateProfile().Id); 
            ViewData["RecentlyPlayed"]= api.GetUsersRecentlyPlayedTracks().Items.GetRange(0, api.GetUsersRecentlyPlayedTracks().Items.Count);
            return View();
        }

        public IActionResult About() => View();


        [Route("Home/Statistics")]
        public IActionResult Statistics()
        {
            return View();
        }

        [Route("Home/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Home/Login")]
        public IActionResult PostLogin()
        {
            IFormCollection form = HttpContext.Request.Form;

            try
            {
                if (SpotifyAuth.LogInUser(form["email"], form["password"], HttpContext.Session))
                {
                    Response.Redirect("/Home");
                    Index();
                    return View("Index");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Incorrect password.";
                }
            }
            #pragma warning disable 0168
            catch (Exception e)
            #pragma warning restore 0168
            {
                ViewData["ErrorMessage"] = "Email not found.";
            }
            return View("Login");
        }

        [Route("Home/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Home/Register")]
        public IActionResult PostRegistration()
        {
            IFormCollection form = Request.Form;

            try
            {
                if (SpotifyAuth.RegisterUser(form["email"], form["password1"], form["password2"], DateTime.Now, HttpContext.Session))
                {
                    Response.Redirect("/Home");
                    Index();
                    return View("Index");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Passwords don't match";
                }
            }
            #pragma warning disable 0168
            catch (Exception e)
            #pragma warning restore 0168
            {
                ViewData["ErrorMessage"] = "Something went wrong while registering your account :( Maybe you already have one.";
            }
            return View("Register");
        }

        [Route("Home/Logout")]
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                Response.Redirect("/Home/Login");
                return View("Login");
            }
            HttpContext.Session.Clear();
            return View("Login");
        }


        public IActionResult Privacy()
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                Response.Redirect("/Home/Login");
                return View("Login");
            }
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public Chart GeneratePieChart()
        {  
            ISpotifyWebAPI api = SpotifyAuth.FetchUserEndpoint(HttpContext.Session);
            Chart chart = new Chart();
            chart.Type = "pie";
           
            Paging<FullArtist> TopArtists= api.GetUsersTopArtists(TimeRangeType.MediumTerm,200);
            Paging<FullTrack> TopTracks= api.GetUsersTopTracks(TimeRangeType.MediumTerm,200);
            CategoryList TopCategories= api.GetCategories("South Africa","",200);
            Double artistCount= (TopArtists== null)?0:TopArtists.Items.Count();
            Double trackCount= (TopTracks==null)?0:TopTracks.Items.Count();
            Double categoryCount= 20;
            Data data = new Data();
            data.Labels = new List<string>() { "Top Artists", "Top Tracks", "Categories" };
            Console.WriteLine(artistCount);
            Console.WriteLine(trackCount);
            PieDataset dataset = new PieDataset()
            {
                Label = "My dataset",
                BackgroundColor = new List<string>() { "#FF6384", "#36A2EB", "#FFCE56" },
                HoverBackgroundColor = new List<string>() { "#FF6384", "#36A2EB", "#FFCE56" },
                Data = new List<double>() { artistCount,trackCount, categoryCount }
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            return chart;
        }
    }
}
