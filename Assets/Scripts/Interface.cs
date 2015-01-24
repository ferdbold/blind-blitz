using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Interface : MonoBehaviour {

	public  Text timerGUI;
	public Image[] options;
	public Image[] secOptions;

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
				options[i].color = Color.red;
			}
			break;
		case choiceType.buttons:
			for(int i=0; i<options.Length; i++) {
				options[i].color = Color.blue;
			}
			break;
		case choiceType.triggers:
			for(int i=0; i<options.Length; i++) {
				options[i].color = Color.green;
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
