using System;

namespace OSM_Analysis
{
    public class Coordinates
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public int? RouteID { get; set; }
        public Coordinates(double[] coordinates, int? RouteID)
        {
            this.lat = coordinates[0];
            this.lon = coordinates[1];
            this.RouteID = RouteID;
        }

        public Coordinates(Double lat, Double lon, int? routeID)
        {
            this.lat = lat;
            this.lon = lon;
            this.RouteID = routeID;
        }


        public Coordinates(double[] coordinates, bool isInverted, int routeID)
        {
            if (isInverted)
            {
                this.lat = coordinates[1];
                this.lon = coordinates[0];
                this.RouteID = routeID;
            }
            else
            {
                new Coordinates(coordinates, routeID);
            }
        }

        public double getLat()
        {
            return this.lat;
        }

        public double getLon()
        {
            return this.lon;
        }

        public int? getrouteID()
        {
            return this.RouteID;
        }

        public String convertToString()
        {
            return this.getLat() + "," + this.getLon();
        }


        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Coordinates other = (Coordinates)obj;
            return other.getLat().Equals(other.getLat()) && other.getLon().Equals(other.getLon());
        }

    }
}
