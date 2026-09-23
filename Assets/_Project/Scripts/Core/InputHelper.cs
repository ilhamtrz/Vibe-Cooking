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
                return Vector3.zero;
#else
                return Input.mousePosition;
#endif
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
            return false;
#else
            return Input.GetMouseButtonDown(button);
#endif
        }

        public static bool IsMouseButton(int button = 0)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                if (button == 0) return Mouse.current.leftButton.isPressed;
                if (button == 1) return Mouse.current.rightButton.isPressed;
            }
            return false;
#else
            return Input.GetMouseButton(button);
#endif
        }

        public static bool IsMouseButtonUp(int button = 0)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                if (button == 0) return Mouse.current.leftButton.wasReleasedThisFrame;
                if (button == 1) return Mouse.current.rightButton.wasReleasedThisFrame;
            }
            return false;
#else
            return Input.GetMouseButtonUp(button);
#endif
        }

        public static Vector3 GetMouseWorldPosition(Camera cam = null)
        {
            if (cam == null) cam = Camera.main;
            if (cam == null) return Vector3.zero;

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -cam.transform.position.z));
                worldPos.z = 0f;
                return worldPos;
            }
            return Vector3.zero;
#else
            Vector3 legacyPos = Input.mousePosition;
            legacyPos.z = -cam.transform.position.z;
            Vector3 res = cam.ScreenToWorldPoint(legacyPos);
            res.z = 0f;
            return res;
#endif
        }
    }
}
