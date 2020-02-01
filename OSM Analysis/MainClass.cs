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
            String json = getJsonResponse(bingURL);
            //parse the json response
            BingApiResponse obj = Newtonsoft.Json.JsonConvert.DeserializeObject<BingApiResponse>(json);
            bingCoordinates = obj.getBingCoordinates();

            // TODO: GSON equiv in C#?
            // get bing coords



            // Process OSM request

            String osmURL = getOsmURL();
               // String json = getJsonResponse(osmURL);
                // TODO: GSON equiv in C#?
                // get osm coords
            


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
            return null;
        }

        private static String getJsonResponse(String theStr)
        {
             var client = new RestClient(theStr);
             var execute = client.Execute(new RestRequest());
             return execute.Content;
        }


    }
}
