using System;
using System.Collections.Generic;

namespace straat.Model.Map
{
	public class River
	{
		public List<Center> path;
		public float width;	// todo: needs different widths along its length

		public River flowsInto;

		//public string name { get;}

		public River()
		{
			path = new List<Center>();
			width = 5.0f;
		}
	}
}
