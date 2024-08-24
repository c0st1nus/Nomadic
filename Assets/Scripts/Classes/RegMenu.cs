using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegMenu : MonoBehaviour
{
    public GameObject regMenuRu;
    public GameObject regMenuKz;
    public InputField[] fieldsRu;
    public InputField[] fieldsKz;

    public void LangChangeRu()
    {
        regMenuRu.SetActive(false);
        regMenuKz.SetActive(true);
        PlayerPrefs.SetString("lang", "kz");
    }
    
    public void LangChangeKz()
    {
        regMenuRu.SetActive(true);
        regMenuKz.SetActive(false);
        PlayerPrefs.SetString("lang", "ru");
    }
    
    public void Start()
    {
        String result = PlayerPrefs.GetString("username");
        if (result != "")
        {
            SceneManager.LoadScene("main");
        }
    }
    
    public void Link(string url)
    {
        Application.OpenURL(url);
    }
    
    public void CheckUser()
    {
        var db = gameObject.GetComponent<DataBaseHandler>() == null
            ? gameObject.AddComponent<DataBaseHandler>()
            : gameObject.GetComponent<DataBaseHandler>();
        String username = regMenuRu.activeSelf ? TextHandler.RemoveWhitespace(fieldsRu[0].text) : TextHandler.RemoveWhitespace(fieldsKz[0].text); 
        Data[] query = db.Select($"SELECT * FROM users WHERE name = '{username}'");
        if (query[0].Password == (regMenuRu.activeSelf ? TextHandler.RemoveWhitespace(fieldsRu[1].text) : TextHandler.RemoveWhitespace(fieldsKz[1].text)))
        {
            PlayerPrefs.SetString("username", query[0].Username);
            SceneManager.LoadScene("main");
        }
        else if (query[0].Password != (regMenuRu.activeSelf ? TextHandler.RemoveWhitespace(fieldsRu[1].text) : TextHandler.RemoveWhitespace(fieldsKz[1].text)))
        {
            Debug.LogWarning("Wrong password");
        }
        else
        {
            Debug.LogWarning("Invalid username");
        }
    }
}

public class TextHandler
{
    public static string RemoveWhitespace(string input)
    {
        return Regex.Replace(input, @"\s+", "");
    }
}
