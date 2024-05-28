using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[DisallowMultipleNodesAttribute]
public class StoryStartNode : Node {

    [Output(ShowBackingValue.Never, ConnectionType.Override)] public bool output;
	public Sprite background;

    // Use this for initialization
    protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}