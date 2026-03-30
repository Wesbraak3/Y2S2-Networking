using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Logger : MonoBehaviour
{
	const int maxChar = 128;
	TextMeshProUGUI text;
	public int maxLines;
	List<string> lines = new List<string>();

	public void AddText(string newtext) {
		string[] newlines = newtext.Split('\n');
		List<string> splitLines = new List<string>();

		foreach (string nl in newlines) {
			string oneLine = nl;
			while (oneLine.Length>maxChar) {
				int splitIndex = oneLine.LastIndexOf(' ', maxChar);
				if (splitIndex < maxChar / 2) splitIndex = maxChar;
				splitLines.Add(oneLine.Substring(0, splitIndex));
				oneLine = oneLine.Substring(splitIndex);
			}
			splitLines.Add(oneLine);
		}
		lines.AddRange(splitLines);
		while (lines.Count>maxLines) {
			lines.RemoveAt(0);
		}
		string output = "";
		foreach (string line in lines) {
			output += line + '\n';
		}
		text.text = output;
	}

	private void Awake() {
		text = GetComponent<TextMeshProUGUI>();
		if (!Application.isEditor) {
			string[] args = System.Environment.GetCommandLineArgs();
			foreach (string w in args) {
				AddText(w);
			}
		}
	}
}
