using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.Control
{
    public enum InputCommand
    {
        EXIT,
        DOWN,		// single keypress
		DOWN_CONT,	// continuous keypress
        UP,			// single keypress
		UP_CONT,	// continuous keypress
		LEFT_CONT,	// continuous keypress
		RIGHT_CONT,	// continuous keypress
        SELECT,
		SPACE,
		A,
		C,
		D,
		E,
		F,
		G,
		// ...
		S,
		// ...
		SCROLL_UP,
		SCROLL_DOWN,
		SHIFT_CONT,
    }

	public enum PointerCommand
	{
		NONE,
		PRIMARY,
		SECONDARY,
		SCROLL,
	}

	public struct PointerEvent
	{
		public int X,Y;
		public PointerCommand Command;
		//public int scroll;

		public PointerEvent(int x, int y, PointerCommand command)
		{
			X = x;
			Y = y;
			Command = command;
		}
	}

    public class Input
    {
		public HashSet<InputCommand> commands { get; private set;}

        private KeyboardState previousKBState;

		private MouseState previousMState;
		public PointerEvent pointerEvent { get; private set;}

		Dictionary<Keys,InputCommand> keyBindings;
		Dictionary<Keys,InputCommand> contKeyBindings;

        public Input()
        {
            commands = new HashSet<InputCommand>();
            previousKBState = Keyboard.GetState();

			keyBindings = new Dictionary<Keys, InputCommand>();
			contKeyBindings = new Dictionary<Keys, InputCommand>();

			setDefaultKeyBindings();
        }

        public void Update()
        {
            commands.Clear();
            handleKeyBoard(Keyboard.GetState());
			handleMouse(Mouse.GetState());
        }

        private void handleKeyBoard(KeyboardState kbstate)
		{
			// method A: List traversal
			List<Keys> ks = kbstate.GetPressedKeys().ToList();

			foreach(Keys key in ks)
			{
				if( !previousKBState.GetPressedKeys().Contains( key ) && keyBindings.ContainsKey( key ) )
					commands.Add( keyBindings[key] );
				if( contKeyBindings.ContainsKey( key ) )
					commands.Add( contKeyBindings[key] );
			}

			previousKBState = kbstate;
		}

        public bool pop(InputCommand command)
        {
            return commands.Remove(command);
        }

        public bool peek(InputCommand command)
        {
            return commands.Contains(command);
        }

		private void handleMouse(MouseState mstate)
		{
			PointerEvent cur = new PointerEvent();
			cur.X = mstate.Position.X;
			cur.Y = mstate.Position.Y;
			if( mstate.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released && mstate.RightButton == ButtonState.Released )
				cur.Command = PointerCommand.PRIMARY;
			else if( mstate.RightButton == ButtonState.Pressed && previousMState.RightButton == ButtonState.Released && mstate.LeftButton == ButtonState.Released )
				cur.Command = PointerCommand.SECONDARY;
			else
				cur.Command = PointerCommand.NONE;

			//if(mstate.ScrollWheelValue > previousMState.ScrollWheelValue)

			// todo: distinguish continuous key presses?

			// TODO: scroll wheel

			pointerEvent = cur;
				

			previousMState = mstate;
		}

		private void setDefaultKeyBindings()
		{
			keyBindings.Add( Keys.Escape, InputCommand.EXIT );

			keyBindings.Add( Keys.Down, InputCommand.DOWN );

			keyBindings.Add( Keys.Up, InputCommand.UP );

			keyBindings.Add( Keys.Enter, InputCommand.SELECT );

			keyBindings.Add( Keys.S, InputCommand.S );

			keyBindings.Add( Keys.PageUp, InputCommand.SCROLL_UP );

			keyBindings.Add( Keys.PageDown, InputCommand.SCROLL_DOWN );

			keyBindings.Add( Keys.Space, InputCommand.SPACE );

			keyBindings.Add( Keys.C, InputCommand.C );
			keyBindings.Add( Keys.D, InputCommand.D );
			keyBindings.Add( Keys.E, InputCommand.E );
			keyBindings.Add( Keys.F, InputCommand.F );
			keyBindings.Add( Keys.G, InputCommand.G );



			contKeyBindings.Add( Keys.Left, InputCommand.LEFT_CONT );

			contKeyBindings.Add( Keys.Right, InputCommand.RIGHT_CONT );

			contKeyBindings.Add( Keys.Up, InputCommand.UP_CONT );

			contKeyBindings.Add( Keys.Down, InputCommand.DOWN_CONT );

			contKeyBindings.Add( Keys.LeftShift, InputCommand.SHIFT_CONT );
		}
    }
}
