﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class Instantiation : MonoBehaviour
{
    public GameObject[,] InstantiationGridSquareGameObjectGrid { get; set; }    // Textures
    public GridSquare[,] InstantiationGridSquareGrid { get; set; }              // Functionality
    public int InstantiationGridWidth { get; set; }
    public int InstantiationGridHeight { get; set; }
	public List<Monster> InstantiationMonsters { get; set; }
    public int InstantiationNextMonsterId { get; set; }
    public int InstantiationSpawnDelay { get; set; }
	private float timer = 60.0f; 

    public Material Blank;
    public Material EnterZero;
    public Material EnterOne;
    public Material ExitZero;
    public Material ExitOne;
    public Material And;
    public Material Or;
    public Material BeltUp;
    public Material BeltRight;
    public Material BeltDown;
    public Material BeltLeft;
	public Material BeltUpLeft;
	public Material BeltUpRight;
	public Material BeltRightUp;
	public Material BeltRightDown;
	public Material BeltDownRight;
	public Material BeltDownLeft;
	public Material BeltLeftDown;
	public Material BeltLeftUp;
	public Material BeltUpT;
	public Material BeltRightT;
	public Material BeltDownT;
	public Material BeltLeftT;
    public Material BiterZero;
    public Material BiterOne;
	public Material Lose;
	public Material Win;

	public const int XOFFSET = 4;
	public const int YOFFSET = 4;

    public void Start()
    {
        InstantiationMonsters = new List<Monster>();
        InstantiationNextMonsterId = 0;
        InstantiationSpawnDelay = 10000;
        Blank = Resources.Load("Blank", typeof(Material)) as Material;
        EnterZero = Resources.Load("EnterZero", typeof(Material)) as Material;
        EnterOne = Resources.Load("EnterOne", typeof(Material)) as Material;
        ExitZero = Resources.Load("ExitZero", typeof(Material)) as Material;
        ExitOne = Resources.Load("ExitOne", typeof(Material)) as Material;
        And = Resources.Load("And", typeof(Material)) as Material;
        Or = Resources.Load("Or", typeof(Material)) as Material;
        BeltUp = Resources.Load("BeltUp", typeof(Material)) as Material;
        BeltRight = Resources.Load("BeltRight", typeof(Material)) as Material;
        BeltDown = Resources.Load("BeltDown", typeof(Material)) as Material;
        BeltLeft = Resources.Load("BeltLeft", typeof(Material)) as Material;
		BeltUpLeft = Resources.Load("BeltUpLeft", typeof(Material)) as Material;
		BeltUpRight = Resources.Load("BeltUpRight", typeof(Material)) as Material;
		BeltRightUp = Resources.Load("BeltRightUp", typeof(Material)) as Material;
		BeltRightDown = Resources.Load("BeltRightDown", typeof(Material)) as Material;
		BeltDownRight = Resources.Load("BeltDownRight", typeof(Material)) as Material;
		BeltDownLeft = Resources.Load("BeltDownLeft", typeof(Material)) as Material;
		BeltLeftDown = Resources.Load("BeltLeftDown", typeof(Material)) as Material;
		BeltLeftUp = Resources.Load("BeltLeftUp", typeof(Material)) as Material;
		BeltUpT = Resources.Load("BeltUpT", typeof(Material)) as Material;
		BeltRightT = Resources.Load("BeltRightT", typeof(Material)) as Material;
		BeltDownT = Resources.Load("BeltDownT", typeof(Material)) as Material;
		BeltLeftT = Resources.Load("BeltLeftT", typeof(Material)) as Material;
        BiterZero = Resources.Load("BiterZero", typeof(Material)) as Material;
        BiterOne = Resources.Load("BiterOne", typeof(Material)) as Material;
		Lose = Resources.Load("Lose", typeof(Material)) as Material;
		Win = Resources.Load("Win", typeof(Material)) as Material;
		Time.timeScale = 1.0f; 

        LoadLevel("Level1.txt");
    }

    public bool LoadLevel(string fileName)
    {
        string filePath = "Assets/Resources/" + fileName;
        try
        {
            InitializeGrid(filePath);
            StreamReader reader = new StreamReader(filePath, Encoding.Default);
            using(reader)
            {
                int y = 0;
                string line = reader.ReadLine();
                while(line != null)
                {
                    string[] entries = line.Split(',');
					for(int x = 0; x < entries.Length; x++)
                    {
						int intType = Convert.ToInt32(entries[x]);
                        new GridSquare(this, (TileType)intType, x, y);
                    }
                    y++;
					line = reader.ReadLine();
                }
                reader.Close();
                return true;
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("There was a problem during file reading", e.Message);
            return false;
        }
    }

    private void InitializeGrid(string filePath)
	{
		InstantiationGridWidth = 0;
		InstantiationGridHeight = 0;
        StreamReader reader = new StreamReader(filePath, Encoding.Default);
		using(reader)
		{
			string line = reader.ReadLine();
			if(line != null)
			{
				string[] entries = line.Split(',');
                InstantiationGridWidth = entries.Length;
                InstantiationGridHeight++;
			}
            line = reader.ReadLine();
			while(line != null)
			{
                InstantiationGridHeight++;
                line = reader.ReadLine();
			}
		}
        InstantiationGridSquareGameObjectGrid = new GameObject[InstantiationGridWidth, InstantiationGridHeight];
        InstantiationGridSquareGrid = new GridSquare[InstantiationGridWidth, InstantiationGridHeight];
	}
	
	void Update () 
	{
		GetMouseRays();
		UpdateSpawnTile();
		UpdateMonsterAction();
		timer -= Time.deltaTime; 
		
		if (timer <= 0) 
		{ 
			timer = 0.0f; 
		} 
	}
	void PauseGame() 
	{ 
		Time.timeScale = 0.000001f; 
	} 
	void OnGUI () 
	{
		GUI.Box (new Rect (Screen.width - 50, 0, 50, 20), "" + timer.ToString ("f0")); 
		if (timer <= 0) 
		{ 
			if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 60, 100, 25), "Try Again?")) 
			{
				Application.LoadLevel (Application.loadedLevelName); 
			} 
		} 
	}

	void GetMouseRays()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100))
			{
				// All cubes will need a ray layer later so that when you click the monster sprites, it won't interfere with changing gates
				GameObject pObject = hit.transform.gameObject;
				if(pObject.renderer.sharedMaterial != And && pObject.renderer.sharedMaterial != Or)
				{
					return;
				}
				TileType tileType = TileType.Or;
				if(pObject.renderer.sharedMaterial == Or)
				{
					pObject.renderer.material = And;
					tileType = TileType.And;
				}
				else if(pObject.renderer.sharedMaterial == And)
				{
					pObject.renderer.material = Or;
					tileType = TileType.Or;
				}
                InstantiationGridSquareGrid[XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y].GridSquareTileType = tileType;
			}
		}
	}
	void UpdateSpawnTile()
	{
		for(int x = 0; x < InstantiationGridWidth; x++) 
		{
            for(int y = 0; y < InstantiationGridHeight; y++) 
			{
				if(InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterZero && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn == 0)
				{
					Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, NumberType.Zero, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
                    InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
				else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterOne && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn == 0)
				{
                    Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, NumberType.One, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
                else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterZero || InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterOne)
				{
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn--;
				}
			}
		}
	}

	void UpdateMonsterAction()
	{
		List<Monster> tempList = new List<Monster>();
		foreach (Monster monster in InstantiationMonsters)
		{
			tempList.Add (monster);
		}
        foreach(Monster m in tempList)
        {
			Monster monster = InstantiationMonsters.Find(n => n.MonsterId == m.MonsterId); 
            if(monster != null)
			{
				switch(monster.MonsterMovementType)
	            {
	                case MovementType.Moving:
	                    MoveMonster(monster);
	                    break;
	                case MovementType.Waiting:
	                    AssignNewStatus(monster);
	                    break;
	                default:
	                    Instantiation.PrintMessage("Invalid MonsterMovementType - UpdateMonsterAction()");
	                    break;
	            }
			}
        }
	}

	void AssignNewStatus(Monster monster)
	{
        int x = XOFFSET + (int)Math.Round(monster.MonsterGameObject.transform.position.x);
		int y = YOFFSET - (int)Math.Round(monster.MonsterGameObject.transform.position.y);
        switch(InstantiationGridSquareGrid[x, y].GridSquareTileType)
        {
			case TileType.EnterZero:
			case TileType.EnterOne:
				InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
				break;
            case TileType.ExitZero: // @RCH: Once win condition is set, remove this
			case TileType.ExitOne: // @RCH: Once win condition is set, remove this
				CheckForWin(x,y,monster);
                break;
            case TileType.And:
            case TileType.Or:
                monster.MonsterMovementType = MovementType.Waiting;
                bool shouldRemove = false;
                int index1 = 0;
                int index2 = 0;
                foreach(Monster m in InstantiationMonsters)
                {
                    if(m.MonsterXPosition == x && m.MonsterYPosition == y && m.MonsterId != monster.MonsterId && m.MonsterMovementType == MovementType.Waiting)
                    {
                        shouldRemove = true;
                        index1 = InstantiationMonsters.IndexOf(monster);
                        index2 = InstantiationMonsters.IndexOf(m);
                        NumberType numberType = NumberType.Zero;
                        if(InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.And && monster.MonsterNumberType == NumberType.One && m.MonsterNumberType == NumberType.One)
                        {
                            numberType = NumberType.One;
                        }
                        else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.Or && (monster.MonsterNumberType == NumberType.One || m.MonsterNumberType == NumberType.One))
                        {
                            numberType = NumberType.One;
                        }
                        Monster tempMonster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, numberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.None);
						InstantiationGridSquareGrid[x, y].CalculateNewDirection(tempMonster);
                        break;
                    }
                }
                if(shouldRemove)
                {
                    Destroy(InstantiationMonsters[index1].MonsterGameObject, 0f);
                    Destroy(InstantiationMonsters[index2].MonsterGameObject, 0f);
					if(index1 > index2)
					{
						InstantiationMonsters.RemoveAt(index1);
						InstantiationMonsters.RemoveAt(index2);
					}
					else
					{
						InstantiationMonsters.RemoveAt(index2);
						InstantiationMonsters.RemoveAt(index1);
					}
                }
                break;
            case TileType.BeltUp:
            case TileType.BeltRight:
            case TileType.BeltDown:
            case TileType.BeltLeft:
			case TileType.BeltUpLeft:
			case TileType.BeltUpRight:
			case TileType.BeltRightUp:
			case TileType.BeltRightDown:
			case TileType.BeltDownRight:
			case TileType.BeltDownLeft:
			case TileType.BeltLeftDown:
			case TileType.BeltLeftUp:
			case TileType.BeltUpT:
			case TileType.BeltRightT:
			case TileType.BeltDownT:
			case TileType.BeltLeftT:
				InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
                break;
            default:
                Instantiation.PrintMessage("Invalid GridSquareTileType - AssignNewStatus(Monster monster)");
                break;
        }
	}

	void MoveMonster(Monster monster)
	{
        int moveX = 0;
        int moveY = 0;

        if(monster.MonsterMovementDirection == MovementDirection.Up)
        {
            moveY = 1;
        }
        else if (monster.MonsterMovementDirection == MovementDirection.Right)
        {
            moveX = 1;
        }
        else if (monster.MonsterMovementDirection == MovementDirection.Down)
        {
            moveY = -1;
        }
        else if (monster.MonsterMovementDirection == MovementDirection.Left)
        {
            moveX = -1;
        }

		monster.MonsterGameObject.transform.position += new Vector3(monster.MonsterMovementIncrement * moveX * Time.timeScale, monster.MonsterMovementIncrement * moveY * Time.timeScale, 0);

        if(monster.FinishedMovingTile())
		{
            monster.MonsterMovementType = MovementType.Waiting;
        }
	}

	void CheckForWin(int x, int y, Monster monster)
	{
		//assign the current tile to win or lose from blank. cant be assigned unless blank or win.(if you lose you lose)
		//check all tiles, if any of the end pieces are lose then return you lose
		//if the amount of winning tiles == the number of end pieces then you win
		bool PlayerLost = false;
		int numWinningExits = 0;
		int totalNumExits = 0;

		if(monster.MonsterNumberType == NumberType.One && InstantiationGridSquareGrid[x,y].GridSquareTileType == TileType.ExitOne || 
		   monster.MonsterNumberType == NumberType.Zero && InstantiationGridSquareGrid[x,y].GridSquareTileType == TileType.ExitZero)
		{
			if(InstantiationGridSquareGrid[x,y].GridSquareHasWinningPiece != WinCondition.Incorrect)
			{
				InstantiationGridSquareGrid[x,y].GridSquareHasWinningPiece = WinCondition.Correct;
			}
		}
		else
		{
			InstantiationGridSquareGrid[x,y].GridSquareHasWinningPiece = WinCondition.Incorrect;
		}

		for (int i = 0; i < InstantiationGridSquareGrid.GetLength(0); i++) 
		{
			for (int j = 0; j < InstantiationGridSquareGrid.GetLength(1); j++) 
			{
				if(InstantiationGridSquareGrid[i,j].GridSquareTileType == TileType.ExitOne ||
				   InstantiationGridSquareGrid[i,j].GridSquareTileType == TileType.ExitZero)
				{
					if(InstantiationGridSquareGrid[i,j].GridSquareHasWinningPiece == WinCondition.Incorrect)
						PlayerLost = true;
					if(InstantiationGridSquareGrid[i,j].GridSquareHasWinningPiece == WinCondition.Correct)
						numWinningExits ++;

					totalNumExits++;
				}
			}
		}

		if (PlayerLost)
		{
			PrintMessage ("YOU LOSE!");
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 0, 0);
			cube.transform.localScale = new Vector3(10, 10, 10);
			cube.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			cube.renderer.material = Lose;
		}
		else if (numWinningExits == totalNumExits)
		{
			PrintMessage ("YOU WIN!");
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 0, 0);
			cube.transform.localScale = new Vector3(10, 10, 10);
			cube.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			cube.renderer.material = Win;
		}
		else
		{
			PrintMessage ("GOOD JOB KEEP GOING!");
		}

		monster.DestroyEntirely ();
	}

	public static void PrintMessage(string printStatement)
	{
		print (printStatement);
	}
}
