using System;
using System.Collections;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace OSM_Analysis
{
    class MainClass
    {
        private static readonly String from = "Seattle";
        private static readonly String to = "Tacoma";
        private static ArrayList bingCoordinates;
        private static ArrayList osmCoordinates;

        public static void Main(string[] args)
        {
            // Process Bing request           
            String bingURL = getBingURL();
            String bingJson = getJsonResponse(bingURL);
            //parse the json response
            BingApiResponse bingObj = Newtonsoft.Json.JsonConvert.DeserializeObject<BingApiResponse>(bingJson);
            bingCoordinates = bingObj.getBingCoordinates();

            // TODO: GSON equiv in C#?
            // get bing coords



            // Process OSM request

            String osmURL = getOsmURL();
            String osmJson = getJsonResponse(osmURL);

            // TODO: GSON equiv in C#?
            // get osm coords

            OsmApiResponse osmObj = Newtonsoft.Json.JsonConvert.DeserializeObject<OsmApiResponse>(bingJson);
           // bingCoordinates = osmObj.getBingCoordinates();


            // TODO: Process Google API request



        }

        private static String getBingURL()
        {
            BingRequestMessage message = new BingRequestMessage();
            ICollection<String> wps = new List<String>();
            wps.Add(from);
            wps.Add(to);
            message.SetWayPoints(wps);

            ICollection<ToAvoid> avoid = new List<ToAvoid>();
            avoid.Add(ToAvoid.minimizeTolls);
            message.SetAvoid(avoid);

            return message.GenerateRequest(); 

           // var client = new RestClient("https://api-mapper.clicksend.com/http/v2/send.php?method=http&username=harnidhk&key=B1EF90CF-7A07-AB43-2FB4-A728AEBE58C6&to=+1");
           // var execute = client.Execute(new RestRequest());
        }

        private static String getOsmURL()
        {
            OsmRequestMessage message = new OsmRequestMessage();
            message.setFrom((Coordinates)bingCoordinates[0]);
            message.setTo((Coordinates)bingCoordinates[bingCoordinates.Count - 1]);
            return message.generateRequest();
        }

        private static String getJsonResponse(String theStr)
        {
             var client = new RestClient(theStr);
             var execute = client.Execute(new RestRequest());
             return execute.Content;
        }


    }
}
