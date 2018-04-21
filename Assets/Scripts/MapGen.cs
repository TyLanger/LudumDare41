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

	// Use this for initialization
	void Start () {
		lastTilePos = transform.position;
		BuildMap ();
	}

	void BuildMap()
	{
		// build starting piece

		MapPiece currentPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);

		for (int i = 0; i < numPiececs; i++) {
			// build pieces
			debugString += i+": ";
			currentPiece = BuildPiece(currentPiece.Exit);
		}
		Debug.Log (debugString);
	}

	MapPiece BuildPiece(DoorDirection entrance)
	{
		MapPiece builtPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);
		// find a piece with the correct entrance
		switch (entrance) {
		case DoorDirection.Top:
			break;
		case DoorDirection.Right:
			builtPiece = LeftEntrance ();
			break;
		case DoorDirection.Bottom:
			break;
		case DoorDirection.Left:
			break;
		}

		return builtPiece;
	}

	MapPiece LeftEntrance()
	{
		int r = Random.Range (0, 5);
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
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, y * tileSpacing), transform.rotation);
					}
					tileCopy.transform.parent = transform;
				}
			}

			// exit is to the right
			// set the lastTilePos to the bottom right tile
			lastTilePos = tileCopy.transform.position + new Vector3 (0, 0, -(hallWidth - 1) * tileSpacing);
			break;
		case 1:
			debugString += "LeftEntrance case 1 ";
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);

			// angled hallway
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);

			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y-(1*x)) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y-(1*x)) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, 0, -(hallWidth - 1) * tileSpacing);
			break;

		case 2:
			debugString += "LeftEntrance case 2 ";
			// entrance on Left
			// exit on right
			thisPiece = new MapPiece(DoorDirection.Left, DoorDirection.Right);

			// angled hallway
			lastTilePos = lastTilePos + new Vector3(tileSpacing, 0, 0);

			for (int x = 0; x < hallLength; x++) {
				for (int y = 0; y < hallWidth; y++) {
					if (y == 0 || y == (hallWidth - 1)) {
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y+(1*x)) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y+(1*x)) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, 0, -(hallWidth - 1) * tileSpacing);
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
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, 0, -(hallWidth - 1) * tileSpacing);
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
						tileCopy = Instantiate (wallPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);
					} else {
						tileCopy = Instantiate (floorPieces [0], lastTilePos + new Vector3 (x * tileSpacing, 0, (y) * tileSpacing), transform.rotation);

					}
					tileCopy.transform.parent = transform;
				}
			}
			lastTilePos = tileCopy.transform.position + new Vector3 (0, 0, -(roomWidth) * tileSpacing);
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
