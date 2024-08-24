using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XNode;
using Button = UnityEngine.UI.Button;

// ReSharper disable once CheckNamespace
public class Say : MonoBehaviour
{
    [SerializeField] private Dialogs dialogs;
    [SerializeField] private GameObject text;
    public float delay = 0.1f;
    private bool _isTyping;
    [SerializeField] private Button[] buttons;
    private Player _player;
    [SerializeField] private Image playerImage;
    public Image background;
    private Node _dialog;
    [SerializeField] private GameObject author;
    private void Start()
    {
        dialogs ??= GameManager.currentStory;
        NextDialog();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextDialog();
        }
    }
    
    private void NextDialog()
    {
        if (dialogs == null) return;
        _dialog ??= dialogs.nodes.Find(node => node is StoryStartNode);
        if (!_isTyping)
        {
            switch (_dialog)
            {
                case StoryStartNode storyStartNode: {
                    if(storyStartNode.background != null)
                    {
                        background.type = Image.Type.Tiled;
                        ChangeBackground(storyStartNode.background);
                    }
                    _dialog = _dialog.GetOutputPort("output").Connection.node;
                    NextDialog();
                    break;
                }
                case Dialog dialog: {
                    text.gameObject.SetActive(true);
                    ShowDialog(dialog);
                    DisActiveButtons();
                    break;
                }
                case AuthorText authorText: {
                    author_Text(authorText);
                    break;
                }
                case Choice choice: {
                    Choice(choice);
                    break;
                }
                case StatChange change: {
                    ChangeStat(change._stats);
                    break;
                }
                case StatCheck check:
                {
                    StatCheck(check.conditions);
                    break;
                }
                case BackGroundChange backGroundChange:
                {
                    background.type = Image.Type.Tiled;
                    ChangeBackground(backGroundChange.background);
                    _dialog = _dialog.GetOutputPort("output").Connection.node;
                    NextDialog();
                    break;
                }
                case Scene scene:
                {
                    Scene(scene);
                    _dialog = _dialog.GetOutputPort("output").Connection.node;
                    break;
                }
                case StoryEndNode:
                {
                    SceneManager.LoadScene("main");
                    break;
                }
            }
            
        }
        else
        {
            _isTyping = false;
            StopAllCoroutines();
            switch (_dialog)
            {
                case Dialog dialog:
                    text.transform.GetChild(0).GetComponent<TMP_Text>().text = dialog.Text;
                    text.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = dialog.personage.name;
                    playerImage.sprite = dialog.personage.image != null ? dialog.personage.image : null;
                    break;
                case AuthorText authorText:
                    author.transform.GetChild(0).GetComponent<TMP_Text>().text = authorText.text;
                    break;
            }
            if (_dialog.GetOutputPort("output").Connection != null)
            {
                _dialog = _dialog.GetOutputPort("output").Connection.node;
            }
        }
    }
    
    IEnumerator UpdateHeight(TMP_Text tx, RectTransform txRect, RectTransform authorRect)
    {
        // Ждем следующего кадра
        yield return new WaitForEndOfFrame();

        // Теперь можно обновить preferredHeight
        tx.text = tx.text + "\n s";
        txRect.sizeDelta = new Vector2(875, tx.preferredHeight);
        authorRect.sizeDelta = new Vector2(900, tx.preferredHeight + 100);
        tx.text = "";
    }
    
    private void ShowText(string message, GameObject obj)
    {
        StartCoroutine(TypeText(message, obj));
    }
    
    IEnumerator TypeText(string message, GameObject obj)
    {
        _isTyping = true;
        TMP_Text tx = obj.transform.GetChild(0).GetComponent<TMP_Text>();
        tx.text = "";
        foreach (char c in message)
        {
            tx.text += c;
            yield return new WaitForSeconds(delay);
        }
        _isTyping = false;
        if (_dialog.GetOutputPort("output").Connection != null)
        {
            _dialog = _dialog.GetOutputPort("output").Connection.node;
        }
    }
    
    private void DisActiveButtons()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        text.SetActive(true);
        author.SetActive(false);
        playerImage.color= new Color(playerImage.color.r, playerImage.color.g, playerImage.color.b, 1f);
    }
    
    private void Choice(Choice ch)
    {
        text.gameObject.SetActive(false);
        if (ch.choiceText.Length == 0) return;
        for (int i = 0; i < ch.choiceText.Length; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].transform.GetChild(1).gameObject.SetActive(ch.choicePrice[i] != 0);
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = ch.choiceText[i];
        }
    }
    
    public void StoryExit(){
        SceneManager.LoadScene("Main");
    }

    private void ChangeStat(Dict stats)
    {
        print("ChangeStat");
        if (stats?._dictElements != null)
        {
            foreach (var v in stats._dictElements)
            {
                switch (v.Key)
                {
                    case playerStats.Agility:
                        _player.Agility += v.Value;
                        break;
                    case playerStats.Charisma:
                        _player.Charisma += v.Value;
                        break;
                    case playerStats.Health:
                        _player.Health += v.Value;
                        break;
                    case playerStats.Intellect:
                        _player.Intellect += v.Value;
                        break;
                    case playerStats.Reputation:
                        _player.Reputation += v.Value;
                        break;
                    case playerStats.Luck:
                        _player.Luck += v.Value;
                        break;
                    case playerStats.Strength:
                        _player.Strength += v.Value;
                        break;
                }
            }
        }
        
        // ReSharper disable once Unity.NoNullPropagation
        if (_dialog?.GetOutputPort("output")?.Connection != null)
        {
            _dialog = _dialog.GetOutputPort("output").Connection.node;
        }
        NextDialog();
    }
    
    private void StatCheck(condition[] stats)
    {
        print("statCheck");
        bool result = false;
        foreach (var v in stats)
        {
            switch (v.conditionType)
            {
                case equality.equal:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    result = _player[v.stat] == v.value;
                    break;
                case equality.notEqual:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    result = _player[v.stat] != v.value;
                    break;
                case equality.less:
                    result = _player[v.stat] < v.value;
                    break;
                case equality.greater:
                    result = _player[v.stat] > v.value;
                    break;
                case equality.lessOrEqual:
                    result = _player[v.stat] <= v.value;
                    break;
                case equality.greaterOrEqual:
                    result = _player[v.stat] >= v.value;
                    break;
            }
        }
        _dialog = result ? _dialog.GetOutputPort("trueOutput").Connection.node : _dialog.GetOutputPort("falseOutput").Connection.node;
        NextDialog();
    }
    
    private void ShowDialog(Dialog dialog)
    {   float screen_comp2 = Screen.height / 1920f;
        RectTransform txRect = text.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform childRect = text.transform.GetChild(1).GetComponent<RectTransform>();
        text.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = dialog.personage.name;
        playerImage.color= new Color(playerImage.color.r, playerImage.color.g, playerImage.color.b, 1f);
        playerImage.gameObject.SetActive(true);
        playerImage.sprite = dialog.personage.image != null ? dialog.personage.image : null;
        RectTransform player = playerImage.GetComponent<RectTransform>();
        float par = dialog.personage.image.rect.width / dialog.personage.image.rect.height;
        par /= dialog.personage.kid? 1.75f : 1.25f;  
        float unit = Screen.height / 300;
        float height = dialog.personage.height * unit;
        float width = height * par;
        player.sizeDelta = new Vector2(width, height);
        player.localPosition = dialog.leftPos? new Vector3(-143, player.localPosition.y) : new Vector3(125, player.localPosition.y);
        txRect.anchorMin = new Vector2(0.5f, 0);
        txRect.anchorMax = new Vector2(0.5f, 0);
        txRect.pivot = new Vector2(0.5f, 0);
        txRect.anchoredPosition = new Vector2(15, 5);
        childRect.anchorMin = new Vector2(0, 1);
        childRect.anchorMax = new Vector2(0, 1);
        childRect.pivot = new Vector2(0, 1);
        TMP_Text tx = text.transform.GetChild(0).GetComponent<TMP_Text>();
        tx.text = dialog.Text + "\n s";
        float screen_comp = Screen.width / 1080f;
        childRect.anchoredPosition = new Vector2(50f, text.GetComponent<RectTransform>().sizeDelta.y * -0.083333333f);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(850 * screen_comp/1.25f, tx.preferredHeight + childRect.sizeDelta.y + 50);
        tx.GetComponent<RectTransform>().sizeDelta = new Vector2(750 * screen_comp/1.25f, tx.preferredHeight);
        tx.text = "";
        ShowText(dialog.Text, text);
    }
    
    private void ChangeBackground(Sprite sprite)
    {
        background.sprite = sprite;
    }

    private void author_Text(AuthorText aT)
    {
        RectTransform authorRect = author.GetComponent<RectTransform>();
        authorRect.anchorMin = new Vector2(0.5f, 0.5f);
        authorRect.anchorMax = new Vector2(0.5f, 0.5f);
        authorRect.pivot = new Vector2(0.5f, 0.5f);
        authorRect.anchoredPosition = new Vector2(0, 15);
        author.SetActive(true);
        text.SetActive(false);
        playerImage.color= new Color(playerImage.color.r, playerImage.color.g, playerImage.color.b, 0f);
        TMP_Text tx = author.transform.GetChild(0).GetComponent<TMP_Text>();
        tx.text = aT.text + "\n s";
        float screen_comp = Screen.width / 1080f;
        tx.GetComponent<RectTransform>().sizeDelta = new Vector2(800 * (screen_comp/1.25f), tx.preferredHeight);
        author.GetComponent<RectTransform>().sizeDelta = new Vector2(850 * (screen_comp/1.25f), tx.preferredHeight + 30);
        tx.text = "";
        ShowText(aT.text, author);
    }

    public void Scene(Scene scene)
    {
        background.type = Image.Type.Sliced;
        ChangeBackground(scene.scene);
    }
    
    public void OnButtonClick(int index)
    {
        _dialog = _dialog.GetOutputPort($"choiceText {index}").Connection.node;
        foreach (var but in buttons)
        {
            but.gameObject.SetActive(false);
        }
        NextDialog();
    }
}
