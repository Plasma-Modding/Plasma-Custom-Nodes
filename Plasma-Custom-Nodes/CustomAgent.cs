using Behavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

namespace PlasmaModding
{
    public class CustomAgent: Agent
    {
        
        internal static Dictionary<Type,Dictionary<string, int>> properties = new Dictionary<Type,Dictionary<string, int>>();

        protected AgentProperty GetProperty(string name)
        {
            return _runtimeProperties[properties[this.GetType()][name]];
        }

        internal static Dictionary<Type,Dictionary<string, int>> outputs = new Dictionary<Type,Dictionary<string, int>>();

        protected void WriteOutput(string name, Data data)
        {
            currentSketchNode.ports[outputs[this.GetType()][name]].Commit(data);
        }
    }
}
