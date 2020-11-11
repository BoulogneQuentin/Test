using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GameControls
{
    /// <summary>
    /// Get the first finger position in the 2D world.
    /// </summary>
    /// <returns>The position, null if no touch found</returns>
    static public Vector3? TouchPositionInWorld ()
    {
#if UNITY_EDITOR
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs (Camera.main.transform.position.z));
        return Camera.main.ScreenToWorldPoint(position);
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 position = new Vector3(touch.position.x, touch.position.y, Mathf.Abs (Camera.main.transform.position.z));
            return Camera.main.ScreenToWorldPoint(position);
        }
        return null;
#endif
    }

    /// <summary>
    /// Checks if the finger is off the screen
    /// </summary>
    /// <returns></returns>
    static public bool TouchUp ()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            return touch.phase == TouchPhase.Ended;
        }
        return true;
#endif
    }
}
