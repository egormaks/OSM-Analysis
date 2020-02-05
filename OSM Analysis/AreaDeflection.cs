using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
    public class AreaDeflection
    {
        private Area area;
        private int rangeUpper;
        private int rangeLower;
        private double avgDeflection;
        private int noOfPoints;

        public AreaDeflection(Area area, int rangeUpper, int rangeLower, double avgDeflection, int noOfPoints)
        {
            this.area = area;
            this.rangeUpper = rangeUpper;
            this.rangeLower = rangeUpper - 5; // can change later
            this.avgDeflection = avgDeflection;
            this.noOfPoints = noOfPoints;
        }

        public Area getArea()
        {
            return area;
        }

        public void setArea(Area area)
        {
            this.area = area;
        }

        public int getRangeUpper()
        {
            return rangeUpper;
        }

        public void setRangeUpper(int rangeUpper)
        {
            this.rangeUpper = rangeUpper;
        }

        public int getRangeLower()
        {
            return rangeLower;
        }

        public void setRangeLower(int rangeLower)
        {
            this.rangeLower = rangeLower;
        }

        public double getAvgDeflection()
        {
            return avgDeflection;
        }

        public void setAvgDeflection(double avgDeflection)
        {
            this.avgDeflection = avgDeflection;
        }

        public int getNoOfPoints()
        {
            return noOfPoints;
        }

        public void setNoOfPoints(int noOfPoints)
        {
            this.noOfPoints = noOfPoints;
        }
    }
}
