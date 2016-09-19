using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace straat
{
	public class Crossing : MapNode
	{

		public List<Street> roads;

		public Crossing(Vector2 pos) : base(pos)
		{
			roads = new List<Street>();
		}
	}
}

