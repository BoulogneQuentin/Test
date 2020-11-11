using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SObjects
{
    /// <summary>
    /// Class used to describe and contain the grid image splitted into ordered pieces.
    /// </summary>
    [CreateAssetMenu(fileName = "GridImage", menuName = "ScriptableObjects/GridImage", order = 1)]
    public class GridImageScriptableObject : ScriptableObject
    {
        public Sprite[] sprites;
    }
}