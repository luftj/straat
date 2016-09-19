using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace straat
{
	public class Center
	{
		public Vector2 position;

		// todo: use properties for redundant information, not seperate collections
		#region graph
		public HashSet<Corner> corners { get;}
		public HashSet<VDEdge> borders;// { get;}
		public HashSet<Center> neighbours;// { get;}

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

		public Center()
		{
			corners = new HashSet<Corner>();
			borders = new HashSet<VDEdge>();
			neighbours = new HashSet<Center>();
			polygon = new LinkedList<Corner>();
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
			
		public override int GetHashCode()
		{
			return position.GetHashCode();
		}
			
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

	public class Corner
	{
		public Vector2 position;

		#region graph
		public HashSet<Center> touches;// { get;}
		public HashSet<Corner> adjacent;// { get;}
		public HashSet<VDEdge> protrudes;// { get;}
		#endregion


		public float elevation //= 10.0f;
		{get{
				float ret = 0.0f;
				foreach(Center c in touches)
				{
					ret += c.elevation;
				}
				ret /= touches.Count;
				return ret;
			}}

		public bool isOcean{get{
				bool ret = float.IsInfinity( position.Length() );
				if( ret )
					return true;
				if( elevation <= 0.0f )
					return true;
					
				foreach(Center c in touches)
				{
					ret = Math.Abs( ( position - c.position ).Length() ) > 200.0f;
				}
				return ret;
			}}

		public Corner()
		{
			touches = new HashSet<Center>();
			adjacent = new HashSet<Corner>();
			protrudes = new HashSet<VDEdge>();
		}

		public override int GetHashCode()
		{
			return position.GetHashCode();
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
		public List<Corner> path;

		public River()
		{
			path = new List<Corner>();
		}
	}

	public class Map
	{
		public Dictionary<int,Center> centers;	// todo: hashset sufficient?
		public Dictionary<int,Corner> corners;	// dto

		public List<River> rivers;

		public Map()
		{
			centers = new Dictionary<int, Center>();
			corners = new Dictionary<int, Corner>();

			rivers = new List<River>();
		}

		public Center getRegionAt(float x, float y)
		{
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
	}
}

