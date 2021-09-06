using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

namespace Battle
{
    public class UI_Navigation : MonoBehaviour
    {
        public int[] yLeave = new int[3];
        [Header("Values")]
        [SerializeField] int yIndex = 0;
        public int YIndex { get => yIndex; }
        [SerializeField] int xIndex = 0;
        public int XIndex { get => xIndex; }
       
        public int ySize = 4;

        /// <summary>
        /// New Value, Direction
        /// </summary>
        [HideInInspector] public UnityEvent<int, int> onYIndexChange;
        /// <summary>
        /// New Value, Direction
        /// </summary>
        [HideInInspector] public UnityEvent<int, int> onXIndexChange;

        public void Reset()
        {
            for (int i = 0; i < yLeave.Length; i++)
            {
                yLeave[i] = 0;
            }
            yIndex = 0;
            xIndex = 0;
            ySize = 4;
            onYIndexChange.Invoke(0, 0);
            onXIndexChange.Invoke(0, 0);
        }
        public void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
                return;
            Vector2 dir = ctx.ReadValue<Vector2>();
            IndexNavigation(dir);
        }
        public void IndexNavigation(Vector2 dir)
        {
            if (dir.y != 0)
            {
                int v = Extensions.LoopValue(dir.y < 0 ? YIndex + 1 : YIndex - 1, ySize);
                if (yIndex == v)
                    return;
                yIndex = v;
                onYIndexChange.Invoke(YIndex, dir.y < 0 ? 1 : -1);
            }
            else if (dir.x != 0)
            {
                yLeave[xIndex] = YIndex;
                xIndex = Mathf.Clamp(dir.x > 0 ? XIndex + 1 : XIndex - 1, 0, yLeave.Length - 1);
                onXIndexChange.Invoke(XIndex, dir.x > 0 ? 1 : -1);
                yIndex = yLeave[xIndex];
            }
        }
        public void OnSubmit(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
                return;
            IndexNavigation(Vector2.right);
        }
    }
}
