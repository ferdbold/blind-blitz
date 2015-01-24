using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Interface : MonoBehaviour {

	public Text timerGUI;
	public Image[] options;
	public Image[] secOptions;
	public Image Background;

	public Animator buttonWrapperAnimator;

	public Sprite[] InputSprites_Arrows; // up, right, down, left
	public Sprite[] InputSprites_Triggers; // RT, LT, RJ, LJ
	public Sprite[] InputSprites_ButtonsPS; // triangle, square, x, round
	public Sprite[] InputSprites_ButtonsXbox; // Y,X,A,B


	private GameManager manager;

	void Start () {
		manager = (GameManager) GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}

	void Update () {
		//Timer
		timerGUI.text = manager.timeLeft.ToString("F2");
	}

	public void ChangeOptions(PlayerChoice p){
		ChangeMainChoice (p.curChoice);
		ChangeSecondaryChoice(p.nextChoices);
	}

	public void ChangeColor(int newColorIndex) {
		if(newColorIndex==0) Background.color = Color.red;
		else if(newColorIndex==1) Background.color = Color.yellow;
		else if(newColorIndex==2) Background.color = Color.green;
		else Background.color = Color.blue;
	}

	private void ChangeMainChoice(choiceType mainChoice){ //Changes main color of choice
		switch (mainChoice) {
		case choiceType.arrows:
			buttonWrapperAnimator.SetBool("IsCrossLayout", true);
			for(int i=0; i<options.Length; i++) {
				options[i].sprite = InputSprites_Arrows[i];
			}
			break;
		case choiceType.buttons:
			buttonWrapperAnimator.SetBool("IsCrossLayout", true);
			for(int i=0; i<options.Length; i++) {
				options[i].sprite = InputSprites_ButtonsPS[i];
				//TODO : If Xbox Controller, Do
				//options[i].sprite = InputSprites_ButtonsXbox[i]
			}
			break;
		case choiceType.triggers:
			buttonWrapperAnimator.SetBool("IsCrossLayout", false);
			for(int i=0; i<options.Length; i++) {
				options[i].sprite = InputSprites_Triggers[i];
			}
			break;
		}
	}

	private void ChangeSecondaryChoice(choiceType[] secChoices){ //Changes color of incoming input

		for(int i=0; i<secOptions.Length; i++){
			switch (secChoices[i]) {
				case choiceType.arrows:
					secOptions[i].sprite = InputSprites_Arrows[0];
					break;
				case choiceType.buttons:
					secOptions[i].sprite = InputSprites_ButtonsPS[0];
					break;
				case choiceType.triggers:
				secOptions[i].sprite = InputSprites_Triggers[0];
					break;
			}
		}
	}
}
