using Microsoft.Xna.Framework;
using straat.View.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		float speed;

		string faction;

		Order standingOrder;


        public Entity()
        {
            worldPos = Vector2.Zero;

			id = idCounter;
			++idCounter;
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
				direction.Normalize();
				worldPos += direction * speed;
				// TODO: pathfinding
				break;

			case OrderType.NONE:
			default:
				break;
			}
		}
    }
}
