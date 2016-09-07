using straat.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.View.Screen
{
    class ScreenManager
    {
        //List<IScreen> screens; // reactivate for making screen state persistent?
        List<IScreen> activeScreens;

        public ScreenManager()
        {
            //screens = new List<IScreen>();
            activeScreens = new List<IScreen>();
        }

        public void activateScreen(IScreen screen)
        {
            activeScreens.Add(screen);
            //screen.Enter();
        }

        public void deactivateScreen(IScreen screen)
        {
            //if (activeScreens.Contains(screen))
            //    screen.Exit();
            activeScreens.Remove(screen);
        }

        public void Draw(double deltaT)
        {
            foreach (IScreen screen in activeScreens)
                screen.Draw(deltaT);
        }
        public void Update(double deltaT, Input input)
        {
            foreach (IScreen screen in activeScreens)
                screen.Update(deltaT, input);
        }
    }
}
