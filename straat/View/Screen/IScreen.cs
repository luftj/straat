using straat.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.View.Screen
{
    interface IScreen
    {
        void Update(double deltaT, Input input);

        void Draw(double deltaT);

        //void Enter();

        //void Exit();
    }
}
