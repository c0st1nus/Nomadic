
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryPanel : MonoBehaviour
{
    private GameObject _storyPanel;
    public Dialogs[] stories;
    public Scrollbar scrollbar;
    private float _yOffset = 800f;
    private float _initialYOffset;
    private GameObject _dialogParent;
    public static Dialogs currentStory;
    private Vector2 _startTouchPosition;
    private bool _isSwiping;

    private void Awake()
    {
        _storyPanel = transform.GetChild(1).gameObject;
        _dialogParent = new GameObject("DialogParent"); // Создаем новый пустой объект
        _dialogParent.transform.SetParent(transform, false); // Делаем его дочерним объектом панели
        scrollbar.value = 0; // Устанавливаем ползунок в самое верхнее положение
    }

    private void Start()
    {
        DisplayDialogs();
        _dialogParent.transform.localPosition = new Vector3(0f, -220f, 0f);
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _isSwiping = true;
                    _startTouchPosition = touch.position;
                    break;
                case TouchPhase.Moved:
                    if (_isSwiping)
                    {
                        float swipeDistance =  _startTouchPosition.y - touch.position.y;
                        if (swipeDistance < 0 && scrollbar.value < 1) // свайп вниз
                        {
                            scrollbar.value -= swipeDistance/2 / Screen.height;
                        }   
                        else if (swipeDistance >= 0 && scrollbar.value > 0)// свайп вверх
                        {
                            scrollbar.value -= swipeDistance/2 / Screen.height;
                        }
                        _startTouchPosition = touch.position; // Обновляем начальное положение касания
                    }
                    break;
                case TouchPhase.Ended:
                    _isSwiping = false;
                    break;
            }
        }
    }
    
    private void DisplayDialogs()
    {
        foreach (var dialog in stories)
        {
            var dialogInstance = Instantiate(_storyPanel, _dialogParent.transform); // Создаем диалоги внутри родительского объекта
            dialogInstance.transform.GetChild(0).GetComponent<Image>().sprite = dialog.storyPreview;
            dialogInstance.transform.GetChild(1).GetComponent<TMP_Text>().text = dialog.storyName;
            var rectTransform = dialogInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, _yOffset);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _yOffset -= 440;
            dialogInstance.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                currentStory = dialog;
                SceneManager.LoadScene(1);
            });
            dialogInstance.gameObject.SetActive(true);
        }
        _initialYOffset = _yOffset + 440 * (stories.Length-2) - 140f;
    }

    private void OnScrollbarValueChanged(float value)
    {
        float newYPosition = _initialYOffset + value * (stories.Length * 440); // Изменяем вычисление позиции
        _dialogParent.transform.localPosition = new Vector3(_dialogParent.transform.localPosition.x, newYPosition, _dialogParent.transform.localPosition.z);
    }
}