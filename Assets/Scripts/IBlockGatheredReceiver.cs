using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IBlockGatheredReceiver {
	void c_newBlockGatheredEvent(object sender, BlockGatheredEventInfo e);
}
