using straat.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.View.Screen
{
    class MainMenuScreen : IScreen
    {
        ScreenManager screenManager;

        List<string> entries;
        int selectedEntry = 0;

        public MainMenuScreen()
        {
            entries = new List<string>();
            entries.Add("Start Game");
            entries.Add("Exit");
        }

        MainMenuScreen(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
        }

        public void Draw(double deltaT)
        {
            throw new NotImplementedException();
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
                switch (selectedEntry)
                {
                    case 0:
                        screenManager.activateScreen(new GameScreen(screenManager));
                        break;
                    case 1:
                        screenManager.deactivateScreen(this);
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
