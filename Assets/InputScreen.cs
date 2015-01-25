using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputScreen : MonoBehaviour {

	private GameManager manager;

	public Image[] ButtonSprites;
	public bool[,] InputTable;

	// Use this for initialization
	void Start () {
		manager = (GameManager) GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		CreateInputTable();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateInputTable(){
		InputTable = new bool[4,12];
	}

	public void OnInput(int input, int color, choiceType type) {
		int jValue = ((int)type)*4 + input;
		MakeInputTrue(color,jValue);
	}

	public void MakeInputTrue(int i, int j){
		Debug.Log ("Made Input true at : " + i + " , " + j);
		InputTable[i,j] = true;
	}

	public void ResetInputTable() {
		for(int i=0; i<InputTable.Length; i++) {
			for(int j=0; i<InputTable.Length; i++) {
				InputTable[i,j] = false;
			}
		}
	}

	public void UpdateInputIcons(){
		for(int i=0 ; i< 48; i++) {
			if(i<12) ActivateButton(i,InputTable[0,i]);
			else if(i<24) ActivateButton(i,InputTable[1,i-12]);
			else if(i<36) ActivateButton(i,InputTable[2,i-24]);
			else ActivateButton(i,InputTable[3,i-36]);
		}
	}

	private void  ActivateButton(int index, bool isActive){

			if(isActive) ButtonSprites[index].color = new Color(1,1,1,1f);
			else ButtonSprites[index].color = new Color(1,1,1,0.2f);

	}



}
