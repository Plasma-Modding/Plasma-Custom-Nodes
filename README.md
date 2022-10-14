# Plasma-Custom-Nodes
A library to unify and slightly streamline the process of adding custom nodes into Plasma


# Example Usage
Agent Gestalt init
```
// - Initial setup -
AgentGestalt gestalt = (AgentGestalt)ScriptableObject.CreateInstance(typeof(AgentGestalt));
gestalt.displayName = "Example";
gestalt.componentCategory = AgentGestalt.ComponentCategories.Behavior;
gestalt.properties = new Dictionary<int, AgentGestalt.Property>();
gestalt.ports = new Dictionary<int, AgentGestalt.Port>();

// - Ports - 
gestalt.ports[0] = CreatePort("Execute", "Runs the node", AgentGestalt.Port.Types.Command, Data.Types.None, 1, 0); // the 0 is the operation port Id used for agent class
gestalt.ports[1] = CreatePort("Result", "", AgentGestalt.Port.Types.Output, Data.Types.None, 2);

// - Properties -
gestalt.properties[0] = CreateProperty("Result", "", new Data(), 0, 0, 2, true);

// - Mappings- 
gestalt.ports[1].injectedProperty = 0 // Same as id set above
// .mappedProperty for inputs

// - Assign agent class -
gestalt.agent = typeof(ExampleAgent);
```

Agent Class
```
class ExampleAgent : Agent {
    protected override void OnSetupFinished()
    {
    
    }
    
    [SketchNodePortOperation(0)] // Same as operation port id set above
    public void Execute(SketchNode node) {
      node.GetPort(1).Commit(new Data("Hello World!"));
    }
}
```
