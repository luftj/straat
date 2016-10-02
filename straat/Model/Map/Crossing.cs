using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace straat
{
	public class Crossing : MapNode
	{

		public List<Road> roads;

		public Crossing(Center pos) : base(pos)
		{
			roads = new List<Road>();
		}
	}
}

