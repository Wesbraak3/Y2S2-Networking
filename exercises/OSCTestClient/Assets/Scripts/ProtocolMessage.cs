using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum FieldType { Int, Float, String, Double, Bool, Long, Color};

public struct Field {
	public FieldType type;
	public string name;
	public Field(FieldType pType, string pName) {
		type = pType;
		name = pName;
	}
	public override string ToString() {
		return type.ToString() + " " + name;
	}
}

public class ProtocolMessage {
	public readonly string name;
	public readonly List<Field> fields;
	public ProtocolMessage(string pName) {
		if (pName.Length == 0 || pName[0] != '/') throw new Exception("Invalid OSC name: " + pName);
		name = pName;
		fields = new List<Field>();
	}

	public void AddField(FieldType type, string name) {
		fields.Add(new Field(type, name));
	}
	public void AddField(string type, string name) {
		switch (type.Trim().ToLower()) {
			case "int": AddField(FieldType.Int, name); break;
			case "float": AddField(FieldType.Float, name); break;
			case "long": AddField(FieldType.Long, name); break;
			case "double": AddField(FieldType.Double, name); break;
			case "string": AddField(FieldType.String, name); break;
			case "bool": AddField(FieldType.Bool, name); break;
			case "color": AddField(FieldType.Color, name); break;
			default: throw new Exception($"Unknown/unsupported field type: {type} (name={name})");
		}
	}

	public override string ToString() {
		StringBuilder builder = new StringBuilder();
		builder.Append(name);
		builder.Append(" ");
		foreach (var f in fields) {
			builder.Append(f.ToString());
			builder.Append(" ");
		}
		return builder.ToString();
	}
}

