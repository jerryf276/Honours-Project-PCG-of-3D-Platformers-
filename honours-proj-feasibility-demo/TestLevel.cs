using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Godot.TextServer;


public enum PlatformTypes {FLAT, INCLINE, BRIDGE};
enum ActionStates {WALK, TURN_LEFT, TURN_RIGHT, JUMP};

//NONE is used for changing direction since a length doesn't exist for a direction change.
public enum Lengths {SHORT, MEDIUM, LONG, NONE};

//which direction the platform will be facing
enum Direction {POSITIVE_X, NEGATIVE_X, POSITIVE_Z, NEGATIVE_Z};


public enum NewDirection { FORWARD, LEFT, RIGHT};


//We might use this enum for the level grammar
enum PlatformYPosition { ASCENDING, NEUTRAL, DESCENDING};
//Player will either jump up, jump only forwards, or jump down.

//short - small platform, medium - medium platform, long - large platform
public partial class TestLevel : Node3D
{
	//Current position so that we can translate the platform when spawning it
	Vector3 currentPosition;
	bool levelSpawned = false;

	//Number to add based on space of a platform or jump
	float numberToAdd = 0;
	Vector3 translationVector;
	//Chance of direction changing, we have made 50 the max 
	[Export] Timer gameTimer;
	[Export] ComponentSpawner compSpawner;
	[Export(PropertyHint.Range, "0, 50")] private uint directionChangeChance;

	//Default values will be 1.
	[ExportGroup("Spawn Chances")]
	//Chance of spawning small platform
	[ExportSubgroup("Platform Spawn Chances")]
	[Export(PropertyHint.Range, "1, 100")] private uint smallPlatformSpawnChance = 1;
	//Chance of spawning medium platform
	[Export(PropertyHint.Range, "1, 100")] private uint mediumPlatformSpawnChance = 1;
	//Chance of spawning large platform
	[Export(PropertyHint.Range, "1, 100")] private uint largePlatformSpawnChance = 1;

	[ExportSubgroup("Jump Gap Spawn Chances")]
	[Export(PropertyHint.Range, "1, 100")] private uint smallJumpGapSpawnChance = 1;
	[Export(PropertyHint.Range, "1, 100")] private uint mediumJumpGapSpawnChance = 1;
	[Export(PropertyHint.Range, "1, 100")] private uint largeJumpGapSpawnChance = 1;

	[ExportSubgroup("Platform Type Spawn Chances")]
	[Export(PropertyHint.Range, "1, 100")] private uint flatPlatformTypeSpawnChance = 1;
	[Export(PropertyHint.Range, "1, 100")] private uint inclinePlatformTypeSpawnChance = 1;
	[Export(PropertyHint.Range, "1, 100")] private uint bridgePlatformTypeSpawnChance = 1;

	[ExportGroup("Number of Sections")]
	[Export(PropertyHint.Range, "1, 10")] private uint numberOfSections = 1;

	


	//Value which combines the three platform spawn chances together
	private uint combinedPlatformSpawnChance;

	//Value which combines three jump gap spawn chances together
	private uint combinedJumpGapSpawnChance;

	private uint combinedPlatformTypeChance;

	//Size of section, which in this case is the size of the level for now.
	//In the future we will make this an array of section sizes when we develop multiple level sections
	[ExportGroup("Sections")]
	[Export] int sectionSize { get; set; }

	bool spawnSection = true;

	private uint sectionsSpawned = 0;

	NewDirection currentDirection = NewDirection.FORWARD;

	private struct LevelComponent
	{
		//Refer to ActionStates enum class
		public ActionStates action;
		//Refer to Lengths enum class
		public Lengths lengthOfComponent;
		public LevelComponent() 
		{
			lengthOfComponent = Lengths.NONE;
		}

		public LevelComponent(ActionStates actionType, Lengths compLength)
		{
			action = actionType;
			lengthOfComponent = compLength;
		}


	}
	public override void _Ready()
	{
		currentPosition = new Vector3(0, 0, 0);

		translationVector = new Vector3(0, 0, 0);

		//GD.Print("Number: " + smallJumpGapSpawnChance);

		combinedPlatformSpawnChance = smallPlatformSpawnChance + mediumPlatformSpawnChance + largePlatformSpawnChance;

		combinedJumpGapSpawnChance = smallJumpGapSpawnChance + mediumJumpGapSpawnChance + largeJumpGapSpawnChance;

		gameTimer.Start();

		//gameTimer.Timeout += OnTimeOut;
	}

	public override void _Process(double delta)
	{

		//Ideally, each spawn we could change the actions.

		if (spawnSection && (sectionsSpawned < numberOfSections))
		{
            List<ActionStates> actionsToAdd = new List<ActionStates> { };

			//Once three platforms are spawned, we will allow directions to be changed.
			//When a direction changes, this number is reset back to 0.
			int platformsSpawned = 0;


            for (int i = 0; i < sectionSize; ++i)
            {

				if (platformsSpawned < 3)
				{
                    actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP });
					platformsSpawned++;
                }

				else
				{
                    uint directionChange = 1 + GD.Randi() % 100;

                    if (directionChange < directionChangeChance)
                    {

                        if (currentDirection == NewDirection.FORWARD)
                        {
                            uint direction = GD.Randi() % 2;
                            if (direction == 0)
                            {
                                //actionsToAdd.AddRange(ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.JUMP);
                                actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.JUMP });
								currentDirection = NewDirection.LEFT;
                            }
                            else
                            {
                                actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_RIGHT, ActionStates.JUMP });
								currentDirection = NewDirection.RIGHT;
                            }
                        }

						else if (currentDirection == NewDirection.LEFT)
						{
                            actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_RIGHT, ActionStates.JUMP });
							currentDirection = NewDirection.FORWARD;
                        }

						else if (currentDirection == NewDirection.RIGHT)
						{
                            actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.JUMP });
							currentDirection = NewDirection.FORWARD;
                        }


							platformsSpawned = 0;
                    }
                    else
                    {
                        //Direction not changing
                        actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP });
                    }
                }
            }

            //Left over from when actions were previously predefined
            //List<ActionStates> actionsToAdd = new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.JUMP, ActionStates.WALK, ActionStates.JUMP, ActionStates.WALK, ActionStates.TURN_RIGHT, ActionStates.JUMP, ActionStates.WALK, ActionStates.JUMP, ActionStates.WALK, ActionStates.JUMP, ActionStates.WALK};

            //int - index of jump or walk in actionsToAdd above, lengths - will be either short, medium or long
            SortedDictionary<int, Lengths> actionSizes = new SortedDictionary<int, Lengths> { };

            //List of components of level that will be generated
            List<LevelComponent> levelComponents = new List<LevelComponent> { };

            //    levelStates.Add(ActionStates.WALK, ActionStates.JUMP, ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.WALK);
            for (int i = 0; i < actionsToAdd.Count; ++i)
            {
                Lengths lengthToAdd;
                if (actionsToAdd[i] == ActionStates.WALK)
                {
                    //Chooses a number between 1 to the total number of each platform spawn chance probability
                    uint rng = 1 + GD.Randi() % combinedPlatformSpawnChance;
                    //	GD.Print(rng);

                    if (rng > smallPlatformSpawnChance + mediumPlatformSpawnChance)
                    {
                        //Adds large platform
                        lengthToAdd = Lengths.LONG;
                    }

                    else if (rng > smallPlatformSpawnChance)
                    {
                        //Adds medium platform
                        lengthToAdd = Lengths.MEDIUM;
                    }

                    else
                    {
                        //Adds small platform
                        lengthToAdd = Lengths.SHORT;
                    }
                }

                else if (actionsToAdd[i] == ActionStates.JUMP)
                {
                    translationVector.Y = 0;
                    //Chooses a number between 1 to the total number of each jump gap probability
                    uint rng = 1 + GD.Randi() % combinedJumpGapSpawnChance;

                    if (rng > smallJumpGapSpawnChance + mediumJumpGapSpawnChance)
                    {
                        //Adds large jump gap
                        lengthToAdd = Lengths.LONG;
                    }

                    else if (rng > smallJumpGapSpawnChance)
                    {
                        //Adds medium jump gap
                        lengthToAdd = Lengths.MEDIUM;
                    }

                    else
                    {
                        //Adds small jump gap
                        lengthToAdd = Lengths.SHORT;
                    }

                }

                else
                {
                    translationVector.Y = 0;
                    //Adds turning direction to the section of the level
                    lengthToAdd = Lengths.NONE;
                }

                levelComponents.Add(new LevelComponent(actionsToAdd[i], lengthToAdd));
            }
            //Generates level once each component of the level has been defined

            GenerateLevel(levelComponents);
            //levelSpawned = true;


            spawnSection = false;
			sectionsSpawned++;

			//GD.Print(sectionSize);
		}

	}

	private void GenerateLevel(List<LevelComponent> componentsToAdd)
	{
		//Default direction is +ve x
		Direction direction = Direction.POSITIVE_X;

		NewDirection newDirection = NewDirection.FORWARD;

		//Default y position of platform will be neutral (in this case platform y position will be same as starting y position if first platform,
		//or same y position as last platform generated
		PlatformYPosition platformYPosition = PlatformYPosition.NEUTRAL;

	
		//How high the jump will be
		List<float> jumpHeight = new List<float> { 3.0f, 6.0f, 9.0f };

		//How long the jump is
		//Index 0 - short, Index 1 - medium, index 2 - long	
		List<float> jumpGaps = new List<float> { 1.5f, 3.0f, 6.0f };

		//Float since the platform size could not be a whole number
		//Index 0 - short, Index 1 - medium, index 2 - long
		List<float> platformSizes = new List<float> { 6.0f, 9.0f, 12.0f };

	  for (int i = 0; i < componentsToAdd.Count; i++)
		{
			if (componentsToAdd[i].action == ActionStates.WALK)
			{
				//Spawns a platform
				if (i >= componentsToAdd.Count - 1)
				{
                    SpawnPlatform(componentsToAdd[i].lengthOfComponent, newDirection, platformSizes, componentsToAdd[i].action);
                }

				else
				{
                    SpawnPlatform(componentsToAdd[i].lengthOfComponent, newDirection, platformSizes, componentsToAdd[i + 1].action);
                }

				//To prevent array errors when checking if the next element is turning left or right
				if (componentsToAdd.Count > (i + 2)) {
					if (componentsToAdd[i + 1].action == ActionStates.TURN_LEFT || componentsToAdd[i + 1].action == ActionStates.TURN_RIGHT)
					{
						//GD.Print("Direction changed!");
						//direction = ChangeDirection(direction, componentsToAdd[i + 1].action);
						newDirection = changeDirection(newDirection, componentsToAdd[i + 1].action);
					}
					AddCurrentPosition(newDirection);
				}
			}

			else if (componentsToAdd[i].action == ActionStates.JUMP)
			{
				AddJumpSpace(componentsToAdd[i].lengthOfComponent, newDirection, jumpGaps, jumpHeight);
			}
 
		}
        AddCurrentPosition(newDirection);
        GenerateCheckpoint();
		compSpawner.generateCoins();
		compSpawner.generateSpikes();
       // AddCurrentPosition(direction);

    }


	private void SpawnPlatform(Lengths platformLength, NewDirection currentDirection, List<float> platSizes, ActionStates nextAction)
	{
		PackedScene platform;

		PlatformTypes platformType = typeToChoose();

		string platformTypeToSpawn = platformPath(platformLength, platformType, nextAction);
		//string platformTypeToSpawn = typeToChoose(platformLength, nextAction);
        platform = ResourceLoader.Load<PackedScene>(platformTypeToSpawn);

        Node3D newPlatform = platform.Instantiate<Node3D>();

		newPlatform.RotationDegrees = new Vector3(0, determineAngle(currentDirection), 0);

        //Translate it by currentPosition.
        newPlatform.Position = currentPosition;
		//GD.Print(newPlatform.Position);
		AddChild(newPlatform);




		if (platformType == PlatformTypes.BRIDGE)
		{
			//if (compSpawner != null)
			//{
				compSpawner.addCoinPlatform(platformType, platformLength, newPlatform.Position, currentDirection);
			//}
		}

		else if (platformType == PlatformTypes.FLAT)
		{
			compSpawner.addSpikePlatform(platformType, platformLength, newPlatform.Position, currentDirection);
		}
		//spawnCoins(currentDirection, 1, 1, "", newPlatform.Position);
	}

	private PlatformTypes typeToChoose()
	{
        uint combinedPlatformTypeChance = bridgePlatformTypeSpawnChance + flatPlatformTypeSpawnChance + inclinePlatformTypeSpawnChance;
        uint rng = 1 + GD.Randi() % combinedPlatformTypeChance;
		if (rng > flatPlatformTypeSpawnChance + inclinePlatformTypeSpawnChance)
		{
			return PlatformTypes.BRIDGE;
		}

		else if (rng > flatPlatformTypeSpawnChance)
		{
			return PlatformTypes.INCLINE;
		}

		else
		{
			return PlatformTypes.FLAT;
		}

        }

	private string platformPath(Lengths lenOfPlatform, PlatformTypes type, ActionStates nextAction)
	{
		if (type == PlatformTypes.BRIDGE)
		{
            translationVector.Y = 0;
            if (lenOfPlatform == Lengths.SHORT)
			{
				numberToAdd = 6.0f;

				return "res://smallBridgePlatform.tscn";
            }

			else if (lenOfPlatform == Lengths.MEDIUM)
			{
				numberToAdd = 10.0f;
				return "res://mediumBridgePlatform.tscn";
            }

			else if (lenOfPlatform == Lengths.LONG)
			{
				numberToAdd = 14.0f;
				return "res://largeBridgePlatform.tscn";
            }
		}

		else if (type == PlatformTypes.INCLINE)
		{
			if (lenOfPlatform == Lengths.SHORT)
			{
                numberToAdd = 9.0f;
                if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                {
                    numberToAdd = 7.5f;
                }
                translationVector.Y = 2;
                return "res://inclinePlatformSmall.tscn";
            }

			else if (lenOfPlatform == Lengths.MEDIUM)
			{
                numberToAdd = 12.0f;

                if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                {
                    numberToAdd = 9.5f;
                }
                translationVector.Y = 3;
                return "res://inclinePlatformMedium.tscn";
            }

			else if (lenOfPlatform == Lengths.LONG)
			{
                numberToAdd = 18.0f;

                if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                {
                    numberToAdd = 11.5f;
                }

                translationVector.Y = 5;
                return "res://inclinePlatformLarge.tscn";
            }
		}

		else
		{
			translationVector.Y = 0;

            if (lenOfPlatform == Lengths.SHORT)
			{
                numberToAdd = 6.0f;
                return "res://small_platform.tscn";
            }

			else if (lenOfPlatform == Lengths.MEDIUM)
			{
                numberToAdd = 9.0f;
                return "res://medium_platform2.tscn";
            }

			else
			{
                numberToAdd = 12.0f;
                return "res://large_platform.tscn";
            }
		}

        return "res://large_platform.tscn";
    }

	private void AddCurrentPosition(NewDirection currentDirection)
	{
		switch (currentDirection)
		{
			case NewDirection.FORWARD:
                currentPosition += new Vector3(numberToAdd, translationVector.Y, 0);
                break;

			case NewDirection.LEFT:
				currentPosition += new Vector3(0, translationVector.Y, -numberToAdd);
				break;

			case NewDirection.RIGHT:
				currentPosition += new Vector3(0, translationVector.Y, numberToAdd);
				break;
		}
	}

    private void AddCurrentPosition(Direction currentDirection, int inclineHeight)
    {
        switch (currentDirection)
        {
            //Make it so its not 0 when its an incline platform
            case Direction.POSITIVE_X:
                currentPosition += new Vector3(numberToAdd, 0, 0);
                break;
            case Direction.NEGATIVE_X:
                currentPosition += new Vector3(-numberToAdd, 0, 0);
                break;
            case Direction.POSITIVE_Z:
                currentPosition += new Vector3(0, 0, numberToAdd);
                break;
            case Direction.NEGATIVE_Z:
                currentPosition += new Vector3(0, 0, -numberToAdd);
                break;
        }
    }


	private int determineAngle(NewDirection currentDirection)
	{
		int angle = 0;
		switch (currentDirection)
		{
			case NewDirection.FORWARD:
				angle = 0;
				break;
			case NewDirection.RIGHT:
				angle = 270;
				break;
			case NewDirection.LEFT:
				angle = 90;
				break;
		}

		return angle;

	}

	void AddJumpSpace(Lengths jumpLength, NewDirection currentDirection, List<float> jumpSizes, List<float> jumpHeight)
	{
		//TO DO: See if you could maybe combine this and the spawn platform functions? (to avoid repetition)
		float numberToAdd = 0;

		//for now we'll do a probability of 1/3 for deciding if the player ascends up, down, or does not.
		uint rng = 1 + GD.Randi() % 3;


		//Look into fixing this?
		float yPosition = 0.0f;

		switch (rng)
		{
			case 1:
				yPosition = 1.5f;
				break;
			case 2:
				yPosition = -1.5f;
				break;
			default:
				break;
		}

		switch (jumpLength)
		{
			case Lengths.SHORT:
				numberToAdd = jumpSizes[0];
				break;
			case Lengths.MEDIUM:
				numberToAdd = jumpSizes[1];
				break;
			case Lengths.LONG:
				yPosition /= 2;
				numberToAdd = jumpSizes[2];
				break;
		}

		switch (currentDirection) 
		{

            case NewDirection.FORWARD:
                currentPosition += new Vector3(numberToAdd, yPosition, 0);
                break;

            case NewDirection.LEFT:
                currentPosition += new Vector3(0, yPosition, -numberToAdd);
                break;

            case NewDirection.RIGHT:
                currentPosition += new Vector3(0, yPosition, numberToAdd);
                break;
		}
	}

	private void GenerateCheckpoint()
	{
        PackedScene platform;

        platform = ResourceLoader.Load<PackedScene>("res://large_platform.tscn");

		Node3D newPlatform = platform.Instantiate<Node3D>();
        //Translate it by currentPosition.
		newPlatform.Position = currentPosition;
        AddChild(newPlatform);

		PackedScene checkpointLoad;


		checkpointLoad = ResourceLoader.Load<PackedScene>("res://Level parts/checkpoint.tscn");
		Node3D checkpoint = checkpointLoad.Instantiate<Node3D>();
		checkpoint.Position = new Vector3(currentPosition.X, currentPosition.Y + 3.0f, currentPosition.Z);
        AddChild(checkpoint);
        //checkpoint.Position.Y = currentPosition.Y + 1;

    }

	private void GenerateGoal()
	{
		PackedScene platform;
        platform = ResourceLoader.Load<PackedScene>("res://large_platform.tscn");


        Node3D goal = platform.Instantiate<Node3D>();
        //Translate it by currentPosition.
        goal.Position = currentPosition;
        AddChild(goal);

		PackedScene goalLoad;

		goalLoad = ResourceLoader.Load<PackedScene>("res://Level parts/goal.tscn");
		goal.Position = new Vector3(currentPosition.X, currentPosition.Y + 3.0f, currentPosition.Z);
		AddChild(goal);

    }
    public void setCheckpointReached(bool checkpointReached)
	{
		spawnSection = checkpointReached;
	}

	private NewDirection changeDirection(NewDirection currentDirection, ActionStates currentActionState)
	{
		if (currentDirection == NewDirection.FORWARD)
		{
			if (currentActionState == ActionStates.TURN_RIGHT)
			{
				currentDirection = NewDirection.RIGHT;
			}

			else
			{
				currentDirection = NewDirection.LEFT;
			}
		}

		else if (currentDirection == NewDirection.RIGHT || currentDirection == NewDirection.LEFT)
		{
			currentDirection = NewDirection.FORWARD;
		}

		return currentDirection;
	}


	private void spawnBridgeCoins()
	{

	}
	private void spawnCoins(NewDirection currentDirection, int platformLength, int platformWidth, string platformType, Vector3 platformPosition)
	{
		PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://Level parts/coin.tscn");
		Node3D coin = coinScene.Instantiate<Node3D>();
		coin.Position = platformPosition;
		Vector3 coinPosition = coin.Position;
		coinPosition.Y += 1;
		coin.Position = coinPosition;
		GetTree().Root.AddChild(coin);


    }
}
