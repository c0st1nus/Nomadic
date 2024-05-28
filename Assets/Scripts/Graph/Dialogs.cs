using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "New Story", menuName = "VisualNovella/New Story")]
public class Dialogs : NodeGraph { 
	public Sprite storyPreview;
	public string storyName;
	public string storyDescription;
	public string[] storyTags;
}