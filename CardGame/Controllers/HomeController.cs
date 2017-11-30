using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;
using CardGame.Models;

namespace CardGame.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Cards()
        {
            string deckID = null;

            if (Session["deckID"] == null)
            {
                HttpWebRequest WR = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");

                HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());

                string cardsData = Reader.ReadToEnd();

                JObject JsonData = JObject.Parse(cardsData);

                deckID = JsonData["deck_id"].ToString();

                Session.Add("deckID", deckID);
            }
            else
            {
                deckID = Session["deckID"].ToString();
            }
            
            HttpWebRequest WR2 = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/draw/?count=5");

            HttpWebResponse Response2 = (HttpWebResponse)WR2.GetResponse();

            StreamReader Reader2 = new StreamReader(Response2.GetResponseStream());

            string DrawnCards = Reader2.ReadToEnd();

            ViewBag.Json = DrawnCards;

            JObject JsonCardData = JObject.Parse(DrawnCards);

            ViewBag.Cards = JsonCardData["cards"];

            return View();
        }
        //[NonAction]
        public ActionResult Redraw(string[] code)
        {
            string deckID = Session["deckID"].ToString();
            string userHand = "userhand" + deckID;

            string keepCards = "";
            int counter = 5 - code.Length;
            for (int i = 0; i < code.Length; i++)
            {
                    if (i == 0)
                    {
                        keepCards += code;
                    }
                    else
                    {
                        keepCards += "," + code;
                    }
            }

            if (counter < 5)
            {
                HttpWebRequest WR = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/pile/{userHand}/add/?cards={keepCards}");
            }

            if (counter != 0)
            {
                HttpWebRequest WR2 = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/draw/?count={counter}");

                HttpWebResponse Response = (HttpWebResponse)WR2.GetResponse();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());

                string drawnCards = Reader.ReadToEnd();

                JObject JsonData = JObject.Parse(drawnCards);

                string newCards = "";
                for (int i = 0; i < counter; i++)
                {
                    if (i < 1)
                    {
                        newCards += JsonData["cards"][i]["code"].ToString();
                    }
                    else
                    {
                        newCards += "," + JsonData["cards"][i]["code"].ToString();
                    }
                }

                WR2 = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/pile/{userHand}/add/?cards={newCards}");
            }

            HttpWebRequest WR3 = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/pile/{userHand}/list");

            HttpWebResponse Response3 = (HttpWebResponse)WR3.GetResponse();

            StreamReader Reader3 = new StreamReader(Response3.GetResponseStream());

            string userCards = Reader3.ReadToEnd();

            JObject JsonData3 = JObject.Parse(userCards);

            ViewBag.Cards = JsonData3["cards"];

            return View("Cards");
        }
    }
}