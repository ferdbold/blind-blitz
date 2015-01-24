using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Interface : MonoBehaviour {

	public Text timerGUI;
    public Text pauseGUI;
    public Text endGameGUI;
	public Image[] options;
	public Image[] secOptions;
	public Image Background;

	public Animator buttonWrapperAnimator;
	public Animator[] buttonEffectsAnimators;

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
        if(!manager.gameIsPaused)
        { pauseGUI.enabled = false; }
        else
        { pauseGUI.enabled = true; }
        if(manager.gameOver)
        { endGameGUI.enabled = true; }
        else { endGameGUI.enabled = false; }

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
					secOptions[i].sprite = InputSprites_Arrows[4];
					break;
				case choiceType.buttons:
					secOptions[i].sprite = InputSprites_ButtonsPS[4];
					break;
				case choiceType.triggers:
				secOptions[i].sprite = InputSprites_Triggers[4];
					break;
			}
		}
	}

	public void OnSelectInput(int choice) {
		for (var i = 0; i < buttonEffectsAnimators.Length; i++) {
			Animator b = buttonEffectsAnimators [i];

			if (i == choice) {
				b.SetTrigger ("SelectInput");
			}

			b.SetBool ("ReadyInput", false);
		}
	}

	public void OnReadyInput() {
		foreach (Animator a in buttonEffectsAnimators) {
			a.SetBool("ReadyInput", true);
		}
	}

	public void OnError() {
		foreach (Animator a in buttonEffectsAnimators) {
			a.SetTrigger("Error");
		}
	}

	public void OnPause() {
		foreach(Animator b in buttonEffectsAnimators) {
			b.speed = 0;
		}
		buttonWrapperAnimator.speed = 0;
	}

	public void OnUnpause() {
		foreach(Animator b in buttonEffectsAnimators) {
			b.speed = 1;
		}
		buttonWrapperAnimator.speed = 1;
	}
}
