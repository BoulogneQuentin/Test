using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The save file's name.")]
    private string saveFile = "/data.save";
    [SerializeField]
    [Tooltip("The high score banner.")]
    private Text text = null;

    private float highScore = 0;

    private string file => Application.persistentDataPath + saveFile;

    public void Awake ()
    {
        highScore = GetHighScore();
        Display();
    }

    /// <summary>
    /// Chack if it is a new high score and save it if it is
    /// </summary>
    public void CheckAndSaveScore ()
    {
        // Get the score
        float score = Timer.current.TimeScore();
        // Check if its higher
        if (highScore > score) return;
        // Set the new high score
        highScore = SetHighScore(score);
        Display();
    }

    /// <summary>
    /// Retrieves the high score from the save file file
    /// </summary>
    /// <returns>The high score</returns>
    private float GetHighScore ()
    {
        if (!File.Exists(file)) return -1;

        using (FileStream fs = new FileStream (file, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (float)bf.Deserialize(fs);
        }
    }

    /// <summary>
    /// Save the high score in the save file
    /// </summary>
    /// <returns>The high score</returns>
    private float SetHighScore(float value)
    {
        using (FileStream fs = new FileStream(file, FileMode.Create))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, value);
            return value;
        }
    }

    /// <summary>
    /// Displays the high scorre or hide it if necessary.
    /// </summary>
    private void Display ()
    {
        text.text = "High score: " + highScore.ToString("000.00");
        text.gameObject.SetActive(highScore > 0);
    }
}
