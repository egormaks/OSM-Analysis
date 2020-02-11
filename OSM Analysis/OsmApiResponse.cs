using System;
using System.Collections.Generic;

namespace OSM_Analysis
{
    public class Lane
    {
        public bool valid { get; set; }
        public List<string> indications { get; set; }
    }

    public class Intersection
    {
        public int @out { get; set; }
        public List<bool> entry { get; set; }
        public List<int> bearings { get; set; }
        public List<double> location { get; set; }
        public int? @in { get; set; }
        public List<string> classes { get; set; }
        public List<Lane> lanes { get; set; }
    }

    public class Maneuver
    {
        public int bearing_after { get; set; }
        public int bearing_before { get; set; }
        public List<double> location { get; set; }
        public string type { get; set; }
        public string modifier { get; set; }
        public int? exit { get; set; }
    }

    public class Step
    {
        public List<Intersection> intersections { get; set; }
        public string driving_side { get; set; }
        public string geometry { get; set; }
        public string mode { get; set; }
        public Maneuver maneuver { get; set; }
        public double weight { get; set; }
        public double duration { get; set; }
        public string name { get; set; }
        public double distance { get; set; }
        public string @ref { get; set; }
        public string destinations { get; set; }
        public string exits { get; set; }
    }

    public class Leg
    {
        public string summary { get; set; }
        public double weight { get; set; }
        public double duration { get; set; }
        public List<Step> steps { get; set; }
        public double distance { get; set; }
    }

    public class Route
    {
        public List<Leg> legs { get; set; }
        public string weight_name { get; set; }
        public double weight { get; set; }
        public double duration { get; set; }
        public double distance { get; set; }
    }

    public class Waypoint
    {
        public string hint { get; set; }
        public double distance { get; set; }
        public string name { get; set; }
        public List<double> location { get; set; }
    }

    public class OsmApiResponse
    {
        public List<Route> routes { get; set; }
        public List<Waypoint> waypoints { get; set; }
        public string code { get; set; }

        internal List<Coordinates> getOsmCoordinates()
        {
            List<Coordinates> allCoordinates = new List<Coordinates>();
            List<Step> steps = routes[0].legs[0].steps;

            foreach (Step step in steps)
            {
                allCoordinates.Add(new Coordinates(step.maneuver.location.ToArray(), true, 1));
            }
            return allCoordinates;
        }
    }
}
