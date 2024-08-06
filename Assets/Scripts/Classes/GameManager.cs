using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityFigmaBridge.Runtime.UI;

public class GameManager : MonoBehaviour
{
    public static Dialogs currentStory;
    public TMP_Text balance;
    public TMP_Text lives;
    public TMP_Text uid;
    public FigmaImage avatar;
    public TMP_Text usernameText;
    public Dialogs testStory;
    public InputField username;
    public InputField password;
    private static User _user = new User();
    public static bool login;

    public void Start()
    {
        print(login);
        var prototypeFlowController =
            GetComponentInParent<Canvas>().rootCanvas?.GetComponent<PrototypeFlowController>();
        if (prototypeFlowController != null && login == false)
        {
            try
            {
                String message = PlayerPrefs.GetString("username");
                DataBaseHandler db = gameObject.GetComponent<DataBaseHandler>() == null
                    ? gameObject.AddComponent<DataBaseHandler>()
                    : gameObject.GetComponent<DataBaseHandler>();
                if (prototypeFlowController.CurrentScreenInstance.name == "iPhone 13 & 14 - 1" ||
                    prototypeFlowController.CurrentScreenInstance.name == "iPhone 13 & 14 - 5")
                {
                    Data[] res = db.Select($"SELECT * FROM users WHERE name = '{message}'");
                    _user = new User(res[0].Username, res[0].Balance, res[0].Lives, res[0].UID, res[0].Avatar);
                }
                balance.text = _user.Balance.ToString();
                lives.text = _user.Lives.ToString();
                if (uid != null) uid.text = _user.ID.ToUpper();
                if (usernameText != null) usernameText.text = _user.Username;
                if (avatar != null) avatar.sprite = _user.Avatar;
            }
            catch (Exception e)
            {
                login = true;
                print(e);
                if (prototypeFlowController != null) prototypeFlowController.TransitionToScreenById("13");
            }
        }
        else
        {
            string lang;
            try
            {
                lang = PlayerPrefs.GetString("lang");
            }
            catch
            {
                lang = "ru";
            }

            // if (lang != "ru" && prototypeFlowController.CurrentScreenInstance.name != "iPhone 13 & 14 - 14")
            // {
            //     if (prototypeFlowController != null) prototypeFlowController.TransitionToScreenById("14");
            // }
        }
    }

    public void HandleDropdownChangeRu(int value)
    {
        var prototypeFlowController =
            GetComponentInParent<Canvas>().rootCanvas?.GetComponent<PrototypeFlowController>();
        switch (value)
        {
            case 0:
                if (prototypeFlowController != null) prototypeFlowController.TransitionToScreenById("4");
                PlayerPrefs.SetString("lang", "ru");
                break;
            case 1:
                if (prototypeFlowController != null) prototypeFlowController.TransitionToScreenById("8");
                PlayerPrefs.SetString("lang", "kz");
                break;
        }
    }

    public void HandleDropdownChangeKz(int value)
    {
        var prototypeFlowController =
            GetComponentInParent<Canvas>().rootCanvas?.GetComponent<PrototypeFlowController>();
        switch (value)
        {
            case 0:
                if (prototypeFlowController != null) prototypeFlowController.TransitionToScreenById("8");
                PlayerPrefs.SetString("lang", "kz");
                break;
            case 1:
                if (prototypeFlowController != null) prototypeFlowController.TransitionToScreenById("4");
                PlayerPrefs.SetString("lang", "ru");
                break;
        }
    }

    public void StartStory()
    {
        currentStory = testStory;
        SceneManager.LoadScene("Story");
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void Link(string url)
    {
        Application.OpenURL(url);
    }

    public void CheckUser()
    {
        var prototypeFlowController =
            GetComponentInParent<Canvas>().rootCanvas?.GetComponent<PrototypeFlowController>();
        var db = gameObject.GetComponent<DataBaseHandler>() == null
            ? gameObject.AddComponent<DataBaseHandler>()
            : gameObject.GetComponent<DataBaseHandler>();
        Data[] data = db.Select($"SELECT * FROM users WHERE name = '{username.text}'");
        try
        {
            if (data[0].Password == password.text)
            {
                PlayerPrefs.SetString("username", username.text);
                Data[] res = db.Select($"SELECT * FROM users WHERE name = '{username.text}'");
                _user = new User(res[0].Username, res[0].Balance, res[0].Lives, res[0].UID, res[0].Avatar);
                if (prototypeFlowController != null) prototypeFlowController.TransitionToScreenById("1");
            }
            else
            {
                Debug.LogWarning("Wrong password");
            }
        }
        catch
        {
            Debug.LogWarning("Invalid username");
        }
    }
}

public class User
{
    public readonly string Username;
    public readonly int Balance;
    public readonly int Lives;
    public readonly string ID;
    public readonly Sprite Avatar;
    public User(string username, int balance, int lives, string id, Sprite avatar = null)
    {
        this.Username = username;
        this.Balance = balance;
        this.Lives = lives;
        this.ID = id;
        this.Avatar = avatar;
    }

    public User()
    {
        this.Username = null;
        this.Balance = 0;
        this.Lives = 0;
    }
}