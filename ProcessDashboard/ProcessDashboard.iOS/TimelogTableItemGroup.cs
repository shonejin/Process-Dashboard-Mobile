using System;
using System.Collections.Generic;
using UIKit;

namespace ProcessDashboard.iOS
{
	public class TimelogTableItemGroup
	{
		public string Name { get; set; }

			public string Footer { get; set; }

			public List<TimelogTableItem> Items
			{
				get { return items; }
				set { items = value; }
			}
			protected List<TimelogTableItem> items = new List<TimelogTableItem>();

	}
}

