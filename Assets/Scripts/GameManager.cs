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
	private float animationTime = 2f;
    const float DELAY = 5f;

	//Inputs
	public int chosenInput = 0; //Input chosen by the player this turn
	private bool isChoosing = false;
	private bool gameIsOn = false;
    private bool gameIsPaused = false;
    private bool pauseIsPressed = false;

	private int amountOfChoices = 4; //Amount of different choices available each turn
	private int amountOfInputOptions = 3; //Arrows, Buttons and Triggers
    private int malusTemps = 1;


	private int amountOfColors = 4; //Amount of different colors
	private int currentColor = -1; //current color, we start with -1 which is none
	private int tickSinceLastChange = 0; //amount of ticks since last change we had. When this reaches tickToChangeColor, Change Color
	public int tickToChangeColor = 3; //Change color every X Tick

   

	//Sounds
	public List<AudioClip> myAudioClips; //List of Audioclips used
	private AudioSource audioSource; //AudioSource Created on Awake
	private bool canPlaySound = true; //Can we currently play sound
	private float timeInBetweenSounds = 1f; //cant have 2 sounds in the same second

	// Use this for initialization
	void Awake(){
		gameObject.tag = "GameManager"; //Give Tag
		LoadSounds(); //Create a AudioSource and gets AudioClips in Resources.
	}

	void Start () {
	}
	

	// Update is called once per frame
    void Update()
    {
        if (gameIsOn)
        {
            //if (gameIsRunning)
            //{
            //    timeLeft -= (Time.deltaTime * malusTemps); //Timer
            //    if (timeLeft <= 0) EndGame();
            //}
            //if (!pauseIsPressed)
            //{
            //    if (Input.GetAxis("Start") > 0) Pause();
            //}

            //if (Input.GetAxis("Start") == 0) pauseIsPressed = false;
            if(gameIsPaused)
            {
                if (!pauseIsPressed)
                {
                    if (Input.GetAxis("Start") > 0) Unpause();
                }
            }
            else
            {
                timeLeft -= (Time.deltaTime * malusTemps); //Timer
                if (timeLeft <= 0) EndGame();
                if (Input.GetAxis("Start") > 0 && !pauseIsPressed) Pause();
            }
        }
        else
        {
            if (Input.GetAxis("Start") > 0) StartGame();
        }
        if (Input.GetAxis("Start") == 0) pauseIsPressed = false;


        //Check for inputs if player can choose
        if (isChoosing)
        {
            CheckInputs();
        }
    }
    

    void Pause() {
        gameIsPaused = true;
        pauseIsPressed = true;
        Time.timeScale = 0;
    }

    void Unpause()
    {
        gameIsPaused = false;
        pauseIsPressed = true;
        Time.timeScale = 1;
        
    }

	private void LoadSounds(){ //Create a AudioSource and gets AudioClips in Resources.
		audioSource = (AudioSource) gameObject.AddComponent<AudioSource>();
		myAudioClips = new List<AudioClip>();
		myAudioClips.Add((AudioClip) Resources.Load("Sounds/Sound_Error"));
	}

	void CheckInputs(){
		if(Input.GetAxis("Left") > 0) {
			Debug.Log ("Pressed Left");
			if(currentChoice.curChoice == choiceType.arrows) ChooseInput(2);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("Right") > 0) {
			if(currentChoice.curChoice == choiceType.arrows) ChooseInput(3);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("Up") > 0) {
			if(currentChoice.curChoice == choiceType.arrows) ChooseInput(1);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("Down") > 0) {
			if(currentChoice.curChoice == choiceType.arrows) ChooseInput(4);
			else PlaySound(myAudioClips[0]); 
		}

		if(Input.GetAxis("Triangle") > 0) {
			Debug.Log ("Pressed Triangle");
			if(currentChoice.curChoice == choiceType.buttons) ChooseInput(1);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("Square") > 0) {
			Debug.Log ("Pressed Square");
			if(currentChoice.curChoice == choiceType.buttons) ChooseInput(2);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("X") > 0) {
			Debug.Log ("Pressed Cross");
			if(currentChoice.curChoice == choiceType.buttons) ChooseInput(4);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("Round") > 0) {
			Debug.Log ("Pressed Circle");
			if(currentChoice.curChoice == choiceType.buttons) ChooseInput(3);
			else PlaySound(myAudioClips[0]); 
		}

		if(Input.GetAxis("LT") > 0) {
			if(currentChoice.curChoice == choiceType.triggers) ChooseInput(1);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("RT") > 0) {
			if(currentChoice.curChoice == choiceType.triggers) ChooseInput(3);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("LJ") > 0) {
			if(currentChoice.curChoice == choiceType.triggers) ChooseInput(2);
			else PlaySound(myAudioClips[0]); 
		}
		if(Input.GetAxis("RJ") > 0) {
			if(currentChoice.curChoice == choiceType.triggers) ChooseInput(4);
			else PlaySound(myAudioClips[0]); 
		}
	}

	void PlaySound(AudioClip soundToPlay){
		if(canPlaySound) {
			audioSource.PlayOneShot(soundToPlay);
			StartCoroutine (SoundCooldown());
		}
	}

	void ChooseInput(int newInput) {
		chosenInput = newInput-1; //Adjust to correspond to table index
		isChoosing = false;
		//Debug.Log ("Chosen Input : " + chosenInput + "    Current Choice : " + currentChoice.curChoice + "    Next Choice : " + currentChoice.nextChoices[chosenInput]); 
		StopCoroutine("ChoiceTimer");
		StartCoroutine("ChoiceTimer");
	}

	void StartGame() {
		//Restart Time
		timeLeft = startTime;
		//Create the first choice at random
		currentChoice = new PlayerChoice();		
		currentChoice.curChoice = (choiceType) Random.Range(0,amountOfInputOptions);
		currentChoice.nextChoices = new choiceType[amountOfChoices];
		for(int i=0; i<amountOfChoices ; i++){
			currentChoice.nextChoices[i] = (choiceType) Random.Range(0,amountOfInputOptions);
		}
		//Restart Coroutine
		gameIsOn = true;
        pauseIsPressed = true;
		StartCoroutine("ChoiceTimer");
	}

	void EndGame() {
		gameIsOn = false;
		timeLeft = 0;
		//StopCoroutine
		StopCoroutine("ChoiceTimer");
		//Remove controls
		isChoosing = false;
	}

	PlayerChoice CreatePlayerChoice(){
		PlayerChoice myNewChoice = new PlayerChoice();
		myNewChoice.curChoice = currentChoice.nextChoices[chosenInput];
		myNewChoice.nextChoices = new choiceType[amountOfChoices];
		for(int i=0; i<amountOfChoices ; i++){
			myNewChoice.nextChoices[i] = (choiceType) Random.Range(0,amountOfInputOptions);
		}
		return myNewChoice;

	}

	void MakeNextChoice() {
		currentChoice = CreatePlayerChoice(); //Make a new choice
		//Update Color
		if(tickSinceLastChange >= tickToChangeColor) {
			tickSinceLastChange = 1; //reset the amounts of ticks back to 1
			currentColor = GetNewColorIndex();
			myInterface.ChangeColor(currentColor);
		} 
		else tickSinceLastChange++;
	}

	private int GetNewColorIndex(){
		int newColor;

		do {
			newColor = Random.Range (0,amountOfColors);
		} while(newColor == currentColor);

		return newColor;
	}



	IEnumerator ChoiceTimer() {
        while (true)
        {
            MakeNextChoice();
            StartCoroutine(AnimateButtons()); //Animate it
            yield return new WaitForSeconds(animationTime); //Wait while we animate buttons
            isChoosing = true; //give back controls
            yield return new WaitForSeconds(DELAY);
            //Go To Next Choice
            isChoosing = false;
        }
	}

	IEnumerator AnimateButtons(){ //This animates the alpha of the buttons
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

	IEnumerator SoundCooldown(){
		canPlaySound = false;
		yield return new WaitForSeconds(timeInBetweenSounds);
		canPlaySound = true;
	}


}
