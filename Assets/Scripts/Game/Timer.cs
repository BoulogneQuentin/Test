using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    static private Timer _current = null;
    static public Timer current => _current;

    [SerializeField]
    [Tooltip("The text in wich the time will be displayed.")]
    private Text text;
    /// <summary>
    /// The time remaining before the game is lost
    /// </summary>
    private float remainingTime;

    /// <summary>
    /// On script creation
    /// </summary>
    protected void Awake ()
    {
        if (_current == null)
        {
            _current = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        enabled = false;
    }

    /// <summary>
    /// On script activation
    /// </summary>
    protected void OnEnable ()
    {
        remainingTime = GameManager.current.allotedTime;
    }

    /// <summary>
    /// Updates every frame
    /// </summary>
    protected void Update ()
    {
        if (GameManager.current.state != GameState.Going)
        {
            enabled = false;
            return;
        }

        remainingTime -= Time.deltaTime;
        // Check if it is the end !
        if (remainingTime <= 0)
        {
            GameManager.current.SetState(GameState.Lost);
            remainingTime = 0;
        }

        text.text = remainingTime.ToString("000.00");
    }

    /// <summary>
    /// On object destroy
    /// </summary>
    protected void OnDestroy()
    {
        if (_current == this)
        {
            _current = null;
        }
    }

    /// <summary>
    /// Get the time score of the last game.
    /// </summary>
    /// <returns>The time score.</returns>
    public float TimeScore ()
    {
        return GameManager.current.allotedTime - remainingTime;
    }
}
