using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
    class Main
    {
        private static readonly String from = "Seattle";
        private static readonly String to = "Tacoma";
        private static ArrayList bingCoordinates;
        private static ArrayList osmCoordinates;

        static void main()
        {
            // Process Bing request
            {
                String bingURL = getBingURL();
                String json = getJsonResponse(bingURL);
                // TODO: GSON equiv in C#?
                // get bing coords
            }


            // Process OSM request
            {
                String osmURL = getOsmURL();
                String json = getJsonResponse(osmURL);
                // TODO: GSON equiv in C#?
                // get osm coords
            }


            // TODO: Process Google API request
            {

            }
        }

        private static String getBingURL()
        {
            return null;
        }

        private static String getOsmURL()
        {
            return null;
        }

        private static String getJsonResponse(String theStr)
        {
            return null;
        }


    }
}
