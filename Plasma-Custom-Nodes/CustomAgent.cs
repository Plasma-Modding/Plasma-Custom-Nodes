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
        /*
        protected static void WriteOutput(string name, Data data, SketchNode node)
        {
            node.ports[outputs[MethodBase.GetCurrentMethod().DeclaringType][name]].Commit(data);
        }*/

        protected void WriteOutput(string name, Data data)
        {
            Debug.Log("1");
            var a = currentSketchNode.ports;
            Debug.Log("2");
            var b = outputs[this.GetType()];
            Debug.Log("3");
            var c = b[name];
            Debug.Log("4");
            var d = a[c];
            Debug.Log("5");
            d.Commit(data);
            //currentSketchNode.ports[outputs[this.GetType()][name]].Commit(data);
        }
    }
}
