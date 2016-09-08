using straat.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace straat.View.Screen
{
    class MainMenuScreen : IScreen
    {
        ScreenManager screenManager;
		Rectangle bounds;

        List<string> entries;
        int selectedEntry = 0;

		#region content
		SpriteFont font;
		#endregion

		public MainMenuScreen(ScreenManager screenManager, Rectangle bounds)
        {
            this.screenManager = screenManager;
			this.bounds = bounds;

			entries = new List<string>();
			entries.Add("Start Game");
			entries.Add("Exit");
        }

		public void Initialize()
		{
			//
		}

		public void LoadContent()
		{
			font = screenManager.game.Content.Load<SpriteFont>("testfont");
		}

		public void UnloadContent()
		{
			//
		}

        public void Draw(double deltaT)
        {
			// draw entries
			Color color;
			Vector2 pos = new Vector2((float)(bounds.Width/2.0 + bounds.Left), (float)bounds.Top );
			for(int i=0;i<entries.Count();++i)
			{
				if( i == selectedEntry )
					color = Color.Red;
				else
					color = Color.White;
				screenManager.game.spriteBatch.DrawString(font,entries[i],pos,color);

				pos.Y += 20;
			}
		}

        //public void Enter()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Exit()
        //{
        //    throw new NotImplementedException();
        //}

        public void Update(double deltaT,Input input)
        {
            if (input.pop(InputCommand.DOWN))
                if (selectedEntry < entries.Count - 1)
                    selectedEntry++;

            if (input.pop(InputCommand.UP))
                if (selectedEntry > 0)
                    selectedEntry--;

            if(input.pop(InputCommand.SELECT))
            {
				switch( selectedEntry )
				{
				case 0:
					screenManager.activateScreen( new GameScreen( screenManager, bounds ) );
					screenManager.deactivateScreen( this );
					break;
				case 1:
					screenManager.deactivateScreen( this );
					break;
				default:
					break;

				}
            }

            if (input.peek(InputCommand.EXIT))
                screenManager.deactivateScreen(this);
        }
    }
}
