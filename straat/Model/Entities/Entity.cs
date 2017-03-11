using Microsoft.Xna.Framework;
using straat.View.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using straat.Model.Map;

namespace straat.Model.Entities
{
	public enum Allegiance
	{
		PLAYER,
		NEUTRAL,
		ENEMY,
		UNKNOWN,
	}

    public class Entity
    {
        public GraphicsComponent gc { get; private set; }
		public SelectableComponent sc { get; }

		public Vector2 worldPos { get { 
				return new Vector2(position.X, position.Y);
			} private set{
				position.X = value.X;
				position.Y = value.Y;
			}}

		public Vector3 position;

		public int id { get; private set;}
		private static int idCounter = 1234;

		float speed = 200.0f; // in m/s

		string faction;

		Map.Map map;
		public Order standingOrder;
		List<Site> movementPath;

		public Entity(Vector3 position)
        {
			this.position = position;

			id = idCounter;
			++idCounter;

			standingOrder = new Order(OrderType.NONE);
        } 

		public Entity( GraphicsComponent gc, SelectableComponent sc ,Vector3 position) : this(position)
		{
			this.gc = gc;
			this.sc = sc;
		}

		public Allegiance getAllegiance()
		{
			if( faction == "player" )
				return Allegiance.PLAYER;
			else
				return Allegiance.ENEMY;
		}

		public void update(double deltaT)
		{
			// handle state

			// handle orders
			switch (standingOrder.type) 
			{
			case OrderType.MOVE:
				//do something
				Vector2 goal = (Vector2)standingOrder.data[0];
				Vector2 direction = goal - worldPos;
				if(direction.Length() < speed * (float)deltaT)
				{
					worldPos = goal;
					standingOrder = new Order(OrderType.NONE);
				} else {
					direction.Normalize();
					worldPos += direction * speed * (float)deltaT;
				}
				// TODO: pathfinding
				//if(movementPath == null)
				//	movementPath = new Pathfinder().findPath(map.getRegionAt(worldPos), map.getRegionAt(goal));

				break;

			case OrderType.NONE:
			default:
				break;
			}
		}
    }
}
