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

	int seed = 0;
	public MapGen map;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RandomizeSeed()
	{
		map.RandomizeSeed ();
		seed = map.GetSeed ();
		seedInput.text = seed.ToString();
	}

	public void StartGame()
	{
		if (seedInput.text.Length > 0) {
			seed = int.Parse( seedInput.text);
			map.UseSeed (seed);
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

	void RaceStarted()
	{

	}

	public void RaceEnded(float time, int timesHit, int enemiesCrushed)
	{
		// display info
		// enable buttons
		StartTutorialButton.gameObject.SetActive (true);
		randomizeSeed.gameObject.SetActive (true);
		seedInput.gameObject.SetActive (true);
		StartGameButton.gameObject.SetActive (true);
		// move camera to look at the car
	}
}
