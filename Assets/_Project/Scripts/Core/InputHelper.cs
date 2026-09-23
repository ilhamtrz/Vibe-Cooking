using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace VibeCooking
{
    public static class InputHelper
    {
        public static Vector3 MouseScreenPosition
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                if (Mouse.current != null)
                {
                    Vector2 pos = Mouse.current.position.ReadValue();
                    return new Vector3(pos.x, pos.y, 0f);
                }
#endif
                return Input.mousePosition;
            }
        }

        public static bool IsMouseButtonDown(int button = 0)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                if (button == 0) return Mouse.current.leftButton.wasPressedThisFrame;
                if (button == 1) return Mouse.current.rightButton.wasPressedThisFrame;
            }
#endif
            return Input.GetMouseButtonDown(button);
        }

        public static bool IsMouseButton(int button = 0)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                if (button == 0) return Mouse.current.leftButton.isPressed;
                if (button == 1) return Mouse.current.rightButton.isPressed;
            }
#endif
            return Input.GetMouseButton(button);
        }

        public static bool IsMouseButtonUp(int button = 0)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                if (button == 0) return Mouse.current.leftButton.wasReleasedThisFrame;
                if (button == 1) return Mouse.current.rightButton.wasReleasedThisFrame;
            }
#endif
            return Input.GetMouseButtonUp(button);
        }

        public static Vector2 GetMouseWorldPosition(Camera cam = null)
        {
            if (cam == null) cam = Camera.main;
            if (cam == null) return Vector2.zero;
            Vector3 screenPos = MouseScreenPosition;
            screenPos.z = -cam.transform.position.z;
            return cam.ScreenToWorldPoint(screenPos);
        }
    }
}
