using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace straat
{
	public class AStarSolver
	{


		public AStarSolver()
		{
			// needed?
		}

		public List<Site> solve(List<Site> graph)
		{
			//
		}
	}

	public class AStarNode : Site
	{
		Vector2 goal;

		AStarNode previousNode;

		public AStarNode(Site position, Site goal) : this(position.position, goal) {}

		public AStarNode(Vector2 position, Site goal) : this(position, goal.position) {}

		public AStarNode(Vector2 position, Vector2 goal) : base()
		{
			base.position = position;
			this.goal = goal;
		}

		public float h()
		{
			return (goal - position).LengthSquared(); // todo: sqrt necessary?
		}

		public float g()
		{
			return previousNode.g() + cost();
		}

		public float cost()
		{
			// todo: get fram map somehow
			return 0.0f;
		}
	}

	public class Pathfinder
	{
		Map map;

		public Pathfinder(Map map)
		{
			this.map = map;
		}

		List<Site> findPath(Vector2 start, Vector2 goal)
		{
			//List<Site> graph = new List<Site>();
			// make first node
			//Site s = new Site();
			//s.position = start;
			//graph.Add(s);
			//graph.AddRange(map.corners.Values);
			//Site g = new Site();
			//g.position = goal;
			//graph.Add(g);

			List<AStarNode> openList = new List<AStarNode>();
			List<AStarNode> closedList = new List<AStarNode>();
			AStarNode curNode = new AStarNode(start,goal);
			while(true)
			{
				
			}

			List<Site> path = new List<Site>();
			return path;
		}
	}
}
