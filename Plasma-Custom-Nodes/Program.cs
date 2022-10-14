using Behavior;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace PlasmaModding {
    public static class CustomNodeManager
    {
        static IEnumerable<AgentGestalt> agentGestalts = Enumerable.Empty<AgentGestalt>();
        static bool loadedNodeResources = false;
        static bool awoken = false;
        static Harmony harmony;

        public static void Awake()
        {
            if (awoken) return;
            awoken = true;

            harmony = new Harmony("CustomNodeManager");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if(Holder.agentGestalts != null)
            {
                loadedNodeResources = true;
            }
        }
        public class LateGestaltRegistrationException : Exception { }
        private static void RegisterGestalt(AgentGestalt gestalt)
        {
            Awake();
            if (loadedNodeResources)
                throw new LateGestaltRegistrationException();
            agentGestalts = agentGestalts.Concat(new[] {gestalt});
        }

        public class InsufficientGestaltDataException : Exception
        {
            public InsufficientGestaltDataException(string message) : base(message)
            {
            }
        }

        public static void CreateNode(AgentGestalt gestalt, string unique_node_name)
        {
            gestalt.id = (AgentGestaltEnum)unique_node_name.GetHashCode()+1000;
            if(gestalt.agent == null)
                throw new InsufficientGestaltDataException("No agent attached to gestalt");
            if (gestalt.displayName == null)
                throw new InsufficientGestaltDataException("Node should have a display name");
            if (gestalt.properties == null)
                gestalt.properties = new Dictionary<int, AgentGestalt.Property>();
            if ( gestalt.ports == null)
                gestalt.ports = new Dictionary<int, AgentGestalt.Port>();
            RegisterGestalt(gestalt);
        }

        public static AgentGestalt.Port CreatePort(string name, string desc, AgentGestalt.Port.Types type, Data.Types datatype, int pos, int operation = -1)
        {
            AgentGestalt.Port port = new AgentGestalt.Port();
            port.name = name;
            port.description = desc;
            port.type = type;
            port.dataType = datatype;
            port.position = pos;

            if (operation > -1)
            {
                port.operation = operation;
            }


            return port;
        }

        public static AgentProperty CreateProperty(string name, string desc, Data defaultData, int driverCommand, int handler, int position, bool configurable)
        {
            AgentProperty prop = new AgentGestalt.Property();
            prop.configurable = configurable;
            prop.defaultData = defaultData;
            prop.description = desc;
            prop.driverCommand = driverCommand;
            prop.handler = handler;
            prop.name = name;
            prop.position = position;

            return prop;
        }

        [HarmonyPatch(typeof(Resources), "LoadAll", new Type[] {typeof(string), typeof(Type)})]
        class LoadResourcesPatch
        {
            public static void Postfix(string path, Type systemTypeInstance, ref UnityEngine.Object[] __result)
            {
                if (path == "Gestalts/Logic Agents" && systemTypeInstance == typeof(AgentGestalt) && !loadedNodeResources)
                {
                    int size = __result.Length;
                    int newSize = size + agentGestalts.Count();
                    UnityEngine.Object[] temp = new UnityEngine.Object[newSize];
                    __result.CopyTo(temp, 0);
                    agentGestalts.ToArray().CopyTo(temp, size);
                    __result = temp;
                    loadedNodeResources = true;
                }
            }
        }

    }
}