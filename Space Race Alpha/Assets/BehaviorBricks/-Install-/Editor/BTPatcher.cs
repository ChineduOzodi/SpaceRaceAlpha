using System.Reflection;
using UnityEditor;
using UnityEngine;

///////////////////////////////////////////////////////////////////////////////////////////
// PLEASE, REMEMBER TO BACKUP YOUR PROJECT BEFORE IMPORTING OR UPDATING BEHAVIOR BRICKS. //
///////////////////////////////////////////////////////////////////////////////////////////

/// Use this patcher ONLY if you are UPDATING Behavior Bricks from a previous version.
/// Enable Visible Meta Files and Turn on Force Text Serialization before proceeding with the update.
/// 
/// If you cannot see your own behaviors in the BB Editor after the patching process,
///		try deleting the assetDatabase3 file and then restart Unity.
///		(into directory: "/ProjectName/Library/assetDatabase3") 
///
/// See IntallationGuide.pdf for more details.


/// <summary>
/// Patching process to update the old BrickAssets (v0.1) to the current version.
/// 
/// This script will change the old fileID referenced in your BrickAssets to the new fileID.
/// It's recommended to restart Unity after the patching process.
/// 
/// After the process is finished, this script can be deleted.
/// </summary>

namespace BBPatch
{
	public class BTPatcher
	{

		/// <summary>
		/// Patch old BrickAssets Menu
		/// </summary>
		[UnityEditor.MenuItem("Window/Behavior Bricks/Patch old BrickAssets", false, 40)]
		static void PatchBrickAssets()
		{
			if (UnityEditor.EditorSettings.serializationMode != SerializationMode.ForceText)
			{
				EditorUtility.DisplayDialog("Force Text Serialization not detected",
						"Please, remember to BACKUP YOUR PROJECT BEFORE UPDATING Behavior Bricks." +
						"\n\nThis patching process must be done BEFORE updating the plugin, and with Force Text Serialization enabled." +
						"\n\nRead carefully and follow the Installation Guide instructions to update the plugin correctly.",
						"Ok");
				return;
			}

			if (EditorUtility.DisplayDialog("Patch all the BrickAsset files?",
						"Please, remember to BACKUP YOUR PROJECT BEFORE UPDATING Behavior Bricks." +
						"\nRead carefully and follow the Installation Guide instructions to update the plugin correctly." +
						"\n\nAre you sure you want to update all the OLD BrickAssets of the current project?",
						"Continue", "Cancel"))
			{
				if (PatchingProcess())
					Debug.Log("PATCHING PROCESS DONE. You can delete the content of the BehaviorBricks directory, and then import the new package.");
				else
					Debug.LogError("UNEXPECTED ERROR IN PATCHING PROCESS. Please, try restarting Unity or patch the BrickAssets manually.");
			}
		}

		private static bool PatchingProcess()
		{
			// 1.- Get the current directory
			string toBePatched = System.Environment.CurrentDirectory;

			if (toBePatched == null)
			{
				Debug.LogWarning("Project to patch not found!");
				return false;
			}

			// 2.- Browse BrickAssets and patch it
			string[] brickAssetsGUIDs = AssetDatabase.FindAssets("t:BrickAsset");

			if (brickAssetsGUIDs.Length == 0)
			{
				Debug.LogWarning("BrickAssets not found in the project.");
				return false;
			}

			foreach (string brickAssetGUID in brickAssetsGUIDs)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(brickAssetGUID);
				string file = System.IO.Directory.GetFiles(assetPath, "*.asset", System.IO.SearchOption.TopDirectoryOnly)[0];

				patch(file);
			}

			return true;
		}

		private static string pattern = "m_Script: {fileID: 1268376496, guid: 34a7c8ca992f915438a96c2077353778, type: 3}";
		private static string replaceWith = "m_Script: {fileID: 11500000, guid: 34a7c8ca992f915438a96c2077353778, type: 3}";

		private static void patch(string file)
		{
			Debug.Log("Patching: " + file);

			// Overwrite the file info
			string fileContent = System.IO.File.ReadAllText(file);

			if (System.Text.RegularExpressions.Regex.Match(fileContent, replaceWith).Success)
			{
				Debug.Log("BrickAsset already patched.");
				return;
			}

			if (!System.Text.RegularExpressions.Regex.Match(fileContent, pattern).Success)
			{
				Debug.Log("Unable to patch BrickAsset. Original fileID and guid not found.");
				return;
			}

			System.IO.File.WriteAllText(file,
				System.Text.RegularExpressions.Regex.Replace(fileContent, pattern, replaceWith));
			Debug.Log("BrickAsset patched correctly.");

			return;
		}
	}
}