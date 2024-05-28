using System;
using UnityEngine;
using XNode;
// ReSharper disable once CheckNamespace
public class Choice : Node {
	[Input] public bool input;
	[Output (dynamicPortList = true)] public string[] choiceText;
	public int[] choicePrice;

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		if (port.fieldName.StartsWith("choiceText")) {
			int index = int.Parse(port.fieldName.Replace("choiceText ", ""));
			return choiceText[index];
		}
		return null;
	}
}