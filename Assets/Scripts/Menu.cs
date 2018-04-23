using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	// shown at start
	public Button StartTutorialButton;
	public Button randomizeSeed;
	public InputField seedInput;
	public Button StartGameButton;

	// shown at end of level
	// time
	// times hit // scorch marks
	// enemies crushed
	// seed
	public Button RetryButton;
	public Button NewSeed;
	public Button MainMenu;

	public Text timesHit;
	public Text enemiesCrushed;
	public Text timer;
	float timeStart = 0;
	bool timerRunning = false;
	public Text seedText;

	int seed = 0;
	public MapGen map;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (timerRunning) {

			// one decimal place
			timer.text = (Time.time - timeStart).ToString ("F1");
		}
	}

	public void RandomizeSeed()
	{
		map.RandomizeSeed ();
		seed = map.GetSeed ();
		seedInput.text = seed.ToString();
		seedText.text = seed.ToString ();
	}

	public void StartGame()
	{
		if (seedInput.text.Length > 0) {
			seed = int.Parse( seedInput.text);
			map.UseSeed (seed);
			seedText.text = seed.ToString ();
		}
		map.BuildMap ();
	}

	public void StartTutorial()
	{

	}

	public void HideMainMenu()
	{
		StartTutorialButton.gameObject.SetActive (false);
		randomizeSeed.gameObject.SetActive (false);
		seedInput.gameObject.SetActive (false);
		StartGameButton.gameObject.SetActive (false);
	}

	public void HideEndScreen ()
	{

	}

	public void PlayerHit(int times)
	{
		// this is the absolute number
		// not an increment
		timesHit.text = times.ToString ();
	}

	public void EnemyCrushed(int number)
	{
		// absolute number
		// not an increment
		enemiesCrushed.text = number.ToString ();
	}

	public void RaceStarted()
	{
		timerRunning = true;
		timeStart = Time.time;
	}

	public void RaceEnded(float time, int timesHit, int enemiesCrushed)
	{
		// don't need to pass this info anymore

		timerRunning = false;
		// display info
		// enable buttons
		StartTutorialButton.gameObject.SetActive (true);
		randomizeSeed.gameObject.SetActive (true);
		seedInput.gameObject.SetActive (true);
		StartGameButton.gameObject.SetActive (true);
		// move camera to look at the car
	}
}
