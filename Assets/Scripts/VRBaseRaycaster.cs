using UnityEngine;
using UnityEngine.UI;

public class VRBaseRaycaster : SingletonMonoBehaviour<VRBaseRaycaster> {

	// レティクル画像パラメーター
	private class ReticleParameter {

		public Transform transform;
		public Vector3 originalScale;
		public Image image;
		public　ReticleParameter(GameObject reticle) {

			transform = reticle.transform;
			originalScale = transform.localScale;
			image = reticle.GetComponent<Image>();
		}
	}

	// レティクル状態
	private enum State : int {
		Prep = 0,	// レティクル準備(エフェクト、アニメーションなど)
		Ready,		// 注視開始可能
		Done		// 決定済み
	}
	
	[SerializeField] private Camera mainCamera;				// VRカメラ
	[SerializeField] private float sightDistance = 10.0f;
	[SerializeField] private LayerMask targetLayers;
	[SerializeField] private GameObject reticlePointer;		// レティクル画像 : ポインター
	[SerializeField] private GameObject reticleGauge;		// レティクル画像 : 注視ゲージ
	[SerializeField] private bool showReticle = true;
	[SerializeField] private bool showDebugRay = false;

	private GameObject preTarget = null;
	private VRBaseAction targetAction;
	

	private State reticleState = State.Prep;

	private ReticleParameter pointerParameter;
	private ReticleParameter gaugeParameter;


	private Vector3 prePosition = Vector3.zero;
	private const float fixedTime = 0.2f;	// 注視判定時間
	private float fixedDistance = 0.02f;	// 注視判定距離
	private float elapsedTime = 0.0f;		// 注視経過時間
	private float decisionTime = 1.0f + fixedTime;		// 注視決定時間
	private Coroutine gazeProgress;

	public bool ShowReticle {
		get { return showReticle; }
		set { showReticle = value; }
	}
	
	protected override void Awake() {

		base.Awake();

		pointerParameter = new ReticleParameter(reticlePointer);
		gaugeParameter = new ReticleParameter(reticleGauge);
	}

	void LateUpdate () {

		// if(isPlayerFocusingToUI()) {
		// 	Debug.Log("look");
		// }
		UpdateRaycaster();
	}

	void UpdateRaycaster() {
		
		if(showDebugRay) {
			Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * sightDistance, Color.red, 0f);
		}

		Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, sightDistance, targetLayers)) {
			Debug.Log("name:"+hit.transform.name);
			
			GameObject hitObj = hit.transform.gameObject;

			// 初回
			if(hitObj != preTarget) {
				
				// 後処理
				if(targetAction != null)
					targetAction.PointerExit.Invoke();

				targetAction = hitObj.GetComponent<VRBaseAction>();

				if(targetAction != null)
					targetAction.PointerEnter.Invoke();
				else
					ProgressInit();

				preTarget = hitObj;
			}

			SetPosition(hit);

			if(targetAction != null)
				targetAction.PointerHover.Invoke();

			HitProcess(hit);

		} else {

			SetPosition();

			// 後処理
			if(preTarget != null && targetAction != null)
				targetAction.PointerExit.Invoke();

			preTarget = null;
			targetAction = null;

			OutProcess();
		}
	}

	public void SetPosition () {

		pointerParameter.transform.position = mainCamera.transform.position + mainCamera.transform.forward * sightDistance;
		pointerParameter.transform.localScale = pointerParameter.originalScale * sightDistance;
		pointerParameter.transform.localRotation = mainCamera.transform.localRotation;

		ProgressInit();
	}

	public void SetPosition (RaycastHit hit) {
		pointerParameter.transform.position = hit.point;
		pointerParameter.transform.localScale = pointerParameter.originalScale * hit.distance;
		
		pointerParameter.transform.localRotation = mainCamera.transform.localRotation;

		if(targetAction != null)
			ReticleProgress();
	}

	public void ReticleProgress() {

		elapsedTime += Time.deltaTime;
		switch (reticleState) {
			case State.Prep :

				gaugeParameter.transform.localScale = Vector3.Lerp(gaugeParameter.transform.localScale, Vector3.one, elapsedTime / 1.0f);

				if (gaugeParameter.transform.localScale.x >= 1.0f) {

					gaugeParameter.transform.localScale = Vector3.one;
					elapsedTime = 0.0f;
					reticleState = State.Ready;
				}
				break;
			case State.Ready :

				float distance = Vector3.Distance( prePosition, pointerParameter.transform.position);
				prePosition = pointerParameter.transform.position;

				if(distance > fixedDistance) {
					
					elapsedTime = 0f;
					gaugeParameter.image.fillAmount = 0.0f;
				} else if (elapsedTime > decisionTime) {
					
					gaugeParameter.image.fillAmount = 1.0f;
					
					targetAction.PointerGaze.Invoke();

					reticleState = State.Done;
				} else if (elapsedTime > fixedTime) {
					
					gaugeParameter.image.fillAmount = (elapsedTime - fixedTime) / (decisionTime - fixedTime);
				}
				break;
			case State.Done :
			default :
				break;
		}
	}

	public void ProgressInit() {

		gaugeParameter.transform.localScale = Vector3.zero;
		gaugeParameter.image.fillAmount = 0.0f;

		elapsedTime = 0.0f;

		prePosition = Vector3.zero;

		reticleState = State.Prep;
	}

	void InitState() {

	}

	bool isPlayerFocusingToUI()
	{
		Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
		return Physics.Raycast(ray, sightDistance, targetLayers.value);
	}

	protected virtual void HitProcess(RaycastHit hit) {
	}

	protected virtual void OutProcess() {
	}
}
