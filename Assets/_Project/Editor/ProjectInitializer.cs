using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityFoundation.Services;

namespace UnityFoundation.Editor
{
    [InitializeOnLoad]
    public static class ProjectInitializer
    {
        static ProjectInitializer()
        {
            EnsureLogSettings();
            EnsureDefineSymbol("ENABLE_LOG");
        }

        private static void EnsureLogSettings()
        {
            const string path = "Assets/_Project/Resources/LogSettings.asset";
            if (AssetDatabase.LoadAssetAtPath<LogSettings>(path) != null) return;

            var asset = ScriptableObject.CreateInstance<LogSettings>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Debug.Log("[ProjectInitializer] Created LogSettings.asset");
        }

        private static void EnsureDefineSymbol(string define)
        {
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup);

            PlayerSettings.GetScriptingDefineSymbols(namedTarget, out string[] defines);
            if (defines.Contains(define)) return;

            PlayerSettings.SetScriptingDefineSymbols(namedTarget, defines.Append(define).ToArray());
            Debug.Log($"[ProjectInitializer] Added '{define}' scripting define symbol");
        }
    }
}
