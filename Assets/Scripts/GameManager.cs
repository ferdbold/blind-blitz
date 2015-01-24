using UnityEngine;
using System.Collections;

public enum choiceType {arrows,buttons,triggers};


public class GameManager : MonoBehaviour {

	public PlayerChoice currentChoice;
	public Interface myInterface;

	public float timeLeft;
	public float startTime = 60;
	public int chosenInput = 0; //Input chosen by the player this turn

	private int amountOfChoices = 4; //Amount of different choices available each turn
	private int amountOfInputOptions = 3; //Arrows, Buttons and Triggers



	// Use this for initialization
	void Awake() {
		//Give Tag
		gameObject.tag = "GameManager";


	}

	void Start () {
		StartGame();
	}
	

	// Update is called once per frame
	void Update () {
		timeLeft -= Time.deltaTime;
	}

	void StartGame() {
		//Restart Time
		timeLeft = startTime;
		//Create the first choice at random
		currentChoice = new PlayerChoice();		
		currentChoice.curChoice = (choiceType) Random.Range(0,amountOfInputOptions);
		currentChoice.nextChoices = new choiceType[4];
		for(int i=0; i<3 ; i++){
			currentChoice.nextChoices[i] = (choiceType) Random.Range(0,amountOfInputOptions);
		}
		//Restart Coroutine
		StartCoroutine("ChoiceTimer");
	}

	void EndGame() {
		//StopCoroutine
		StopCoroutine("ChoiceTimer");
	}

	PlayerChoice CreatePlayerChoice(){
		PlayerChoice myNewChoice = new PlayerChoice();
		myNewChoice.curChoice = currentChoice.nextChoices[chosenInput];
		myNewChoice.nextChoices = new choiceType[4];
		for(int i=0; i<3 ; i++){
			myNewChoice.nextChoices[i] = (choiceType) Random.Range(0,amountOfInputOptions);
		}
		return myNewChoice;

	}

	IEnumerator ChoiceTimer() {
		while(true) {
			myInterface.ChangeOptions(currentChoice); //Update Interface
			yield return new WaitForSeconds(3f);
			//Go To Next Choice
			currentChoice = CreatePlayerChoice();
		}
	}


}
