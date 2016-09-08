using straat.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.View.Screen
{
    public interface IScreen
    {
		void Initialize();
		void LoadContent();
		void UnloadContent();

        void Update(double deltaT, Input input);

        void Draw(double deltaT);

        //void Enter();

        //void Exit();
    }
}
