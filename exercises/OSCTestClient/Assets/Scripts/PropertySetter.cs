using UnityEngine;
using TMPro;
using OSCTools;

public abstract class PropertySetter : MonoBehaviour
{
	[SerializeField]
	protected TextMeshProUGUI nameTex;

	protected Field field;

	public abstract void Initialize(Field field);

	public abstract void AddValue(OSCMessageOut message);
}
