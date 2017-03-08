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

		public Vector2 worldPos { get; private set;}

		public int id { get; private set;}
		private static int idCounter = 1234;

		float speed = 2.0f; // in m/s

		string faction;

		Map.Map map;
		public Order standingOrder;
		List<Site> movementPath;

        public Entity()
        {
            worldPos = Vector2.Zero;

			id = idCounter;
			++idCounter;

			standingOrder = new Order(OrderType.NONE);
        } 

		public Entity( GraphicsComponent gc, SelectableComponent sc ) : this()
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
