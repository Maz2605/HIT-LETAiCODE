using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadSystem
{

	public static bool HasKey(string key, string folder = "")
	{
		string path = Application.persistentDataPath;
		if (folder.Length == 0)
			path += "/" + key + ".vp";
		else
			path += "/" + folder + "/" + key + ".vp";
		return File.Exists(path);
	}

	public static void Save<T>(T data, string key, string folder = "")
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath;
		if (folder.Length > 0)
		{
			path += "/" + folder;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
		path += "/" + key + ".vp";

		FileStream stream = new FileStream(path, FileMode.Create);

		formatter.Serialize(stream, data);
		stream.Close();
	}

	public static T Load<T>(string key, string folder = "")
	{
		string path = Application.persistentDataPath;
		Debug.Log(path);
		if (folder.Length == 0)
			path += "/" + key + ".vp";
		else
			path += "/" + folder + "/" + key + ".vp";

		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			T data = (T)formatter.Deserialize(stream);
			stream.Close();
			Debug.Log("save file found in " + path);
			return data;
		}

		Debug.Log("save file not found in " + path);
		return default;
	}

	public static void Delete(string key, string folder = "")
	{
		if (!HasKey(key, folder)) return;

		string path = Application.persistentDataPath;
		if (folder.Length == 0)
			path += "/" + key + ".vp";
		else
			path += "/" + folder + "/" + key + ".vp";
		File.Delete(path);

	}

	public static void Clear()
	{
		string path = Application.persistentDataPath;
		Debug.Log(Application.persistentDataPath);
		DirectoryInfo di = new DirectoryInfo(path);

		if (di != null)
		{
			foreach (FileInfo file in di.GetFiles())
			{
				file.Delete();
			}
			foreach (DirectoryInfo dir in di.GetDirectories())
			{
				dir.Delete(true);
			}
		}

		PlayerPrefs.DeleteAll();
	}



	public static void SaveText(string data, string key, string folder)
	{
		string path = Application.persistentDataPath;
		if (folder.Length > 0)
		{
			path += "/" + folder;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
		path += "/" + key + ".txt";

		File.WriteAllText(path, data);
	}

	public static string LoadText(string key, string folder)
	{
		string path = Application.persistentDataPath;
		if (folder.Length > 0)
		{
			path += "/" + folder;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
		path += "/" + key + ".txt";

		if (File.Exists(path))
			return File.ReadAllText(path);

		return null;
	}

}
