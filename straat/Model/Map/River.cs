using System;
using System.Collections.Generic;

namespace straat.Model.Map
{
	public class River
	{
		public List<Center> path;
		public float width = 10.0f;

		public River flowsInto;

		//public string name { get;}

		public River()
		{
			path = new List<Center>();
		}
	}
}
