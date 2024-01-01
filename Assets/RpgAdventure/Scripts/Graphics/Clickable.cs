using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public class Clickable : MonoBehaviour
    {
        public Texture2D questionCursor;

        private void OnMouseEnter()
        {
            Vector2 hotspot = new Vector2(questionCursor.width / 2, questionCursor.height / 2);
            Cursor.SetCursor(questionCursor, hotspot, CursorMode.Auto);
        }

        private void OnMouseExit()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

    }
}

