using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class ScriptableObjectCreatorOnInspector
    {
        static ScriptableObjectCreatorOnInspector()
        {
            Editor.finishedDefaultHeaderGUI -= OnGUI;
            Editor.finishedDefaultHeaderGUI += OnGUI;
        }

        private static void OnGUI( Editor editor )
        {
            if ( !EditorUtility.IsPersistent( editor.target ) ) return;

            var assetPath = AssetDatabase.GetAssetPath( editor.target );
            var type      = AssetDatabase.GetMainAssetTypeAtPath( assetPath );

            if ( type != typeof( MonoScript ) ) return;

            var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>( assetPath );

            if ( monoScript == null ) return;

            var classType = monoScript.GetClass();

            if ( classType == null ) return;

            var isScriptableObject = classType.IsSubclassOf( typeof( ScriptableObject ) );

            if ( !isScriptableObject ) return;
            if ( !GUILayout.Button( "Create Asset" ) ) return;

            var fullPath = EditorUtility.SaveFilePanel
            (
                title: "",
                directory: "Assets",
                defaultName: classType.Name,
                extension: "asset"
            );

            if ( string.IsNullOrWhiteSpace( fullPath ) ) return;

            var relativePath = FileUtil.GetProjectRelativePath( fullPath );
            var instance     = ScriptableObject.CreateInstance( classType );

            AssetDatabase.CreateAsset( instance, relativePath );
            AssetDatabase.Refresh();
        }
    }
}