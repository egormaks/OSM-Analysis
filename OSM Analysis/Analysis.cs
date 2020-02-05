using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OSM_Analysis
{
    class Analysis
    {

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
    }
}
