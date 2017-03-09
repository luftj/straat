using System;
using System.Collections.Generic;

namespace straat.Model.Map
{
	public class Road
	{
		public List<Center> path;

		public float width = 5.0f;

		public Road()
		{
			path = new List<Center>();
		}
	}
}
