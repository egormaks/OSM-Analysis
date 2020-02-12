using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace OSM_Analysis
{
    class MainClass
    {
        private static String from = "";
        private static String to = "";
        public static String area = "Seattle";
        private static List<Coordinates> bingCoordinates;
        private static List<Coordinates> osmCoordinates;
        private static List<Coordinates> googleCoordinates;

        public static void Main(string[] args)
        {
            List<Coordinates> areaCoordinates = generateAreaCoordinates();

            for (int runCount = 0; runCount < Properties.Settings.Default.noOfRuns; runCount++)
            {
                Console.WriteLine("Processing " + (runCount + 1) + "nd route in " + area);
                try
                {
                    processOneRoute(areaCoordinates);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not process " + (runCount + 1) + "th count");
                    Console.WriteLine(e);
                    Thread.Sleep(5000);
                }
            }
        }

        private static void processOneRoute(List<Coordinates> areaCoordinates)
        {
            Coordinates fromCor = null;
            Coordinates toCor = null;
            var rand = new Random();

            while (fromCor == null
                || fromCor.getrouteID() != null
                )
            {
                fromCor = areaCoordinates[(int)(rand.NextDouble() * areaCoordinates.Count)];
                from = fromCor.convertToString();
            }

            while (toCor == null
                || toCor.getrouteID() != null
                || to.Equals(from)
                || GetDist(fromCor, toCor) < Properties.Settings.Default.fromToMinDist)
            {
                toCor = areaCoordinates[(int)(rand.NextDouble() * areaCoordinates.Count)];
                to = toCor.convertToString();
            }
            Console.WriteLine("   From " + from + " to " + to);

            // Process Bing request
            {
                String bingURL = GetBingURL();
                String bingJson = GetJsonResponse(bingURL);
                //parse the json response
                BingApiResponse bingObj = Newtonsoft.Json.JsonConvert.DeserializeObject<BingApiResponse>(bingJson);
                bingCoordinates = bingObj.getBingCoordinates();
            }

            // Process OSM request
            {
                String osmURL = GetOsmURL();
                String osmJson = GetJsonResponse(osmURL);
                Console.WriteLine("   " + osmJson);
                OsmApiResponse osmObj = Newtonsoft.Json.JsonConvert.DeserializeObject<OsmApiResponse>(osmJson);
                osmCoordinates = osmObj.getOsmCoordinates();
            }

            // TODO: Process Google API request
            {
                String googleUrl = GetGoogleUrl();
                String googleJson = GetJsonResponse(googleUrl);
                Console.WriteLine(googleJson);
                GoogleApiResponse googleObj = 
                    Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleApiResponse>(googleJson);
                googleCoordinates = googleObj.GetGoogleCoordinates();


            }
            InsertNewCordinates(areaCoordinates);
            Analysis.doAnalysis(bingCoordinates, osmCoordinates);

        }

        private static void InsertNewCordinates(List<Coordinates> areaCoordinates)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.jdbcUrl))
            {
                connection.Open();
                foreach (Coordinates cor in bingCoordinates)
                {
                    if (!areaCoordinates.Contains(cor))
                    {
                        InsertCoordinate(cor.getLat(), cor.getLon(), connection);
                        areaCoordinates.Add(cor);
                        count++;
                    }
                }

                foreach (Coordinates cor in osmCoordinates)
                {
                    if (!areaCoordinates.Contains(cor))
                    {
                        InsertCoordinate(cor.getLat(), cor.getLon(), connection);
                        areaCoordinates.Add(cor);
                        count++;
                    }
                }
                connection.Close();
            }
            Console.WriteLine("   Sucessfully inserted " + count + " new coordinates in " + area);
        }
        
        private static string GetGoogleUrl()
        {
            GoogleRequestMessage message = new GoogleRequestMessage();
            List<Coordinates> route = new List<Coordinates>();
            route.Add((Coordinates) bingCoordinates[0]);
            route.Add((Coordinates) bingCoordinates[bingCoordinates.Count - 1]);
            message.setRouteCoords(route);
            message.setOutputFormat(OutputFormats.json);
            return message.generateGRequest();
        }

        private static double GetDist(Coordinates fromCor, Coordinates toCor)
        {
            String disQuery = Properties.Settings.Default.disQuery;
            disQuery = disQuery.Replace("<POINT1_COR>", fromCor.getLon() + " " + fromCor.getLat());
            disQuery = disQuery.Replace("<POINT2_COR>", toCor.getLon() + " " + toCor.getLat());
            Double dist = ConnectionUtils.ExecuteJdbcSingleOutputQuery(disQuery);
            return dist;
        }

        private static String GetBingURL()
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
        }

        private static String GetOsmURL()
        {
            OsmRequestMessage message = new OsmRequestMessage();
            message.setFrom((Coordinates)bingCoordinates[0]);
            message.setTo((Coordinates)bingCoordinates[bingCoordinates.Count - 1]);
            return message.generateRequest();
        }

        private static String GetJsonResponse(String theStr)
        {
            var client = new RestClient(theStr);
            var execute = client.Execute(new RestRequest());
            return execute.Content;
        }

        private static void InsertDBCommand(string Command)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(Properties.Settings.Default.jdbcUrl))
                {
                    myConnection.Open();
                    SqlCommand myCommand = new SqlCommand(Command, myConnection);
                    myCommand.ExecuteNonQuery();

                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong with the database connection/query" + ex);
            }

        }


        private static void InsertCoordinate(double lat, double lon, SqlConnection connection)
        {
            String query = Properties.Settings.Default.InsertCoordinateQuery;
            query = query.Replace("<AREA>", area);
            query = query.Replace("<LAT>", lat.ToString());
            query = query.Replace("<LON>", lon.ToString());
            ConnectionUtils.ExecuteJdbcQueryUsingCon(query, connection);
        }

        private static List<Coordinates> generateAreaCoordinates()
        {
            List<Coordinates> areaCoordinates = new List<Coordinates>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.jdbcUrl))
                {
                    String query = Properties.Settings.Default.areaSelectQuery;
                    query = query.Replace("<AREA>", area);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double lat = (double)reader.GetDecimal(reader.GetOrdinal("Lat"));
                                double lon = (double)reader.GetDecimal(reader.GetOrdinal("Long"));
                                int? routeID;
                                if (reader["Priority"].GetType().Name.Equals("Int32"))
                                {
                                    routeID = (int)reader["Priority"];
                                }
                                else
                                {
                                    routeID = null;
                                }
                                areaCoordinates.Add(new Coordinates(lat, lon, routeID));
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong with the database connection/query" + ex);
                Console.WriteLine("Could not get Coordinates for " + area);
            }
            return areaCoordinates;
        }
    }
}
