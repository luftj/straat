using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using straat.Control;

namespace straat.View.Screen
{
    class GameScreen : IScreen
    {
        ScreenManager screenManager;
        Camera cam;
        public GameScreen(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
        }

        public void Draw(double deltaT)
        {
            //

        }

        public void Update(double deltaT, Input input)
        {
            // move camera according to input
            if (input.peek(InputCommand.LEFT))  cam.position.X--;
            if (input.peek(InputCommand.RIGHT)) cam.position.X++;
            if (input.peek(InputCommand.UP))    cam.position.Y--;
            if (input.peek(InputCommand.DOWN))  cam.position.Y++;
        }
        
    }
}
