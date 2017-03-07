using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using straat.Model.Entities;
using straat.View.Drawing;
using straat.Model.Map;

namespace straat.Model
{
	class GameManager
	{
		public List<Entity> entities;

		public Map.Map map;

		public int seed;


		public GameManager()
		{
			entities = new List<Entity>();
		}

		/// <summary>
		/// Update the simulation by the specified time.
		/// </summary>
		/// <param name="deltaT">time elapsed since last tick.</param>
		public void Update(double deltaT)
		{
			// update world (towns, ...)

			// move entities

			// handle world events

			// update ai

		}

		/// <summary>
		/// Gets all drawable entities.
		/// </summary>
		/// <returns>The drawable entities.</returns>
		public List<Entity> getDrawableEntities()
		{
			List<Entity> ret = new List<Entity>();
			foreach( Entity item in entities )
			{
				if( item.gc != null )
					ret.Add( item );
			}
			return ret;
		}

		/// <summary>
		/// Gets all selectable entities.
		/// </summary>
		/// <returns>The selectable entities.</returns>
		public List<Entity> getSelectableEntities()
		{
			List<Entity> ret = new List<Entity>();
			foreach( Entity item in entities )
			{
				if( item.sc != null )
					ret.Add( item );
			}
			return ret;
		}
	}
}
