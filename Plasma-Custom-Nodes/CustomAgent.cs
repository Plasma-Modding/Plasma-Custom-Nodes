using Behavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaModding
{
    public class CustomAgent: Agent
    {
        protected static Dictionary<string, int> properties = new Dictionary<string, int>();

        protected AgentProperty GetProperty(string name)
        {
            return _runtimeProperties[properties[name]];
        }

        protected static Dictionary<string, int> outputs = new Dictionary<string, int>();

        protected static void WriteOutput(string name, Data data, SketchNode node)
        {
            node.ports[outputs[name]].Commit(data);
        }

        protected void WriteOutput(string name, Data data)
        {
            currentSketchNode.ports[outputs[name]].Commit(data);
        }
    }
}
