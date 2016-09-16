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


		private List<ScreenMessage> messageQueue;
		private List<ScreenMessage> prevMessageQueue;


		public ScreenManager(Game1 game, Rectangle maxBounds)
        {
			this.game = game;
			this.maxBounds = maxBounds;
            //screens = new List<IScreen>();
            activeScreens = new List<IScreen>();
			removeList = new List<IScreen>();

			messageQueue = new List<ScreenMessage>();
			prevMessageQueue = new List<ScreenMessage>();
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
			messageQueue.Clear();

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

			prevMessageQueue = messageQueue;
        }

		public List<ScreenMessage> getRelevantMessages(Type screenType)
		{
			List<ScreenMessage> relevantMsgs = new List<ScreenMessage>();
			foreach(ScreenMessage item in prevMessageQueue)
			{
				if(screenType == item.target || screenType.IsSubclassOf(item.target))
				{
					relevantMsgs.Add(item);
				}
			}
			return relevantMsgs;
		}

		public void addMessage(ScreenMessage newMsg)
		{
			messageQueue.Add(newMsg);
		}

		public void changeBounds(Rectangle newBounds)
		{
			float widthfactor = newBounds.Width / (float)maxBounds.Width;
			float heightfactor = newBounds.Height / (float)maxBounds.Height;

			maxBounds = newBounds;
			foreach(IScreen screen in activeScreens)
			{
				screen.changeViewport( widthfactor, heightfactor );
			}
		}
    }
}
