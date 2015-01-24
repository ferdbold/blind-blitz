using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TimerAnimation : MonoBehaviour {

	GameManager manager;
	RectTransform UITextTransform;
	public int currentSecond;
	
	void Start () {
		manager = (GameManager) GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		currentSecond = 60;
		UITextTransform = gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		//if(GameManager.isGameOn()){
			if(manager.timeLeft < currentSecond) {
				currentSecond--;
				StartCoroutine ("PopTimer");
			}
		//}
	}

	IEnumerator PopTimer(){
		StartCoroutine(ScaleTimeText(1,1.2f,0.2f));
		yield return null;
		StartCoroutine(ScaleTimeText(1.2f,1.0f,0.2f));
	}

	IEnumerator ScaleTimeText(float startScale,float endScale, float time){
		for(float i = 0; i < 1; i += Time.deltaTime/time) {
			float newScale = Mathf.Lerp (startScale,endScale,i);

			UITextTransform.localScale = new Vector3(newScale,newScale,newScale);
			yield return null;
		}
	}

}
