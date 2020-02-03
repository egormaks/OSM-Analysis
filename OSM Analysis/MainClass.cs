using System;
using System.Collections;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Data;

namespace OSM_Analysis
{
    class MainClass
    {
        private static string area = "Seattle";
        private static readonly String from = "Seattle";
        private static readonly String to = "Tacoma";
        private static List<Coordinates> bingCoordinates;
        private static List<Coordinates> osmCoordinates;
        private static string conStr = "Data Source=DESKTOP-V8S66SP;Initial Catalog=BingMaps;Integrated Security=True";

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


            //insertCoordinate(bingCoordinates[0].lat,bingCoordinates[0].lon);


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

        private static void insertDBCommand(string command)
        {
            try
            {
                string Command = command;
                using (SqlConnection myConnection = new SqlConnection(conStr))
                {
                    myConnection.Open();
                    SqlCommand myCommand = new SqlCommand(Command, myConnection);
                    myCommand.ExecuteNonQuery();

                    // is it necessary???
                    myConnection.Close();
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong with the database connection/querry");
               

            }

        }


        private static void insertCoordinate(double lat, double lon)
        {
            String query = "INSERT INTO COORDINATES" +
                    "([CITY]\n" +
                    ",[Lat]\n" +
                    ",[Long]\n" +
                    ",[Priority])\n" +
                    "VALUES\n" +
                    "('" + area +
                    "'," + lat +
                    "," + lon +
                    "," + 1 + ")";

            insertDBCommand(query);

        }


    }
}
