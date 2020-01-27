using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
    public enum TravelMode
    {
        Driving, Walking, Transit
    }

    public enum ToAvoid
    {
        highways,
        tolls,
        ferry,
        minimizeHighways,
        minimizeTolls,
        borderCrossing
    }

    public enum Optimize
    {
        distance,
        time,
        timeWithTraffic,
        timeAvoidClosure
    }

    public class BingRequestMessage
    {
        private String header = "http://dev.virtualearth.net/REST/V1/Routes/";
        TravelMode travelMode = TravelMode.Driving;
        List<String> wayPoints = new List<String>();
        List<String> viaWayPoints = new List<String>();
        int heading;
        List<ToAvoid> avoid = new List<ToAvoid>();
        int distanceBeforeFirstTurn;
        Optimize optimize = Optimize.time;
        String bingMapsKey = "AqGyWa8wWbEbXaFePdL2ASXBXyWdLLgvoIHn4JDJWKSpS7xick_HlF3p0LBglxPl";

        public BingRequestMessage()
        {

        }

        public TravelMode GetTravelMode()
        {
            return travelMode;
        }

        public void SetTravelMode(TravelMode travelMode)
        {
            this.travelMode = travelMode;
        }

        public ICollection<String> GetWayPoints()
        {
            return wayPoints;
        }

        public void SetWayPoints(ICollection<String> wayPoints)
        {
            this.wayPoints = (List<String>)wayPoints;
        }

        public ICollection<String> GetViaWayPoints()
        {
            return viaWayPoints;
        }

        public void SetViaWayPoints(ICollection<String> viaWayPoints)
        {
            this.viaWayPoints = (List<String>)viaWayPoints;
        }

        public int GetHeading()
        {
            return heading;
        }

        public void SetHeading(int heading)
        {
            this.heading = heading;
        }

        public ICollection<ToAvoid> GetAvoid()
        {
            return avoid;
        }

        public void SetAvoid(ICollection<ToAvoid> avoid)
        {
            this.avoid = (List<ToAvoid>)avoid;
        }

        public int GetDistanceBeforeFirstTurn()
        {
            return distanceBeforeFirstTurn;
        }

        public void SetDistanceBeforeFirstTurn(int distanceBeforeFirstTurn)
        {
            this.distanceBeforeFirstTurn = distanceBeforeFirstTurn;
        }

        public Optimize GetOptimize()
        {
            return optimize;
        }

        public void SetOptimize(Optimize optimize)
        {
            this.optimize = optimize;
        }

        public String GenerateRequest()
        {
            String toReturn = header;
            toReturn = toReturn + travelMode.ToString() + "?";
            if (wayPoints != null && wayPoints.Count != 0)
            {
                int wpCount = 0;

                foreach (String wp in wayPoints)
                {
                    if (wpCount == 0)
                    {
                        toReturn = toReturn + "wp." + wpCount + "=" + wp;
                    }
                    else
                    {
                        toReturn = append(toReturn, "wp." + wpCount + "=" + wp);
                    }
                    wpCount++;
                }
            }

            if (avoid.Count != 0)
            {
                using (IEnumerator<ToAvoid> value = avoid.GetEnumerator())
                {
                    value.MoveNext();
                    toReturn = append(toReturn, "avoid=" + value.Current.ToString());
                    while (value.MoveNext())
                    {
                        toReturn = append(toReturn, "," + value.Current.ToString());
                    }
                }
            }

            toReturn = (distanceBeforeFirstTurn != null) ? append(toReturn, "dbft=" + distanceBeforeFirstTurn) : toReturn;
            toReturn = (heading != null) ? append(toReturn, "hd=" + heading) : toReturn;
            toReturn = append(toReturn, "optimize=" + optimize.ToString());
            toReturn = append(toReturn, "key=" + bingMapsKey);

            return toReturn;
        }

        public String append(String parent, String child)
        {
            return parent + "&" + child;
        }
    }
}
