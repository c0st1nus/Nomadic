using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;

[Serializable]
public class Dict
{
    [SerializeField] public DictElement[] _dictElements;
    
    private Dictionary<playerStats, float> _internalDict;
    
    public Dict()
    {
        _internalDict = new Dictionary<playerStats, float>();
        try
        {
            foreach (var element in _dictElements)
            {
                _internalDict[element.Key] = element.Value;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }

    public float this[playerStats key]
    {
        get { return _internalDict[key]; }
        set { _internalDict[key] = value; }
    }

    public bool ContainsKey(playerStats key)
    {
        return _internalDict.ContainsKey(key);
    }

    public bool Remove(playerStats key)
    {
        return _internalDict.Remove(key);
    }

    public void Add(playerStats key, float value)
    {
        _internalDict.Add(key, value);
    }

    public void Clear()
    {
        _internalDict.Clear();
    }
}

[Serializable]
public class DictElement
{
    [SerializeField] public playerStats Key;
    [SerializeField] public float Value;
    public DictElement(){}

    public float this[playerStats vKey]
    {
        get { throw new NotImplementedException(); }
    }
}

public enum equality{
    equal,
    notEqual,
    greater,
    less,
    greaterOrEqual,
    lessOrEqual
}
[Serializable]
public class condition
{
    public equality conditionType;
    public playerStats stat;
    public float value;
}

