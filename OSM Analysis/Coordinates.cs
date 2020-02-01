using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
    class Coordinates
    {
        private double lat;
        private double lon;
        private int routeID;
        public Coordinates(double[] coordinates, int routeId)
        {
            this.lat = coordinates[0];
            this.lon = coordinates[1];
            this.routeID = routeId;
        }
    }
}
