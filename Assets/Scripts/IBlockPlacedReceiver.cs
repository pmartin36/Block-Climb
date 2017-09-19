using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IBlockPlacedReceiver
{
	void c_newBlockPlacedEvent(object sender, BlockPlacedEventInfo e);
}

