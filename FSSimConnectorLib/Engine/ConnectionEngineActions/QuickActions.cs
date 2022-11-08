using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class QuickActions
    {
        internal FSSimConnectorEngine engine;
        public QuickActions(FSSimConnectorEngine engine)
        {
            this.engine = engine;
        }

        public void SetHeading(uint heading)
        {
            engine.AddSendEvent("HEADING_BUG_SET", heading);
        }
    }
}
