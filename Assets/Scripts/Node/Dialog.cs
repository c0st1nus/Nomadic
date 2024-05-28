using System.ComponentModel;
using UnityEngine;
using XNode;
public class Dialog : Node
{
	[Input] public bool input;
	[Output] public bool output;
	[SerializeField] public Personage personage;
	[SerializeField] [TextArea(5, 10)] private string text;
	[SerializeField] public bool leftPos;

	public string Name => name;
	public string Text => text;

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {	
		return null; // Replace this
	}
}