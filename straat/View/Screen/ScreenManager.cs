using straat.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace straat.View.Screen
{
    public class ScreenManager
    {
		public Game1 game;

        //List<IScreen> screens; // reactivate for making screen state persistent?
        List<IScreen> activeScreens;

		List<IScreen> removeList;

		public Rectangle maxBounds;

		public ScreenManager(Game1 game, Rectangle maxBounds)
        {
			this.game = game;
			this.maxBounds = maxBounds;
            //screens = new List<IScreen>();
            activeScreens = new List<IScreen>();
			removeList = new List<IScreen>();
        }

        public void activateScreen(IScreen screen)
		{
			screen.Initialize();
			screen.LoadContent();

            activeScreens.Add(screen);
            //screen.Enter();
        }

        public void deactivateScreen(IScreen screen)
		{
			removeList.Add( screen );
		}

        public void Draw(double deltaT)
        {
            foreach (IScreen screen in activeScreens)
                screen.Draw(deltaT);
        }
        public void Update(double deltaT, Input input)
		{
			// update all active screens
			for( int i = 0; i < activeScreens.Count(); ++i )
			{
				activeScreens[i].Update( deltaT, input );
			}

			// remove all deactivated screens
			for( int i = 0; i < removeList.Count(); ++i )
			{
				activeScreens.Remove( removeList[i] );
			}

			removeList.Clear();
        }
    }
}
