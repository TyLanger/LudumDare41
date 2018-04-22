using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour {

	public enum DoorDirection {Top, Right, Bottom, Left };

	public int numPiececs = 3;
	//int currentNumPieces = 0;

	public float tileSpacing = 1f;
	Vector3 lastTilePos;

	public GameObject startLine;
	public GameObject finishLine;

	// arrays so I can have random tiles
	public GameObject[] wallPieces;
	public GameObject[] floorPieces;

	// 0 is skeleton
	// 1 is spiral mage
	// 2 is pulse mage
	public GameObject[] enemies;

	public ItemPool itemPool;

	int hallLength = 24;
	int hallWidth = 12;

	// these get used interchangebly on accident a lot
	// so good luck changing them from being the same
	int roomLength = 24;
	int roomWidth = 24;

	string debugString = "";

	//public TrackPiece[] LeftRight;
	//Vector3 lastExit;

	public bool useRandomSeed = false;
	public int seed = 0;

	// Use this for initialization
	void Start () {
		if (useRandomSeed) {
			seed = Random.Range (0, 1000);
		}
		Random.InitState (seed);

		lastTilePos = transform.position;
		BuildMap ();
		//lastExit = transform.position;
		//BuildTrackPieces ();
	}

	/*
	void BuildTrackPieces()
	{
		int r = 0;
		TrackPiece trackCopy = LeftRight [0];
		for (int i = 0; i < numPiececs; i++) {
			r = Random.Range (0, LeftRight.Length);
			trackCopy = Instantiate (LeftRight [r]);
			trackCopy.transform.position = lastExit + (trackCopy.transform.position - trackCopy.Entrance.transform.position);
			lastExit = trackCopy.Exit.position;
			trackCopy.transform.parent = transform;
		}
	}
	*/

	void BuildMap()
	{
		// build starting piece

		//MapPiece currentPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);
		MapPiece currentPiece = BuildStartPiece ();
		for (int i = 0; i < numPiececs; i++) {
			// build pieces
			debugString += i+": ";
			currentPiece = BuildPiece(currentPiece.Exit);
		}
		BuildEndPiece (currentPiece.Exit);
		Debug.Log (debugString);
		//transform.localScale *= 2;
	}

	MapPiece BuildPiece(DoorDirection entrance)
	{
		MapPiece builtPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);
		// find a piece with the correct entrance
		switch (entrance) {
		case DoorDirection.Top:
			builtPiece = BottomEntrance ();
			break;
		case DoorDirection.Right:
			builtPiece = LeftEntrance ();
			break;
		case DoorDirection.Bottom:
			builtPiece = TopEntrance ();
			break;
		case DoorDirection.Left:
			break;
		}

		return builtPiece;
	}

	void SpawnEnemies(Vector3[] corners, DoorDirection entrance)
	{
		SpawnEnemies (corners [0], corners [1], corners [2], corners [3], entrance);

	}

	void SpawnEnemies(Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, DoorDirection entrance)
	{
		// spawn enemies between the corners
		// what happens if I make pieces with a hole in the middle?

		// if I know the shape of the pieces
		// enum trackShape { Hallway, AngledHall, Room, Corner};
		// and the facing
		// I can make shapes of enemies
		Vector3 center = new Vector3((topLeft.x + topRight.x + bottomLeft.x + bottomRight.x)*0.25f, 0, (topLeft.z + topRight.z + bottomLeft.z + bottomRight.z)*0.25f);
		GameObject armyGo = new GameObject ();
		Transform army = armyGo.transform;
		army.position = center;
		GameObject enemyCopy;
		// randomly choose something to spawn
		int r = Random.Range(0, 2);
		switch(r)
		{
		case 0:
		// army of skeletons
			for (int x = 0; x < 4; x++) {
				for (int y = 0; y < 4; y++) {
					// 16 skeletons
					// don't use tileScaling on the skeletons, they are better bunched up
					enemyCopy = Instantiate (enemies [0], army.position + new Vector3 ((x - 1.5f), 1.3f, (y - 1.5f)), army.rotation);
					enemyCopy.GetComponent<Mage> ().SetItemPool (itemPool);
					enemyCopy.transform.parent = army;
				}
			}
			break;
		case 1:
		// some wizards
			// 4 wizards in an enclosed space may be too much
			/*
			for (int x = 0; x < 2; x++) {
				for (int y = 0; y < 2; y++) {
					// 16 skeletons
					enemyCopy = Instantiate (enemies [1], army.position + new Vector3 ((x - 0.5f)*2, 1.3f, (y - 0.5f)*2), army.rotation);
					enemyCopy.transform.parent = army;
				}
			}
			*/
			// spiral wizards
			enemyCopy = Instantiate (enemies [1], army.position + new Vector3 (1 * tileSpacing, 1.3f, 3 * tileSpacing), army.rotation);
			enemyCopy.GetComponent<Mage> ().SetItemPool (itemPool);
			enemyCopy.transform.parent = army;
			enemyCopy = Instantiate (enemies [1], army.position + new Vector3 (-1 * tileSpacing, 1.3f, -3 * tileSpacing), army.rotation);
			enemyCopy.GetComponent<Mage> ().SetItemPool (itemPool);
			enemyCopy.transform.parent = army;
			break;

		case 2:
			// pulse wizards
			// They're meh. Harder to dodge pulses
			// pulses cause frame drop
			/*
			enemyCopy = Instantiate (enemies [2], army.position + new Vector3 (2 * tileSpacing, 1.3f, 3.5f * tileSpacing), army.rotation);
			enemyCopy.transform.parent = army;
			enemyCopy = Instantiate (enemies [2], army.position + new Vector3 (-2 * tileSpacing, 1.3f, -3.5f * tileSpacing), army.rotation);
			enemyCopy.transform.parent = army;
			*/
			break;
		case 3:
			// this doesn't exist yet
			/*
			enemyCopy = Instantiate (enemies [3], army.position + new Vector3 (0 * tileSpacing, 1.3f, 0 * tileSpacing), army.rotation);
			enemyCopy.transform.parent = army;
			*/
			break;
		}
		// rotate the enemies
		switch(entrance)
		{
		case DoorDirection.Bottom:
			//army.RotateAround (army.position, Vector3.up, 180);
			//- ((bottomLeft + bottomRight) * 0.5f
			army.rotation = Quaternion.FromToRotation (Vector3.forward, army.position - new Vector3(bottomLeft.x + bottomRight.x, 0, bottomLeft.z + bottomLeft.z)*0.5f);
			break;
		case DoorDirection.Left:
			//army.RotateAround (army.position, Vector3.up, -90);
			//- ((topLeft + bottomLeft) * 0.5f
			army.rotation = Quaternion.FromToRotation (Vector3.forward, army.position - new Vector3(topLeft.x + bottomLeft.x, 0, topLeft.z + bottomLeft.z)*0.5f );
			break;
		case DoorDirection.Top:
			//army.RotateAround (army.position, Vector3.up, 0);
			// - ((topLeft + topRight) * 0.5f))
			// on the straight halways, the enemies will spawn underneath because both parts of FromToRotation are ~ (0,0,1)
			// adding 0.1f is a work around
			army.rotation = Quaternion.FromToRotation (Vector3.forward, (army.position - new Vector3(topLeft.x + topRight.x + 0.1f, 0, topLeft.z + topLeft.z)*0.5f));
			break;
		}
		army.parent = transform;
	}

	MapPiece BuildStartPiece()
	{
		MapPiece thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];

		// instantitate all the objects
		for (int x = 0; x < hallLength; x++) {
			for (int y = 0; y < hallWidth; y++) {
				if (y == 0 || y == (hallWidth - 1) || x==0) {
					// walls are spawned higher than the floor tiles (the 0.5f in y of the vector3)
					tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
				} else {
					tileCopy = Instantiate (floorPieces [Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
				}
				tileCopy.transform.parent = transform;
			}
		}



		// exit is to the right
		// set the lastTilePos to the bottom right tile
		lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);

		var start = Instantiate (startLine, lastTilePos + new Vector3 (0, 0, ((hallWidth -1)* 0.5f) * tileSpacing), transform.rotation);
		start.transform.parent = transform;

		return thisPiece;
	}

	MapPiece BuildEndPiece(DoorDirection entrance)
	{
		MapPiece thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];
		GameObject end;

		switch (entrance) {
		case DoorDirection.Bottom:
			thisPiece.Entrance = DoorDirection.Top;

			// hallway straight down
			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);
			end = Instantiate (finishLine, lastTilePos + new Vector3 (((hallWidth-1)*0.5f) * tileSpacing, 0, 0), Quaternion.AngleAxis(-90, Vector3.up));
			end.transform.parent = transform;
			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) || y==(hallLength-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f-(y*0.5f), -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0-(y*0.5f), -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			// don't need for the end tile
			//lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);

			break;
		case DoorDirection.Right:
			thisPiece.Entrance = DoorDirection.Left;
			// exit isn't needed for the end piece

			// entrance is to the left
			lastTilePos = lastTilePos + new Vector3 (tileSpacing, 0, 0);
			end = Instantiate (finishLine, lastTilePos + new Vector3 (0, 0, ((hallWidth -1)* 0.5f) * tileSpacing), transform.rotation);
			end.transform.parent = transform;
			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1) || x==(hallLength-1)) {
						// walls are spawned higher than the floor tiles (the 0.5f in y of the vector3)
						// make stairs going down. Each new column is lower than the last
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f-(x*0.5f), y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0-(x*0.5f), y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}



			break;
		case DoorDirection.Top:
			thisPiece.Entrance = DoorDirection.Bottom;

			// hallway straight up
			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);
			end = Instantiate (finishLine, lastTilePos + new Vector3 (((hallWidth-1)*0.5f) * tileSpacing, 0, 0), Quaternion.AngleAxis(90, Vector3.up));
			end.transform.parent = transform;
			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) || y==(hallLength-1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f-(y*0.5f), y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0-(y*0.5f), y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			break;
		}


		return thisPiece;
	}

	MapPiece BottomEntrance()
	{
		int r = Random.Range (1, 5);
		MapPiece thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];
		Vector3[] corners = new Vector3[4];
		Vector3[] roomCorners = new Vector3[9];

		switch (r) {
		case 0:
			debugString += "BottomEntrance case 0 ";
			// entrance on bottom
			// exit right
			thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Right);

			// 45 degree corner

			// want to remove these


			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);
			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < x + 1; y++) {
					if (y == x) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);
			break;
		case 1:
			debugString += "BottomEntrance case 1 ";
			// entrance on bottom
			// exit top
			thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Top);

			// hallway straight up
			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);

			corners [2] = lastTilePos;
			corners [3] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);
			// do last 2 corners afer lastTilePos is changed
			corners [0] = lastTilePos;
			corners [1] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);
			SpawnEnemies (corners, DoorDirection.Bottom);
			break;
		case 2:
			debugString += "BottomEntrance case 2 ";
			// entrance on bottom
			// exit top
			thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Top);

			// hallway angled right
			// hallway angled left might collide with previous map pieces
			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);
			corners [2] = lastTilePos;
			corners [3] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 ((x+(1*y)*0.5f) * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 ((x+(1*y)*0.5f) * tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);
			// do last 2 corners afer lastTilePos is changed
			corners [0] = lastTilePos;
			corners [1] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);
			SpawnEnemies (corners, DoorDirection.Bottom);
			break;
		case 3:
			debugString += "BottomEntrance case 3 ";
			// entrance on bottom
			// exit right
			thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Right);

			// room with entrance bottom, exit top right
			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);
			// lastTilePos is in the top left corner
			roomCorners[6] = lastTilePos;

			for (int x = 0; x < roomWidth; x++) {
				for (int y = 0; y < roomLength; y++) {
					// build walls
					// (x==0) left wall
					// y == romLength-1 bottom wall
					// (y==0 && x > hallWidth) Top wall leaving room for entrance
					// (x==roomWidth-1 && y < (roomLength-hallWidth) right wall leaving room for exit
					if ((x==0) || (y==roomLength-1) || (y==0 && x > hallWidth-1) || (x==roomWidth-1 && y < (roomLength-hallWidth))) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x* tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth-1));


			roomCorners [0] = roomCorners [6] + new Vector3 (0, 0, (roomWidth - 1) * tileSpacing);
			roomCorners [1] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [2] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing, 0, 0);
			roomCorners [3] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [4] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [5] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing*0.5f);
			//roomCorners [6] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [7] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [8] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing);

			SpawnEnemies (roomCorners [0], roomCorners [1], roomCorners [3], roomCorners [4], DoorDirection.Top);
			SpawnEnemies (roomCorners [3], roomCorners [4], roomCorners [6], roomCorners [7], DoorDirection.Top);
			SpawnEnemies (roomCorners [1], roomCorners [2], roomCorners [4], roomCorners [5], DoorDirection.Left);
			SpawnEnemies (roomCorners [4], roomCorners [5], roomCorners [7], roomCorners [8], DoorDirection.Left);

			break;
		case 4:
			debugString += "BottomEntrance case 4 ";
			// entrance on bottom
			// exit bottom
			thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Bottom);

			// room with entrance bottom left, exit bottom right
			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);
			// lastTilePos is in the top left corner
			roomCorners[6] = lastTilePos;

			for (int x = 0; x < roomLength; x++) {
				for (int y = 0; y < roomWidth; y++) {
					if ((x==0) || (y==roomWidth-1) || (x==roomLength-1) || (x==hallWidth && y < roomWidth- hallWidth)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x* tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth-1) * tileSpacing, -0.5f, -(roomWidth-1)*tileSpacing);

			roomCorners [0] = roomCorners [6] + new Vector3 (0, 0, (roomWidth - 1) * tileSpacing);
			roomCorners [1] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [2] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing, 0, 0);
			roomCorners [3] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [4] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [5] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing*0.5f);
			//roomCorners [6] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [7] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [8] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing);

			SpawnEnemies (roomCorners [0], roomCorners [1], roomCorners [3], roomCorners [4], DoorDirection.Top);
			SpawnEnemies (roomCorners [3], roomCorners [4], roomCorners [6], roomCorners [7], DoorDirection.Top);
			SpawnEnemies (roomCorners [1], roomCorners [2], roomCorners [4], roomCorners [5], DoorDirection.Left);
			SpawnEnemies (roomCorners [4], roomCorners [5], roomCorners [7], roomCorners [8], DoorDirection.Left);

			break;
		}

		return thisPiece;
	}

	MapPiece TopEntrance()
	{
		int r = Random.Range (1, 5);
		MapPiece thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];
		Vector3[] corners = new Vector3[4];
		Vector3[] roomCorners = new Vector3[9];

		switch (r) {
		case 0:
			debugString += "TopEntrance case 0 ";
			// entrance on top
			// exit right
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Right);

			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);

			// 45 degree corner

			// want to remove these

			//corners [0] = lastTilePos;
			//corners [1] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < x + 1; y++) {
					// the (x == (hallWidth - 1) && y == 0) adds a wall in the inside of the corner
					//if (y == x || (x == (hallWidth - 1) && y == 0)) {
					if (y == x) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, 0);
			// do last 2 corners afer lastTilePos is changed
			//corners [2] = lastTilePos;
			//corners [3] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);
			//SpawnEnemies (corners, DoorDirection.Top);
			break;
		case 1:
			debugString += "TopEntrance case 1 ";
			// entrance on top
			// exit bottom
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Bottom);

			// hallway straight down
			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);

			// lastTilePos is the top left corner
			// do first 2 corners before lastTile is changed

			corners [0] = lastTilePos;
			corners [1] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);
			//SpawnEnemies(lastTilePos, lastTilePos + new Vector3((hallWidth-1) * tileSpacing, 0, 0), );

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth - 1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);
			// do last 2 corners afer lastTilePos is changed
			corners [2] = lastTilePos;
			corners [3] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);
			SpawnEnemies (corners, DoorDirection.Top);
			break;
		case 2:
			debugString += "TopEntrance case 2 ";
			// entrance on top
			// exit bottom
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Bottom);

			// hallway angled right
			// hallway angled left might collide with previous map pieces
			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);

			corners [0] = lastTilePos;
			corners [1] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 ((x+(1*y)*0.5f) * tileSpacing, 0.5f, -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 ((x+(1*y)*0.5f) * tileSpacing, 0, -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);
			// do last 2 corners afer lastTilePos is changed
			corners [2] = lastTilePos;
			corners [3] = lastTilePos + new Vector3 ((hallWidth - 1) * tileSpacing, 0, 0);
			SpawnEnemies (corners, DoorDirection.Top);
			break;
		case 3:
			debugString += "TopEntrance case 3 ";
			// entrance on top
			// exit right
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Right);

			// room with entrance top, exit bottom right
			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);
			// lastTilePos is in the top left corner
			roomCorners[0] = lastTilePos;

			for (int x = 0; x < roomWidth; x++) {
				for (int y = 0; y < roomLength; y++) {
					// build walls
					// (x==0) left wall
					// y == romLength-1 bottom wall
					// (y==0 && x > hallWidth) Top wall leaving room for entrance
					// (x==roomWidth-1 && y < (roomLength-hallWidth) right wall leaving room for exit
					if ((x==0) || (y==roomLength-1) || (y==0 && x > hallWidth-1) || (x==roomWidth-1 && y < (roomLength-hallWidth))) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x* tileSpacing, 0, -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, 0);
			// lastTile Pos is the bottom right corner

			//roomCorners [0] = roomCorners [8] + new Vector3 (-(roomLength - 1) * tileSpacing, 0, (roomWidth - 1) * tileSpacing);
			roomCorners [1] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [2] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing, 0, 0);
			roomCorners [3] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [4] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [5] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [6] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [7] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [8] = lastTilePos;

			SpawnEnemies (roomCorners [0], roomCorners [1], roomCorners [3], roomCorners [4], DoorDirection.Top);
			SpawnEnemies (roomCorners [3], roomCorners [4], roomCorners [6], roomCorners [7], DoorDirection.Top);
			SpawnEnemies (roomCorners [1], roomCorners [2], roomCorners [4], roomCorners [5], DoorDirection.Left);
			SpawnEnemies (roomCorners [4], roomCorners [5], roomCorners [7], roomCorners [8], DoorDirection.Left);

			break;
		case 4:
			debugString += "TopEntrance case 4 ";
			// entrance on top
			// exit top
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Top);

			// room with entrance topleft, exit top right
			lastTilePos = lastTilePos + new Vector3 (0, 0, -roomLength *tileSpacing);
			// lastTilePos is in the top left corner
			roomCorners[6] = lastTilePos;

			for (int x = 0; x < roomWidth; x++) {
				for (int y = 0; y < roomLength; y++) {
					if ((x==0) || (y==0) || (x==roomLength-1) || (x==hallWidth && y > hallWidth)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x* tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth-1) * tileSpacing, -0.5f, 0);

			roomCorners [0] = roomCorners [6] + new Vector3 (0, 0, (roomWidth - 1) * tileSpacing);
			roomCorners [1] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [2] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing, 0, 0);
			roomCorners [3] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [4] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [5] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing*0.5f);
			//roomCorners [6] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [7] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [8] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing);

			SpawnEnemies (roomCorners [0], roomCorners [1], roomCorners [3], roomCorners [4], DoorDirection.Top);
			SpawnEnemies (roomCorners [3], roomCorners [4], roomCorners [6], roomCorners [7], DoorDirection.Top);
			SpawnEnemies (roomCorners [1], roomCorners [2], roomCorners [4], roomCorners [5], DoorDirection.Left);
			SpawnEnemies (roomCorners [4], roomCorners [5], roomCorners [7], roomCorners [8], DoorDirection.Left);

			break;
		}

		return thisPiece;
	}

	MapPiece LeftEntrance()
	{
		int r = Random.Range (0, 7);
		MapPiece thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];
		//GameObject enemyCopy = enemies [0];
		Vector3[] corners = new Vector3[4];
		Vector3[] roomCorners = new Vector3[9];

		switch (r) {
		case 0:
			debugString += "LeftEntrance case 0 ";
			// entrance on left
			// exit on right
			thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Right);

			// this hall is going left to right
			// it starts building in the bottom left
			// lastTile Pos would have been in the bottom right of the exit last time
			// move over 1 space from the exit of the last mapPiece
			lastTilePos = lastTilePos + new Vector3 (tileSpacing, 0, 0);
			// straight hallway

			corners [2] = lastTilePos;
			corners [0] = lastTilePos + new Vector3 (0, 0, (hallWidth - 1) * tileSpacing);

			// instantitate all the objects
			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
						/*
						if ((x > hallLength * 0.3f) && x < hallLength*0.6f ){
							// in the middle ~third of the hallway
							if ((y > 2) && (y < 7)) {
								// spawn skeleton
								enemyCopy = Instantiate(enemies[0], tileCopy.transform.position + new Vector3(0, 1.3f, 0), transform.rotation);
								enemyCopy.transform.parent = transform;
							}
						}
						*/
					}
					tileCopy.transform.parent = transform;
				}
			}

			// exit is to the right
			// set the lastTilePos to the bottom right tile
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);

			// do last 2 corners afer lastTilePos is changed
			corners [3] = lastTilePos;
			corners [1] = lastTilePos + new Vector3 (0, 0, (hallWidth - 1) * tileSpacing);
			SpawnEnemies (corners, DoorDirection.Left);
			break;
		case 1:
			debugString += "LeftEntrance case 1 ";
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);

			// angled hallway
			// angled down
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);
			corners [2] = lastTilePos;
			corners [0] = lastTilePos + new Vector3 (0, 0, (hallWidth - 1) * tileSpacing);

			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1)) {
						// (y-(1*x) * 0.5f) is to make each new coloumn be lower than the last (to make it an angled hallway)
						// the 0.5 makes the blocks spawn half as far apart to make the hallway less steep
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y-(1*x) * 0.5f) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, (y-(1*x) * 0.5f) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);
			// do last 2 corners afer lastTilePos is changed
			corners [3] = lastTilePos;
			corners [1] = lastTilePos + new Vector3 (0, 0, (hallWidth - 1) * tileSpacing);
			SpawnEnemies (corners, DoorDirection.Left);
			break;

		case 2:
			debugString += "LeftEntrance case 2 ";
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);

			// angled hallway
			// angled up
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);
			corners [2] = lastTilePos;
			corners [0] = lastTilePos + new Vector3 (0, 0, (hallWidth - 1) * tileSpacing);

			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y+(1*x) * 0.5f) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, (y+(1*x) * 0.5f) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);
			// do last 2 corners afer lastTilePos is changed
			corners [3] = lastTilePos;
			corners [1] = lastTilePos + new Vector3 (0, 0, (hallWidth - 1) * tileSpacing);
			SpawnEnemies (corners, DoorDirection.Left);
			break;
		case 3:
			debugString += "LeftEntrance case 3 ";
			// build a room
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Right);
			lastTilePos = lastTilePos + new Vector3 (tileSpacing, 0, 0);

			// lastTilePos is bottom right corner
			roomCorners [6] = lastTilePos;


			// entrance in bottom left
			// exit in top right
			for (int x = 0; x < roomLength; x++) {
				for (int y = 0; y < roomWidth; y++) {
					if (y == 0 || y == (roomWidth - 1) || (x == 0 && y >= hallWidth) || (x == (roomLength - 1) && y <= (roomWidth - hallWidth))) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			// lastTilePos is now in the top right corner
			roomCorners [2] = tileCopy.transform.position + new Vector3 (0, -0.5f, 0);
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);

			roomCorners [0] = roomCorners [6] + new Vector3 (0, 0, (roomWidth - 1) * tileSpacing);
			roomCorners [1] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0);
			//roomCorners [2] = 2;
			roomCorners [3] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth - 1) * tileSpacing * 0.5f);
			roomCorners [4] = roomCorners [3] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [5] = roomCorners [2] + new Vector3 (0, 0, -(roomWidth - 1) * tileSpacing * 0.5f);
			//roomCorners [6] = 2;
			roomCorners [7] = roomCorners [6] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [8] = roomCorners [6] + new Vector3 ((roomLength - 1) * tileSpacing, 0, 0);
			/*
			for (int i = 0; i < 9; i++) {
				Debug.Log (roomCorners[i]);
			}*/
			// 4 spawns in the rooms
			SpawnEnemies (roomCorners [0], roomCorners [1], roomCorners [3], roomCorners [4], DoorDirection.Bottom);
			SpawnEnemies (roomCorners [3], roomCorners [4], roomCorners [6], roomCorners [7], DoorDirection.Left);
			SpawnEnemies (roomCorners [1], roomCorners [2], roomCorners [4], roomCorners [5], DoorDirection.Bottom);
			SpawnEnemies (roomCorners [4], roomCorners [5], roomCorners [7], roomCorners [8], DoorDirection.Left);

			break;
		case 4:
			debugString += "LeftEntrance case 4 ";
			// build a room
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Right);
			lastTilePos = lastTilePos + new Vector3 (tileSpacing, 0, -(roomWidth - hallWidth) * tileSpacing);

			// entrance in top left
			// exit in bottom right
			for (int x = 0; x < roomLength; x++) {
				for (int y = 0; y < roomWidth; y++) {
					if (y == 0 || y == (roomWidth - 1) || (x == (roomLength - 1) && y >= hallWidth) || (x == 0 && y <= (roomWidth - hallWidth))) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(roomWidth - 1) * tileSpacing);
			// lastTilePos is the bottom right corner
			// get it into terms of the top left corner
			// might make it easier to copy-paste
			roomCorners [8] = lastTilePos;
			roomCorners [0] = roomCorners [8] + new Vector3 (-(roomLength - 1) * tileSpacing, 0, (roomWidth - 1) * tileSpacing);
			roomCorners [1] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [2] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing, 0, 0);
			roomCorners [3] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [4] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [5] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [6] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [7] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing);

			SpawnEnemies (roomCorners [0], roomCorners [1], roomCorners [3], roomCorners [4], DoorDirection.Left);
			SpawnEnemies (roomCorners [3], roomCorners [4], roomCorners [6], roomCorners [7], DoorDirection.Top);
			SpawnEnemies (roomCorners [1], roomCorners [2], roomCorners [4], roomCorners [5], DoorDirection.Left);
			SpawnEnemies (roomCorners [4], roomCorners [5], roomCorners [7], roomCorners [8], DoorDirection.Top);

			break;
		case 5:
			debugString += "LeftEntrance case 5 ";

			thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Bottom);
			/*
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);

			// entrance in left
			// exit in bottom
			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < (hallWidth-x); y++) {
					if (y == (hallWidth - x -1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}*/

			lastTilePos = lastTilePos + new Vector3 (tileSpacing, 0, (hallWidth - roomLength) * tileSpacing);
			roomCorners [6] = lastTilePos;
			for (int x = 0; x < roomLength; x++) {
				for (int y = 0; y < roomWidth; y++) {
					if (x==(roomLength-1) || y==(roomWidth-1) || (x==0 && y<(roomWidth-hallWidth)) || (y==0 && x<(roomLength-hallWidth))) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}

			// when exit is bottom, lastTile pos is the leftmost exit tile
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth-1)*tileSpacing, -0.5f, -(roomWidth-1)*tileSpacing);

			roomCorners [0] = roomCorners [6] + new Vector3 (0, 0, (roomWidth - 1) * tileSpacing);
			roomCorners [1] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [2] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing, 0, 0);
			roomCorners [3] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [4] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [5] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing*0.5f);
			//roomCorners [6] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [7] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [8] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing);

			SpawnEnemies (roomCorners [0], roomCorners [1], roomCorners [3], roomCorners [4], DoorDirection.Top);
			SpawnEnemies (roomCorners [3], roomCorners [4], roomCorners [6], roomCorners [7], DoorDirection.Top);
			SpawnEnemies (roomCorners [1], roomCorners [2], roomCorners [4], roomCorners [5], DoorDirection.Left);
			SpawnEnemies (roomCorners [4], roomCorners [5], roomCorners [7], roomCorners [8], DoorDirection.Left);

			break;
		case 6:
			debugString += "LeftEntrance case 6 ";

			thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Top);
			lastTilePos = lastTilePos + new Vector3 (tileSpacing, 0, 0);
			roomCorners [6] = lastTilePos;
			// entrance in left
			// exit in top
			/*
			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < (hallWidth-x); y++) {
					if (y == 0) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y+x) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y+x) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			// when exit is top, lastTile pos is the leftmost exit tile
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth-1)*tileSpacing, -0.5f, 0);
			*/
			///////// New room code
			for (int x = 0; x < roomLength; x++) {
				for (int y = 0; y < roomWidth; y++) {
					if (x == (roomLength - 1) || y == 0 || (x==0 && y > hallWidth-1) || (y == roomWidth-1 && x < (roomLength-hallWidth))) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces[Random.Range(0, floorPieces.Length)], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth-1)*tileSpacing, -0.5f, 0);

			roomCorners [0] = roomCorners [6] + new Vector3 (0, 0, (roomWidth - 1) * tileSpacing);
			roomCorners [1] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing * 0.5f, 0, 0);
			roomCorners [2] = roomCorners [0] + new Vector3 ((roomLength - 1) * tileSpacing, 0, 0);
			roomCorners [3] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [4] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing*0.5f);
			roomCorners [5] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing*0.5f);
			//roomCorners [6] = roomCorners [0] + new Vector3 (0, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [7] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing*0.5f, 0, -(roomWidth-1)*tileSpacing);
			roomCorners [8] = roomCorners [0] + new Vector3 ((roomLength-1)*tileSpacing, 0, -(roomWidth-1)*tileSpacing);

			SpawnEnemies (roomCorners [0], roomCorners [1], roomCorners [3], roomCorners [4], DoorDirection.Top);
			SpawnEnemies (roomCorners [3], roomCorners [4], roomCorners [6], roomCorners [7], DoorDirection.Top);
			SpawnEnemies (roomCorners [1], roomCorners [2], roomCorners [4], roomCorners [5], DoorDirection.Left);
			SpawnEnemies (roomCorners [4], roomCorners [5], roomCorners [7], roomCorners [8], DoorDirection.Left);

			break;
		}

		return thisPiece;

	}

	public struct MapPiece
	{
		public DoorDirection Entrance;
		public DoorDirection Exit;

		//public MapPiece nextPiece;

		public MapPiece(DoorDirection entrance, DoorDirection exit)
		{
			Entrance = entrance;
			Exit = exit;
		}
	}
}
