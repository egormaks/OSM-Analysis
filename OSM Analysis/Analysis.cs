using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OSM_Analysis
{
    class Analysis
    {
        private static Dictionary<Coordinates, AreaDeflection> areaHeatmap = 
            new Dictionary<Coordinates, AreaDeflection>();
        private static string conStr = "Data Source=DESKTOP-V8S66SP;Initial Catalog=BingMaps;Integrated Security=True";


        private static double getLeastDistanceFromSetOfLinesUsingSql(Coordinates osmCoordinate, List<Coordinates> bingCoordinates)
        {
             string selectTest = "DECLARE @g geography; SET @g = geography::STGeomFromText('LINESTRING(<CORDINATE_STRING>)', 4326); " +
               "DECLARE @source geography = 'POINT(<OSM_POINT>)';  SELECT @source.STDistance(@g) as DISTANCE;";

             String bingString = "";

            foreach(Coordinates cor in bingCoordinates)
            {
                bingString += cor.getLon() + " " + cor.getLat() + ",";
            }
               
            
            bingString = bingString.TrimEnd(','); // remove last comma

            selectTest = selectTest.Replace("<CORDINATE_STRING>", bingString);
            selectTest = selectTest.Replace("<OSM_POINT>", osmCoordinate.getLon() + " " + osmCoordinate.getLat());

            try
            {
                //We conneect to the DB using the connection string
                SqlConnection conn = new SqlConnection(conStr);
                              
                SqlCommand cmd = new SqlCommand(selectTest, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                double min = 0;
                
                //Loop through every row of the querry
                foreach (DataRow dr in dt.Rows)
                {
                   min = double.Parse(dr["DISTANCE"].ToString());
                }

                return min;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong with the database connection/querry");
                return -1;

            }
        }

        private static void updateHeatmap(Dictionary<Coordinates, double> osmDistances)
        {
            /*
            if (osmDistances.Count == 0)
            {
                areaHeatmap = intializeAreaHeatmap(Main.area);
            }

            foreach (KeyValuePair<Coordinates, double> dist 
                in new HashSet<KeyValuePair<Coordinates, double>>(osmDistances))
            {
                double lat = dist.Key.getLat();
                double lon = dist.Key.getLon();

                lat = Math.Ceiling(lat / 0.05) * 0.05;
                lon = Math.Ceiling(lon / 0.05) * 0.05;
                double[] coord = { lat, lon };
                Coordinates cor = new Coordinates(coord, 1);
                if (areaHeatmap.ContainsKey(cor))
                {
                    AreaDeflection avgDef = areaHeatmap[cor];
                    int noOfPoints = avgDef.getNoOfPoints();
                    double avgDeflection = avgDef.getAvgDeflection();
                    avgDeflection = (avgDeflection * noOfPoints + dist.Value) / (noOfPoints + 1);
                    avgDef.setAvgDeflection(avgDeflection);
                    avgDef.setNoOfPoints(noOfPoints + 1);
                    areaHeatmap.Add(cor, avgDef);
                }
                else
                {
                    // AreaDeflection avgDef = new AreaDeflection(new Area(Main.area, null, null), null, null, dist.getValue(), 1);
                    // areaHeatmap.Add(cor, avgDef);
                }
            }
            Console.WriteLine("   Updated heatmap using " + osmDistances.Count + " points");
            updateheatmapTable();
            */
        }

        private static void updateheatmapTable()
        {
            /*
            String city = Main.area;
            String clearQuery = PropertiesReader.getString("clearPrevHeatmapEntries");
            clearQuery = clearQuery.replace("<CITY>", city);
            ConnectionUtils.executeJdbcQuery(clearQuery);

            String insertQuery = "";
            List<String> queries = new List<String>();

            // Display the TreeMap which is naturally sorted
            foreach (KeyValuePair<Coordinates, AreaDeflection> x
                in new HashSet<KeyValuePair<Coordinates, AreaDeflection>>(areaHeatmap))
            {
                AreaDeflection areaDeflection = x.Value;
                insertQuery = PropertiesReader.getString("heatMapInsertionQuery");
                insertQuery = insertQuery.replace("<CITY>", city);
                insertQuery = insertQuery.replace("<STATE>", "");
                insertQuery = insertQuery.replace("<COUNTRY>", "");
                insertQuery = insertQuery.replace("<LAT_MAX>", x.Key.getLat().ToString());
                insertQuery = insertQuery.replace("<LONG_MAX>", x.Key.getLon().ToString());
                insertQuery = insertQuery.replace("<DATASET_PTS_COUNT>", areaDeflection.getNoOfPoints().ToString());
                queries.add(insertQuery);
            }
            ConnectionUtils.executeJdbcBatchQuery(queries);
            */
        }

        private static Dictionary<Coordinates, AreaDeflection> intializeAreaHeatmap(String areaStr)
        {
            /*
            Dictionary<Coordinates, AreaDeflection> areaHeatmap = 
                new Dictionary<Coordinates, AreaDeflection>();
            Area area = new Area(areaStr);
            Connection con = ConnectionUtils.getConnection();
            String query = PropertiesReader.getString("heatmapSelectQuery");
            if (area.getCity() != null)
            {
                query = query.replace("<CITY>", area.getCity());
            }
            else
            {
                query = query.replace("CITY = '<CITY>'", "");
            }
            if (area.getState() != null)
            {
                query = query.replace("<STATE>", area.getState());
            }
            else
            {
                query = query.replace(" and STATE = '<STATE>'", "");
            }
            if (area.getCountry() != null)
            {
                query = query.replace("<COUNTRY>", area.getCountry());
            }
            else
            {
                query = query.replace(" and COUNTRY = '<COUNTRY>'", "");
            }

            PreparedStatement ps = null;
            try
            {
                ps = con.prepareStatement(query);
                ResultSet rs = ps.executeQuery();
                while (rs.next())
                {
                    double[] coords = { rs.getDouble("LAT_MAX"), rs.getDouble("LONG_MAX") };
                    Coordinates cor = new Coordinates(coords, 1);
                    AreaDeflection areaDef = new AreaDeflection(area, null, null, rs.getDouble("AVG_DEFLECTION"), rs.getInt("DATASET_PTS_COUNT"));
                    areaHeatmap.Add(cor, areaDef);
                }
                rs.close();
                con.close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return areaHeatmap;
            */
            return null;

        }
    }
}
