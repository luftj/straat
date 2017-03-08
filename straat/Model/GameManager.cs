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

		double simSpeed = 1.0;
		double oldSimSpeed = 1.0;

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
			// handele simulation time
			double simDT = deltaT * simSpeed;

			// update world (towns, ...)

			// move entities
			foreach(Entity e in entities)
			{
				e.update(simDT);
			}

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

		public void pauseUnpauseGame()
		{
			if(simSpeed == 0.0)
				simSpeed = oldSimSpeed;
			else
			{
				oldSimSpeed = simSpeed;
				simSpeed = 0.0;
			}
		}
	}
}
