using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace straat
{
	public class MapNode
	{
//		public Vector2 position{ get; protected set;}

		public Center region;

		//public List<Region> touchingRegions;

		public MapNode(Center pos)
		{
			region = pos;
		}
	}
}

