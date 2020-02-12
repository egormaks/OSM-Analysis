using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
    class ConnectionUtils
    {
        public static double ExecuteJdbcSingleOutputQuery(String query)
        {
            Double toReturn = 0d;
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
                                toReturn = (Double)reader["VALUE"];
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong with the database connection/query; " + ex);
            }
            return toReturn;
        }

        internal static void ExecuteJdbcQueryUsingCon(string query, SqlConnection connection)
        {
            SqlCommand myCommand = new SqlCommand(query, connection);
            myCommand.ExecuteNonQuery();
        }

        internal static void ExecuteJdbcQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.jdbcUrl))
            {
                connection.Open();
                SqlCommand myCommand = new SqlCommand(query, connection);
                myCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        internal static void ExecuteJdbcBatchQuery(List<string> Queries)
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.jdbcUrl))
            {
                connection.Open();
                foreach (String Query in Queries)
                {
                    SqlCommand myCommand = new SqlCommand(Query, connection);
                    myCommand.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
