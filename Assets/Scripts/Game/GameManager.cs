using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SObjects;
using UnityEngine.Events;

public enum GameState
{
    Ready = 0,
    Going,
    Won,
    Lost
}

/// <summary>
/// Singleton that globally configures and manages the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    static private GameManager _current = null;
    static public GameManager current => _current;

    // Configurable variables
    [SerializeField]
    [Tooltip("The game allotted time in seconds.")]
    private float _allottedTime = 120f;
    [SerializeField]
    [Tooltip("Image used by IOS devices.")]
    private GridImageScriptableObject _IOSImage = null;
    [SerializeField]
    [Tooltip("Image used by Android devices.")]
    private GridImageScriptableObject _AndroidImage = null;
    [SerializeField]
    [Tooltip("The position of the empty tile in the game grid.")]
    [Range(1, 9)]
    private int _emptyTilePosition = 5;

    [SerializeField]
    [Tooltip("Triggered when game is won (state = Won).")]
    private UnityEvent onVictory = new UnityEvent();
    [SerializeField]
    [Tooltip("Triggered when game is lost (state = Lost).")]
    private UnityEvent onLost = new UnityEvent();
    [SerializeField]
    [Tooltip("Triggered when game just started (state = Going).")]
    private UnityEvent onStart = new UnityEvent();
    /// <summary>
    /// The state of the game.
    /// </summary>
    private GameState _state = GameState.Ready;

    /// <summary>
    /// The game allotted time in seconds.
    /// </summary>
    public float allotedTime => _allottedTime;
    /// <summary>
    /// Image used by the game.
    /// </summary>
    public GridImageScriptableObject image
    {
        get
        {
        // By default, they use the andriod image, even for testing on the editor.
        // They use the IOS image when the game is compiled for IOS only.
#if UNITY_IPHONE
            return _IOSImage;
#else
            return _AndroidImage;
#endif
        }
    }

    /// <summary>
    /// The position of the empty tile in the game grid.
    /// </summary>
    public int emptyTileIndex => _emptyTilePosition - 1;

    public GameState state => _state;

    /// <summary>
    /// Ensures this gameobject is not instanciated twice !
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
        }
    }

    /// <summary>
    /// Make sure to destroy the reference to this object, as it will not exist anymore.
    /// </summary>
    protected void OnDestroy ()
    {
        if (_current == this)
        {
            _current = null;
        }
    }

    /// <summary>
    /// Sets the state of the game.
    /// Accessible through UnityEditor.
    /// </summary>
    /// <param name="state"></param>
    public void SetState (GameState state)
    {
        _state = state;

        switch (state)
        {
            case GameState.Won:
                onVictory.Invoke();
                break;
            case GameState.Lost:
                onLost.Invoke();
                break;
            case GameState.Going:
                onStart.Invoke();
                break;
        }
    }
}
