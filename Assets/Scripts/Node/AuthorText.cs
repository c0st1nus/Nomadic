using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class AuthorText : Node {
	[Input] public bool input;
	[Output] public bool output;
	public string text;
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}