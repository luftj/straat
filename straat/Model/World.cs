using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using straat.View.Drawing;

namespace straat.Model
{
	class World
	{
		public List<Entity> entities;

		public List<Crossing> roadNetwork;

		public Map map;
		public MapBuilder mapBuilder;


		public World()
		{
			entities = new List<Entity>();

			//roadNetwork = MapBuilder.Instance.createRandomRoadNetwork( MapBuilder.Instance.createRandomPoints( 30, 500 ) );

			mapBuilder = new MapBuilder( 1024.0f, 2048, 256.0f );

			BenTools.Mathematics.VoronoiGraph voronoiGraph = mapBuilder.createVoronoiGraph();
			map = mapBuilder.buildMapFromGraph( voronoiGraph );

			mapBuilder.applyElevation(map);

			//mapBuilder.applyRivers(map);
		}

		/// <summary>
		/// Update the simulation by the specified time.
		/// </summary>
		/// <param name="deltaT">time elapsed since last tick.</param>
		public void Update(double deltaT)
		{
			//
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
