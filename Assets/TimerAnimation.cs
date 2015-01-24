using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TimerAnimation : MonoBehaviour {

	GameManager manager;
	RectTransform UITextTransform;
	public int currentSecond;

	public float Size_Standard = 1.2f;
	public float Size_Rumbling = 1.6f;
	
	void Start () {
		manager = (GameManager) GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		currentSecond = ((int)manager.startTime)-1;
		UITextTransform = gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		if(manager.isGameOn()){ //While game isn't paused
			if(manager.timeLeft < currentSecond) { //Each time an absolute number is reached, start the PopTimer Coroutine
				currentSecond--;
				StartCoroutine ("PopTimer");
			}
		} 
		//Watch for timer reset and reset "CurrentSecond" Accordingly
		if((int)manager.timeLeft - currentSecond > 10) currentSecond = ((int)manager.startTime)-1;
	}

	IEnumerator PopTimer(){ //calls the Scale Coroutine in order to make the timer pop each second.
		if(manager.Get_RumblingHard()){
			StartCoroutine(ScaleTimeText(1,Size_Rumbling,0.2f));
			yield return null;
			StartCoroutine(ScaleTimeText(Size_Rumbling,1.0f,0.2f));
		} else {
			StartCoroutine(ScaleTimeText(1,Size_Standard,0.2f));
			yield return null;
			StartCoroutine(ScaleTimeText(Size_Standard,1.0f,0.2f));
		}
	}

	IEnumerator ScaleTimeText(float startScale,float endScale, float time){ //Change the scale of the Timer Text according to sent values
		for(float i = 0; i < 1; i += Time.deltaTime/time) {
			float newScale = Mathf.Lerp (startScale,endScale,i);

			UITextTransform.localScale = new Vector3(newScale,newScale,newScale);
			yield return null;
		}
	}

}
