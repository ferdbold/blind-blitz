using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum choiceType {arrows,buttons,triggers};


public class GameManager : MonoBehaviour {

	public PlayerChoice currentChoice;
	public Interface myInterface;

	public float timeLeft;
	public float startTime = 60;
	public float animationTime = 2f;

	//Inputs
	public int chosenInput = 0; //Input chosen by the player this turn
	private bool isChoosing = false;

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
		timeLeft -= Time.deltaTime; //Timer
		//Check for inputs if player can choose
		if(isChoosing) {
			CheckInputs();
		}

	}

	void CheckInputs(){
		if(Input.GetAxis("Left") > 0) {
			Debug.Log ("Pressed Left");
			if(currentChoice.curChoice == choiceType.arrows) ChooseInput(2);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("Right") > 0) {
			if(currentChoice.curChoice == choiceType.arrows) ChooseInput(3);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("Up") > 0) {
			if(currentChoice.curChoice == choiceType.arrows) ChooseInput(1);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("Down") > 0) {
			if(currentChoice.curChoice == choiceType.arrows) ChooseInput(4);
			//else PLAY ERROR SOUND 
		}

		if(Input.GetAxis("Triangle") > 0) {
			Debug.Log ("Pressed Triangle");
			if(currentChoice.curChoice == choiceType.buttons) ChooseInput(1);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("Square") > 0) {
			Debug.Log ("Pressed Square");
			if(currentChoice.curChoice == choiceType.buttons) ChooseInput(2);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("X") > 0) {
			Debug.Log ("Pressed Cross");
			if(currentChoice.curChoice == choiceType.buttons) ChooseInput(4);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("Round") > 0) {
			Debug.Log ("Pressed Circle");
			if(currentChoice.curChoice == choiceType.buttons) ChooseInput(3);
			//else PLAY ERROR SOUND 
		}

		if(Input.GetAxis("LT") > 0) {
			if(currentChoice.curChoice == choiceType.triggers) ChooseInput(1);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("RT") > 0) {
			if(currentChoice.curChoice == choiceType.triggers) ChooseInput(3);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("LJ") > 0) {
			if(currentChoice.curChoice == choiceType.triggers) ChooseInput(2);
			//else PLAY ERROR SOUND 
		}
		if(Input.GetAxis("RJ") > 0) {
			if(currentChoice.curChoice == choiceType.triggers) ChooseInput(4);
			//else PLAY ERROR SOUND 
		}
	}

	void ChooseInput(int newInput) {
		chosenInput = newInput-1; //Adjust to correspond to table index
		isChoosing = false;
		Debug.Log ("Chosen Input : " + chosenInput + "    Current Choice : " + currentChoice.curChoice + "    Next Choice : " + currentChoice.nextChoices[chosenInput]);
		//TODO : Change this so it plays the animation 
		StopCoroutine("ChoiceTimer");
		StartCoroutine("ChoiceTimer");
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
		//Remove controls
		isChoosing = false;
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
			currentChoice = CreatePlayerChoice();
			StartCoroutine(AnimateButtons());
			yield return new WaitForSeconds(animationTime); //Wait while we animate buttons
			isChoosing = true; //give back controls
			yield return new WaitForSeconds(5f);
			//Go To Next Choice
			isChoosing = false;

		}
	}

	IEnumerator AnimateButtons(){
		//Create A list of all Images to change alpha
		List<Image> myImages = new List<Image>();
		foreach(Image image in myInterface.options) myImages.Add(image);
		foreach(Image image in myInterface.secOptions) myImages.Add(image);
		//Add Transparency
		for(float i = 1; i > 0; i -= Time.deltaTime/(animationTime/2)){
			foreach(Image image in myImages) image.color = new Color(1,1,1,i);
			yield return null;
		}
		//Update Interface
		myInterface.ChangeOptions(currentChoice); 
		//Remove Transparency
		for(float i = 0; i < 1; i += Time.deltaTime/(animationTime/2)){
			foreach(Image image in myImages) image.color = new Color(1,1,1,i);
			yield return null;
		}
		foreach(Image image in myImages) image.color = new Color(1,1,1,1);
	}


}
