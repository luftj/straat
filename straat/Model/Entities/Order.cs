using System;
using System.Collections.Generic;

namespace straat.Model.Entities
{
	public enum OrderType
	{
		MOVE,
		ATTACK,
		HOLD,

		NONE,

		NUM_ORDERTYPES
	}

	public class Order
	{
		public OrderType type { get; }
		public List<object> data { get; }

		public Order (OrderType type, params object[] data)
		{
			this.type = type;
			this.data = new List<object>(data);
		}
	}
}
