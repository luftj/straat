using Microsoft.Xna.Framework;
using straat.View.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.Model
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

		public Vector2 worldPos {get;}

		public int id { get; private set;}
		private static int idCounter = 1234;


		string faction;
        

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
    }
}
