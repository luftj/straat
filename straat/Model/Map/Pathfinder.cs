using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace straat.Model.Map
{
	public class Pathfinder
	{
		public enum CostFxnType
		{
			ROAD,
			TRAVEL,
		}

		public class AStarNode
		{
			public Site site { get; }   // has to be site to typecast to center/corner
			public Site goal { get; }

			public float h { get { return euclidHeur(goal.position);} }

			public delegate float CostFxn(AStarNode a, AStarNode b);
			public static CostFxn c; // todo: really static? might have multiple pathfinders?

			private float cost_Roads(AStarNode a, AStarNode b)
			{
				if(!(a.site is Center && b.site is Center))
					throw new NotImplementedException("ROAD pathfinding so far only works on region Centers");

				float factor;
				// map access for elevation
				// no roads with a slope above 45°
				float angle = Center.angle(a.site as Center, b.site as Center);
				//if(slope > 45.0f) return float.PositiveInfinity;
				if(angle <= 10.0f) factor = 1.0f;
				else factor = angle / 5.0f;  // MAGIC_NUMBER: think of a more useful factor
				// more altitude difference between a and b -> higher cost
				return factor * (b.site.position - a.site.position).Length(); // may not be squared (not associative)
			}

			private float cost_Travel(AStarNode a, AStarNode b)
			{
				if(!(a.site is Center && b.site is Center))
					throw new NotImplementedException("TRAVEL pathfinding so far only works on region Centers");

				float factor = 1.0f;
				if((a.site as Center).roads.Union((b.site as Center).roads).Count() > 0)
				{
					factor = 0.3f;
				} //else // todo: handle terrain types

				// terrain factor * euclidian distance
				return factor * (b.site.position - a.site.position).Length(); // may not be squared (not associative)
			}


			public AStarNode previousNode;

			public AStarNode(Site position, Site goal, CostFxnType type = CostFxnType.TRAVEL)
			{
				site = position;
				this.goal = goal;

				switch(type)
				{
				case CostFxnType.ROAD:
					c = new CostFxn(cost_Roads);
					break;
				case CostFxnType.TRAVEL:
				default:
					c = new CostFxn(cost_Travel);
					break;
				}
			}

			float euclidHeur(Vector2 goal)
			{
				return (goal - site.position).Length(); // todo: heuristic applicable? may not overestimate cost!
			}

			public float g()
			{
				if(previousNode == null) return 0.0f;
				return previousNode.g() + cost();
			}

			float cost()
			{
				return c(previousNode, this);
			}

			public float f()
			{
				return g() + h;
			}

			public override bool Equals(object obj)
			{
				object o = (obj as AStarNode).site as Center;
				return (this.site as Center).Equals(o);
			}
		}

		CostFxnType type;

		List<AStarNode> openList;
		List<AStarNode> closedList;


		public Pathfinder(CostFxnType type = CostFxnType.TRAVEL)
		{
			openList = new List<AStarNode>();
			closedList = new List<AStarNode>();

			this.type = type;
		}


		public List<Site> findPath(Site start, Site goal)
		{
			AStarNode curNode = new AStarNode(start,goal);
			openList.Add(curNode);

			while(openList.Count != 0)
			{
				// get new node with min f
				float minF = float.PositiveInfinity;
				AStarNode minNode = null;
				for(int i = 0; i < openList.Count;++i)
				{
					if(openList[i].f ()<minF)
					{
						minF = openList[i].f();
						minNode = openList[i];
					}
				}
				if(minNode != null)
				{
					curNode = minNode;
				}
				openList.Remove(curNode);

				if(curNode.site == goal) 
					break;

				closedList.Add(curNode);
				expandNode(curNode);
			}

			return makePath(curNode);
		}

		void expandNode(AStarNode curNode)
		{
			// fill neighbour list
			foreach(Center c in (curNode.site as Center).neighbours)
			{
				AStarNode neighbour = new AStarNode(c, curNode.goal, type);
			//}

			//List<AStarNode> neighbours = new List<AStarNode>();

			//foreach(AStarNode neighbour in neighbours)
			//{
				if(closedList.Contains(neighbour)) continue;    // already handled node

				if(openList.Contains(neighbour))
				{
					float new_g = curNode.g() + AStarNode.c(curNode, neighbour);
					if(new_g >= neighbour.g()) continue;
				} 
				else
					openList.Add(neighbour);
				
				neighbour.previousNode = curNode;
			}
		}

		List<Site> makePath(AStarNode goalNode)
		{
			List<Site> path = new List<Site>();
			AStarNode curNode = goalNode;

			while(true)
			{
				path.Add(curNode.site);
				if(curNode.previousNode == null) break;
				curNode = curNode.previousNode;
			}

			return path;
		}
	}
}
