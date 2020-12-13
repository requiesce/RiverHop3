using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

[System.Serializable] public class Score
{
    public Score(string s, float t)
    {
        name = s;
        time = t;
    }

    public string name;
    public float time;
}


public class ScoreList : MonoBehaviour
{
    public List<Score> scores = new List<Score>();
    public string fileName;
    public GameObject input;

    public GameObject finalPanel;
    public GameObject scorePanel;
    public GameObject entryPrefab;
   

    // Start is called before the first frame update
    void Start()
    {
        if (File.Exists(fileName))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                while (true)
                {
                    try
                    {
                        string name = reader.ReadString();
                        float time = reader.ReadSingle();

                        scores.Add(new Score(name, time));
                    }
                    catch (EndOfStreamException)
                    {
                        break;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewEntry()
    {
        // get name input from input field
        string name = input.GetComponent<TMP_InputField>().text;
        // get GameData component
        // get name current time and subtract gamePlayStart
        float time = Time.time - GameData.GamePlayStart;

        scores.Insert(0, new Score(name, time)); // Add()s at position 0 - front of list

        int offset = 0;
        foreach(Score score in scores)
        {
            GameObject temp = GameObject.Instantiate(entryPrefab);
            Transform[] children = temp.GetComponentsInChildren<Transform>();
            children[1].GetComponent<TextMeshProUGUI>().text = score.name;
            children[2].GetComponent<TextMeshProUGUI>().text = score.time.ToString("F2"); // float with 2 points of precision

            temp.transform.SetParent(scorePanel.transform);
            RectTransform rtrans = temp.GetComponent<RectTransform>();
            rtrans.anchorMin = new Vector2(0.5f, 0.5f);
            rtrans.anchorMax = new Vector2(0.5f, 0.5f);
            rtrans.pivot = new Vector2(0.5f, 0.5f);
            rtrans.localPosition = new Vector3(0, offset, 0);

            offset += 35;

            // can use List<T>.Sort and customise based on Score object
        }

        finalPanel.SetActive(false);
        scorePanel.SetActive(true);
    }

    private void OnDestroy()
    {
        using (BinaryWriter write = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate)))
        {
            foreach(Score score in scores)
            {
                write.Write(score.name);
                write.Write(score.time);
            }
        }
    }
}
