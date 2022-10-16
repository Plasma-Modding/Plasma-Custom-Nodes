# Plasma-Custom-Nodes
A library to unify and slightly streamline the process of adding custom nodes into Plasma

# Create A custom node using this Library
*Note: This library requires that its methods are called before `[Assembly-CSharp.dll]Holder.Awake` so make sure the mod entry is happening at least prior to that*

1. Create a new class that inherits `CustomAgent`
2. Create a gestalt using `CustomNodeManager.CreateGestalt`
3. Add any ports to the gestalt using `CustomNodeManager.CreateCommandPort`, `CustomNodeManager.CreatePropertyPort` and `CustomNodeManager.CreateNode`
4. Create the node using `CustomNodeManager.CreateNode`

For extra information on each function and class, view the [Wiki](https://github.com/Plasma-Modding/Plasma-Custom-Nodes/wiki)
