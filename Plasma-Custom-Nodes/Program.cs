using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Behavior;
using System.Linq;

namespace PlasmaModding
{
    public static class CustomNodeManager
    {
        static IEnumerable<AgentGestalt> agentGestalts = Enumerable.Empty<AgentGestalt>();
        static bool loadedNodeResources = false;
        static bool awoken = false;
        static Dictionary<string, AgentCategoryEnum> customCategories = new Dictionary<string, AgentCategoryEnum>();
        private static int highestCategoryId = 3;
        static Harmony? harmony;
        private static int recent_port_dict_id;

        public static void Awake()
        {
            if (awoken) return;
            awoken = true;

            harmony = new Harmony("CustomNodeManager");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if (Holder.agentGestalts != null)
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
            agentGestalts = agentGestalts.Concat(new[] { gestalt });
        }

        public class InsufficientGestaltDataException : Exception
        {
            public InsufficientGestaltDataException(string message) : base(message)
            {
            }
        }

        public static void CreateNode(AgentGestalt gestalt, string unique_node_name)
        {
            gestalt.id = (AgentGestaltEnum)unique_node_name.GetHashCode() + 1000;
            if (gestalt.agent == null)
                throw new InsufficientGestaltDataException("No agent attached to gestalt");
            if (gestalt.displayName == null)
                throw new InsufficientGestaltDataException("Node should have a display name");
            if (gestalt.properties == null)
                gestalt.properties = new Dictionary<int, AgentGestalt.Property>();
            if (gestalt.ports == null)
                gestalt.ports = new Dictionary<int, AgentGestalt.Port>();
            RegisterGestalt(gestalt);
        }
        public static AgentGestalt CreateGestalt(Type agent, string displayName, string? description = null, AgentCategoryEnum category = AgentCategoryEnum.Misc)
        {
            AgentGestalt gestalt = (AgentGestalt)ScriptableObject.CreateInstance(typeof(AgentGestalt));
            gestalt.componentCategory = AgentGestalt.ComponentCategories.Behavior;
            gestalt.properties = new Dictionary<int, AgentGestalt.Property>();
            gestalt.ports = new Dictionary<int, AgentGestalt.Port>();
            gestalt.type = AgentGestalt.Types.Logic;

            gestalt.agent = agent;
            gestalt.displayName = displayName;
            gestalt.description = description;
            gestalt.nodeCategory = category;

            return gestalt;
        }
        private static AgentGestalt.Port CreateGenericPort(AgentGestalt gestalt, string name, string description)
        {
            AgentGestalt.Port port = new AgentGestalt.Port();
            int port_dict_id = 1;
            try
            {
                port_dict_id = GetHighestKey(gestalt.ports) + 1;
            }
            catch (Exception) { }

            int position = 1;
            try
            {
                position = gestalt.ports[port_dict_id - 1].position + 1;
            }
            catch (Exception) { }


            port.position = position;
            gestalt.ports.Add(port_dict_id, port);
            port.name = name;
            port.description = description;
            recent_port_dict_id = port_dict_id;
            return port;
        }

        public static AgentGestalt.Port CreateCommandPort(AgentGestalt gestalt, string name, string description, int operation)
        {
            AgentGestalt.Port port = CreateGenericPort(gestalt, name, description);
            port.operation = operation;
            port.type = AgentGestalt.Port.Types.Command;
            return port;
        }

        public static AgentGestalt.Port CreatePropertyPort(AgentGestalt gestalt, string name, string description, Data.Types datatype = Data.Types.None, bool configurable = true, Data? defaultData = null, string? reference_name = null)
        {
            if (defaultData == null)
                defaultData = new Data();
            AgentGestalt.Port port = CreateGenericPort(gestalt, name, description);
            AgentGestalt.Property property = new AgentGestalt.Property();
            int property_dict_id = 1;
            try
            {
                property_dict_id = GetHighestKey(gestalt.ports) + 1;
            }
            catch (Exception e) { }


            property.position = port.position;
            gestalt.properties.Add(property_dict_id, property);
            if (gestalt.agent.IsSubclassOf(typeof(CustomAgent)))
            {
                if (!CustomAgent.properties.ContainsKey(gestalt.agent))
                {
                    CustomAgent.properties.Add(gestalt.agent, new Dictionary<string, int>());

                }
                CustomAgent.properties[gestalt.agent].Add(reference_name ?? name, property_dict_id);
            }
            property.defaultData = defaultData;
            property.configurable = configurable;
            property.name = name;
            property.description = description;
            port.dataType = datatype;
            port.mappedProperty = property_dict_id;
            port.type = AgentGestalt.Port.Types.Property;
            port.expectsData = true;

            return port;
        }

        private static int GetHighestKey(Dictionary<int, AgentGestalt.Port> l)
        {
            return l.Keys.OrderBy(b => b).Last(); 
        }

        public static AgentGestalt.Port CreateOutputPort(AgentGestalt gestalt, string name, string description, Data.Types datatype = Data.Types.None, bool configurable = true, Data? defaultData = null, string? reference_name = null)
        {
            if (defaultData == null)
                defaultData = new Data();
            AgentGestalt.Port port = CreateGenericPort(gestalt, name, description);
            AgentGestalt.Property property = new AgentGestalt.Property();
            int property_dict_id = 1;
            try
            {
                property_dict_id = GetHighestKey(gestalt.ports) + 1;
            }
            catch (Exception) { }
            property.position = port.position;
            gestalt.properties.Add(property_dict_id, property);
            if (gestalt.agent.IsSubclassOf(typeof(CustomAgent)))
            {
                if (!CustomAgent.outputs.ContainsKey(gestalt.agent))
                {
                    CustomAgent.outputs.Add(gestalt.agent, new Dictionary<string, int>());

                }
                CustomAgent.outputs[gestalt.agent].Add(reference_name ?? name, recent_port_dict_id);
            }
            property.defaultData = defaultData;
            property.configurable = configurable;
            property.name = name;
            property.description = description;
            port.dataType = datatype;
            port.injectedProperty = property_dict_id;
            port.type = AgentGestalt.Port.Types.Output;
            return port;
        }

        public static AgentCategoryEnum CustomCategory(string name)
        {
            name = name.ToUpperInvariant();
            if (customCategories.ContainsKey(name))
                return customCategories[name];
            customCategories.Add(name, (AgentCategoryEnum)(++highestCategoryId));
            return (AgentCategoryEnum) highestCategoryId;
        }

        [HarmonyPatch(typeof(Resources), "LoadAll", new Type[] { typeof(string), typeof(Type) })]
        private class LoadResourcesPatch
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

        [HarmonyPatch(typeof(Visor.ProcessorUICategoryItem), nameof(Visor.ProcessorUICategoryItem.Setup))]
        private class AddCategoryToDictPatch
        {
            static int applied = 0;
            public static void Prefix()
            {
                if (Holder.instance.agentCategories != null && applied < customCategories.Count())
                    foreach (string categoryName in customCategories.Keys)
                    {
                        if (!Holder.instance.agentCategories.ContainsKey(customCategories[categoryName])){
                            applied++;
                            Holder.instance.agentCategories.Add(customCategories[categoryName], categoryName);
                        }
                    }
            }
        }

        [HarmonyPatch(typeof(System.Enum), "GetNames")]
        public class EnumNamePatch
        {
            public static void Postfix(System.Type enumType, ref string[] __result)
            {
                if (enumType == typeof(AgentCategoryEnum))
                {
                    string[] tabs = customCategories.Keys.ToArray();
                    string[] names = new string[__result.Length + tabs.Length];
                    __result.CopyTo(names, 0);
                    tabs.CopyTo(names, __result.Length);
                    __result = names;
                }
            }
        }

        [HarmonyPatch(typeof(System.Enum), "TryParseEnum")]
        public class EnumParsePatch
        {
            public static void Postfix(System.Type enumType, string value, ref object parseResult, ref bool __result)
            {
                if (!__result && enumType == typeof(AgentCategoryEnum) && customCategories.ContainsKey(value))
                {
                    __result = true;
                    System.Type EnumResult = typeof(System.Enum).GetNestedType("EnumResult", BindingFlags.NonPublic);
                    MethodInfo Init = EnumResult.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo parsedEnum = EnumResult.GetField("parsedEnum", BindingFlags.NonPublic | BindingFlags.Instance);
                    var presult = System.Convert.ChangeType(System.Activator.CreateInstance(EnumResult), EnumResult);
                    Init.Invoke(presult, new object[] { false });
                    parsedEnum.SetValue(presult, customCategories[value]);
                    parseResult = presult;
                }
            }
        }
    }
}
