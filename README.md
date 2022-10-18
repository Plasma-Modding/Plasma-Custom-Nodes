# Plasma-Custom-Nodes
A library to unify and streamline the process of adding custom nodes into Plasma

# Create a custom node using this Library
*Note: This library requires that its methods are called before `[Assembly-CSharp.dll]Holder.Awake` so make sure the mod entry is happening at least prior to that*

1. Create a new class that inherits `PlasmaModding.CustomAgent` [[?]](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Writing-a-CustomAgent-class)
2. Create a gestalt using `PlasmaModding.CustomNodeManager.CreateGestalt`
3. Add any ports to the gestalt using `PlasmaModding.CustomNodeManager.CreateCommandPort`, `PlasmaModding.CustomNodeManager.CreatePropertyPort` and `PlasmaModding.CustomNodeManager.CreateNode`
4. Create the node from the gestalt using `PlasmaModding.CustomNodeManager.CreateNode`

# `PlasmaModding.CustomNodeManager`
> This class is static. It should not be instantiated, all methods can be called directly on the type `PlasmaModding.CustomNodeManager`
#### - [Creating Gestalt For Node](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/CreateGestalt-Method) 
   - [Create Gestalt](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/CreateGestalt-Method#creategestalttype-string-string-agentcategoryenum): `PlasmaModding.CustomNodeManager.CreateGestalt(Type, string, string?, AgentCategoryEnum)`
#### - [Add Ports to Gestalt](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Port-Creation-Methods)
   - [Create Command Port](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Port-Creation-Methods#createcommandportagentgestalt-string-string-int): `CreateCommandPort(AgentGestalt, string, string, int)`
   - [Create Property Port](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Port-Creation-Methods#createpropertyportagentgestalt-string-string-behaviordatatypes-bool-behaviordata-string): `CreatePropertyPort(AgentGestalt, string, string, Behavior.Data.Types, bool, Behavior.Data?, string?)`
   - [Create Output Port](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Port-Creation-Methods#createoutputportagentgestalt-string-string-behaviordatatypes-bool-behaviordata-string): `CreateOutputPort(AgentGestalt, string, string, Behavior.Data.Types, bool, Behavior.Data?, string?)`
#### - [Creating Node From Gestalt](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Create-Node-Method)
   - [Create Node](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Create-Node-Method#createnodeagentgestalt-string): `CreateNode(AgentGestalt, string)`
#### - [Getting/Creating Custom Categories](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Custom-Category-Method)
   - [Custom Category](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Custom-Category-Method#customcategorystring): `CustomCategory(string)`

# `PlasmaModding.CustomAgent`
> This class should only be extended, not instantiated. The reason it is not an interface is because the `Agent` class is not an interface
#### - [Writing A CustomAgent Class](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Writing-a-CustomAgent-class)
#### - [Port Operations](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Port-Operations)
   - [Get Property](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Port-Operations#getpropertystring): `GetProperty(string)`
   - [Write Output](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki/Port-Operations#writeoutputstring-behaviordata): `WriteOutput(string, Behavior.Data`
