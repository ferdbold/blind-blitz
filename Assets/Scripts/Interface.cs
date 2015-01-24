using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Interface : MonoBehaviour {

	public  Text timerGUI;
	public Image[] options;
	public Image[] secOptions;

	public Sprite[] InputSprites_Arrows; // up, right, down, left
	public Sprite[] InputSprites_Triggers; // RT, LT, RJ, LJ
	public Sprite[] InputSprites_ButtonsPS; // triangle, square, x, round
	public Sprite[] InputSprites_ButtonsXbox; // Y,X,A,B
	/*
	 * 0-3: up, right, down, left
	 * 4-7: triangle, square, x, round
	 * 8-11: RT, LT, RJ, LJ
	 * */

	private GameManager manager;
	
	// Use this for initialization
	void Start () {
		manager = (GameManager) GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		//Timer
		timerGUI.text = manager.timeLeft.ToString("F2");

	}

	public void ChangeOptions(PlayerChoice p){
		ChangeMainChoice (p.curChoice);
		ChangeSecondaryChoice(p.nextChoices);
		
	}


	private void ChangeMainChoice(choiceType mainChoice){ //Changes main color of choice
		switch (mainChoice) {
		case choiceType.arrows:
			for(int i=0; i<options.Length; i++) {
				options[i].sprite = InputSprites_Arrows[i];
			}
			break;
		case choiceType.buttons:
			for(int i=0; i<options.Length; i++) {
				options[i].sprite = InputSprites_ButtonsPS[i];
				//TODO : If Xbox Controller, Do
				//options[i].sprite = InputSprites_ButtonsXbox[i]

			}
			break;
		case choiceType.triggers:
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
					secOptions[i].color = Color.red;
					break;
				case choiceType.buttons:
					secOptions[i].color = Color.blue;
					break;
				case choiceType.triggers:
					secOptions[i].color = Color.green;
					break;
			}
		}
	}



	void OnGUI(){

	}
}
