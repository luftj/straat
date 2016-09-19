using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace straat
{
	public class MapNode
	{
		public Vector2 position{ get; protected set;}

		//public List<Region> touchingRegions;

		public MapNode(Vector2 pos)
		{
			position = pos;
		}
	}
}

