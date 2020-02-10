using System.Collections.Generic;

namespace OSM_Analysis
{
    public class OsmApiResponse
    {
        private List<Routes> routes = new List<Routes>();

         

        public void setRoutes(List<Routes> routes)
        {
            this.routes = routes;
        }

        public List<Coordinates> getOsmCoordinates()
        {
            List<Coordinates> allCoordinates = new List<Coordinates>();
            List<Steps> steps = routes[0].getLegs()[0].getSteps();
            //        allCoordinates.add();
            foreach (Steps s in steps)
            {
                allCoordinates.Add(new Coordinates(s.getManeuver().getLocation(), 1));
            }
            // steps.forEach(step->allCoordinates.add(new Coordinates(step.getManeuver().getLocation(), true, 1)));
            return allCoordinates;
        }
    }

    public class Routes
    {
        private List<Legs> legs = new List<Legs>();
        private string weight_name = "";
        private float weight;
        private float duration;
        private float distance;

        public float getDuration()
        {
            return duration;
        }

        public void setDuration(float duration)
        {
            this.duration = duration;
        }

        public float getDistance()
        {
            return distance;
        }

        public void setDistance(float distance)
        {
            this.distance = distance;
        }

        public List<Legs> getLegs()
        {
            return legs;
        }

        public void setLegs(List<Legs> legs)
        {
            this.legs = legs;
        }

        public string getWeight_name()
        {
            return weight_name;
        }

        public void setWeight_name(string weight_name)
        {
            this.weight_name = weight_name;
        }

        public float getWeight()
        {
            return weight;
        }

        public void setWeight(float weight)
        {
            this.weight = weight;
        }
    }

     public class Legs
    {
        private string summary = "";
        private double weight;
        private double duration;
        private List<Steps> steps = new List<Steps>();
        private float distance;

        public string getSummary()
        {
            return summary;
        }

        public void setSummary(string summary)
        {
            this.summary = summary;
        }

        public double getWeight()
        {
            return weight;
        }

        public void setWeight(double weight)
        {
            this.weight = weight;
        }

        public double getDuration()
        {
            return duration;
        }

        public void setDuration(double duration)
        {
            this.duration = duration;
        }

        public List<Steps> getSteps()
        {
            return steps;
        }

        public void setSteps(List<Steps> steps)
        {
            this.steps = steps;
        }

        public float getDistance()
        {
            return distance;
        }

        public void setDistance(float distance)
        {
            this.distance = distance;
        }
    }

    public class Steps
    {
        private List<Intersections> intersections = new List<Intersections>();
        private string driving_side;
        private string mode;
        private Maneuver maneuver;

        public List<Intersections> getIntersections()
        {
            return intersections;
        }

        public void setIntersections(List<Intersections> intersections)
        {
            this.intersections = intersections;
        }

        public string getDriving_side()
        {
            return driving_side;
        }

        public void setDriving_side(string driving_side)
        {
            this.driving_side = driving_side;
        }

        public string getMode()
        {
            return mode;
        }

        public void setMode(string mode)
        {
            this.mode = mode;
        }

        public Maneuver getManeuver()
        {
            return maneuver;
        }

        public void setManeuver(Maneuver maneuver)
        {
            this.maneuver = maneuver;
        }
    }

    public class Intersections
    {
        private double[] location = new double[2];
    }

    public class Maneuver
    {
        private double[] location = new double[2];

        public double[] getLocation()
        {
            return location;
        }
    }
}
