using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace straat.Model.Map
{
	public class Settlement
	{
		public Center region;

		public List<Road> roads;	// todo: move to center?

		public Settlement(Center pos)
		{
			region = pos;

			roads = new List<Road>();
		}
	}
}

