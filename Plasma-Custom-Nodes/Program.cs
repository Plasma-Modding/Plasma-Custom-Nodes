using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Behavior;

namespace PlasmaModding {
    public static class CustomNodeManager
    {
        static IEnumerable<AgentGestalt> agentGestalts = Enumerable.Empty<AgentGestalt>();
        static bool loadedNodeResources = false;
        static bool awoken = false;
        static Harmony? harmony;

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
        private static AgentGestalt.Port CreateGenericPort(AgentGestalt gestalt, string name, string description)
        {
            AgentGestalt.Port port = new AgentGestalt.Port();
            int port_dict_id = (gestalt.ports.Count() > 0) ? gestalt.ports.Keys.Max() + 1 : 1;
            int position = (gestalt.ports.Count() > 0) ? gestalt.ports[gestalt.ports.Keys.Max()].position + 1 : 1;
            port.position = position;
            gestalt.ports.Add(port_dict_id, port);
            port.name = name;
            port.description = description;
            return port;
        }

        public static AgentGestalt.Port CreateCommandPort(AgentGestalt gestalt, string name, string description, int operation)
        {
            AgentGestalt.Port port = CreateGenericPort(gestalt, name, description);
            port.operation = operation;
            return port;
        }

        public static AgentGestalt.Port CreatePropertyPort(AgentGestalt gestalt, string name, string description, Data.Types datatype = Data.Types.None, bool configurable = true, Data? defaultData = null)
        {
            if (defaultData == null)
                defaultData = new Data();
            AgentGestalt.Port port = CreateGenericPort(gestalt, name, description);
            AgentGestalt.Property property = new AgentGestalt.Property();
            int property_dict_id = (gestalt.ports.Count() > 0) ? gestalt.properties.Keys.Max() + 1 : 1;
            property.position = port.position;
            gestalt.properties.Add(property_dict_id, property);
            property.defaultData = defaultData;
            property.configurable = configurable;
            port.dataType = datatype;
            port.mappedProperty = property_dict_id;
            return port;
        }
        
        public static AgentGestalt.Port CreateOutputPort(AgentGestalt gestalt, string name, string description, Data.Types datatype=Data.Types.None,  bool configurable=true, Data? defaultData = null)
        {
            if (defaultData == null)
                defaultData = new Data();
            AgentGestalt.Port port = CreateGenericPort(gestalt, name, description);
            AgentGestalt.Property property = new AgentGestalt.Property();
            int property_dict_id = (gestalt.ports.Count() > 0) ? gestalt.properties.Keys.Max() + 1 : 1;
            property.position = port.position;
            gestalt.properties.Add(property_dict_id, property);
            property.defaultData = defaultData;
            property.configurable = configurable;
            port.dataType = datatype;
            port.injectedProperty = property_dict_id;
            return port;
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