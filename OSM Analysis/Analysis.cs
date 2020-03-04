using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OSM_Analysis
{
    class Analysis
    {
        private static Dictionary<int, AreaDeflection> AreaDefMap = new Dictionary<int, AreaDeflection>();
        private static Dictionary<Coordinates, AreaDeflection> AreaHeatmap = new Dictionary<Coordinates, AreaDeflection>();

        private static double getLeastDistanceFromSetOfLinesUsingSql(Coordinates osmCoordinate, List<Coordinates> bingCoordinates)
        {
            String query = Properties.Settings.Default.MinDistanceQuery;
            String bingString = "";
            foreach (Coordinates cor in bingCoordinates)
            {
                bingString += cor.getLon() + " " + cor.getLat() + ",";
            }
            bingString = bingString.TrimEnd(','); // remove last comma

            query = query.Replace("<CORDINATE_STRING>", bingString);
            query = query.Replace("<OSM_POINT>", osmCoordinate.getLon() + " " + osmCoordinate.getLat());

            return ConnectionUtils.ExecuteJdbcSingleOutputQuery(query);
        }

        private static void UpdateGoogleHeatmap(Dictionary<Coordinates, double> googleDistances)
        {
            if (AreaHeatmap.Count == 0)
            {
                AreaHeatmap = IntializeAreaHeatmap(MainClass.area);
            }

            foreach (KeyValuePair<Coordinates, double> dist
                in new HashSet<KeyValuePair<Coordinates, double>>(googleDistances))
            {
                double lat = dist.Key.getLat();
                double lon = dist.Key.getLon();

                lat = Math.Round(Math.Ceiling(lat / 0.05) * 0.05 * 100) / 100;
                lon = Math.Round(Math.Ceiling(lon / 0.05) * 0.05 * 100) / 100;
                Coordinates cor = new Coordinates(lat, lon, 1);
                if (AreaHeatmap.ContainsKey(cor))
                {
                    AreaDeflection avgDef = AreaHeatmap[cor];
                    int noOfPoints = avgDef.getNoOfPoints();
                    double avgDeflection = avgDef.getAvgDeflection();
                    avgDeflection = (avgDeflection * noOfPoints + dist.Value) / (noOfPoints + 1);
                    avgDef.setAvgDeflection(avgDeflection);
                    avgDef.setNoOfPoints(noOfPoints + 1);
                    AreaHeatmap[cor] = avgDef;
                }
                else
                {
                    AreaDeflection avgDef = new AreaDeflection(new Area(MainClass.area, null, null), null, null, dist.Value, 1);
                    AreaHeatmap.Add(cor, avgDef);
                }
            }
            UpdateGoogleHeatmapTable();
            Console.WriteLine("   Updated heatmap using " + googleDistances.Count + " points");
        }
        private static void UpdateHeatmap(Dictionary<Coordinates, double> osmDistances)
        {
            if (AreaHeatmap.Count == 0)
            {
                AreaHeatmap = IntializeAreaHeatmap(MainClass.area);
            }

            foreach (KeyValuePair<Coordinates, double> dist
                in new HashSet<KeyValuePair<Coordinates, double>>(osmDistances))
            {
                double lat = dist.Key.getLat();
                double lon = dist.Key.getLon();

                lat = Math.Round(Math.Ceiling(lat / 0.05) * 0.05 * 100) / 100;
                lon = Math.Round(Math.Ceiling(lon / 0.05) * 0.05 * 100) / 100;
                Coordinates cor = new Coordinates(lat, lon, 1);
                if (AreaHeatmap.ContainsKey(cor))
                {
                    AreaDeflection avgDef = AreaHeatmap[cor];
                    int noOfPoints = avgDef.getNoOfPoints();
                    double avgDeflection = avgDef.getAvgDeflection();
                    avgDeflection = (avgDeflection * noOfPoints + dist.Value) / (noOfPoints + 1);
                    avgDef.setAvgDeflection(avgDeflection);
                    avgDef.setNoOfPoints(noOfPoints + 1);
                    AreaHeatmap[cor]= avgDef;
                }
                else
                {
                    AreaDeflection avgDef = new AreaDeflection(new Area(MainClass.area, null, null), null, null, dist.Value, 1);
                    AreaHeatmap.Add(cor, avgDef);
                }
            }
            UpdateheatmapTable();
            Console.WriteLine("   Updated heatmap using " + osmDistances.Count + " points");
        }

        internal static void doAnalysis(List<Coordinates> bingCoordinates, List<Coordinates> osmCoordinates)
        {
            // each pair of adjacent points in Bing are connected through a line and the minimum distance is calculated for each point on OSM
            GetDeflectionBasedOnLineSegments(bingCoordinates, osmCoordinates, false);
        }

        internal static void doGoogleAnalysis(List<Coordinates> googleCoordinates, List<Coordinates> osmCoordinates)
        {
            // each pair of adjacent points in Google are connected through a line and the minimum distance is calculated for each point on OSM
            GetDeflectionBasedOnLineSegments(googleCoordinates, osmCoordinates, true);
        }

        //for each osm point im checking the minimum distance to the bing curve to make sure it is less than a specified tolerance, using SQL
        private static void GetDeflectionBasedOnLineSegments(List<Coordinates> bingCoordinates, List<Coordinates> osmCoordinates, bool isGoogleAnalysis)
        {
            Dictionary<Coordinates, Double> osmDistancesMap = new Dictionary<Coordinates, Double>();
            int count = 0;
            foreach (Coordinates osmCoordinate in osmCoordinates)
            {
                count++;
                double dist1 = getLeastDistanceFromSetOfLinesUsingSql(osmCoordinate, bingCoordinates);

                //if its less than the threshold, we add it to osmDistance arraylist
                if (dist1 < Properties.Settings.Default.tolerance)
                {
                    osmDistancesMap.Add(osmCoordinate, dist1);
                }
                else
                {
                    //      System.out.println("deflection for " + count + "th element " + osmCoordinate.getLat() + ", " + osmCoordinate.getLat() + " with distance " + dist1 + " is skipped");
                }
            }
            if (!isGoogleAnalysis)
            {
                UpdateAvgDeflection(osmDistancesMap.Values);
                UpdateHeatmap(osmDistancesMap);
            } else {
                UpdateAvgDeflection(osmDistancesMap.Values);
                UpdateGoogleHeatmap(osmDistancesMap);
            }
        }

        private static void UpdateAvgDeflection(Dictionary<Coordinates, double>.ValueCollection osmDistances)
        {
            if (AreaDefMap.Count == 0)
            {
                AreaDefMap = IntializeAreaDef(MainClass.area);
            }

            foreach (Double dist in osmDistances)
            {
                int rangeUpper = (int)Math.Ceiling(dist / 5) * 5;
                if (AreaDefMap.ContainsKey(rangeUpper))
                {
                    AreaDeflection avgDef = AreaDefMap[rangeUpper];
                    Int32 noOfPoints = avgDef.getNoOfPoints();
                    Double avgDeflection = avgDef.getAvgDeflection();
                    avgDeflection = (avgDeflection * noOfPoints + dist) / (noOfPoints + 1);
                    avgDef.setAvgDeflection(avgDeflection);
                    avgDef.setNoOfPoints(noOfPoints + 1);
                    AreaDefMap[rangeUpper] = avgDef;
                }
                else
                {
                    AreaDeflection avgDef = new AreaDeflection(new Area(MainClass.area, null, null), rangeUpper, null, dist, 1);
                    AreaDefMap.Add(rangeUpper, avgDef);
                }
            }
            UpdateAvgDefTable();
            Console.WriteLine("   Updated average deflection using " + osmDistances.Count + " points");
        }

        private static void UpdateAvgDefTable()
        {
            String city = MainClass.area;
            String clearQuery = Properties.Settings.Default.ClearPrevAreaDefEntries;
            clearQuery = clearQuery.Replace("<CITY>", city);
            ConnectionUtils.ExecuteJdbcQuery(clearQuery);

            String insertQuery = "";
            List<String> queries = new List<String>();

            List<int> sortedKeys =
                    new List<int>(AreaDefMap.Keys);
            sortedKeys.Sort();

            // Display the TreeMap which is naturally sorted
            foreach (int x in sortedKeys)
            {
                AreaDeflection areaDeflection = AreaDefMap[x];
                insertQuery = Properties.Settings.Default.AreaDefInsertQuery;
                insertQuery = insertQuery.Replace("<CITY>", city);
                insertQuery = insertQuery.Replace("<STATE>", "");
                insertQuery = insertQuery.Replace("<COUNTRY>", "");
                insertQuery = insertQuery.Replace("<MAX_DEF_RANGE>", areaDeflection.getRangeUpper().ToString());
                insertQuery = insertQuery.Replace("<AVG_DEFLECTION>", areaDeflection.getAvgDeflection().ToString());
                insertQuery = insertQuery.Replace("<DATASET_PTS_COUNT>", areaDeflection.getNoOfPoints().ToString());
                queries.Add(insertQuery);
            }
            ConnectionUtils.ExecuteJdbcBatchQuery(queries);
        }

        private static Dictionary<int, AreaDeflection> IntializeAreaDef(String areaStr)
        {
            Dictionary<int, AreaDeflection> areaDefMap = new Dictionary<int, AreaDeflection>();
            Area area = new Area(areaStr);

            String query = Properties.Settings.Default.AreaDefQuery;
            if (area.getCity() != null)
            {
                query = query.Replace("<CITY>", area.getCity());
            }
            else
            {
                query = query.Replace("CITY = '<CITY>'", "");
            }
            if (area.getState() != null)
            {
                query = query.Replace("<STATE>", area.getState());
            }
            else
            {
                query = query.Replace(" and STATE = '<STATE>'", "");
            }
            if (area.getCountry() != null)
            {
                query = query.Replace("<COUNTRY>", area.getCountry());
            }
            else
            {
                query = query.Replace(" and COUNTRY = '<COUNTRY>'", "");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.jdbcUrl))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int maxDefRange = reader.GetInt32(reader.GetOrdinal("MAX_DEF_RANGE"));
                                areaDefMap.Add(maxDefRange, new AreaDeflection(new Area(reader.GetString(reader.GetOrdinal("CITY")), reader.GetString(reader.GetOrdinal("STATE")),
                                        reader.GetString(reader.GetOrdinal("COUNTRY"))), maxDefRange, null, (double)reader.GetDecimal(reader.GetOrdinal("AVG_DEFLECTION")), reader.GetInt32(reader.GetOrdinal("DATASET_PTS_COUNT"))));
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
            return areaDefMap;
        }

        private static void UpdateheatmapTable()
        {
            String city = MainClass.area;
            String clearQuery = Properties.Settings.Default.ClearPrevHeatmapEntries;
            clearQuery = clearQuery.Replace("<CITY>", city);
            ConnectionUtils.ExecuteJdbcQuery(clearQuery);
            List<String> queries = new List<String>();

            // Display the TreeMap which is naturally sorted
            foreach (KeyValuePair<Coordinates, AreaDeflection> x
                in new HashSet<KeyValuePair<Coordinates, AreaDeflection>>(AreaHeatmap))
            {
                AreaDeflection areaDeflection = x.Value;
                string insertQuery = Properties.Settings.Default.HeatMapInsertionQuery;
                insertQuery = insertQuery.Replace("<CITY>", city);
                insertQuery = insertQuery.Replace("<STATE>", "");
                insertQuery = insertQuery.Replace("<COUNTRY>", "");
                insertQuery = insertQuery.Replace("<LAT_MAX>", x.Key.getLat().ToString());
                insertQuery = insertQuery.Replace("<LONG_MAX>", x.Key.getLon().ToString());
                insertQuery = insertQuery.Replace("<DATASET_PTS_COUNT>", areaDeflection.getNoOfPoints().ToString());
                queries.Add(insertQuery);
            }
            ConnectionUtils.ExecuteJdbcBatchQuery(queries);
        }

        private static void UpdateGoogleHeatmapTable()
        {
            String city = MainClass.area;
            String clearQuery = Properties.Settings.Default.ClearPrevGoogleHeatmap;
            clearQuery = clearQuery.Replace("<CITY>", city);
            ConnectionUtils.ExecuteJdbcQuery(clearQuery);
            List<String> queries = new List<String>();

            // Display the TreeMap which is naturally sorted
            foreach (KeyValuePair<Coordinates, AreaDeflection> x
                in new HashSet<KeyValuePair<Coordinates, AreaDeflection>>(AreaHeatmap))
            {
                AreaDeflection areaDeflection = x.Value;
                string insertQuery = Properties.Settings.Default.GoogleHeatMapInsertionQuery;
                insertQuery = insertQuery.Replace("<CITY>", city);
                insertQuery = insertQuery.Replace("<STATE>", "");
                insertQuery = insertQuery.Replace("<COUNTRY>", "");
                insertQuery = insertQuery.Replace("<LAT_MAX>", x.Key.getLat().ToString());
                insertQuery = insertQuery.Replace("<LONG_MAX>", x.Key.getLon().ToString());
                insertQuery = insertQuery.Replace("<DATASET_PTS_COUNT>", areaDeflection.getNoOfPoints().ToString());
                queries.Add(insertQuery);
            }
            ConnectionUtils.ExecuteJdbcBatchQuery(queries);
        }

        private static Dictionary<Coordinates, AreaDeflection> IntializeAreaHeatmap(String areaStr)
        {
            Dictionary<Coordinates, AreaDeflection> AreaHeatmap =
                new Dictionary<Coordinates, AreaDeflection>();
            Area area = new Area(areaStr);
            String query = Properties.Settings.Default.GoogleHeatmapSelectQuery;
            if (area.getCity() != null)
            {
                query = query.Replace("<CITY>", area.getCity());
            }
            else
            {
                query = query.Replace("CITY = '<CITY>'", "");
            }
            if (area.getState() != null)
            {
                query = query.Replace("<STATE>", area.getState());
            }
            else
            {
                query = query.Replace(" and STATE = '<STATE>'", "");
            }
            if (area.getCountry() != null)
            {
                query = query.Replace("<COUNTRY>", area.getCountry());
            }
            else
            {
                query = query.Replace(" and COUNTRY = '<COUNTRY>'", "");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.jdbcUrl))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Coordinates cor = new Coordinates((double)reader.GetDecimal(reader.GetOrdinal("LAT_MAX")), (double)reader.GetDecimal(reader.GetOrdinal("LONG_MAX")), 1);
                                AreaDeflection areaDef = new AreaDeflection(area, null, null, (double)reader.GetDecimal(reader.GetOrdinal("AVG_DEFLECTION")), reader.GetInt32(reader.GetOrdinal("DATASET_PTS_COUNT")));
                                AreaHeatmap.Add(cor, areaDef);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return AreaHeatmap;
        }
    }
}