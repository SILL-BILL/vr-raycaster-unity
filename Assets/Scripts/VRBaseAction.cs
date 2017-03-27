using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VRBaseAction : MonoBehaviour {

	[SerializeField] private  float decisionTime = 0.0f;
	[SerializeField] private  UnityEvent Enter;
	[SerializeField] private  UnityEvent Hover;
	[SerializeField] private  UnityEvent Exit;
	[SerializeField] private  UnityEvent Gaze;

	public UnityEvent PointerEnter {
		get { return Enter; }
		// set { pointerEnter = value; }
	}
	public UnityEvent PointerHover {
		get { return Hover; }
		// set { pointerHover = value; }
	}
	public UnityEvent PointerExit {
		get { return Exit; }
		// set { pointerExit = value; }
	}
	public UnityEvent PointerGaze {
		get { return Gaze; }
		// set { PointerEnter = value; }
	}

	// void Awake () {
		
	// 	defaultEvent.AddListener(Unregistered);
	// }

	// private void Unregistered() {
	// 	Debug.Log("Unregistered.");
	// }
}
