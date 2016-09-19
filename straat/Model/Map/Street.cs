using System;
using System.Collections.Generic;

namespace straat
{
	public class Street
	{
		public MapNode[] endpoints;

		public Street(MapNode a, MapNode b)
		{
			endpoints = new MapNode[2];
			endpoints[0] = a;
			endpoints[1] = b;
		}
	}
}

