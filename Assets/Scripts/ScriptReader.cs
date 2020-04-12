using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ScriptReader : MonoBehaviour
{
    public TextAsset Script;
    public TextMeshProUGUI Prompt;
    List<ScriptPhrase> phrases = new List<ScriptPhrase>();

    private void Start()
    {
        Prompt = GetComponentInChildren<TextMeshProUGUI>();

        foreach(string line in Regex.Split(Script.text, "\n|\r|\r\n"))
        {
            phrases.Add(PhraseParser(line));
        }
    }

    void Update()
    {
        foreach(ScriptPhrase phrase in phrases)
        {
            if(Time.unscaledTime > phrase.time)
            {
                Prompt.text = phrase.text;
                ClearPrompt(phrase.text.Length * 0.1f);
                phrases.Remove(phrase);
                break;
            }
        }
    }

    IEnumerator ClearPrompt(float delay)
    {
        yield return new WaitForSeconds(delay);
        Prompt.text = "";
    }


    ScriptPhrase PhraseParser(string input)
    {
        try
        {
            int minutes, seconds;
            string min, sec;
            string timeString = input.Substring(0, input.IndexOf(',')); 
            min = Regex.Match(timeString, ".+?(?=:)").Value;
            minutes = int.Parse(min);
            sec = Regex.Match(timeString, "(?<=:).*").Value;
            seconds = int.Parse(sec);

            input = input.Remove(0, input.IndexOf('"') -1).Replace("\"", string.Empty);

            string inputPhrase = Regex.Match(input, "(\"[sS]*?\")").Value;

            return new ScriptPhrase
            {
                time = minutes * 60 + seconds,
                text = inputPhrase
            };
        }
        catch (Exception ex)
        {
            Debug.LogError("Script Parsing Failed on line:\n" + input);
            return null;
        }
        
    }

}

class ScriptPhrase
{
    public float time;
    public string text;
}
