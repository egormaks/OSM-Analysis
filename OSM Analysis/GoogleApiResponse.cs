using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Analysis
{
	enum OutputFormats
	{
		json,
		xml
	}

	enum TransModesGM
	{
		driving,
		bicycling,
		transit
	}

	enum ToAvoidGM
	{
		tolls,
		highways,
		ferries,
		indoor
	}
	class GoogleApiResponse
	{
		private static readonly String API_KEY = "AIzaSyAioRxicWzq2ZrYzjjhejXp0QMW9FjdItA";
		private static readonly String HEADER = "https://maps.googleapis.com/maps/api/directions/";

		private TransModesGM myMode;
		private OutputFormats myFormat;
		//	private String myOrigin;
		private Coordinates myOriginCoordinates;
		//	private String myDest;
		private Coordinates myDestCoordinates;
		private List<ToAvoidGM> myAvoid;

		public String generateGRequest()
		{
			string url = "";

			url += HEADER;
			url += myFormat.ToString() + "?";
			//		sb.append("origin=" + myOrigin + "&");
			//		sb.append("destination=" + myDest);
			url += "origin=" + coordinateToString(myOriginCoordinates) + "&";
			url += "destination=" + coordinateToString(myDestCoordinates);
			if (myAvoid != null && !(myAvoid.Count() == 0))
			{
				url += "?";
				foreach (ToAvoidGM av in myAvoid)
				{
					url += av.ToString() + "|";
				}
				url = url.Substring(0, url.Length - 1);
			}
			if (myMode != null)
			{
				url += "?" + myMode.ToString();
			}
			url += "&key=" + API_KEY;
			return url;
		}

		private string coordinateToString(Coordinates c)
		{
			return c.lat + "," + c.lon;
		}

		public void setRouteCoords(List<Coordinates> theRoute)
		{
			myOriginCoordinates = theRoute[0];
			myDestCoordinates = theRoute[1];
		}

		public void setAvoid(List<ToAvoidGM> toAvoid)
		{
			this.myAvoid = toAvoid;
		}

		public void setOutputFormat(OutputFormats theFormat)
		{
			this.myFormat = theFormat;
		}

		private void setMode(TransModesGM theMode)
		{
			this.myMode = theMode;
		}

		private static String getJsonResponse(String theStr)
		{
			var client = new RestClient(theStr);
			var execute = client.Execute(new RestRequest());
			return execute.Content;
		}
	}
}
