using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
    class Coordinates
    {
        public double lat { get; set; }
        public  double lon { get; set; }
        public  int routeID { get; set; }
        public Coordinates(double[] coordinates, int routeId)
        {
            this.lat = coordinates[0];
            this.lon = coordinates[1];
            this.routeID = routeId;
        }

        public double getLat()
        {
            return this.lat;
        }

        public double getLon()
        {
            return this.lon;
        }
        
        public int getrouteID()
        {
            return this.routeID;
        }
    }
}
