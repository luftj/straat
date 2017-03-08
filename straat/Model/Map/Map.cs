﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace straat.Model.Map
{
	public class Site
	{
		public Vector2 position;
	}

	public class Center : Site
	{
		private static int idC = 0;
		public int id { get;}
//		public Vector2 position;

		// todo: use properties for redundant information, not seperate collections
		#region graph
		public HashSet<Corner> corners { get;}
		public HashSet<VDEdge> borders { get;}
		public HashSet<Center> neighbours { get;}

		public LinkedList<Corner> polygon { get;}

		#endregion

		public float elevation = 0; /*{
			get{
				if( corners.Count == 0 )
					return 0.0f;
				if( isOcean )
					return 0.0f;
				float ret = 0.0f;
				foreach( Corner c in corners )
					ret += c.elevation;
				return ret /= corners.Count;
			}
		}*/

		public bool isOcean {get{
				foreach(Corner c in corners)
				{
					if( c.isOcean )
						return true;
				}
				return false;
			}}

		public bool isLake = false;

		public bool isEndOfTheWorld{get{
				foreach( Corner c in corners )
					if( c.isEndOfTheWorld )
						return true;
				return false;
			}}

		public Center drain {get{
				if( isEndOfTheWorld )
					return null;
				float slope = float.PositiveInfinity;
				Center ret = null;
				foreach(Center n in neighbours)
				{
					float nSlope = n.elevation - elevation;
					if( nSlope <= slope )
					{
						slope = nSlope;
						ret = n;
					}	
				}
				return ret;
			}}

		public List<Road> roads;

		public Center()
		{
			id = idC;
			++idC;

			corners = new HashSet<Corner>();
			borders = new HashSet<VDEdge>();
			neighbours = new HashSet<Center>();
			polygon = new LinkedList<Corner>();

			roads = new List<Road>();
		}

		public void addCorner(Corner c)
		{
			if(corners.Add(c))
			{
				// not present yet, add into polygon in order
				if( polygon.Count == 0 )
				{
					polygon.AddFirst( c );
					return;
				}
				// iterate over polygon -- insertion sort
				LinkedListNode<Corner> it = polygon.First;
				while(true)
				{
					if(isLessThan(c.position,it.Value.position,this.position))	// clockwise (?)
					{
						polygon.AddBefore(it,c);
						break;
					}
					if( polygon.Last == it )
					{
						polygon.AddLast(c);
						break;
					}
					it = it.Next;
				}
			}
		}
			
		public void addBordering(VDEdge e)
		{
			if( !borders.Contains( e ) )
				borders.Add( e );
		}

		public void addNeighbour(Center c)
		{
			if( !neighbours.Contains( c ) )
				neighbours.Add( c );
		}

		/// <summary>
		/// Elevation-angle between the specified centers a and b.
		/// </summary>
		/// <returns>The angle in degrees.</returns>
		public static float angle(Center a, Center b)
		{
			float distance = Math.Abs((b.position - a.position).Length());
			float heightDiff = Math.Abs(b.elevation - a.elevation);
			float angle = (float)Math.Atan2(heightDiff, distance);
			return MathHelper.ToDegrees(angle);
		}


		/// <summary>
		///  don't use this! hashes not unique - use hashString() instead
		/// </summary>
		public override int GetHashCode()
		{
//			throw new NotSupportedException( "Don't use integer hashing, since it wont guarantee unique hashes for different positions!" );
			return position.GetHashCode();
		}

		public string hashString()
		{
			return position.ToString();
		}

		public override bool Equals(object obj)
		{
			Center o = obj as Center;
			return o?.position.X == position.X && o?.position.Y == position.Y;
		}
			

		/// <summary>
		/// Used for determining clockwise orientation between two points in relation to a "center" point.
		/// </summary>
		/// <returns><c>true</c>, if a is "more clockwise" than b <c>false</c> otherwise.</returns>
		/// <param name="reference">Reference or center point.</param>
		static bool isLessThan(Vector2 a, Vector2 b, Vector2 reference) 
		{
			if (Vector2.Equals(a,b))
				return false;
			if (a.X - reference.X >= 0 && b.X - reference.X < 0)
				return true;
			if (a.X - reference.X < 0 && b.X - reference.X >= 0)
				return false;
			if (a.X - reference.X == 0 && b.X - reference.X == 0)
			{
				if (a.Y - reference.Y >= 0 || b.Y - reference.Y >= 0)
					return a.Y > b.Y;
				return a.Y < b.Y;
			}

			// compute the cross product of vectors (center -> a) x (center -> b)
			float det = (a.X - reference.X) * (b.Y - reference.Y) - (b.X - reference.X) * (a.Y - reference.Y);
			if (det < 0)
				return true;
			if (det > 0)
				return false;

			// points a and b are on the same line from the center
			// check which point is closer to the center
			float d1 = (a.X - reference.X) * (a.X - reference.X) + (a.Y - reference.Y) * (a.Y - reference.Y);
			float d2 = (b.X - reference.X) * (b.X - reference.X) + (b.Y - reference.Y) * (b.Y - reference.Y);
			return d1 > d2;
		}
	}

	public class Corner : Site
	{
		private static int idC = 0;
		public int id { get;}

		#region graph
		public List<Center> touches;// { get;}
		public List<Corner> adjacent;// { get;}
		public List<VDEdge> protrudes;// { get;}
		#endregion


		public float elevation
		{get{
				float ret = 0.0f;
				foreach(Center c in touches)
				{
					ret += c.elevation;
				}
				ret /= touches.Count;
				return ret;
			}}

		public bool isOcean{ get{
				bool ret = isEndOfTheWorld;
				if( ret )
					return true;
				if( elevation <= 0.0f )
					return true;
				return ret;
			}}

		public bool isEndOfTheWorld { get {
				bool ret = float.IsInfinity( position.Length() );
				if( !ret )
					foreach( Center c in touches )
					{
						ret = Math.Abs( ( position - c.position ).Length() ) > 2000.0f;	// MAGIC_NUMBER: adjust cutoff to map dimension
					}
				return ret;
			}
		}

		public Vector3 surfaceNormal{get{
				if( touches.Count < 3 )
					return Vector3.UnitZ;

				Vector2 au = touches.ElementAt( 0 ).position;
				au.Normalize();
				Vector2 bu = touches.ElementAt( 1 ).position;
				bu.Normalize();
				Vector3 a = new Vector3( au, touches.ElementAt(0).elevation );
				Vector3 b = new Vector3( bu, touches.ElementAt(1).elevation );

				Vector3 N = Vector3.Cross( a, b );
				N.Normalize();

				// check wether N points up or down
				if( N.Z < 0.0f )
					N *= -1.0f;	// flip

				return N;
			}}

		public float angle {get{ return (float)Math.Acos( Vector3.Dot( surfaceNormal, Vector3.UnitZ ) );}}	// UnitZ correct?

		public float slope {get{return (float)Math.Tan(angle);}}

		public float aspect{get{ return (float)( Math.Atan2( surfaceNormal.Y, surfaceNormal.X ) ); }}

		public Corner()
		{
			id = idC;
			++idC;

			touches = new List<Center>();
			adjacent = new List<Corner>();
			protrudes = new List<VDEdge>();
		}


		public void addTouching(Center c)
		{
			if( !touches.Contains( c ) )
				touches.Add( c );
		}

		public void addAdjacent(Corner c)
		{
			if( !adjacent.Contains( c ) )
				adjacent.Add( c );
		}

		public void addProtruding(VDEdge e)
		{
			if( !protrudes.Contains( e ) )
				protrudes.Add( e );
		}


		/// <summary>
		///  don't use this! hashes not unique - use hashString() instead
		/// </summary>
		public override int GetHashCode()
		{
//			throw new NotSupportedException( "Don't use integer hashing, since it wont guarantee unique hashes for different positions!" );
			return position.GetHashCode();
		}
			
		public string hashString()
		{
			return position.ToString();
		}

		public override bool Equals(object obj)
		{
			Corner o = obj as Corner;
			return o?.position.X == position.X && o?.position.Y == position.Y;
		}

		public bool equals(Corner other, float threshhold)
		{
			if (position.X+threshhold < other.position.X) return false;
			if (position.X-threshhold > other.position.X) return false;
			if (position.Y+threshhold < other.position.Y) return false;
			if (position.Y-threshhold > other.position.Y) return false;
			return true;
		}
	}

	public class Edge
	{
		
	}

	public class VDEdge : Edge
	{
		//public VDEdge dual;

		public List<Corner> endpoints;

		#region graph
		public HashSet<Edge> continues;// {get{return ((HashSet<Edge>) endpoints[0].protrudes.UnionWith( endpoints[1].protrudes ) ).ExceptWith(this);}}
		public HashSet<Center> joins;
		#endregion

		public VDEdge()
		{
			endpoints = new List<Corner>(2);
			continues = new HashSet<Edge>();
			joins = new HashSet<Center>();
		}
	}



	public class River
	{
		public List<Center> path;

		public River flowsInto;

		//public string name { get;}

		public River()
		{
			path = new List<Center>();
		}
	}

	public class Map
	{
		public Dictionary<string,Center> centers;	// todo: hashset sufficient?
		public Dictionary<string,Corner> corners;	// dto

		public List<River> rivers;
		public List<Settlement> settlements;
		public List<Road> roads;

		public Map()
		{
			centers = new Dictionary<string, Center>();
			corners = new Dictionary<string, Corner>();

			rivers = new List<River>();
			settlements = new List<Settlement>();
			roads = new List<Road>();
		}

		public Center getRegionAt(Vector2 worldPos)
		{
			return getRegionAt(worldPos.X, worldPos.Y);
		}
			
		public Center getRegionAt(float x, float y)
		{
			// todo: use delaunay traversal instead
			float dist = float.PositiveInfinity;
			Center closest = null;
			Vector2 query = new Vector2( x, y );
			foreach( Center c in centers.Values )
			{
				float curDist = (c.position - query).Length();
				if( curDist < dist )
				{
					dist = curDist;
					closest = c;
				}
			}
			return closest;
		}

		public LinkedList<Center> getCentersSortedByElevation()
		{
			LinkedList<Center> ret = new LinkedList<Center>();
			foreach(Center c in centers.Values)
			{
				if( ret.Count == 0 )
					ret.AddLast( c );
				else
				{
					// find position
					LinkedListNode<Center> cur = ret.First;
					while( true )
					{
						if( c.elevation < cur.Value.elevation )
							ret.AddBefore( cur, c );
						else if(cur == ret.Last)
						{
							ret.AddLast(c);
							break;
						}
						else
							cur = cur.Next;
					}
				}
			}
			return ret;
		}

		public Road findClosestRoad(Center c, out Center closest)
		{
			float minDist = float.PositiveInfinity;
			closest = null;
			Road closestRoad = null;

			foreach(Road r in roads)
			{
				foreach(Center p in r.path)
				{
					if( c.hashString() == p.hashString() )
						return r;
					float curDist = (float)Math.Abs((c.position - p.position).Length());
					if(curDist< minDist)
					{
						minDist = curDist;
						closest = p;
						closestRoad = r;
					}
				}
			}
			return closestRoad;
		}

		public River findClosestRiver(Center c, out Center closest)
		{
			float minDist = float.PositiveInfinity;
			closest = null;
			River closestRiver = null;

			foreach(River r in rivers)
			{
				foreach(Center p in r.path)
				{
					if( c.hashString() == p.hashString() )
						return r;
					float curDist = (float)Math.Abs((c.position - p.position).Length());
					if(curDist< minDist)
					{
						minDist = curDist;
						closest = p;
						closestRiver = r;
					}
				}
			}
			return closestRiver;
		}
	}
}

