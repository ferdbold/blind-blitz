using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;



public enum choiceType {arrows,buttons,triggers};


public class GameManager : MonoBehaviour {

	[HideInInspector] public PlayerChoice previousChoice;
	public PlayerChoice currentChoice;
	public Interface myInterface;

    int cptAffichageMenu = 0;
    const int NOMBRE_MENUS = 4;

	public float timeLeft;
	public float startTime = 100;

    const float AUTO_SWITCH_DELAY = 10f;
	public float animationTime = 0.5f;
    const float DELAY = 5f;

	private bool[] dPadPressed = new bool[4];

	//Inputs
	[HideInInspector] public bool isFirstInput = true; //Is is the first input of the game that does not matter ? 
	public int chosenInput = 0; //Input chosen by the player this turn
	public bool gameIsOn = false;
    public bool gameIsPaused = false;
    public bool gameOver = false;
    public bool tutorial = false;

	//Rumble
	private float timeUntilLightRumble; //Time until controller starts to vibrate and player can make a choice
	public float timeUntilLightRumble_MIN = 2;
	public float timeUntilLightRumble_MAX = 5;
	private float timeUntilHeavyRumble = 2; //Time until controller starts to vibrate heavily and player is losing time until he makes a choice
	private bool isLightRumbling = false; //Is it Light Rumbling ?
	private bool isHeavyRumbling = false; //Is it Heavy Rumbling ?

	private int amountOfChoices = 4; //Amount of different choices available each turn
	private int amountOfInputOptions = 3; //Arrows, Buttons and Triggers
    private float malusTemps = 1;
    const int CHOICE_DELAY = 5;


	//Colors
	[HideInInspector] public int previousColor;
	private int currentColor = -1; //current color, we start with -1 which is none
	//Colors Method 1 (random every 3 frames)
	private int amountOfColors = 4; //Amount of different colors
	private int tickSinceLastChange = 0; //amount of ticks since last change we had. When this reaches tickToChangeColor, Change Color
	public int tickToChangeColor = 3; //Change color every X Tick
	//Colors Method 2 (Other Way to change colors) This changes color every frame with a color pool so it gets all almost equally
	public List<int> ColorPool;
	private int poolSize = 3;
   

	//Sounds
	public List<AudioClip> myAudioClips; //List of Audioclips used
	private AudioSource audioSource; //AudioSource Created on Awake
	private bool canPlaySound = true; //Can we currently play sound
	private float timeInBetweenSounds = 0.5f; //cant have 2 sounds in the same second

	// Use this for initialization
	void Awake(){
		gameObject.tag = "GameManager"; //Give Tag
		LoadSounds(); //Create a AudioSource and gets AudioClips in Resources.
	}

	void Start () {
	}

	void OnApplicationQuit(){
		RumbleController(0);
	}
	

	// Update is called once per frame
    void Update()
    {
        if (gameIsOn)
        {
            if (gameIsPaused)
            {
                if (Input.GetButtonDown("Start")) Unpause();
            }
            else
            {
                //Tick Timer, Check for Ending
                timeLeft -= (Time.deltaTime * malusTemps); //Timer
                if (timeLeft <= 0) EndGame();
                //Check for inputs if player can choose
                CheckInputs();
                //Pause
                if (Input.GetButtonDown("Start")) Pause();
            }
        }
        else
        { // ELse, start game if start pressed
            if (Input.GetButtonDown("Start")) StartGame();
			if (Input.GetButtonDown("share")) myInterface.OpenTutorial();
        }

		//Rumble if needed :
		if(!gameIsPaused) {
			try {
				//System.Reflection.Assembly.GetExecutingAssembly().GetType("GamePad", true);
				if(isHeavyRumbling) {
					RumbleController(1);
				} else if(isLightRumbling) {
					RumbleController(0.35f);
				}

			} catch (TypeLoadException e) {
				Debug.Log ("No rumble support, continuing: " + e.Data);
			}
   	 	}
	}
    

    void Pause() {
		//Debug.Log ("Paused");
        gameIsPaused = true;

        Debug.Log("game is paused");
        Time.timeScale = 0;

		//Remove Rumble
		RumbleController(0);

		myInterface.OnPause ();

    }

    void Unpause() {
        gameIsPaused = false;
        Time.timeScale = 1;

		//Resume Rumble
		if(isLightRumbling) RumbleController(0.35f);
		else if(isHeavyRumbling) RumbleController(1f);
        
		myInterface.OnUnpause ();
    }

	private void BuildColorPool(){
		ColorPool = new List<int>();
		for(int i=0; i<poolSize; i++) {
			ColorPool.Add (0);
			ColorPool.Add (1);
			ColorPool.Add (2);
			ColorPool.Add (3);
		}

	}

	public bool isGameOn() {
		if(gameIsOn) return true;
		else return false;
	}
	public bool Get_RumblingHard(){
		return isHeavyRumbling;
	}
	public int Get_CurrentColor(){
		return currentColor;
	}

	private void LoadSounds(){ //Create a AudioSource and gets AudioClips in Resources.
		audioSource = (AudioSource) gameObject.AddComponent<AudioSource>();
		myAudioClips = new List<AudioClip>();
		myAudioClips.Add((AudioClip) Resources.Load("Sounds/Sound_Error"));
		myAudioClips.Add((AudioClip) Resources.Load("SoundS/Confirmed_v1"));
        myAudioClips.Add((AudioClip) Resources.Load("SoundS/Confirmed_v2"));
		myAudioClips.Add((AudioClip) Resources.Load("Sounds/EndRound"));
	}

	private void CheckDPadInputs() {
		if (Input.GetAxis ("Up") == 0) {
			dPadPressed [0] = false;
		}
		if (Input.GetAxis ("Left") == 0) {
			dPadPressed [1] = false;
		} 
		if (Input.GetAxis ("Right") == 0) {
			dPadPressed [2] = false;
		}
		if (Input.GetAxis ("Down") == 0) {
			dPadPressed [3] = false;
		}

		if(GetAxisDown("Left") || Input.GetButtonDown("Debug Left")) {
			Debug.Log ("Pressed Left");
            if (currentChoice.curChoice == choiceType.arrows && isLightRumbling) ChooseInput(2);
            else OnInputError(myAudioClips[0]); 
		}
		if(GetAxisDown("Right") || Input.GetButtonDown("Debug Right")) {
			Debug.Log ("Pressed Right");
			if(currentChoice.curChoice == choiceType.arrows && isLightRumbling) ChooseInput(3);
			else OnInputError(myAudioClips[0]); 
		}
		if(GetAxisDown("Up") || Input.GetButtonDown("Debug Up")) {
			Debug.Log ("Pressed Up");
			if(currentChoice.curChoice == choiceType.arrows && isLightRumbling) ChooseInput(1);
			else OnInputError(myAudioClips[0]); 
		}
		if(GetAxisDown("Down") || Input.GetButtonDown("Debug Down")) {
			Debug.Log ("Pressed Down");
			if(currentChoice.curChoice == choiceType.arrows && isLightRumbling) ChooseInput(4);
			else OnInputError(myAudioClips[0]); 
		}
	}

	private bool GetAxisDown(string axisName) {
		int idButton = 0;
		switch (axisName) {
			case "Up":
					idButton = 0;
					break;
			case "Left":
					idButton = 1;
					break;
			case "Right":
					idButton = 2;
					break;
			case "Down":
					idButton = 3;
					break;
		}

		if (Input.GetAxis (axisName) > 0 && !dPadPressed [idButton]) {
			dPadPressed[idButton] = true;
			return true;
		}

		return false;
	}

	void CheckInputs(){
		CheckDPadInputs ();

		if(Input.GetButtonDown("Triangle")) {
			Debug.Log ("Pressed Triangle");
			if(currentChoice.curChoice == choiceType.buttons && isLightRumbling) ChooseInput(1);
			else OnInputError(myAudioClips[0]); 
		}
		if(Input.GetButtonDown("Square")) {
			Debug.Log ("Pressed Square");
			if(currentChoice.curChoice == choiceType.buttons && isLightRumbling) ChooseInput(2);
			else OnInputError(myAudioClips[0]); 
		}
		if(Input.GetButtonDown("X")) {
			Debug.Log ("Pressed Cross");
			if(currentChoice.curChoice == choiceType.buttons && isLightRumbling) ChooseInput(4);
			else OnInputError(myAudioClips[0]); 
		}
		if(Input.GetButtonDown("Round")) {
			Debug.Log ("Pressed Circle");
			if(currentChoice.curChoice == choiceType.buttons && isLightRumbling) ChooseInput(3);
			else OnInputError(myAudioClips[0]); 
		}

		if(Input.GetButtonDown("LT")) {
			if(currentChoice.curChoice == choiceType.triggers && isLightRumbling) ChooseInput(1);
			else OnInputError(myAudioClips[0]); 
		}
		if(Input.GetButtonDown("RT")) {
			if(currentChoice.curChoice == choiceType.triggers && isLightRumbling) ChooseInput(3);
			else OnInputError(myAudioClips[0]); 
		}
		if(Input.GetButtonDown("LJ")) {
			if(currentChoice.curChoice == choiceType.triggers && isLightRumbling) ChooseInput(2);
			else OnInputError(myAudioClips[0]); 
		}
		if(Input.GetButtonDown("RJ")) {
            if (currentChoice.curChoice == choiceType.triggers && isLightRumbling)
            {
                ChooseInput(4);

            }
            else OnInputError(myAudioClips[0]); 
		}
	}

	void OnInputError(AudioClip soundToPlay){
		//Debug.Log ("error");
		PlaySound(soundToPlay); //play error Sound
		myInterface.OnError();
	}

	void PlaySound(AudioClip soundToPlay){
		if(canPlaySound) {
			audioSource.PlayOneShot(soundToPlay);
			//StartCoroutine (SoundCooldown());
		}
	}

	void ChooseInput(int newInput) {
		PlaySound (myAudioClips[1]); //Plays Confirm Sound
		chosenInput = newInput-1; //Adjust to correspond to table index
		//Restart The coroutine that manages Choices and Input
		StopCoroutine ("ChoiceTimer");
		//Send Event to Interface
		myInterface.OnSelectInput(chosenInput);
		StartCoroutine ("ChoiceTimer");
	}

	void StartGame() {
		// Close menu
		myInterface.CloseMenu ();

		isFirstInput = true; //Game has restarted, back to 1st input
		//Restart Time
        gameOver = false;
		timeLeft = startTime;
		//Build Color pool
		BuildColorPool();
		//Create the first choice at random
		currentChoice = new PlayerChoice();		
		currentChoice.curChoice = (choiceType) UnityEngine.Random.Range(0,amountOfInputOptions);
		currentChoice.nextChoices = new choiceType[amountOfChoices];
		for(int i=0; i<amountOfChoices ; i++){
			currentChoice.nextChoices[i] = (choiceType) UnityEngine.Random.Range(0,amountOfInputOptions);
		}
		previousChoice = currentChoice;

		//Restart Coroutine
		gameIsOn = true;
		StartCoroutine("ChoiceTimer");

	}

	void EndGame() {
		PlaySound (myAudioClips[3]);//Play End Sound
		gameIsOn = false;
        gameOver = true;
		timeLeft = 0;
		//StopCoroutine
		StopCoroutine("ChoiceTimer");
		//Stop Rumbling
		RumbleController(0);
		isHeavyRumbling = false;
		isLightRumbling = false;
	}

	public void RumbleController(float intensity) {
		try {
			//System.Reflection.Assembly.GetExecutingAssembly().GetType("GamePad", true);
			GamePad.SetVibration(PlayerIndex.One, intensity, intensity);
		} catch (TypeLoadException e) {
			Debug.Log ("No rumble support, continuing: " + e.Data);
		} 
	}


	PlayerChoice CreatePlayerChoice(){

		PlayerChoice myNewChoice = new PlayerChoice();
		myNewChoice.curChoice = currentChoice.nextChoices[chosenInput];
		myNewChoice.nextChoices = new choiceType[amountOfChoices];
		for(int i=0; i<amountOfChoices ; i++){
			myNewChoice.nextChoices[i] = (choiceType) UnityEngine.Random.Range(0,amountOfInputOptions);
		}
		return myNewChoice;

	}

	void MakeNextChoice() {
		//Reset Rumble Values
		isHeavyRumbling = false;
		isLightRumbling = false;
		timeUntilLightRumble = UnityEngine.Random.Range (timeUntilLightRumble_MIN,timeUntilLightRumble_MAX);
		malusTemps = 1;

		//stop Rumble
		RumbleController(0);

		//Make a new choice
		previousChoice = currentChoice;
		currentChoice = CreatePlayerChoice(); 
		//Update Color
		//UpdateColor(); //NOT USED ANYMORE !
		UpdateColorPoolMethod();

		//Send Event
		myInterface.OnMadeChoice(chosenInput);

	}

	private void UpdateColorPoolMethod() {
		int randomIndex = UnityEngine.Random.Range(0,ColorPool.Count); //Get Random Index From list
		previousColor = currentColor;
		currentColor = ColorPool[randomIndex]; // Get Color from that index
		ColorPool.Remove(currentColor); // Remove selected Color from the list
		ReequilibrateColorPool(); //Fill back Color Pool if one of each color was removed from it

	}

	private void ReequilibrateColorPool(){//Fill back Color Pool if one of each color was removed from it
		int zeros = 0; int ones = 0; int twos = 0;int threes = 0;
		for(int i=0; i<ColorPool.Count; i++){ //Count Amount of each colors left
			if(ColorPool[i] == 0) zeros++;
			else if(ColorPool[i] == 1) ones++;
			else if(ColorPool[i] == 2) twos++;
			else threes++;
		}
		if(zeros < poolSize && ones < poolSize && twos < poolSize && threes < poolSize){ //If it isn't full, fill it up EQUALLY.
			ColorPool.Add (0); 
			ColorPool.Add (1); 
			ColorPool.Add (2); 
			ColorPool.Add (3);
		}
	}

	private void UpdateColor(){
		if(tickSinceLastChange >= tickToChangeColor) {
			tickSinceLastChange = 1; //reset the amounts of ticks back to 1
			previousColor = currentColor;
			currentColor = GetNewColorIndex();
			myInterface.ChangeColor(currentColor);
		} 
		else tickSinceLastChange++;
	}


	private int GetNewColorIndex(){
		int newColor;

		do {
			newColor = UnityEngine.Random.Range (0,amountOfColors);
		} while(newColor == currentColor);

		return newColor;
	}

	IEnumerator ChoiceTimer() {
        //while (true) {
            MakeNextChoice();
            StartCoroutine(AnimateButtons()); //Animate it
            yield return new WaitForSeconds(animationTime); //Wait while we animate buttons

            yield return new WaitForSeconds(timeUntilLightRumble);
			isLightRumbling = true;
			myInterface.OnReadyInput ();
			yield return new WaitForSeconds(timeUntilHeavyRumble);
			isHeavyRumbling = true;
			malusTemps = 3f;

            //Go To Next Choice
            //isChoosing = false;
        //}
	}
	
	IEnumerator AnimateButtons(){
		// Start layout animation
		myInterface.buttonWrapperAnimator.SetBool("IsCrossLayout", isCrossLayout());

		//Create A list of all Images to change alpha
		List<Image> myImages = new List<Image>();
		foreach(Image image in myInterface.options) myImages.Add(image);
		foreach(Image image in myInterface.secOptions) myImages.Add(image);

		//Wait for highlight
		for(float i = 1; i > 0; i -= Time.deltaTime/(animationTime/3)){
			yield return null;
		}

		//StartChanging Color in Interface
		myInterface.ChangeColor(currentColor); //Change Color

		//Add Transparency
		for(float i = 1; i > 0; i -= Time.deltaTime/(animationTime/3)){
			foreach(Image image in myImages) image.color = new Color(1,1,1,i);
			yield return null;
		}
		//Update Interface
		myInterface.ChangeOptions(currentChoice); 
		//Remove Transparency
		for(float i = 0; i < 1; i += Time.deltaTime/(animationTime/3)){
			foreach(Image image in myImages) image.color = new Color(1,1,1,i);
			yield return null;
		}
		foreach(Image image in myImages) image.color = new Color(1,1,1,1);
	}

	private bool isCrossLayout() {
		return currentChoice.curChoice != choiceType.triggers;
	}

	IEnumerator SoundCooldown(){
		canPlaySound = false;
		yield return new WaitForSeconds(timeInBetweenSounds);
		canPlaySound = true;
	}
	
}
