using UnityEngine;
using System.Collections;

public class InputScreen : MonoBehaviour {

	private GameManager manager;

	public Sprite[] ButtonSprites;
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


}
