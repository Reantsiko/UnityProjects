using System.Collections.Generic;
using System;
using UnityEngine;
using System.Xml;
public class WordLists : MonoBehaviour
{
    public static WordLists instance = null;
    public Dictionary<string, List<string>> wordLists = new Dictionary<string, List<string>>();
    [Header("XML dependencies")]
    [SerializeField] private string wordlistName = null;
    [Tooltip("Tag for the wordlists in the XML file.")]
    [SerializeField] private string wordlistTag = "wordlist";
    [Tooltip("Tag for the words within the lists of the XML file.")]
    [SerializeField] private string wordTag = "word";

    private void InitializeLists()
    {
        if (string.IsNullOrEmpty(wordlistName))
        {
            Debug.LogError($"No filename for the wordlist!");
            return;
        }
        XmlDocument file = new XmlDocument(); ;
        TextAsset xmlTextAsset = Resources.Load<TextAsset>(wordlistName);
        file.LoadXml(xmlTextAsset.text);
        foreach (var diff in Enum.GetValues(typeof(Difficulty)))
        {
            wordLists.Add(diff.ToString(), new List<string>());
            var lists = file.SelectNodes($"difficulties/{diff.ToString()}/{wordlistTag}");
            var selectedList = lists[UnityEngine.Random.Range(0, lists.Count)]?.SelectNodes($"{wordTag}");
            for (int i = 0; i < selectedList?.Count; i++)
                wordLists[diff.ToString()].Add(selectedList[i].InnerText);
        }
    }

    public string GetRandomWord()
    {
        int modifier = GameManager.instance.difficulty.GetHashCode() > 4 ? 2 : 0;
        var diff = UnityEngine.Random.Range(1, GameManager.instance.difficulty.GetHashCode() + 1);
        diff = (diff + modifier) <= GameManager.instance.difficulty.GetHashCode() ? (diff + modifier) : diff;
        var selecteDiff = (Difficulty)diff;
        var list = wordLists[selecteDiff.ToString()];
        return list.Count != 0 ? list[UnityEngine.Random.Range(0, list.Count)] : string.Empty;
    }

    void Start()
    {
        instance = this;
        InitializeLists();
    }
}
