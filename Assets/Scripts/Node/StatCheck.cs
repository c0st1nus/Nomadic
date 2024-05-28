using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class StatCheck : Node {
	[Input] public bool input;
	public condition[] conditions;
	[Output] public bool trueOutput;
	[Output] public bool falseOutput;
	protected override void Init() {
		base.Init();
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}