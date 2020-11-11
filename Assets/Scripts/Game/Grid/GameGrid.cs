using SObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Grid
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GameGrid : MonoBehaviour
    {
        static private GameGrid _current = null;
        static public GameGrid current => _current;

#if UNITY_EDITOR
        // Only give access to the random seed if in unity editor
        // Use  this to test/retest bad random cases
        [SerializeField]
        [Tooltip("The random seed used when shuffling. Set to 0 or less for a non seed using game.")]
        private int randomSeed = 9;
#endif
        [SerializeField]
        [Tooltip("The image of an empty tile.")]
        private Sprite _empty = null;
        [SerializeField]
        [Tooltip("The image color of an empty tile.")]
        private Color _emptyColor = new Color(1, 1, 1, 0f);

        [SerializeField]
        [Tooltip("The color of the image when hovers durung tile movement.")]
        private Color _hoverColor = new Color(1, 1, 1, 0.5f);

        /// <summary>
        /// The tiles of the grid.
        /// From top left to bottom right.
        /// </summary>
        private GameTile[] tiles = null;

        /// <summary>
        /// The image of an empty tile.
        /// </summary>
        public Sprite empty => _empty;
        /// <summary>
        /// The image color of an empty tile.
        /// </summary>
        public Color emptyColor => _emptyColor;
        /// <summary>
        /// The color of the image when hovers durung tile movement.
        /// </summary>
        public Color hoverColor => _hoverColor;

        /// <summary>
        /// Ensures this gameobject is not instanciated twice !
        /// </summary>
        private void Awake ()
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
            tiles = GetComponentsInChildren<GameTile>();
        }

        /// <summary>
        /// Make sure to destroy the reference to this object, as it will not exist anymore.
        /// </summary>
        protected void OnDestroy()
        {
            if (_current == this)
            {
                _current = null;
            }
        }

        /// <summary>
        /// After Awake and OnEnbaled
        /// </summary>
        protected void Start ()
        {
            // Assignes the image in the right order
            for (int i = 0; i < GameManager.current.image.sprites.Length; i++)
            {
                tiles[i].SetSprite(i == GameManager.current.emptyTileIndex ? -1 : i);
            }
        }

        /// <summary>
        /// Shuffles the grid.
        /// </summary>
        public void Shuffle ()
        {
#if UNITY_EDITOR
            // Seed usage (for test and debug only !)
            if (randomSeed > 0) Random.InitState (randomSeed);
#endif
            // Get the random amount of shuffles
            // Get the grid to shuffle
            List<int> grid = CreateGridArray();
            // Shuffle the image
            for (int i = 0; i < GameManager.current.image.sprites.Length - 1; i++)
            {
                int random = Random.Range(i + 1, grid.Count);
                Shuffle(grid, i, random);
            }

            // Make the grid solvable only if it's not
            if (!IsSolvable (grid))
            {
                MakeSolvable (grid);
            }

            // Assignes the new images
            for (int i = 0; i < grid.Count; i++)
            {
                tiles[i].SetSprite(grid[i]);
            }

            GameManager.current.SetState(GameState.Going);
        }

        /// <summary>
        /// Creates the array for the shuffle.
        /// The data stored in it is the image tile index.
        /// The position in the array is the position in the grid.
        /// </summary>
        /// <returns>The array of positions</returns>
        protected List<int> CreateGridArray ()
        {
            List<int> positions = new List<int> ();
            // Init the grid
            for (int i = 0; i < GameManager.current.image.sprites.Length; i++)
            {
                positions.Add (GameManager.current.emptyTileIndex == i ? -1 : i);
            }
            return positions;
        }

        /// <summary>
        /// Checks if a grid is solvable.
        /// </summary>
        /// <param name="grid">The grid to check.</param>
        /// <returns>true if solvable, false otherwise</returns>
        protected bool IsSolvable (List<int> grid)
        {
            List<int> copy = new List<int> (grid);
            int emptyIndex = copy.IndexOf(-1);
            // Get the number of transposition for the empty tile
            int emptyDistance = Mathf.Abs(GameManager.current.emptyTileIndex - emptyIndex);
            // Place the empty index
            copy.RemoveAt(emptyIndex);
            copy.Insert(GameManager.current.emptyTileIndex, -1);
            // The transpotision amount for the rest of the grid.
            int transpositionNumber = 0;
            for (int i = copy.Count - 1; i >= 0; i--)
            {
                if (i == copy[i] || i == GameManager.current.emptyTileIndex) continue;
                transpositionNumber++;
                Shuffle(copy, i, copy.IndexOf(i));
            }
            // Check if both numbers are odd or both are even
            return (transpositionNumber + emptyDistance) % 2 == 1;
        }

        /// <summary>
        /// Turns un unsolvable grid into a solvable one.
        /// </summary>
        /// <param name="grid">An unsolvable grid</param>
        protected void MakeSolvable (List<int> grid)
        {
            // The aim is to change the number of transposition by 1
            // We just need to shuffle two tiles so that one will be well placed but not the other
            for (int i = 0; i < grid.Count; i++)
            {
                if (i == GameManager.current.emptyTileIndex) continue;
                int index = grid.IndexOf(i);
                if (grid[i] == index) continue; // They will be both well placed
                Shuffle(grid, index, i); // Only one of the two will be well placed
                return;
            }
        }

        /// <summary>
        /// Shuffles two position in the array.
        /// </summary>
        /// <param name="positions">The array in which shuffle</param>
        /// <param name="first">The index to shuffle with 'second'</param>
        /// <param name="last">The index to shuffle with 'first'</param>
        protected void Shuffle (List<int> positions, int first, int last)
        {
            int tmp = positions[first];
            positions[first] = positions[last];
            positions[last] = tmp;
        }

        /// <summary>
        /// Function called to check if the game is finiched by the victory of the player
        /// </summary>
        public void CheckVictory ()
        {
            // Check if the game is running
            if (GameManager.current.state != GameState.Going) return;

            // Check if all tiles are well placed
            for (int i = 0; i < tiles.Length; i++)
            {
                if (i == GameManager.current.emptyTileIndex) continue;
                if (tiles[i].imageIndex != i) return;
            }
            // Set the game state as won
            GameManager.current.SetState(GameState.Won);
        }
    }
}
