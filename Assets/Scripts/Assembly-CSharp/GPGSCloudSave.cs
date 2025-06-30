using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
/*using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;*/
using UnityEngine;

public class GPGSCloudSave : MonoBehaviour
{
	private static bool mSaving;

	public static bool syncWithCloud;

	public static void CloudSync(bool loadFromCloud)
	{
		Debug.Log("CloudSync, loadFromCloud - " + loadFromCloud);
	/*	if (PlayGamesPlatform.Instance.IsAuthenticated())
		{
			Debug.Log("Authenticated, start sync");
			mSaving = !loadFromCloud;
			if (!mSaving || syncWithCloud)
			{
				syncWithCloud = false;
				((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution("PlayerData.dat", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseMostRecentlySaved, SavedGameOpened);
			}
		}*/
	}

	/*public static void SavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		Debug.Log("SavedGameOpened");
		if (status == SavedGameRequestStatus.Success)
		{
			if (mSaving)
			{
				byte[] binary = GetBinary(DataLoader.playerData);
				Debug.Log("Saving to " + game);
				SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).Build();
				((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updateForMetadata, binary, SavedGameWritten);
			}
			else
			{
				((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, SavedGameLoaded);
			}
		}
		else
		{
			Debug.LogWarning("Error opening game: " + status);
			syncWithCloud = true;
		}
	}*/

	/*public static void SavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		Debug.Log("SavedGameWritten");
		if (status == SavedGameRequestStatus.Success)
		{
			Debug.Log("Game " + game.Description + " written");
		}
		else
		{
			Debug.LogWarning("Error saving game: " + status);
		}
		syncWithCloud = true;
	}*/

	/*public static void SavedGameLoaded(SavedGameRequestStatus status, byte[] data)
	{
		Debug.Log("SavedGameLoaded");
		if (status == SavedGameRequestStatus.Success)
		{
			if (data.Length <= 0)
			{
				Debug.Log("data in cloud is empty");
				syncWithCloud = true;
				return;
			}
			SaveData collection = GetCollection(data);
			if (collection != null)
			{
				if (collection.gamesPlayed <= DataLoader.playerData.gamesPlayed)
				{
					Debug.Log("Count games in cloud is less then at device, try load local data to cloud");
					syncWithCloud = true;
					CloudSync(false);
				}
				else
				{
					collection.CheckNewData();
					SaveManager.Save(collection, StaticConstants.PlayerSaveDataPath);
					DataLoader.dataUpdateManager.UpdateAfterConnect();
					DataLoader.playerData = collection;
					SetTutorialsComplete();
					if (DataLoader.gui != null)
					{
						DataLoader.gui.UpdateMenuContent();
					}
					Debug.Log("SaveGameLoaded, success=" + status);
				}
			}
		}
		else
		{
			Debug.LogWarning("Error reading game: " + status);
		}
		syncWithCloud = true;
	}
*/
	/*private static void ResolveConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
	{
		Debug.Log("Resolving conflict");
		if (originalData == null)
		{
			resolver.ChooseMetadata(unmerged);
			return;
		}
		if (unmergedData == null)
		{
			resolver.ChooseMetadata(original);
			return;
		}
		SaveData collection = GetCollection(originalData);
		SaveData collection2 = GetCollection(unmergedData);
		if (collection2 == null)
		{
			resolver.ChooseMetadata(original);
		}
		else if (collection == null)
		{
			resolver.ChooseMetadata(unmerged);
		}
		else if (collection.gamesPlayed > collection2.gamesPlayed)
		{
			resolver.ChooseMetadata(original);
		}
		else
		{
			resolver.ChooseMetadata(unmerged);
		}
	}
*/
	private static byte[] GetBinary(SaveData savesCollection)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, savesCollection);
		return memoryStream.GetBuffer();
	}

	private static SaveData GetCollection(byte[] data)
	{
		SaveData result = null;
		if (data.Length > 0)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(data);
			try
			{
				Debug.Log("Try deserialize data");
				result = (SaveData)binaryFormatter.Deserialize(serializationStream);
			}
			catch (Exception ex)
			{
				Debug.LogError("Deserialize is failed with error - " + ex.Message);
			}
		}
		return result;
	}

	private static void SetTutorialsComplete()
	{
		PlayerPrefs.SetInt(StaticConstants.TutorialCompleted, 1);
		PlayerPrefs.SetInt(StaticConstants.UpgradeTutorialCompleted, 1);
		PlayerPrefs.SetInt(StaticConstants.AbilityTutorialCompleted, 1);
		if (DataLoader.Instance.GetCurrentPlayerLevel() > 3)
		{
			PlayerPrefs.SetInt(StaticConstants.LeagueTutorialCompleted, 1);
		}
		PlayerPrefs.Save();
	}
}
