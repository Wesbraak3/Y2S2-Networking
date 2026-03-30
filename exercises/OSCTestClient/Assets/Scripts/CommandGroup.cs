using UnityEngine;
using TMPro;
using OSCTools;
using System.Collections.Generic;

public class CommandGroup : MonoBehaviour
{
	[SerializeField]
	PropertySetter InputFieldPrefab;
	[SerializeField]
	PropertySetter BoolPrefab;
	[SerializeField]
	TextMeshProUGUI CommandNameText;

	ProtocolMessage message;
	List<PropertySetter> propertySetters = new List<PropertySetter>();

	public void CreateElements(ProtocolMessage message) {
		this.message = message;
		CommandNameText.text = message.name;
		foreach (var f in message.fields) {
			PropertySetter ps;
			if (f.type==FieldType.Bool) {
				ps = Instantiate(BoolPrefab,transform);
			} else {
				ps = Instantiate(InputFieldPrefab, transform);
			}
			ps.Initialize(f);
			propertySetters.Add(ps);
		}
	}

	public OSCMessageOut CreateMessage() {
		OSCMessageOut mess = new OSCMessageOut(message.name);
		foreach (var ps in propertySetters) {
			ps.AddValue(mess);
		}
		return mess;
	}

	public void OnButtonPress() {
		OSCMessageOut mess = CreateMessage();
		Debug.Log(mess);
		var manager = FindFirstObjectByType<NetworkManager>();
		manager.SendMessage(mess);
	}
}
