using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace straat.Model.Map
{
	public class Crossing
	{
		public Center region;

		public List<Road> roads;

		public Crossing(Center pos)
		{
			region = pos;
			roads = new List<Road>();
		}
	}
}

