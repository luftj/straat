using System;
using straat.View.Screen;
using System.Collections.Generic;

namespace straat
{
	public enum ScreenMessageType
	{
		SELECTION_CHANGE,
	}

	public class ScreenMessage
	{
		public Type target { get;}
		public ScreenMessageType messageType {get;}
		public List<object> message {get;}

		// todo: reflect message types which can only appear once at a time?

		public ScreenMessage(Type target, ScreenMessageType type, params object[] message)
		{
			this.target = target;
			this.messageType = type;
			this.message = new List<object>( message );
		}
	}
}

