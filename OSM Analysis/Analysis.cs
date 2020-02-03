using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
    class Analysis
    {

        private static string conStr = "Data Source=DESKTOP-V8S66SP;Initial Catalog=BingMaps;Integrated Security=True";

        private static string selectTest = "DECLARE @g geography; SET @g = geography::STGeomFromText('LINESTRING(<CORDINATE_STRING>)', 4326); " +
  "DECLARE @source geography = 'POINT(<OSM_POINT>)';  SELECT @source.STDistance(@g) as DISTANCE;";


            private static double getLeastDistanceFromSetOfLinesUsingSql(Coordinates osmCoordinate, List<Coordinates> bingCoordinates)
        {
            try
            {
                //We conneect to the DB using the connection string
                SqlConnection conn = new SqlConnection(conStr);

                //This querry retrieves for every arc on every day: the slowdown % on every interval and its FreeFlow Speed 
                SqlCommand cmd = new SqlCommand(selectTest, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

        

                //Loop through every row of the querry
                foreach (DataRow dr in dt.Rows)
                {
                    //here we can calculate the speed of every arch depending on the day and the interval.
                    // if we want for example to retrieve the arcID as an int from each row, we type: int.Parse(dr["rnArcId"].ToString())
                    //averageSpeed += (double.Parse(dr["FreeFlowSpeed"].ToString()) * ((double.Parse(dr["Percentage"].ToString()) / 100))) / dt.Rows.Count;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong with the database connection/querry");
                return 0;

            }
        }
    }
}
