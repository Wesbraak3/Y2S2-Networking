using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UICreator : MonoBehaviour {
	[SerializeField]
	string filename;
	[SerializeField]
	CommandGroup CommandPrefab;

	List<ProtocolMessage> protocol = new List<ProtocolMessage>();

	void Start() {
		if (!Application.isEditor) {
			string[] args = System.Environment.GetCommandLineArgs();
			if (args.Length > 1) filename = args[1];
		}
		ReadFile(Application.streamingAssetsPath + "/" + filename + ".csv");
	}

	void AddMessage(ProtocolMessage mess) {
		protocol.Add(mess);
		Debug.Log("Message: " + mess);
		CommandGroup command = Instantiate(CommandPrefab, transform);
		command.CreateElements(mess);
	}

	void ReadFile(string filePath) {
		// Will show exception if file not found
		string[] lines = File.ReadAllLines(filePath);

		ProtocolMessage mess = null;

		for (int i = 0; i < lines.Length; i++) {
			string l = lines[i];
			// Split the line on commas, and trim whitespace for each word, using a LINQ iterator (Select):
			string[] words = l.Split(',').Select(s => s.Trim()).ToArray();
			Debug.Log($"Line {l} has {words.Length} words");

			if (words.Length > 0 && words[0].Length > 0) { // First column non-empty: new protocol message line
				if (mess != null) { // First, store the old message:
					AddMessage(mess);
				}
				mess = new ProtocolMessage("/" + words[0]);
			}
			if (words.Length > 2 && words[1].Length > 0 && words[2].Length > 0) {
				Debug.Log($"Line {i}: Parameter type {words[1]} name {words[2]}");
				// If mess is null here, there's a file format error!:
				mess.AddField(words[1], words[2]);
			}
		}
		if (mess != null) AddMessage(mess);
	}
}
