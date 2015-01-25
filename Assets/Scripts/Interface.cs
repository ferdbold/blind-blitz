using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Interface : MonoBehaviour {

	public Text timerGUI;
    public Text pauseGUI;
    public Text startGUI;

	public CanvasGroup mainMenu;
	public CanvasGroup menuMainScreen;
    public CanvasGroup tutorialScreen;
	public CanvasGroup endGameScreen;

	public Image[] options;
	public Image[] secOptions;
	public Image Background;
	public Text LastSelectionText;
    public Image bgMenu;

	public Animator buttonWrapperAnimator;
	public Animator[] buttonEffectsAnimators;

	public Sprite[] InputSprites_Arrows; // up, right, down, left
	public Sprite[] InputSprites_Triggers; // RT, LT, RJ, LJ
	public Sprite[] InputSprites_ButtonsPS; // triangle, square, x, round
	public Sprite[] InputSprites_ButtonsXbox; // Y,X,A,B
	
	private GameManager manager;



	void Start () {
		manager = (GameManager) GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		OpenMenu ();
	}

	void Update () {
		//Timer
		timerGUI.text = manager.timeLeft.ToString("F2");

        if(!manager.gameIsPaused)
        { pauseGUI.enabled = false; }
        else
        { pauseGUI.enabled = true; }
	}

	public void OpenTutorial() {
		menuMainScreen.alpha = 0;
		tutorialScreen.alpha = 1;
	}

	public void OpenMenu() {
		mainMenu.alpha = 1;
		menuMainScreen.alpha = 1;
		tutorialScreen.alpha = 0;
		endGameScreen.alpha = 0;
	}

	public void CloseMenu() {
		mainMenu.alpha = 0;
	}

	public void OpenEndGameMenu() {
		endGameScreen.alpha = 1;
	}

	public void CloseEndGameMenu() {
		endGameScreen.alpha = 0;
	}

	public void ChangeOptions(PlayerChoice p){
		ChangeMainChoice (p.curChoice);
		ChangeSecondaryChoice(p.nextChoices);
	}

	public void ChangeColor(int newColorIndex) {
		Color nColor;
		if(newColorIndex==0) nColor = new Color(204f/255f,51f/255f,63f/255f);
		else if(newColorIndex==1) nColor = new Color(237f/255f,201f/255f,81f/255f);
		else if(newColorIndex==2) nColor = new Color(235f/255f,104f/255f,65f/255f);
		else nColor = new Color(0f/255f,160f/255f,176f/255f);

		StartCoroutine (LerpBackgroundColor(Background.color,nColor));
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
	 
	public void OnMadeChoice(int choice){
		//Change Selection Text
		if(manager.isFirstInput == false) {
			ChangeLastSelectionText(manager.previousColor, choice, manager.previousChoice.curChoice);
			Debug.Log ("Previous : " + manager.previousChoice.curChoice + "       Current : " +  manager.currentChoice.curChoice);
		} else manager.isFirstInput = false;
			
	}


	private void ChangeLastSelectionText(int color, int choice, choiceType type){
		string choiceString;
		string typeString;
		string colorString;

		//Get Color String
		if(color==0) colorString = "Red ";
		else if(color==1) colorString = "Yellow ";
		else if(color==2)  colorString = "Orange ";
		else  colorString = "Blue ";

		//Get Choice String and Type
		switch(type) {
			case choiceType.arrows: //up left right down
				if(choice == 0) choiceString = "Up ";
				else if(choice == 1) choiceString = "Left ";
				else if(choice == 2) choiceString = "Right ";
				else choiceString = "Down ";
				typeString = "Arrow ";
				break;
			case choiceType.buttons: //triangle, square, circle, x
				if(choice == 0) choiceString = "Triangle ";
				else if(choice == 1) choiceString = "Square ";
				else if(choice == 2) choiceString = "Circle ";
				else choiceString = "Cross ";
				typeString = "Button ";
				break;
			default: //lt, ls, rt, rs
				if(choice == 0) choiceString = "Left Trigger ";
				else if(choice == 1) choiceString = "Left Joystick ";
				else if(choice == 2) choiceString = "Right Trigger ";
				else choiceString = "Right Joystick ";
				typeString = " ";
				break;
		}

		LastSelectionText.text = "Last Selection : " + colorString + choiceString + typeString;
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

	//Coroutine that lerps the color of the backgroudn
	IEnumerator LerpBackgroundColor(Color start, Color end) {
		float time = manager.animationTime/3f;
		for(float i = 0; i<1; i += Time.deltaTime/time) {
			Background.color = Color.Lerp(start,end,i);
			yield return null;
		}
	}
}
