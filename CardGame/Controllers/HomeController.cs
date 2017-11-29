using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;

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
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Cards()
        {

            HttpWebRequest WR = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");

            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();

            StreamReader Reader = new StreamReader(Response.GetResponseStream());

            string cardsData = Reader.ReadToEnd();

            JObject JsonData = JObject.Parse(cardsData);

            string deckID = JsonData["deck_id"].ToString();

            Cookie c = new Cookie("deckID", deckID);
            Response.Cookies.Add(c);

            WR = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/draw/?count=5");

            Response = (HttpWebResponse)WR.GetResponse();

            StreamReader Reader2 = new StreamReader(Response.GetResponseStream());

            string DrawnCards = Reader2.ReadToEnd();

            JsonData = JObject.Parse(DrawnCards);

            ViewBag.Cards = JsonData["cards"];

            return View();
        }

        public ActionResult Redraw(int number)
        {
            HttpCookie c = Request.Cookies["deckID"];

            string deckID = c.Value.ToString();

            HttpWebRequest WR = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/draw/?count={number}");

            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();

            StreamReader Reader = new StreamReader(Response.GetResponseStream());

            string drawnCards = Reader.ReadToEnd();

            JObject JsonData = JObject.Parse(drawnCards);

            return View("Cards");
        }

    }
}