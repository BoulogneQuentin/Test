using Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof (SpriteRenderer))]
public class TileSelection : MonoBehaviour
{
    static private TileSelection _current = null;
    static public TileSelection current => _current;

    [SerializeField]
    [Tooltip("Triggers whenever a tile is successfully moved.")]
    private UnityEvent onMovedTile = new UnityEvent();
    /// <summary>
    /// The sprite renderer attached
    /// </summary>
    private SpriteRenderer _sRenderer = null;

    /// <summary>
    /// The selected tile.
    /// The tile being moved.
    /// </summary>
    private GameTile _selected = null;
    /// <summary>
    /// The target tile.
    /// The tile in which you can release the selection.
    /// </summary>
    private GameTile _target = null;
    /// <summary>
    /// The position of the finger in the scene.
    /// </summary>
    private Vector2 basePosition;
    /// <summary>
    /// The selected tile.
    /// The tile being moved
    /// </summary>
    public GameTile selected => _selected;
    /// <summary>
    /// The target tile.
    /// The tile in which you can release the selection.
    /// </summary>
    public GameTile target => _target;

    /// <summary>
    /// On object creation
    /// Check if it is not already instaciated
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

        _sRenderer = GetComponent<SpriteRenderer>();
        enabled = false;
    }

    /// <summary>
    /// On script activation
    /// </summary>
    protected void OnEnable ()
    {
        _sRenderer.enabled = true;
    }

    /// <summary>
    /// Update every frame
    /// </summary>
    protected void Update ()
    {
        Vector3? position = GameControls.TouchPositionInWorld();
        if (position == null) return;

        Vector2 delta = (Vector2)position.Value - basePosition;

        transform.position = selected.transform.position + (Vector3)delta;

        if (GameControls.TouchUp ())
        {
            ReleaseTile();
        }
    }

    /// <summary>
    /// On script deactivation
    /// </summary>
    protected void OnDisable ()
    {
        _sRenderer.enabled = false;
    }

    /// <summary>
    /// On object destroy
    /// </summary>
    protected void OnDestroy ()
    {
        if (_current == this)
        {
            _current = null;
        }
    }

    /// <summary>
    /// Select a givent tile
    /// </summary>
    /// <param name="tile">the tile to select</param>
    /// <returns>true if successfuly selected, false overwise</returns>
    public bool SelectTile (GameTile tile)
    {
        if (tile == _selected) return false;

        Vector3? position = GameControls.TouchPositionInWorld();
        if (position == null) return false;

        _selected = tile;
        basePosition = position.Value;
        transform.position = tile.transform.position;

        _sRenderer.sprite = GameManager.current.image.sprites[tile.imageIndex];
        _sRenderer.size = Vector2.one;

        enabled = true;
        return true;
    }

    /// <summary>
    /// Set the tile as a target
    /// </summary>
    /// <param name="tile">The tile to set as a target</param>
    /// <returns>true if the tile is successfuly set as the target, false otherwise</returns>
    public bool SetTarget (GameTile tile)
    {
        if (tile == _target) return false;
        if (_target != null) _target.SetHover(-1);
        _target = tile;
        if (_target != null) _target.SetHover(_selected.imageIndex);
        return true;
    }

    /// <summary>
    /// Release the tile in it's new place.
    /// </summary>
    /// <returns>true if it is a new position, false if it's the old one</returns>
    public bool ReleaseTile ()
    {
        bool result = false;
        if (_target != null && _target != _selected)
        {
            _target.SetSprite(_selected.imageIndex);
            _selected.SetSprite(-1);

            onMovedTile.Invoke();
            result = true;
        }
        else
        {
            _selected.SetSprite(_selected.imageIndex);
        }
        _target = null;
        _selected = null;
        enabled = false;
        return result;
    }
}
