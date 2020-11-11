using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    [RequireComponent (typeof(SpriteRenderer))]
    public class GameTile : MonoBehaviour
    {
        /// <summary>
        /// The sprite renderer of the object
        /// </summary>
        private SpriteRenderer _sRenderer = null;
        /// <summary>
        /// The image of the tile
        /// </summary>
        private int _imageIndex = -1;

        [SerializeField]
        [Tooltip("The tile up from this one.")]
        private GameTile up = null;
        [SerializeField]
        [Tooltip("The tile down from this one.")]
        private GameTile down = null;
        [SerializeField]
        [Tooltip("The tile left from this one.")]
        private GameTile left = null;
        [SerializeField]
        [Tooltip("The tile right from this one.")]
        private GameTile right = null;

        /// <summary>
        /// The image of the tile
        /// </summary>
        public int imageIndex => _imageIndex;

        /// <summary>
        /// Checks if the tile is movable
        /// </summary>
        public bool canMove => _imageIndex != -1
            && ((up?.imageIndex ?? 0) == -1
            || (down?.imageIndex ?? 0) == -1
            || (left?.imageIndex ?? 0) == -1
            || (right?.imageIndex ?? 0) == -1);

        /// <summary>
        /// On object creation
        /// </summary>
        protected void Awake ()
        {
            // Finds the sprite render
            _sRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Sets the index of the image.
        /// </summary>
        /// <param name="index"></param>
        public void SetSprite (int index)
        {
            _imageIndex = index;
            // Update tile itself
            SetSprite(_imageIndex >= 0 ? GameManager.current.image.sprites[_imageIndex] : null);
        }

        /// <summary>
        /// Changes the sprite of the current tile
        /// </summary>
        /// <param name="sprite">The new sprite. If null it applies the empty sprite</param>
        public void SetSprite (Sprite sprite)
        {
            if (sprite == null)
            {
                _sRenderer.sprite = GameGrid.current.empty;
                _sRenderer.color = GameGrid.current.emptyColor;
            }
            else
            {
                _sRenderer.sprite = sprite;
                _sRenderer.color = Color.white;
            }
            _sRenderer.size = Vector2.one;
        }

        /// <summary>
        /// Changes the current tile as a hover image.
        /// </summary>
        /// <param name="index">The index of the image. If null it applies the empty sprite</param>
        public void SetHover(int index)
        {
            if (index < 0)
            {
                SetSprite(null);
            }
            else
            {
                _sRenderer.sprite = GameManager.current.image.sprites[index];
                _sRenderer.color = GameGrid.current.hoverColor;
                _sRenderer.size = Vector2.one;
            }
        }

        /// <summary>
        /// On user touch the tile
        /// </summary>
        protected void OnMouseDown ()
        {
            if (GameManager.current.state != GameState.Going) return;

            if (TileSelection.current.selected == null && canMove)
            {
                TileSelection.current.SelectTile(this);
            }
        }

        /// <summary>
        /// On user hovers the tile (not sure it works with phones)
        /// </summary>
        protected void OnMouseOver ()
        {
            if (TileSelection.current.selected != null && (imageIndex == -1 || TileSelection.current.selected == this)) // Set target
            {
                TileSelection.current.SetTarget(this);
            }
        }
    }
}
