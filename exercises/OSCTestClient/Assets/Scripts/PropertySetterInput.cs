using UnityEngine;
using TMPro;
using OSCTools;

public class PropertySetterInput : PropertySetter 
{
	[SerializeField]
	protected TMP_InputField inputField;

	public override void Initialize(Field field) {
		this.field = field;

		nameTex.text = field.name;

		if (field.type == FieldType.Float || field.type == FieldType.Float) {
			inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
		} else if (field.type == FieldType.Int || field.type == FieldType.Long) {
			inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
		} else {
			inputField.contentType = TMP_InputField.ContentType.Standard;
		}
	}

	public override void AddValue(OSCMessageOut message) {
		Debug.Log($"Parsing [{inputField.text}] as {field.type}");
		string inp = inputField.text;
		switch (field.type) {
			case FieldType.Int:
				message.AddInt(inp.Length > 0 ? int.Parse(inp) : 0);
				break;
			case FieldType.Long:
				message.AddLong(inp.Length > 0 ? long.Parse(inp) : 0);
				break;
			case FieldType.Float:
				message.AddFloat(inp.Length > 0 ? float.Parse(inp) : 0);
				break;
			case FieldType.Double:
				message.AddDouble(inp.Length > 0 ? double.Parse(inp) : 0);
				break;
			case FieldType.String:
				message.AddString(inputField.text);
				break;
			case FieldType.Color: 
				message.AddColor(new OSCColor(GetHex(inputField.text, 0), GetHex(inputField.text, 2), GetHex(inputField.text, 4), GetHex(inputField.text, 6)));
				break;
		}
	}

	int GetHex(string input, int start, int length=2) {
		int output = 0;
		for (int i=start;i<start+length;i++) {
			int ascii = 0;
			if (input.Length>i) {
				output *= 16;
				ascii = (int)input[i];
				int digit = 0;
				if (ascii>=(int)'a') {
					digit = 10 + ascii - (int)'a';
				}
				else if (ascii >= (int)'A') {
					digit = 10 + ascii - (int)'A';
				} else {
					digit = ascii - (int)'0';
				}
				if (digit >= 0 && digit < 16) output += digit;
			}
		}
		return output;
	}

}
