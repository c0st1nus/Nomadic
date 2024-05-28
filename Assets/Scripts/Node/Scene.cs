using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class Scene : Node {
	[Input] public bool input;
	[Output] public bool output;
	public Sprite scene;
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}