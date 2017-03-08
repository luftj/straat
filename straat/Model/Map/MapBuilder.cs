using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace straat.Model.Map
{
	public class MapBuilder
	{
		// doesn't really need to be a singleton, however...
//		private static MapBuilder instance;
//		public static MapBuilder Instance { get {
//				if(instance == null)
//					instance = new MapBuilder();
//				return instance;
//			}}

		public int seed{ get;}
		Random rng;

		#region params
		float dimensions;
		int numberOfRegions;
		public float maxElevation { get; private set;}
		private float desiredElevation; // todo: combine maxelevation and desiredelevation into one

		int numberOfRivers = 30;
		#endregion

		Map map;

		public MapBuilder(float dimensions,int numberOfRegions,float maxElevation, int? seed = null)
		{
			if( seed == null )
			{
				rng = new Random();
				this.seed = rng.Next();
			}
			else
			{
				this.seed = (int)seed;
			}
			rng = new Random( this.seed );
			BenTools.Mathematics.MathTools.R = new Random( this.seed );

			this.dimensions = dimensions;
			this.numberOfRegions = numberOfRegions;
			this.maxElevation = maxElevation;
			desiredElevation = maxElevation;
		}

		public BenTools.Mathematics.VoronoiGraph createVoronoiGraph()
		{
			int numPoints = numberOfRegions;
			double min = -dimensions/2;
			double max = dimensions/2;
			int it = numPoints;
			List<BenTools.Mathematics.Vector> points = new List<BenTools.Mathematics.Vector>();
			while(it>0)
			{
				BenTools.Mathematics.Vector v = new BenTools.Mathematics.Vector( 2 );
				v.Randomize(min,max);
				while( Math.Sqrt(v.SquaredLength) > ( dimensions/2 ) )	// makes it round
					v.Randomize( min, max );
				points.Add(v);
				--it;
			}

			BenTools.Mathematics.VoronoiGraph graph = BenTools.Mathematics.Fortune.ComputeVoronoiGraph(points);

			return graph;
		}

		public Map buildMapFromGraph(BenTools.Mathematics.VoronoiGraph voronoiGraph)
		{
			Map ret = new Map();

			foreach( BenTools.Mathematics.VoronoiEdge edge in voronoiGraph.Edges )
			{
				Corner cornerA = new Corner();
				cornerA.position = new Vector2( (float)edge.VVertexA[0], (float)edge.VVertexA[1] );
				Corner cornerB = new Corner();
				cornerB.position = new Vector2( (float)edge.VVertexB[0], (float)edge.VVertexB[1] );
				string hashA = cornerA.hashString();
				string hashB = cornerB.hashString();
				if( ret.corners.ContainsKey( hashA ) )
					cornerA = ret.corners[hashA];
				else
					ret.corners.Add( hashA, cornerA );
				
				if( ret.corners.ContainsKey( hashB ) )
					cornerB = ret.corners[hashB];
				else
					ret.corners.Add( hashB, cornerB );

				cornerA.addAdjacent( cornerB );
				cornerB.addAdjacent( cornerA );

				VDEdge vEdge = new VDEdge();

				foreach( VDEdge e in cornerA.protrudes )
					vEdge.continues.Add( e );
				foreach( VDEdge e in cornerB.protrudes )
					vEdge.continues.Add( e );

				vEdge.endpoints.Add( cornerA );
				vEdge.endpoints.Add( cornerB );

				Center centerA = new Center();
				centerA.position = new Vector2( (float)edge.LeftData[0], (float)edge.LeftData[1] );
				Center centerB = new Center();
				centerB.position = new Vector2( (float)edge.RightData[0], (float)edge.RightData[1] );

				string hashcA = centerA.hashString();
				string hashcB = centerB.hashString();
				if( ret.centers.ContainsKey( hashcA ) )
				{
					System.Console.WriteLine("Hash hit: "+ret.centers[hashcA].id+ret.centers[hashcA].position.ToString()+" | "+centerA.position.ToString()+" -- "+ret.centers[hashcA].Equals(centerA));
					centerA = ret.centers[hashcA];
				}
				else
					ret.centers.Add( hashcA, centerA );

				if( ret.centers.ContainsKey( hashcB ) )
				{
					System.Console.WriteLine("Hash hit: "+ret.centers[hashcB].id+ret.centers[hashcB].position.ToString()+" | "+centerB.position.ToString()+" -- "+ret.centers[hashcB].Equals(centerB));
					centerB = ret.centers[hashcB];
				}
				else
					ret.centers.Add( hashcB, centerB );


				centerA.addCorner( cornerA );
				centerA.addCorner( cornerB );
				centerB.addCorner( cornerA );
				centerB.addCorner( cornerB );

				//VDEdge dEdge = new VDEdge();
				//dEdge.endpoints.Add(centerA);
				//dEdge.endpoints.Add(centerB);

				//vEdge.dual = dEdge;
				//dEdge.dual = vEdge;

				cornerA.addTouching( centerA );
				cornerA.addTouching( centerB );
				cornerB.addTouching( centerA );
				cornerA.addTouching( centerA );

				cornerA.addProtruding( vEdge );
				cornerB.addProtruding( vEdge );


				vEdge.joins.Add( centerA );
				vEdge.joins.Add( centerB );


				centerA.addBordering( vEdge );
				centerB.addBordering( vEdge );

				centerA.addNeighbour( centerB );
				centerB.addNeighbour( centerA );

				// todo: delaunay missing

			}

			map = ret;
			return map;
		}

		public void applyElevation()
		{
			Vector2 start = Vector2.Zero;
			Center curCenter = map.getRegionAt(start.X,start.Y);
			int numberOfRidges = 246;
			while(numberOfRidges > 0)
			{
				float angle = (float)rng.NextDouble() * MathHelper.TwoPi - MathHelper.Pi;
				drawCenterRidge(curCenter,angle,maxElevation,0.1f);
				curCenter = curCenter.neighbours.ElementAt( rng.Next( curCenter.neighbours.Count ) );
				--numberOfRidges;
			}
		}

		public void applyCone(float max)
		{
			foreach(Center c in map.centers.Values)
			{
				float length = ( c.position.Length() );
				float factor = length/(dimensions/2);
				c.elevation = 1-(max * factor); // todo: respect maxElevation
			}
			maxElevation = max;
		}

		public void drawCenterRidge(Center start, float direction, float startElevation, float branchProbability, HashSet<string> bt = null)
		{
			// walk outwards

			List<Center> ridge = new List<Center>();
			HashSet<string> beenThere = bt;
			if( bt == null )
				beenThere = new HashSet<string>();
			Center curC = start;


			while(true)
			{
				ridge.Add(curC);
				beenThere.Add(curC.hashString());

				//Vector2 direction = ( curC.position - start.position );
				float angle = direction;// (float)Math.Atan2( direction.X, direction.Y );
				angle += (float)rng.NextDouble() * 2.0f - 1.0f;
				// find edge closest to direction
				float minangle = float.PositiveInfinity;
				Center minC = null;
				foreach(Center c in curC.neighbours)
				{
					if( c.isEndOfTheWorld )
						continue;

					Vector2 edge = c.position - curC.position;
					float cAngle = (float)Math.Atan2( edge.X, edge.Y );

					float anglediff = Math.Abs( angle - cAngle );

					if (anglediff < minangle)
					{
						minangle = anglediff;
						minC = c;
					}
				}
				if( minC != null && !beenThere.Contains( minC.hashString() ) )
					curC = minC;
				else
					break;
				
			}

			float curElevation = startElevation;

			if( ridge.Count == 0 )
				return;

			float elevationstep = startElevation / ridge.Count;
			foreach(Center ce in ridge)
			{
				ce.elevation = curElevation;
				curElevation -= elevationstep;

				// branch
				float x = (float)rng.NextDouble();
				if(x<branchProbability)
				{
					float curAngle = direction + (float)rng.NextDouble() * 2.0f - 1.0f;
					drawCenterRidge(ce,curElevation,curAngle,branchProbability,beenThere);
				}
			}
		}

		public void normaliseElevation()
		{
			foreach(Center c in map.centers.Values)
			{
				if(c.elevation < 0.0f)
				{
					c.elevation = 0.0f;
					continue;
				}

				c.elevation /= maxElevation;
			}
			maxElevation = 1.0f;
		}

		public void raiseElevation(float amount)
		{
			foreach( Center c in map.centers.Values )
				c.elevation += amount;

			maxElevation += amount;
		}
			
		public void scaleElevation()
		{
			foreach(Center c in map.centers.Values)
			{
				c.elevation *= desiredElevation;
			}
			maxElevation = desiredElevation;
		}

		public void smoothen()
		{
			float maxValue = float.NegativeInfinity;
			Dictionary<string,float> newElevations = new Dictionary<string, float>();
			foreach(Center c in map.centers.Values)
			{
				// simple filter
				float average = 0.0f;
				foreach(Center n in c.neighbours)
				{
					average += n.elevation;
				}
				average += c.elevation * c.neighbours.Count;
				average /= (float)(c.neighbours.Count+1);
				if( average > maxValue )
					maxValue = average;
				newElevations.Add(c.hashString(),average);
			}
			maxElevation = maxValue;
			foreach(KeyValuePair<string,float> item in newElevations)
			{
				map.centers[item.Key].elevation = item.Value;
			}
		}

		/// <summary>
		/// Smoothens the minima by raising them towards their neighbours average elevation.
		/// </summary>
		/// <param name="threshold">Threshold [0,1].</param>
		/// <param name="factor">Factor [0,1].</param>
		public void smoothenMinima(float threshold, float factor)
		{
			foreach(Center c in map.centers.Values)
			{
				float greatestDifference = float.PositiveInfinity;
				float meanElev = 0.0f;

				foreach(Center n in c.neighbours)
				{
					float currentDifference = c.elevation - n.elevation;
					if( currentDifference < greatestDifference )
						greatestDifference = currentDifference;
					meanElev += n.elevation;
				}
				meanElev /= c.neighbours.Count;

				if( Math.Abs(greatestDifference) > threshold )
				{
					c.elevation = meanElev * factor + c.elevation * ( 1.0f - factor );
				}
			}
		}

		/// <summary>
		/// Fills the local minima using the Planchon-Darboux algorithm. Assures every point will have a lower neighbour.
		/// </summary>
		public void fillMinima()
		{
			float minSlope = 0.01f;

			Dictionary<string,float> newElevations = new Dictionary<string, float>();

			foreach(Center c in map.centers.Values)
			{
				if( c.isEndOfTheWorld )
					newElevations.Add( c.hashString(), c.elevation );
				else
					newElevations.Add( c.hashString(), float.PositiveInfinity );
			}

			while(true)
			{
				bool change = false;
				foreach( Center c in map.centers.Values )
				{
					if( c.isEndOfTheWorld )
						continue;

					string index = c.hashString();
					float lowestNeighbour = float.PositiveInfinity;

					// find neighbouring center with lowest elevation
					foreach( Center n in c.neighbours )
					{
						if( newElevations[n.hashString()] < lowestNeighbour )
							lowestNeighbour = newElevations[n.hashString()];
					}
					// can't do anything here yet
					if( lowestNeighbour == float.PositiveInfinity )
						continue;

					float oldValue = newElevations[index];

					float newValue = Math.Max( lowestNeighbour + minSlope, c.elevation );

					if( newValue != oldValue )
					{
						newElevations[index] = newValue;
						change = true;
					}
				}
				if( !change )
					break;
			}

			foreach(KeyValuePair<string,float> item in newElevations)
			{
				map.centers[item.Key].elevation = item.Value;
			}
			
		}

		public void fixHoles()
		{
			foreach(Corner c in map.corners.Values)
			{
				if(c.touches.Count==2)
				{
					// try to fix
					HashSet<Center> newTouch = c.touches.ElementAt(0).neighbours;
					newTouch.Intersect(c.touches.ElementAt(1).neighbours);
					foreach(Center ce in newTouch)
					{
						if(ce.corners.Contains(c))
						{
							c.touches.Add( ce );
							break;
						}
					}
				}
			}
		}

		public void applyRivers()
		{
			int it = numberOfRivers;
			while(it > 0)
			{
				drawRiver();
				--it;
			}
		}

		private void drawRiver()
		{
			Center curC = map.centers.Values.ElementAt( rng.Next( map.centers.Count ) );
			while( curC.elevation < 0.5f * maxElevation)
				curC = map.centers.Values.ElementAt( rng.Next( map.centers.Count ) );
			River river = new River();

			while( true )
			{
				river.path.Add( curC );

				if( curC.isOcean )
					break;

				bool mounded = false;
				foreach( River r in map.rivers )
				{
					foreach( Center c in r.path )
						if( curC.Equals( c ) )
						{
							river.flowsInto = r;
							mounded = true;
							break;
						}
					if( mounded )
						break;
				}
				if( mounded )
					break;
				
				if( curC.elevation == curC.drain.elevation )
					curC.isLake = true;

				curC = curC.drain;
			}
			if( river.path.Count <= 1 )
				return;
			map.rivers.Add( river );
		}
	
		public void generateCities()
		{
			int numberOfCities = 2;

			while(numberOfCities > 0)
			{
				float x = rng.Next( -(int)dimensions / 2, (int)dimensions / 2 );
				float y = rng.Next( -(int)dimensions / 2, (int)dimensions / 2 );

				Center pos = map.getRegionAt( x, y );

				Settlement newSettlement = new Settlement( pos );


				map.settlements.Add(newSettlement);
				// connect to road network
				//connectToRoadNetwork(newSettlement);



				--numberOfCities;

				if(numberOfCities == 0)
					connectToRoadNetwork(newSettlement);
			}
		}

		/// <summary>
		/// Calculates the settlement interest function as a sum of all interest criteria.
		/// </summary>
		/// <returns>The settlement interest function at c in [-1,1] </returns>
		/// <param name="c">Region to consider</param>
		private double calculateSettlementInterestFxn(Center c)
		{
			double interest = 0.0f;

			// todo: criteria OOP -> delegates <3

			// criterion: slope


			// criterion: distance to water
			// rivers
			Center closest;
			map.findClosestRiver( c, out closest );
			double dist = Math.Abs( (c.position - closest.position).Length() );
			// ocean



			// criterion: distance to roads
			map.findClosestRoad( c, out closest );
			dist = Math.Abs( (c.position - closest.position).Length() );



			// return -1 if any one criterium is -1

			// else return sum over all interest criteria

			return 0.0f;
		}

		private void connectToRoadNetwork(Settlement curSett)
		{
			if( map.settlements.Count == 0 )
				return;

			// if no roads present, create one

			// else connect
			// todo

			// add road to Center of curSett

			// TESTING:
			if(map.settlements.Count > 1)
			{
				foreach(Settlement otherS in map.settlements)
				{
					if(curSett == otherS) continue;
					Road r = drawRoad(curSett.region,otherS.region);

					curSett.roads.Add(r);
					otherS.roads.Add(r);

					map.roads.Add(r);
				}
			}
		}

		private Road drawRoad(Center a, Center b)
		{
			var road = new Road();

			var pathf = new Pathfinder(Pathfinder.CostFxnType.ROAD);

			var path = pathf.findPath(a, b);
			foreach(Site s in path)
			{
				road.path.Add(s as Center);
			}

			return road;
		}

		private Settlement sampleSettlementSeed()
		{
			while(true)
			{
				// find a new spot
				Center c = map.centers.Values.ElementAt(rng.Next(map.centers.Count));

				// calculate interest fxn at this spot
				double I = calculateSettlementInterestFxn(c);

				// sample
				double X = rng.NextDouble();
				if( X > I )
					continue;	// don't use this spot

				// use this spot -> create seed
				return new Settlement(c);
			}
		}
	}
}

