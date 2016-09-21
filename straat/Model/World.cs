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

			mapBuilder = new MapBuilder( 2048.0f, 4096, 256.0f  , 4231337 );

			BenTools.Mathematics.VoronoiGraph voronoiGraph = mapBuilder.createVoronoiGraph();
			map = mapBuilder.buildMapFromGraph( voronoiGraph );

			mapBuilder.fixHoles();

			mapBuilder.applyElevation();
			mapBuilder.normaliseElevation();
//			mapBuilder.raiseElevation( -0.2f );
//			mapBuilder.normaliseElevation();

			mapBuilder.fillMinima();
//						mapBuilder.smoothenMinima( 0.5f, 1.0f );
//						mapBuilder.smoothenMinima( 0.4f, 0.6f );
//						mapBuilder.smoothenMinima( 0.1f, 0.3f );

//			mapBuilder.raiseElevation( -0.10f );
//			mapBuilder.normaliseElevation();

			mapBuilder.applyRivers();
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
