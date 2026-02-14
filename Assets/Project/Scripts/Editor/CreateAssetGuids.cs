using SubjectArena.Data;
using UnityEditor;

namespace SubjectArena.Editor
{
    public class CreateAssetGuids : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                var asset = AssetDatabase.LoadAssetAtPath<SubjectArenaBaseData>(assetPath);
                if (asset == null) { continue; }

                // GUID real do asset
                var realGuid = AssetDatabase.AssetPathToGUID(assetPath);

                if (string.IsNullOrEmpty(asset.Guid) || !string.Equals(asset.Guid, realGuid))
                {
                    // Atribui o GUID do Asset
                    asset.EditorSetGuid(realGuid);
                    // Salva o asset com o ID atualizado
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssetIfDirty(asset);
                }
            }
        }
    }
}