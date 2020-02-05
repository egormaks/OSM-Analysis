using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
    public class Area
    {
        private String city;
        private String state;
        private String country;

        public Area(String city, String state, String country)
        {
            this.city = city;
            this.state = state;
            this.country = country;
        }

        public Area(String city)
        {
            this.city = city;
        }

        public String getCity()
        {
            return city;
        }

        public void setCity(String city)
        {
            this.city = city;
        }

        public String getState()
        {
            return state;
        }

        public void setState(String state)
        {
            this.state = state;
        }

        public String getCountry()
        {
            return country;
        }

        public void setCountry(String country)
        {
            this.country = country;
        }
    }
}
