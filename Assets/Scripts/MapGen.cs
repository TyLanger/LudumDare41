using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour {

	public enum DoorDirection {Top, Right, Bottom, Left };

	public int numPiececs = 3;
	//int currentNumPieces = 0;

	public float tileSpacing = 1f;
	Vector3 lastTilePos;

	// arrays so I can have random tiles
	public GameObject[] wallPieces;
	public GameObject[] floorPieces;

	int hallLength = 20;
	int hallWidth = 10;

	int roomLength = 20;
	int roomWidth = 20;

	string debugString = "";

	//public TrackPiece[] LeftRight;
	//Vector3 lastExit;

	// Use this for initialization
	void Start () {
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
					tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
				}
				tileCopy.transform.parent = transform;
			}
		}

		// exit is to the right
		// set the lastTilePos to the bottom right tile
		lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);

		return thisPiece;
	}

	MapPiece BuildEndPiece(DoorDirection entrance)
	{
		MapPiece thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];

		switch (entrance) {
		case DoorDirection.Bottom:
			thisPiece.Entrance = DoorDirection.Top;

			// hallway straight down
			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) || y==(hallLength-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f-(y*0.5f), -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0-(y*0.5f), -y * tileSpacing), transform.rotation);
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
			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1) || x==(hallLength-1)) {
						// walls are spawned higher than the floor tiles (the 0.5f in y of the vector3)
						// make stairs going down. Each new column is lower than the last
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f-(x*0.5f), y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0-(x*0.5f), y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}

			break;
		case DoorDirection.Top:
			thisPiece.Entrance = DoorDirection.Bottom;

			// hallway straight up
			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) || y==(hallLength-1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f-(y*0.5f), y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0-(y*0.5f), y * tileSpacing), transform.rotation);
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
		int r = Random.Range (0, 3);
		MapPiece thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];

		switch (r) {
		case 0:
			debugString += "BottomEntrance case 0 ";
			// entrance on bottom
			// exit right
			thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Right);

			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);
			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < x + 1; y++) {
					if (y == x) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);
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

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);
			break;
		case 2:
			debugString += "BottomEntrance case 2 ";
			// entrance on bottom
			// exit top
			thisPiece = new MapPiece (DoorDirection.Bottom, DoorDirection.Top);

			// hallway angled right
			// hallway angled left might collide with previous map pieces
			lastTilePos = lastTilePos + new Vector3 (0, 0, tileSpacing);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 ((x+(1*y)*0.5f) * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 ((x+(1*y)*0.5f) * tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);
			break;
		}

		return thisPiece;
	}

	MapPiece TopEntrance()
	{
		int r = Random.Range (0, 4);
		MapPiece thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];

		switch (r) {
		case 0:
			debugString += "TopEntrance case 0 ";
			// entrance on top
			// exit right
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Right);

			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < x + 1; y++) {
					// the (x == (hallWidth - 1) && y == 0) adds a wall in the inside of the corner
					//if (y == x || (x == (hallWidth - 1) && y == 0)) {
					if (y == x) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, 0);
			break;
		case 1:
			debugString += "TopEntrance case 1 ";
			// entrance on top
			// exit bottom
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Bottom);

			// hallway straight down
			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);
			break;
		case 2:
			debugString += "TopEntrance case 2 ";
			// entrance on top
			// exit bottom
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Bottom);

			// hallway angled right
			// hallway angled left might collide with previous map pieces
			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);

			for (int x = 0; x < hallWidth; x++) {
				for (int y = 0; y < hallLength; y++) {
					if (x == 0 || x == (hallWidth-1) ) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 ((x+(1*y)*0.5f) * tileSpacing, 0.5f, -y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 ((x+(1*y)*0.5f) * tileSpacing, 0, -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth - 1) * tileSpacing, -0.5f, 0);
			break;
		case 3:
			debugString += "TopEntrance case 3 ";
			// entrance on top
			// exit right
			thisPiece = new MapPiece (DoorDirection.Top, DoorDirection.Right);

			// room with entrance top, exit bottom right
			lastTilePos = lastTilePos + new Vector3 (0, 0, -tileSpacing);

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
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x* tileSpacing, 0, -y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, 0);
			break;
		}

		return thisPiece;
	}

	MapPiece LeftEntrance()
	{
		int r = Random.Range (0, 7);
		MapPiece thisPiece = new MapPiece (DoorDirection.Left, DoorDirection.Right);
		GameObject tileCopy = wallPieces [0];

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


			// instantitate all the objects
			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}

			// exit is to the right
			// set the lastTilePos to the bottom right tile
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);
			break;
		case 1:
			debugString += "LeftEntrance case 1 ";
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);

			// angled hallway
			// angled down
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);

			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1)) {
						// (y-(1*x) * 0.5f) is to make each new coloumn be lower than the last (to make it an angled hallway)
						// the 0.5 makes the blocks spawn half as far apart to make the hallway less steep
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y-(1*x) * 0.5f) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y-(1*x) * 0.5f) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);
			break;

		case 2:
			debugString += "LeftEntrance case 2 ";
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);

			// angled hallway
			// angled up
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);

			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y+(1*x) * 0.5f) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y+(1*x) * 0.5f) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);
			break;
		case 3:
			debugString += "LeftEntrance case 3 ";
			// build a room
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);

			// entrance in bottom left
			// exit in top right
			for (int x = 0; x < roomLength; x++) {
				for (int y = 0; y < roomWidth; y++) {
					if (y == 0 || y == (roomWidth - 1) || (x == 0 && y >= hallWidth) || (x == (roomLength-1) && y <= (roomWidth-hallWidth))) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(hallWidth - 1) * tileSpacing);
			break;
		case 4:
			debugString += "LeftEntrance case 4 ";
			// build a room
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, -(roomWidth-hallWidth) * tileSpacing);

			// entrance in top left
			// exit in bottom right
			for (int x = 0; x < roomLength; x++) {
				for (int y = 0; y < roomWidth; y++) {
					if (y == 0 || y == (roomWidth - 1) || (x == (roomLength-1) && y >= hallWidth) || (x == 0 && y <= (roomWidth-hallWidth))) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0.5f, (y) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, -0.5f, -(roomWidth-1) * tileSpacing);
			break;
		case 5:
			debugString += "LeftEntrance case 5 ";

			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Bottom);
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
			}
			// when exit is bottom, lastTile pos is the leftmost exit tile
			lastTilePos = tileCopy.transform.position + new Vector3 (-(hallWidth-1)*tileSpacing, -0.5f, 0);
			break;
		case 6:
			debugString += "LeftEntrance case 6 ";

			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Top);
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);

			// entrance in left
			// exit in top
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
