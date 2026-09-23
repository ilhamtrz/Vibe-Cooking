using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VibeCooking.Editor
{
    public static class ResetMCPHistory
    {
        [MenuItem("Tools/Reset MCP Approval History")]
        public static void ClearHistory()
        {
            try
            {
                var storeType = Type.GetType("Unity.AI.MCP.Editor.ConnectionStore, Unity.AI.MCP.Editor");
                if (storeType != null)
                {
                    var clearMethod = storeType.GetMethod("ClearAllConnections", BindingFlags.Public | BindingFlags.Static);
                    clearMethod?.Invoke(null, null);
                    Debug.Log("<color=green>[MCP]</color> Connection history cleared successfully! Next connection will show the approval popup.");
                }
                else
                {
                    Debug.LogWarning("[MCP] ConnectionStore type not found.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[MCP] Failed to reset MCP connection history: {e.Message}");
            }
        }
    }
}
