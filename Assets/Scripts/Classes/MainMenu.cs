using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public Dialogs[] stories;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backButton;
    private int _currentStoryIndex = 0;    
    
    private void nextStory()
    {
        if (_currentStoryIndex == 0)
        {
            forwardButton.SetActive(true);
            backButton.SetActive(false);
            storyPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = stories[_currentStoryIndex].name;
            storyPanel.GetComponent<Image>().sprite = stories[_currentStoryIndex].storyPreview;
            _currentStoryIndex++;
        }
    }
}
