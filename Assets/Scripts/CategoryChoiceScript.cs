using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryChoiceScript : MonoBehaviour {

	string[] categoryBlank = new string[13];
	public string[] category1 = new string[13];
	public string[] category2 = new string[13];
	public string[] category3 = new string[13];
	public string[] category4 = new string[13];


	float [] dotsBlank = new float [10];
	public float[] category1Dots = new float[10];
	public float[] category2Dots = new float[10];
	public float[] category3Dots = new float[10];
	public float[] category4Dots = new float[10];

	public int blankImageNum;
	public int imageNum1;
	public int imageNum2;
	public int imageNum3;
	public int imageNum4;

	bool first = true;
	bool second = false;
	bool third = false;
	bool forth = false;

	public bool downObject = false;
	public bool upObject = false;
	Vector2 pos;
	Vector2 screenPos;
	Vector2 startPos;
	RectTransform rectTran;
	bool alreadyChosen = false;

	GameObject myButton;

	string[] categories = new string[7];

	public GameObject categoryButt;
	public Transform buttonHolder;

	public int playerNumber;

	public Color normalYellow;
	public Color dullYellow;

	public Text catOneText;
	public Text catTwoText;
	public Text catThreeText;
	public Text catFourText;

	Text myCatText;

	//public NewRoundManager newRound;

	// Use this for initialization
	void Start () {

		rectTran = GetComponent<RectTransform> ();
		startPos = rectTran.anchoredPosition;
		screenPos = new Vector2 (rectTran.anchoredPosition.x, 0.0f);

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.T)) {
			MoveDown ();
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			MoveUp ();
		}

		if (downObject == true) {

			pos = new Vector2 (rectTran.anchoredPosition.x,
				rectTran.anchoredPosition.y);

			rectTran.anchoredPosition = Vector2.Lerp (pos, screenPos, Time.deltaTime * 6.0f);

			if (pos.y - screenPos.y < 2.5f) {
				downObject = false;
				rectTran.anchoredPosition = screenPos;

			}
		}

		if (upObject == true) {

			pos = new Vector2 (rectTran.anchoredPosition.x,
				rectTran.anchoredPosition.y);

			rectTran.anchoredPosition = Vector2.Lerp (pos, startPos, Time.deltaTime * 6.0f);

			if (startPos.y - pos.y < 10.0f) {
				upObject = false;
				rectTran.anchoredPosition = startPos;

			}
		}
		
	}

	public void MoveDown () {

		if (myButton.GetComponent<Image> ().color == dullYellow) {
			return;
		}

		if (alreadyChosen == false) {
			upObject = false;
			downObject = true;
		}
	}

	public void MoveUp () {

		downObject = false;
		upObject = true;

	}

	void FillEmptyArray (){

		if (forth == true) {
			category4 = new string[13];
			category4 = categoryBlank;
			category4Dots = dotsBlank;
			imageNum4 = blankImageNum;
			forth = false;
			//newRound.BeginNewRound ();
		} else if (third == true) {
			category3 = new string[13];
			category3 = categoryBlank;
			category3Dots = dotsBlank;
			imageNum3 = blankImageNum;
			third = false;
			forth = true;
		} else if (second == true) {
			category2 = new string[13];
			category2 = categoryBlank;
			category2Dots = dotsBlank;
			imageNum2 = blankImageNum;
			third = true;
			second = false;
		} else if (first == true) {
			category1 = new string[13];
			category1 = categoryBlank;
			category1Dots = dotsBlank;
			imageNum1 = blankImageNum;
			second = true;
			first = false;

		}

		categoryBlank = new string[13];
		dotsBlank = new float [10];
		blankImageNum = 0;
	
	}

	public void FillButtonList(int playerNum){

		playerNumber = playerNum;

		if (playerNumber == 1) {
			myButton = GameObject.FindGameObjectWithTag ("Cat Butt 1");
		} else if (playerNumber == 2) {
			myButton = GameObject.FindGameObjectWithTag ("Cat Butt 2");
		} else if (playerNumber == 3) {
			myButton = GameObject.FindGameObjectWithTag ("Cat Butt 3");
		} else if (playerNumber == 4) {
			myButton = GameObject.FindGameObjectWithTag ("Cat Butt 4");
		}

		if (playerNumber == 1) {
			myCatText = catOneText;
		} else if (playerNumber == 2) {
			myCatText = catTwoText;
		} else if (playerNumber == 3) {
			myCatText = catThreeText;
		} else if (playerNumber == 4) {
			myCatText = catFourText;
		}

		myButton.GetComponent<Image> ().color = normalYellow;
		myCatText.text = "SELECT CATEGORY";

		categories [0] = "FOUR LEGGED CREATURES";
		categories [1] = "OVAL SHAPES";
		//categories [2] = "WHY SO GLOOMY";
		categories [2] = "GROUP ACTIVITIES";
		categories [3] = "MACHINES";
		categories [4] = "CELEBRITIES";
		//categories [6] = "BE SQUARE";
		//categories [7] = "ON THE GRID";
		//categories [8] = "HAPPY THOUGHTS";
		//categories [9] = "BEST MOVIE EVER";
		//categories [10] = "THE MAN CAN ACT";
		//categories [11] = "BESTEST ACTRESS";
		categories [5] = "MESSY MESSES";
		categories [6] = "SUPER HEROES";

		for (int i = 0; i < categories.Length; i++) {

			GameObject catButt = Instantiate (categoryButt, Vector3.zero, Quaternion.identity);
			catButt.transform.SetParent (buttonHolder, false);
//			catButt.GetComponent<CategoryButton> ().AddTitle (categories [i], playerNumber, i);

		}
	
//		GameObject.FindGameObjectWithTag ("Local Artist").GetComponent<LevelSetup> ().CmdLookForTakens (playerNumber);

	}

	public void ReceiveTakens (int playerNum, string[] catsss){

//		Debug.Log (catsss [0]);

		if (playerNum != playerNumber) {
			return;
		}

		for (int i = 0; i < catsss.Length; i++) {

			if (catsss [i] != "") {
			
				MarkAsTaken (catsss[i]);

				if (i == 0) {
					catOneText.text = catsss [i];
				} else if (i == 1) {
					catTwoText.text = catsss [i];
				} else if (i == 2) {
					catThreeText.text = catsss [i];
				} else if (i == 3) {
					catFourText.text = catsss [i];
				}
			}
		}
	}

	public void StartTheGame (string[] selectedCategories){

		for (int i = 0; i < selectedCategories.Length; i++) {

			SendCategoryData (selectedCategories[i]);

		}
			
	}

	void SendCategoryData (string catName){

		for (int i = 0; i < categories.Length; i++) {

			if (catName == categories [i]) {
				string funcName = "Category" + i;
				Invoke (funcName, 0.0f);
			}
		}
	}

	public void SendItBack (string cat, int playerNum, int button){
	
		if (playerNum == playerNumber) {

			MarkAsTaken (cat);
			upObject = false;
			downObject = true;
		
		}
	}

	public void CategoryTaken (string cat, int playerNum){

		if (playerNumber == playerNum) {
			MoveUp ();
			alreadyChosen = true;
			myButton.GetComponent<Image> ().color = dullYellow;

		}

		ChangeCategory (cat, playerNum);
		MarkAsTaken (cat);

	}

	void ChangeCategory (string cat, int playerNum){
	
		if (playerNum == 1) {
			catOneText.text = cat;
		} else if (playerNum == 2) {
			catTwoText.text = cat;
		} else if (playerNum == 3) {
			catThreeText.text = cat;
		} else if (playerNum == 4) {
			catFourText.text = cat;
		}

	}

	void MarkAsTaken (string cat) {

//		for (int i = 0; i < buttonHolder.childCount; i++) {

//			CategoryButton catButt = buttonHolder.GetChild (i).GetComponent<CategoryButton> ();

//			if (cat == catButt.categoryLabel.text) {
//				catButt.taken.SetActive (true);
//				catButt.clicked = true;
				return;
//			}
//		}
	}

	void Category0(){

	//	Debug.Log ("FFFOOUOUUR");

		categoryBlank [0] = "FOUR LEGGED CREATURES";
		categoryBlank [1] = "Bear";
		categoryBlank [2] = "Cow";
		categoryBlank [3] = "Skunk";
		categoryBlank [4] = "Horse";
		categoryBlank [5] = "Pig";
		categoryBlank [6] = "Rhino";
		categoryBlank [7] = "Lion";
		categoryBlank [8] = "Dog";
		categoryBlank [9] = "Camel";
		categoryBlank [10] = "Gerbil";
		categoryBlank [11] = "Sloth";
		categoryBlank [12] = "Cat";

		dotsBlank [0] = -8.05f;
		dotsBlank [1] = 100;
		dotsBlank [2] = 100;
		dotsBlank [3] = 100;
		dotsBlank [4] = 6.33f;

		dotsBlank [5] = 5.69f;
		dotsBlank [6] = 100;
		dotsBlank [7] = 100;
		dotsBlank [8] = 100;
		dotsBlank [9] = -1.8f;

		blankImageNum = 1;

		FillEmptyArray ();

	}

	void Category1(){

		categoryBlank [0] = "OVAL SHAPES";
		categoryBlank [1] = "Blimp";
		categoryBlank [2] = "Hotdog";
		categoryBlank [3] = "Armadillo";
		categoryBlank [4] = "UFO";
		categoryBlank [5] = "Missile";
		categoryBlank [6] = "Raft";
		categoryBlank [7] = "Burrito";
		categoryBlank [8] = "Fish";
		categoryBlank [9] = "Avocado";
		categoryBlank [10] = "Belt Buckle";
		categoryBlank [11] = "Lady Bug";
		categoryBlank [12] = "Sandwich";

		dotsBlank [0] = 8.4f;
		dotsBlank [1] = 100;
		dotsBlank [2] = 100;
		dotsBlank [3] = 100;
		dotsBlank [4] = -8.4f;

		dotsBlank [5] = 4.6f;
		dotsBlank [6] = 100;
		dotsBlank [7] = 100;
		dotsBlank [8] = 100;
		dotsBlank [9] = -4.6f;

		blankImageNum = 3;

		FillEmptyArray ();

	}

//	void Category2(){
//
//		categoryBlank [0] = "WHY SO GLOOMY";
//		categoryBlank [1] = "Frustrated";
//		categoryBlank [2] = "Painful";
//		categoryBlank [3] = "Embarrassed";
//		categoryBlank [4] = "Scared";
//		categoryBlank [5] = "Starving";
//		categoryBlank [6] = "Guilty";
//		categoryBlank [7] = "Annoyed";
//		categoryBlank [8] = "Depressed";
//		categoryBlank [9] = "Tired";
//		categoryBlank [10] = "Cowardly";
//		categoryBlank [11] = "Doubtful";
//		categoryBlank [12] = "Jealous";
//
//		dotsBlank [0] = -10;
//		dotsBlank [1] = -4;
//		dotsBlank [2] = -1;
//		dotsBlank [3] = 4;
//		dotsBlank [4] = 11;
//
//		dotsBlank [5] = 6;
//		dotsBlank [6] = 3;
//		dotsBlank [7] = -2;
//		dotsBlank [8] = -4;
//		dotsBlank [9] = -7;
//
//		blankImageNum = 0;
//
//		FillEmptyArray ();
//
//	}

	void Category2(){

		categoryBlank [0] = "GROUP ACTIVITIES";
		categoryBlank [1] = "Family Photo";
		categoryBlank [2] = "Rock Concert";
		categoryBlank [3] = "Army Invasion";
		categoryBlank [4] = "Ghostbusters";
		categoryBlank [5] = "Drum Circle";
		categoryBlank [6] = "Twister Game";
		categoryBlank [7] = "Ring Around the Rosie";
		categoryBlank [8] = "Protest March";
		categoryBlank [9] = "Doubles Tennis";
		categoryBlank [10] = "Tea Party";
		categoryBlank [11] = "Group Hug";
		categoryBlank [12] = "Business Meeting";

		dotsBlank [0] = -6.9f;
		dotsBlank [1] = -6.15f;
		dotsBlank [2] = 6.4f;
		dotsBlank [3] = 7.2f;
		dotsBlank [4] = 100;

		dotsBlank [5] = 2.2f;
		dotsBlank [6] = 6.7f;
		dotsBlank [7] = -.4f;
		dotsBlank [8] = -5;
		dotsBlank [9] = 100;

		blankImageNum = 4;

		FillEmptyArray ();

	}

	void Category3(){

		categoryBlank [0] = "MACHINES";
		categoryBlank [1] = "Transformer";
		categoryBlank [2] = "Tractor";
		categoryBlank [3] = "Train";
		categoryBlank [4] = "Chainsaw";
		categoryBlank [5] = "Tank";
		categoryBlank [6] = "Time Bomb";
		categoryBlank [7] = "Typewriter";
		categoryBlank [8] = "Camera";
		categoryBlank [9] = "Barbecue Grill";
		categoryBlank [10] = "Millennium Falcon";
		categoryBlank [11] = "Espresso Machine";
		categoryBlank [12] = "Helicopter";

		dotsBlank [0] = -8.5f;
		dotsBlank [1] = 8.5f;
		dotsBlank [2] = -3;
		dotsBlank [3] = 2;
		dotsBlank [4] = 6;

		dotsBlank [5] = 4;
		dotsBlank [6] = 1;
		dotsBlank [7] = -2;
		dotsBlank [8] = -4.5f;
		dotsBlank [9] = 100;

		blankImageNum = 6;

		FillEmptyArray ();

	}

//	void Category4(){
//
//		categoryBlank [0] = "US PRESIDENTS";
//		categoryBlank [1] = "Donald Trump";
//		categoryBlank [2] = "Barack Obama";
//		categoryBlank [3] = "George W Bush";
//		categoryBlank [4] = "Bill Clinton";
//		categoryBlank [5] = "Ronald Reagan";
//		categoryBlank [6] = "Richard Nixon";
//		categoryBlank [7] = "John F Kennedy";
//		categoryBlank [8] = "Franklin R Roosevelt";
//		categoryBlank [9] = "Teddy Roosevelt";
//		categoryBlank [10] = "Abraham Lincoln";
//		categoryBlank [11] = "Thomas Jefferson";
//		categoryBlank [12] = "George Washington";
//
//		dotsBlank [0] = -3.9f;
//		dotsBlank [1] = -5;
//		dotsBlank [2] = 100;
//		dotsBlank [3] = 3.8f;
//		dotsBlank [4] = 5;
//
//		dotsBlank [5] = 5.7f;
//		dotsBlank [6] = 4;
//		dotsBlank [7] = .5f;
//		dotsBlank [8] = -2;
//		dotsBlank [9] = -3.7f;
//
//		blankImageNum = 2;
//
//		FillEmptyArray ();
//
//	}

	void Category4(){

		categoryBlank [0] = "CELEBRITIES";
		categoryBlank [1] = "Johnny Depp";
		categoryBlank [2] = "Marilyn Monroe";
		categoryBlank [3] = "John Wayne";
		categoryBlank [4] = "Kim Kardashian";
		categoryBlank [5] = "Tom Hanks";
		categoryBlank [6] = "Julia Roberts";
		categoryBlank [7] = "Jack Nicholson";
		categoryBlank [8] = "Judy Garland";
		categoryBlank [9] = "Arnold Schwarzenegger";
		categoryBlank [10] = "Oprah";
		categoryBlank [11] = "Will Smith";
		categoryBlank [12] = "Angelina Jolie";

		dotsBlank [0] = -3.9f;
		dotsBlank [1] = -5;
		dotsBlank [2] = 100;
		dotsBlank [3] = 3.8f;
		dotsBlank [4] = 5;

		dotsBlank [5] = 5.7f;
		dotsBlank [6] = 4;
		dotsBlank [7] = .5f;
		dotsBlank [8] = -2;
		dotsBlank [9] = -3.7f;

		blankImageNum = 2;

		FillEmptyArray ();

	}

//	void Category6(){
//
//		categoryBlank [0] = "BE SQUARE";
//		categoryBlank [1] = "Boom Box";
//		categoryBlank [2] = "SpongeBob";
//		categoryBlank [3] = "Monopoly";
//		categoryBlank [4] = "TV";
//		categoryBlank [5] = "Smartphone";
//		categoryBlank [6] = "Couch";
//		categoryBlank [7] = "Basketball Court";
//		categoryBlank [8] = "The Flag";
//		categoryBlank [9] = "Upright Piano";
//		categoryBlank [10] = "Lasagna";
//		categoryBlank [11] = "Candy Bar";
//		categoryBlank [12] = "Harmonica";
//
//		dotsBlank [0] = -10.1f;
//		dotsBlank [1] = 10.1f;
//		dotsBlank [2] = 100;
//		dotsBlank [3] = 5;
//		dotsBlank [4] = -5;
//
//		dotsBlank [5] = 6;
//		dotsBlank [6] = -5.8f;
//		dotsBlank [7] = 2;
//		dotsBlank [8] = -2;
//		dotsBlank [9] = 100;
//
//		blankImageNum = 5;
//
//		FillEmptyArray ();
//
//	}
//
//	void Category7(){
//
//		categoryBlank [0] = "ON THE GRID";
//		categoryBlank [1] = "Building";
//		categoryBlank [2] = "Rubiks Cube";
//		categoryBlank [3] = "Hersheys Bar";
//		categoryBlank [4] = "Waffle";
//		categoryBlank [5] = "Bookshelf";
//		categoryBlank [6] = "Dresser";
//		categoryBlank [7] = "Brick House";
//		categoryBlank [8] = "Brownies";
//		categoryBlank [9] = "Chess Game";
//		categoryBlank [10] = "Keyboard";
//		categoryBlank [11] = "Toothy Grin";
//		categoryBlank [12] = "Football Field";
//
//		dotsBlank [0] = -8.6f;
//		dotsBlank [1] = -2.2f;
//		dotsBlank [2] = 100;
//		dotsBlank [3] = 1.9f;
//		dotsBlank [4] = 8.7f;
//
//		dotsBlank [5] = -6.2f;
//		dotsBlank [6] = -2.2f;
//		dotsBlank [7] = 2.5f;
//		dotsBlank [8] = 6.1f;
//		dotsBlank [9] = 100;
//
//		blankImageNum = 7;
//
//		FillEmptyArray ();
//
//	}

//	void Category8(){
//
//		categoryBlank [0] = "HAPPY THOUGHTS";
//		categoryBlank [1] = "Love";
//		categoryBlank [2] = "Joy";
//		categoryBlank [3] = "Pride";
//		categoryBlank [4] = "Hope";
//		categoryBlank [5] = "Relaxed";
//		categoryBlank [6] = "Determined";
//		categoryBlank [7] = "Energized";
//		categoryBlank [8] = "Supportive";
//		categoryBlank [9] = "Triumphant";
//		categoryBlank [10] = "Cool";
//		categoryBlank [11] = "Intelligent";
//		categoryBlank [12] = "Cute";
//
//		dotsBlank [0] = -10;
//		dotsBlank [1] = -4;
//		dotsBlank [2] = -1;
//		dotsBlank [3] = 4;
//		dotsBlank [4] = 11;
//
//		dotsBlank [5] = 6;
//		dotsBlank [6] = 3;
//		dotsBlank [7] = -2;
//		dotsBlank [8] = -4;
//		dotsBlank [9] = -7;
//
//		blankImageNum = 0;
//
//		FillEmptyArray ();
//
//	}
//
//	void Category9(){
//
//		categoryBlank [0] = "BEST MOVIE EVER";
//		categoryBlank [1] = "Saddest Movie";
//		categoryBlank [2] = "Funniest Movie";
//		categoryBlank [3] = "Scariest Movie";
//		categoryBlank [4] = "Most Beautiful Movie";
//		categoryBlank [5] = "Stupidest Movie";
//		categoryBlank [6] = "Most Romantic Movie";
//		categoryBlank [7] = "Most Epic Movie";
//		categoryBlank [8] = "Childhood Favorite Movie";
//		categoryBlank [9] = "Best Sports Movie";
//		categoryBlank [10] = "Most Offensive Movie";
//		categoryBlank [11] = "Guilty Pleasure Movie";
//		categoryBlank [12] = "Best Cartoon";
//
//		dotsBlank [0] = -10;
//		dotsBlank [1] = -8;
//		dotsBlank [2] = -3;
//		dotsBlank [3] = 6;
//		dotsBlank [4] = 10;
//
//		dotsBlank [5] = 4;
//		dotsBlank [6] = 1;
//		dotsBlank [7] = -2;
//		dotsBlank [8] = -4;
//		dotsBlank [9] = -5;
//
//		blankImageNum = 0;
//
//		FillEmptyArray ();
//
//	}

//	void Category10(){
//
//		categoryBlank [0] = "THE MAN CAN ACT";
//		categoryBlank [1] = "Bruce Willis";
//		categoryBlank [2] = "John Wayne";
//		categoryBlank [3] = "Johnny Depp";
//		categoryBlank [4] = "George Clooney";
//		categoryBlank [5] = "Jim Carrey";
//		categoryBlank [6] = "Arnold Schwarzenegger";
//		categoryBlank [7] = "Charlie Chaplin";
//		categoryBlank [8] = "Brad Pitt";
//		categoryBlank [9] = "Will Smith";
//		categoryBlank [10] = "Jack Nicholson";
//		categoryBlank [11] = "Morgan Freeman";
//		categoryBlank [12] = "Tom Hanks";
//
//		dotsBlank [0] = -3.9f;
//		dotsBlank [1] = -5;
//		dotsBlank [2] = 100;
//		dotsBlank [3] = 3.8f;
//		dotsBlank [4] = 5;
//
//		dotsBlank [5] = 5.7f;
//		dotsBlank [6] = 4;
//		dotsBlank [7] = .5f;
//		dotsBlank [8] = -2;
//		dotsBlank [9] = -3.7f;
//
//		blankImageNum = 2;
//
//		FillEmptyArray ();
//	}
//
//	void Category11(){
//
//		categoryBlank [0] = "BESTEST ACTRESS";
//		categoryBlank [1] = "Meryl Streep";
//		categoryBlank [2] = "Oprah";
//		categoryBlank [3] = "Julia Roberts";
//		categoryBlank [4] = "Angela Jolie";
//		categoryBlank [5] = "Halle Berry";
//		categoryBlank [6] = "Elizabeth Taylor";
//		categoryBlank [7] = "Ellen Degeneres";
//		categoryBlank [8] = "Maggie Smith";
//		categoryBlank [9] = "Audrey Hepburn";
//		categoryBlank [10] = "Melissa McCarthy";
//		categoryBlank [11] = "Marilyn Monroe";
//		categoryBlank [12] = "Judy Garland";
//
//		dotsBlank [0] = -3.9f;
//		dotsBlank [1] = -5;
//		dotsBlank [2] = 100;
//		dotsBlank [3] = 3.8f;
//		dotsBlank [4] = 5;
//
//		dotsBlank [5] = 5.7f;
//		dotsBlank [6] = 4;
//		dotsBlank [7] = .5f;
//		dotsBlank [8] = -2;
//		dotsBlank [9] = -3.7f;
//
//		blankImageNum = 2;
//
//		FillEmptyArray ();
//	}

	void Category5(){

		categoryBlank [0] = "MESSY MESSES";
		categoryBlank [1] = "Confetti";
		categoryBlank [2] = "Vomit";
		categoryBlank [3] = "Bomb Blast";
		categoryBlank [4] = "Messy Bed";
		categoryBlank [5] = "Snake Pit";
		categoryBlank [6] = "Rain Storm";
		categoryBlank [7] = "Wheat Field";
		categoryBlank [8] = "Spaghetti";
		categoryBlank [9] = "Landfill";
		categoryBlank [10] = "Tornado";
		categoryBlank [11] = "Scrambled Eggs";
		categoryBlank [12] = "Forest Fire";

		dotsBlank [0] = 1.3f;
		dotsBlank [1] = 6.7f;
		dotsBlank [2] = 10.3f;
		dotsBlank [3] = -1.68f;
		dotsBlank [4] = -10.06f;

		dotsBlank [5] = 2.24f;
		dotsBlank [6] = 5.94f;
		dotsBlank [7] = -2.61f;
		dotsBlank [8] = -4.68f;
		dotsBlank [9] = 100;

		blankImageNum = 8;

		FillEmptyArray ();
	}

//	void Category6(){
//
//		categoryBlank [0] = "SUPER HEROES";
//		categoryBlank [1] = "Super Nerd";
//		categoryBlank [2] = "Sweatpants Boy";
//		categoryBlank [3] = "Killer Carrot";
//		categoryBlank [4] = "Paparazzi Nazi";
//		categoryBlank [5] = "The Cockroach";
//		categoryBlank [6] = "Candy Crusher";
//		categoryBlank [7] = "Lady Lunch";
//		categoryBlank [8] = "Kangaroo Kid";
//		categoryBlank [9] = "The Great Aardvark";
//		categoryBlank [10] = "Battery Man";
//		categoryBlank [11] = "Wonder Waffle";
//		categoryBlank [12] = "Calculator Woman";
//
//		dotsBlank [0] = -1.86f;
//		dotsBlank [1] = -4.36f;
//		dotsBlank [2] = 2.68f;
//		dotsBlank [3] = 4.02f;
//		dotsBlank [4] = 100;
//
//		dotsBlank [5] = 7.4f;
//		dotsBlank [6] = -1.74f;
//		dotsBlank [7] = -4.77f;
//		dotsBlank [8] = 100;
//		dotsBlank [9] = 100;
//
//		blankImageNum = 9;
//
//		FillEmptyArray ();
//	}

	void Category6(){

		categoryBlank [0] = "SUPER HEROES";
		categoryBlank [1] = "Superman";
		categoryBlank [2] = "Green Lantern";
		categoryBlank [3] = "The Hulk";
		categoryBlank [4] = "Batman";
		categoryBlank [5] = "The Stretch";
		categoryBlank [6] = "Donotello TMNT";
		categoryBlank [7] = "The Tick";
		categoryBlank [8] = "Thor";
		categoryBlank [9] = "Iron Man";
		categoryBlank [10] = "Wonder Woman";
		categoryBlank [11] = "Aquaman";
		categoryBlank [12] = "Wolverine";

		dotsBlank [0] = -1.86f;
		dotsBlank [1] = -4.36f;
		dotsBlank [2] = 2.68f;
		dotsBlank [3] = 4.02f;
		dotsBlank [4] = 100;

		dotsBlank [5] = 7.4f;
		dotsBlank [6] = -1.74f;
		dotsBlank [7] = -4.77f;
		dotsBlank [8] = 100;
		dotsBlank [9] = 100;

		blankImageNum = 9;

		FillEmptyArray ();
	}

}
