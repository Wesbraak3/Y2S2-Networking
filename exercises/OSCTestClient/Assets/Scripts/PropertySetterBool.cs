using UnityEngine;
using OSCTools;
using UnityEngine.UI;

public class PropertySetterBool : PropertySetter 
{
	[SerializeField]
	Toggle checkBox;

	public override void Initialize(Field field) {
		this.field = field;

		nameTex.text = field.name;
	}

	public override void AddValue(OSCMessageOut message) {
		message.AddBool(checkBox.isOn); 
	}
}
