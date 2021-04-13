using System.Collections.Generic;
using System;
using UnityEngine;
using System.Xml;
public class WordLists : MonoBehaviour
{
    public static WordLists instance = null;
    public Dictionary<Difficulty, List<string>> wordLists = new Dictionary<Difficulty, List<string>>();

    private void InitializeLists()
    {
        XmlDocument file;
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("text");
        file = new XmlDocument();
        file.LoadXml(xmlTextAsset.text);
        foreach (var diff in Enum.GetValues(typeof(Difficulty)))
        {
            wordLists.Add((Difficulty)diff.GetHashCode(), new List<string>());
            var lists = file.SelectNodes($"difficulties/{diff}/wordlist");
            var selectedList = lists[UnityEngine.Random.Range(0, lists.Count)]?.SelectNodes("word");
            for (int j = 0; j < selectedList?.Count; j++)
                wordLists[(Difficulty)diff.GetHashCode()].Add(selectedList[j].InnerText);
        }
    }

    public string GetRandomWord()
    {
        int modifier = GameManager.instance.difficulty.GetHashCode() > 4 ? 2 : 0;
        var diff = UnityEngine.Random.Range(1, GameManager.instance.difficulty.GetHashCode() + 1);
        diff = (diff + modifier) <= GameManager.instance.difficulty.GetHashCode() ? (diff + modifier) : diff;
        var list = wordLists[(Difficulty)diff];
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    void Start()
    {
        instance = this;
        InitializeLists();
    }
}
