﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spotifly.Models;

namespace Spotifly.Controllers
{
    public class SpotiflyController : Controller
    {
        public SpotiflyController(ISpotifyWebAPI spotifyWebAPI_instance)
        {
        }


        // GET: Spotifly
        [Route("Spotifly")]
        [Route("Spotifly/Statistics")]
        public ActionResult Statistics()
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                Response.Redirect("/Home/Login");
                return View("../Home/Login");
            }

            ISpotifyWebAPI api = SpotifyAuth.FetchUserEndpoint(HttpContext.Session);

            ViewData["TopGenre"] = ChartGen.GenerateUserGenreChart(UsersTopGenres(api));
            ViewData["TopArtists"] = api.GetUsersTopArtists().Items.GetRange(0, api.GetUsersTopArtists().Items.Count);
            ViewData["TopTracks"] = api.GetUsersTopTracks().Items.GetRange(0, api.GetUsersTopArtists().Items.Count);

            List<FullArtist> topArtists = api.GetUsersTopArtists().Items;
            int artistCount = (topArtists.Count > 5) ? 5 : topArtists.Count;
            ViewData["TopArtists"] = topArtists.GetRange(0,artistCount);

            List<FullTrack> topTracks = api.GetUsersTopTracks().Items;
            int trackCount = (topTracks.Count > 5) ? 5 : topTracks.Count;
            ViewData["TopTracks"] = topTracks.GetRange(0, trackCount);
            return View();
        }

        [Route("Spotifly/TrackDetails")]
        public ActionResult TrackDetails(string id)
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                Response.Redirect("/Home/Login");
                return View("../Home/Login");
            }

            try
            {
                ISpotifyWebAPI api = SpotifyAuth.FetchUserEndpoint(HttpContext.Session);
                ViewData["Track"] = api.GetTrack(id);
                ViewData["RecommendedTracks"] = api.GetRecommendations(trackSeed: new List<string>() { id }).Tracks;
                ViewData["SpotifyWebAPI"] = api;
                ViewData["RadarPlot"] = ChartGen.GenerateAudioFeaturesChart(api.GetAudioFeatures(id));

                return View();
            }
            catch
            {
                return RedirectToAction();
            }
           
        }

        [Route("Spotifly/ArtistDetails")]
        public ActionResult ArtistDetails(string id)
        {
            try
            {
                ISpotifyWebAPI api = SpotifyAuth.FetchUserEndpoint(HttpContext.Session);
                ViewData["Artist"] = api.GetArtist(id);
                ViewData["RecommendedArtist"] = api.GetRelatedArtists(id);//or artist, possible break point
                ViewData["SpotifyWebAPI"] = api;
               // ViewData["RadarPlot"] = ChartGen.GenerateAudioFeaturesChart(api.GetAudioFeatures(id));

                return View();
            }
            catch
            {
                return RedirectToAction();
            }

        }

        [Route("Spotifly/Recommendations")]
        public IActionResult Recommendations()
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                Response.Redirect("/Home/Login");
                return View("../Home/Login");
            }
            ISpotifyWebAPI api = SpotifyAuth.FetchUserEndpoint(HttpContext.Session);

            List<PlayHistory> recent = api.GetUsersRecentlyPlayedTracks().Items;
            int recentPlayed = (recent.Count > 5) ? 5 : recent.Count;
            List<PlayHistory> myRecent = recent.GetRange(0, recentPlayed);

            List<FullTrack> topTracks = api.GetUsersTopTracks().Items;
            int trackCount = (topTracks.Count > 5) ? 5 : topTracks.Count;
            List<FullTrack> myTracks = topTracks.GetRange(0, trackCount);
            var vTracks = new List<SimpleTrack>();
            List<string> id = new List<string>();

            foreach (var track in myRecent)
            {
                id.Add(track.Track.Id);
            }
            foreach (var track in myTracks)
            {
                id.Add(track.Id);
            }
            vTracks = api.GetRecommendations(trackSeed: id).Tracks;
            if (vTracks == null)
                vTracks = new List<SimpleTrack>();
            return View(vTracks);
        }
        #region PrivateMethods

        public Dictionary<string, int> UsersTopGenres(ISpotifyWebAPI api)
        {
            //Get Users Top Artists
            List<FullArtist> topArtists = api.GetUsersTopArtists().Items;

            //Get the genre
            Dictionary<string, int> usersTopGenres = new Dictionary<string, int>();
            topArtists.ForEach(artist => artist.Genres.ForEach(genre => {

                //Check if this genre already occures
                if (usersTopGenres.ContainsKey(genre))
                {
                    usersTopGenres[genre]++;
                }
                else
                {
                    usersTopGenres.Add(genre, 1);
                }

            })
            );

            //Order the list
            Dictionary<string, int> orderedGenres = usersTopGenres.OrderByDescending(genre => genre.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return orderedGenres;
        }

        #endregion
    }
}