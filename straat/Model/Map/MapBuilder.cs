using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace straat
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

		Random rng;

		#region params
		float dimensions;
		int numberOfRegions;
		public float maxElevation { get; private set;}
		#endregion

		public MapBuilder(float dimensions,int numberOfRegions,float maxElevation)
		{
			rng = new Random();

			this.dimensions = dimensions;
			this.numberOfRegions = numberOfRegions;
			this.maxElevation = maxElevation;
		}

		public List<Vector2> createRandomPoints(int numberOfPoints, int radius)
		{
			List<Vector2> ret = new List<Vector2>();
			int i = numberOfPoints;
			while(i>0)
			{
				float x = rng.Next( radius ) - radius / 2.0f;
				float y = rng.Next( radius ) - radius / 2.0f;
				ret.Add( new Vector2( x, y ) );
				--i;
			}
			return ret;
		}

		public List<Crossing> createRandomRoadNetwork(List<Vector2> points)
		{
			float p = 0.05f;

			List<Crossing> nodes = new List<Crossing>();

			foreach(Vector2 point in points)
			{
				Crossing curNode = new Crossing( point );

				foreach(Crossing node in nodes)
				{
					float xdistance = curNode.position.X - node.position.X;
					//xdistance *= xdistance;
					float ydistance = curNode.position.Y - node.position.Y;
					//ydistance *= ydistance;

					//p = 1.0f / (( Math.Abs(xdistance) + Math.Abs(ydistance )/10.0f));
					float X = (float)rng.NextDouble();
					if(X<p)
					{
						Street newStreet = new Street( curNode, node );
						curNode.roads.Add(newStreet);
						node.roads.Add(newStreet);
					}
				}
				nodes.Add(curNode);
			}

			return nodes;
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
				int hashA = cornerA.GetHashCode();
				int hashB = cornerB.GetHashCode();
				if( ret.corners.ContainsKey( hashA ) )
					cornerA = ret.corners[hashA];
				else
					ret.corners.Add( hashA, cornerA );
				
				if( ret.corners.ContainsKey( hashB ) )
					cornerB = ret.corners[hashB];
				else
					ret.corners.Add( hashB, cornerB );

				cornerA.adjacent.Add( cornerB );
				cornerB.adjacent.Add( cornerA );

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

				int hashcA = centerA.GetHashCode();
				int hashcB = centerB.GetHashCode();
				if( ret.centers.ContainsKey( hashcA ) )
					centerA = ret.centers[hashcA];
				else
					ret.centers.Add( hashcA, centerA );

				if( ret.centers.ContainsKey( hashcB ) )
					centerB = ret.centers[hashcB];
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

				cornerA.touches.Add( centerA );
				cornerA.touches.Add( centerB );
				cornerB.touches.Add( centerA );
				cornerA.touches.Add( centerA );

				cornerA.protrudes.Add( vEdge );
				cornerB.protrudes.Add( vEdge );


				vEdge.joins.Add( centerA );
				vEdge.joins.Add( centerB );


				centerA.borders.Add( vEdge );
				centerB.borders.Add( vEdge );

				centerA.neighbours.Add( centerB );
				centerB.neighbours.Add( centerA );

				// todo: delaunay missing

			}

			return ret;
		}

		public void applyElevation(Map map)
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

		public void drawCenterRidge(Center start, float direction, float startElevation, float branchProbability, HashSet<int> bt = null)
		{
			// walk outwards

			List<Center> ridge = new List<Center>();
			HashSet<int> beenThere = bt;
			if( bt == null )
				beenThere = new HashSet<int>();
			Center curC = start;


			while(true)
			{
				ridge.Add(curC);
				beenThere.Add(curC.GetHashCode());

				//Vector2 direction = ( curC.position - start.position );
				float angle = direction;// (float)Math.Atan2( direction.X, direction.Y );
				angle += (float)rng.NextDouble() * 2.0f - 1.0f;
				// find edge closest to direction
				float minangle = float.PositiveInfinity;
				Center minC = null;
				foreach(Center c in curC.neighbours)
				{
					Vector2 edge = c.position - curC.position;
					float cAngle = (float)Math.Atan2( edge.X, edge.Y );

					float anglediff = Math.Abs( angle - cAngle );

					if (anglediff < minangle)
					{
						minangle = anglediff;
						minC = c;
					}
				}
				if( !beenThere.Contains( minC.GetHashCode() ) )
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

		public void drawCornerRidge()
		{
			
		}


//		public void applyElevation(Map map)
//		{
//			float min = -dimensions/2;
//			float max = dimensions/2;
//			// draw ridges
//			int numberRidges = 30;
//			while(numberRidges >0)
//			{
//				// start somewhere
//				// todo: bias starting point towards the center
//				Corner curCorner = map.corners.Values.ElementAt(rng.Next(map.corners.Count));
//				if( curCorner.isOcean )
//					continue;
//
//				float maxheight = maxElevation;
//				float maxheightstep = 30.0f;
//
//				float distfromcenter = ( curCorner.position ).Length();
//				float leftheight = maxheight;
//
//
//				// random walk to the ocean
//				// todo: bias random walk direction to the ocean
//				while(true)
//				{
//					if( curCorner.elevation < maxheight - maxheightstep )
//					{
//						curCorner.elevation += (float)rng.NextDouble() * maxheightstep;
////						if( curCorner.elevation <= 0.0f )
////							curCorner.isOcean = true;
//					}
//					leftheight -= maxheightstep;
//					curCorner = curCorner.adjacent.ElementAt( rng.Next( curCorner.adjacent.Count ) );
//					if (curCorner.isOcean)
//						break;
//				}
//				--numberRidges;
//			}
//		}

//		public void applyElevation2(Map map)
//		{
//			Vector2 start = Vector2.Zero;
//			Center curCenter = map.getRegionAt(start.X,start.Y);
//			curCenter.elevation = 0.0f;
//
//			List<Center> todo = new List<Center>();
//
//			//float maxElev = float.NegativeInfinity;
//			float minElev = float.PositiveInfinity;
//
//			float prevElev;
//			do
//			{
//				prevElev = curCenter.elevation;
//				foreach( Center c in curCenter.neighbours )
//				{
//					if( c.elevation == float.PositiveInfinity )
//						todo.Add( c );
//				}
//
//				curCenter = todo[0];
//				curCenter.elevation = prevElev - (float)rng.NextDouble();
//
//				if( curCenter.elevation < minElev )
//					minElev = curCenter.elevation;
//
//				todo.Remove( curCenter );
//			} while( todo.Count > 0 );
//
//			foreach(Center ce in map.centers.Values)
//			{
//				ce.elevation += -(minElev);
//			}
//			maxElevation = -minElev;
//		}
//
//		public void applyRivers(Map map)
//		{
//			int numberOfRivers = 10;
//			while(numberOfRivers>0)
//			{
//				River curRiver = new River();
//				// start somewhere
//				Corner curCorner = map.corners.Values.ElementAt(rng.Next(map.corners.Count));
//				if( curCorner.elevation < maxElevation/3.0f )
//					continue;
//				curRiver.path.Add( curCorner );
//
//				bool riverAdded = false;
//				// random walk downhill
//				while(true)
//				{
//					// find steepest downslope
//					float maxSlope = float.NegativeInfinity;
//					Corner lowestAdjacent = null;
//					foreach(Corner c in curCorner.adjacent)
//					{
//						float curSlope = curCorner.elevation - c.elevation;
//						if( curSlope > maxSlope)
//						{
//							maxSlope = curSlope;
//							lowestAdjacent = c;
//						}
//					}
//					if( maxSlope <= 0.0f )
//					{
//						lowestAdjacent.elevation = curCorner.elevation-1.0f;
//					}
//					curCorner = lowestAdjacent;
//					curRiver.path.Add(curCorner);
//					if( curCorner.isOcean )
//					{
//						riverAdded = true;
//						break;
//					}
//				}
//				//if( riverAdded )
//					map.rivers.Add( curRiver );
////				else
////					continue;
//				--numberOfRivers;
//			}
//		}
	}
}

