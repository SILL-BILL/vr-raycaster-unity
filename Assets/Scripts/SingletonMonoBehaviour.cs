using UnityEngine;

/// <summary>
/// MonoBehaviourにシングルトンパターンを追加するジェネリッククラス
/// </summary>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
	
	/// <summary>
	/// DontDestroyOnLoad設定,trueで永続か
	/// </summary>
	public bool isPermanent = false;

	private static T instance = null;

	/// <summary>
	/// インスタンスを返す
	/// </summary>
	/// <returns>自クラスのインスタンス</returns>
	public static T Instance {
		get {
			// MonoBehaviourはGameObjectで管理するためAwakeで生成
			// if (instance == null) {

			// 	Debug.LogError (typeof(T) + " is nothing");
			// 	// instance = (T)FindObjectOfType(typeof(T));
				
			// 	// if (instance == null) {
			// 	// 	Debug.LogError (typeof(T) + " is nothing");
					
			// 	// 	GameObject go = new GameObject(typeof(T).ToString());
			// 	// 	instance = go.AddComponent<T>();
			// 	// }
			// }
			
			return instance;
		}
	}

	/// <summary>
	/// インスタンスの取得と重複時の削除
	/// </summary>
	protected virtual void Awake() {
		Debug.Log (typeof(T) + " : Awake");
		
		if (instance == null) {
			
			instance = (T)FindObjectOfType(typeof(T));
		}
		
		if (instance != this) {
			Debug.Log (typeof(T) + " : Destroy");
			
			Destroy(gameObject);
			//Destroy(this);
			return;
		}

		// スクリプトのRootを永続化
		if(isPermanent) {
			
			DontDestroyOnLoad(this.gameObject.transform.root);
		}
	}
}