namespace OSM_Analysis
{
    class OsmRequestMessage
    {
        private string header = "https://router.project-osrm.org/route/v1/driving/";
        private string tailer = "?overview=false&alternatives=true&steps=true&hints=;";
        private Coordinates from;
        private Coordinates to;

        public void setFrom(Coordinates from) { this.from = from; }
        public void setTo(Coordinates to) { this.to = to; }

        public string generateRequest()
        {
            string toReturn = header;
            toReturn = appendCoordinates(toReturn, from);
            toReturn += ";";
            toReturn = appendCoordinates(toReturn, to);
            toReturn += tailer;
            return toReturn;
        }

        private string appendCoordinates(string parent, Coordinates coordinates)
        {
            return parent + coordinates.lon + "," + coordinates.lat;
        }
    }
}
