using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Godot.TextServer;


public enum PlatformTypes {FLAT, INCLINE, BRIDGE};
enum ActionStates {WALK, TURN_LEFT, TURN_RIGHT, JUMP, BOUNCE};

//NONE is used for changing direction since a length doesn't exist for a direction change.
public enum Lengths {SHORT, MEDIUM, LONG, LONGEST, NONE};

//New, refined, direction
//which direction the platform will be facing
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
    private uint directionChangeChance;

	//Default values will be 1.
	//Chance of spawning small platform
	private uint smallPlatformSpawnChance = 1;
	//Chance of spawning medium platform
	private uint mediumPlatformSpawnChance = 1;
	//Chance of spawning large platform
	private uint largePlatformSpawnChance = 1;
    //Chance of spawning extra large platform
    private uint extraLargePlatformSpawnChance = 1;

	//chance of spawning jump gaps
	private uint smallJumpGapSpawnChance = 1;
	private uint mediumJumpGapSpawnChance = 1;
	private uint largeJumpGapSpawnChance = 1;

	//chance of spawning platform types
	private uint flatPlatformTypeSpawnChance = 1;
	private uint inclinePlatformTypeSpawnChance = 1;
	private uint bridgePlatformTypeSpawnChance = 1;

	//chance of spawning bouncer, coin, or spikes
	private uint bounceSpawnRate = 5;
	private uint coinSpawnRate = 0;
	private uint spikeSpawnRate = 0;

	private uint numberOfSections = 1;

	


	//Value which combines the three platform spawn chances together
	private uint combinedPlatformSpawnChance;

	//Value which combines three jump gap spawn chances together
	private uint combinedJumpGapSpawnChance;

	private uint combinedPlatformTypeChance;

	private uint easySpikeChance = 1;
	private uint hardSpikeChance = 1;

	//size of a section
	int sectionSize { get; set; }

	bool spawnSection = false;

	private uint sectionsSpawned = 0;

	NewDirection currentDirection = NewDirection.FORWARD;

	//Counting the number of spikes
	//If there is 3 spike areas, we will spawn a health pack 
	int spikeAreaCount = 0;


	//Used to separate platforms by section in the json file
	int currentSection = 1;


	//This will be used to spawn a health pack when there is 3 spikes 
	bool spawnHealthPack = false;


	//If a bounce pad on the previous platform spawned and the next platform is an extra large platform, we will translate the current position by 2 to prevent the extra large platform being
	//directly above the bounce platform.
	bool bounceSpawned = false;

	//For spawning the coin and spike patterns onto the platform
	CoinPatterns coinPatterns;
	SpikePatterns spikePatterns;
	//For writing data of the generated level to a json file
	JsonWriter jsonWriter;
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

		combinedPlatformSpawnChance = smallPlatformSpawnChance + mediumPlatformSpawnChance + largePlatformSpawnChance + extraLargePlatformSpawnChance;

		combinedJumpGapSpawnChance = smallJumpGapSpawnChance + mediumJumpGapSpawnChance + largeJumpGapSpawnChance;

		gameTimer.Start();

		coinPatterns = GetNode<CoinPatterns>("../CoinPatterns");
		spikePatterns = GetNode<SpikePatterns>("../SpikePatterns");
		jsonWriter = GetNode<JsonWriter>("../JsonWriter");
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
            spikeAreaCount = 0;

            for (int i = 0; i < sectionSize; ++i)
            {

				if (platformsSpawned < 3)
				{
					if (i != 0)
					{
						if (bouncerDecider())
						{
							//Adds a bounce platform to the actions list
							actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.BOUNCE });

						}

                        else
                        {
							//Adds a platform to the actions list
                            actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP });
                        }
                    }

					else
					{
						actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP });
					}
					platformsSpawned++;
                }

				else if (i != (sectionSize - 1))
				{
                    uint directionChange = 1 + GD.Randi() % 100;

                    if (directionChange < directionChangeChance)
                    {

                        if (currentDirection == NewDirection.FORWARD)
                        {
                            uint direction = GD.Randi() % 2;
                            if (direction == 0)
                            {
								//Changes direction to left
                                actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.JUMP });
								currentDirection = NewDirection.LEFT;
                            }
                            else
                            {
								//Changes direction to right
                                actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_RIGHT, ActionStates.JUMP });
								currentDirection = NewDirection.RIGHT;
                            }
                        }

						else if (currentDirection == NewDirection.LEFT)
						{
							//Changes direction to forward
                            actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_RIGHT, ActionStates.JUMP });
							currentDirection = NewDirection.FORWARD;
                        }

						else if (currentDirection == NewDirection.RIGHT)
						{
                            //Changes direction to forward
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

            //int - index of jump or walk in actionsToAdd above, lengths - will be either short, medium or long
            SortedDictionary<int, Lengths> actionSizes = new SortedDictionary<int, Lengths> { };

            //List of components of level that will be generated
            List<LevelComponent> levelComponents = new List<LevelComponent> { };

            for (int i = 0; i < actionsToAdd.Count; ++i)
            {
                Lengths lengthToAdd;
	
                if (actionsToAdd[i] == ActionStates.JUMP)
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

            spawnSection = false;
			spawnHealthPack = false;
			sectionsSpawned++;
            currentSection++;
            jsonWriter.outputGeneratedLevelToJson();
		}

	}

	private void GenerateLevel(List<LevelComponent> componentsToAdd)
	{
		//Default direction is forward
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
					//For last platform, it will ignore the last action stuff for this function since the current action is spawning a platform
                    SpawnPlatform(componentsToAdd[i].lengthOfComponent, newDirection, platformSizes, componentsToAdd[i].action);
                }

				else
				{
					//Spawning a platform that is not the last one
                    SpawnPlatform(componentsToAdd[i].lengthOfComponent, newDirection, platformSizes, componentsToAdd[i + 1].action);
                }

				//To prevent array errors when checking if the next element is turning left or right
				if (componentsToAdd.Count > (i + 2)) {
					if (componentsToAdd[i + 1].action == ActionStates.TURN_LEFT || componentsToAdd[i + 1].action == ActionStates.TURN_RIGHT)
					{
						newDirection = changeDirection(newDirection, componentsToAdd[i + 1].action);
					}

                    else if (componentsToAdd[i + 1].action == ActionStates.BOUNCE)
                    {
                        spawnBouncer();
						//spawn a bouncer
						bounceSpawned = true;
                    }

					if (componentsToAdd[i + 1].action != ActionStates.BOUNCE)
					{
						//do not spawn a bouncer
                        bounceSpawned = false;
                    }

					//Translate the current position after spawning a platform
                    AddCurrentPosition(newDirection);
				}
			}

			else if (componentsToAdd[i].action == ActionStates.JUMP)
			{
				//Adding a jump space
				AddJumpSpace(componentsToAdd[i].lengthOfComponent, newDirection, jumpGaps, jumpHeight);
			}
 
		}
	  //Jump gap between second last and last platform
        AddCurrentPosition(newDirection);

		if (sectionsSpawned == numberOfSections - 1)
		{
			//When it is the last section, spawn the goal
			GenerateGoal();
		}
		else
		{
			//Otherwise generate a checkpoint
            GenerateCheckpoint();
        }

    }


	private void SpawnPlatform(Lengths platformLength, NewDirection currentDirection, List<float> platSizes, ActionStates nextAction)
	{
		PackedScene platform;
		PlatformTypes platformType;

		bool bounceMode;
        if (nextAction == ActionStates.BOUNCE) { 
			//If a bounce pad will be spawned, make the next platform a flat platform
			platformType = PlatformTypes.FLAT;
			bounceMode = true;
		}
		else
		{
			//Otherwise let the type to choose function select the platform type
            platformType = typeToChoose();
			bounceMode = false;
        }

		//determines the size of the platform
		platformLength = determinePlatformLength(platformType);
		string platformTypeToSpawn = platformPath(platformLength, platformType, nextAction, currentDirection);
        platform = ResourceLoader.Load<PackedScene>(platformTypeToSpawn);

        Node3D newPlatform = platform.Instantiate<Node3D>();

		//Determining platform rotation
		newPlatform.RotationDegrees = new Vector3(0, determineAngle(currentDirection), 0);

        //Translate it by currentPosition.
        newPlatform.Position = currentPosition;
		AddChild(newPlatform);
		jsonWriter.addPlatform(currentPosition, newPlatform.SceneFilePath, currentSection);
		jsonWriter.addLevelPartToJson(currentPosition, newPlatform.SceneFilePath, currentSection);

		if (platformType == PlatformTypes.FLAT && bounceMode == false)
		{
			//Choosing between a spike and a coin or nothing
			uint rng = 1 + GD.Randi() % 200;
			if (rng <= spikeSpawnRate)
			{
				//Spawning a spike
				if (currentPosition != new Vector3(0, 0, 0))
				{
					//Choosing between easy and hard spikes
					uint rng2 = 1 + GD.Randi() % +(easySpikeChance + hardSpikeChance);
					if (rng2 <= hardSpikeChance)
					{
						spikePatterns.spawnSpikes(platformLength, currentPosition, currentDirection, true);
					}

					else
					{
						spikePatterns.spawnSpikes(platformLength, currentPosition, currentDirection, false);
					}
					spikeAreaCount++;
					spawnHealthPack = false;
				}
			}
			else if (rng <= coinSpawnRate + spikeSpawnRate)
			{
				//Spawning a coin
                coinPatterns.spawnCoins(platformType, platformLength, currentPosition, currentDirection);
            }
		}

		else if (bounceMode == false)
		{
			//Choosing between a coin or nothing
            uint rng = 1 + GD.Randi() % 100;
			if (rng <= coinSpawnRate)
			{
				coinPatterns.spawnCoins(platformType, platformLength, currentPosition, currentDirection);
			}
        }

        if (spawnHealthPack == true)
        {
			//Spawning a health pack
            PackedScene healthPackScene = ResourceLoader.Load<PackedScene>("res://levelParts/healthOrb.tscn");

            Node3D healthOrb = healthPackScene.Instantiate<Node3D>();
            healthOrb.Position = new Vector3(currentPosition.X, currentPosition.Y + 2, currentPosition.Z);
			if (bounceMode == true)
			{
				healthOrb.Position += new Vector3(0, 3, 0);
			}
            AddChild(healthOrb);
            spawnHealthPack = false;
            spikeAreaCount = 0;
        }
        
		if (spikeAreaCount == 3)
		{
			//A health pack will be spawned at the next platform
			spawnHealthPack = true;
		}
	}

	private PlatformTypes typeToChoose()
	{
		//Choosing between bridge, incline, or flat platform
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

	private Lengths determinePlatformLength(PlatformTypes type)
	{
       
        Lengths lenOfPlatform;
		if (type == PlatformTypes.FLAT)
		{
            uint rng = 1 + GD.Randi() % combinedPlatformSpawnChance;
            if (rng > smallPlatformSpawnChance + mediumPlatformSpawnChance + largePlatformSpawnChance)
			{
				lenOfPlatform = Lengths.LONGEST;
			}
			else if (rng > smallPlatformSpawnChance + mediumPlatformSpawnChance)
			{
				//Adds large platform
				lenOfPlatform = Lengths.LONG;
			}

			else if (rng > smallPlatformSpawnChance)
			{
				//Adds medium platform
				lenOfPlatform = Lengths.MEDIUM;
			}

			else
			{
				//Adds small platform
				lenOfPlatform = Lengths.SHORT;
			}

		}

		else
		{
            uint rng = 1 + GD.Randi() % (combinedPlatformSpawnChance - extraLargePlatformSpawnChance);

            if (rng > smallPlatformSpawnChance + mediumPlatformSpawnChance)
            {
                //Adds large platform
                lenOfPlatform = Lengths.LONG;
            }

            else if (rng > smallPlatformSpawnChance)
            {
                //Adds medium platform
                lenOfPlatform = Lengths.MEDIUM;
            }

            else
            {
                //Adds small platform
                lenOfPlatform = Lengths.SHORT;
            }
        }

		return lenOfPlatform;
    }
	private string platformPath(Lengths lenOfPlatform, PlatformTypes type, ActionStates nextAction, NewDirection currentDirection)
	{
        if (type == PlatformTypes.FLAT)
		{
			//Number to add is how much to translate the current position by when spawning a platform
            if (lenOfPlatform == Lengths.SHORT)
            {
                numberToAdd = 6.0f;
                return "res://levelParts/smallPlatform.tscn";
            }

            else if (lenOfPlatform == Lengths.MEDIUM)
            {
                numberToAdd = 9.0f;
                return "res://levelParts/mediumPlatform.tscn";
            }

            else if (lenOfPlatform == Lengths.LONGEST)
            {
				if (currentDirection == NewDirection.FORWARD)
				{
					//translating by 3 to prevent overlap from previous platform
					currentPosition += new Vector3(3.0f, 0, 0);
                }

				else if (currentDirection == NewDirection.LEFT)
				{
                    //translating by 3 to prevent overlap from previous platform
                    currentPosition += new Vector3(0, 0, -3.0f);
                }

				else
				{
                    //translating by 3 to prevent overlap from previous platform
                    currentPosition += new Vector3(0, 0, 3.0f);
                }
				
				numberToAdd = 21.0f;
                return "res://levelParts/extraLargePlatform.tscn";

            }

            else
            {
                numberToAdd = 12.0f;
                return "res://levelParts/largePlatform.tscn";
            }
        }

		else
		{
            if (type == PlatformTypes.BRIDGE)
            {
                translationVector.Y = 0;
                if (lenOfPlatform == Lengths.SHORT)
                {
                    numberToAdd = 9.0f;

                    return "res://levelParts/smallBridgePlatform.tscn";
                }

                else if (lenOfPlatform == Lengths.MEDIUM)
                {
                    numberToAdd = 13.0f;
                    return "res://levelParts/mediumBridgePlatform.tscn";
                }

                else if (lenOfPlatform == Lengths.LONG)
                {
                    numberToAdd = 17.0f;
                    return "res://levelParts/largeBridgePlatform.tscn";
                }
            }

            else if (type == PlatformTypes.INCLINE)
            {
                if (lenOfPlatform == Lengths.SHORT)
                {
                    numberToAdd = 12.0f;
                    if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                    {
						//Translate it less when the direction changes so that it is not too far away
                        numberToAdd = 8.5f;
                    }

					//translates by y a bit to experience climbing up an incline
                    translationVector.Y = 2;
                    return "res://levelParts/inclinePlatformSmall.tscn";
                }

                else if (lenOfPlatform == Lengths.MEDIUM)
                {
                    numberToAdd = 15.0f;

                    if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                    {
                        //Translate it less when the direction changes so that it is not too far away
                        numberToAdd = 10.5f;
                    }

                    //translates by y a bit to experience climbing up an incline
                    translationVector.Y = 3;
                    return "res://levelParts/inclinePlatformMedium.tscn";
                }

                else if (lenOfPlatform == Lengths.LONG)
                {
                    numberToAdd = 21.0f;

                    if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                    {
                        //Translate it less when the direction changes so that it is not too far away
                        numberToAdd = 12.5f;
                    }

                    //translates by y a bit to experience climbing up an incline
                    translationVector.Y = 5;
                    return "res://levelParts/inclinePlatformLarge.tscn";
                }
            }

        }

        return "res://levelParts/largePlatform.tscn";
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
		float numberToAdd = 0;
		uint rng = 1 + GD.Randi() % 3;

		//translating up, down, or no translation
		float yPosition = 0.0f;

		switch (rng)
		{
			//Rng for ascending up or down a little, or keeping the next platform's y position to be the same as the previous one
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
				//Adding a short jump gap
                numberToAdd = jumpSizes[0];
				jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "Small Jump Gap", currentSection);
				break;
			case Lengths.MEDIUM:
                //Adding a medium jump gap
                numberToAdd = jumpSizes[1];
				jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "Medium Jump Gap", currentSection);
				break;
			case Lengths.LONG:
                //Adding a long jump gap
				numberToAdd = jumpSizes[2];
                jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "Long Jump Gap", currentSection);
                break;
		}

		switch (currentDirection) 
		{
			//Translating based on the direction of where the generation is going
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
		//Spawning the platform
        PackedScene platform;
        platform = ResourceLoader.Load<PackedScene>("res://levelParts/largePlatform.tscn");
		Node3D newPlatform = platform.Instantiate<Node3D>();
        //Translate it by currentPosition.
		newPlatform.Position = currentPosition;
        AddChild(newPlatform);
		jsonWriter.addPlatform(currentPosition, newPlatform.SceneFilePath, currentSection);
		jsonWriter.addLevelPartToJson(currentPosition, newPlatform.SceneFilePath, currentSection);

		//Spawning the checkpoint
		PackedScene checkpointLoad;
		checkpointLoad = ResourceLoader.Load<PackedScene>("res://levelParts/checkpoint.tscn");
		Node3D checkpoint = checkpointLoad.Instantiate<Node3D>();
		checkpoint.Position = new Vector3(currentPosition.X, currentPosition.Y + 3.0f, currentPosition.Z);
        AddChild(checkpoint);
		numberToAdd = 12.0f;
		translationVector.Y = 0;
		AddCurrentPosition(currentDirection);

    }

	private void GenerateGoal()
	{
		//Spawning the platform
        PackedScene platform;
        platform = ResourceLoader.Load<PackedScene>("res://levelParts/largePlatform.tscn");
        Node3D newPlatform = platform.Instantiate<Node3D>();
		//Translate it by currentPosition.
		newPlatform.Position = currentPosition;
		AddChild(newPlatform);
        jsonWriter.addPlatform(currentPosition, newPlatform.SceneFilePath, currentSection);
        jsonWriter.addLevelPartToJson(new Vector3(0, 100, 0), newPlatform.SceneFilePath, currentSection);

		//Spawning the goal
        PackedScene goalLoad;
		goalLoad = ResourceLoader.Load<PackedScene>("res://levelParts/goal.tscn");
		Node3D goal = goalLoad.Instantiate<Node3D>();
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
				//Changing direction to right
				currentDirection = NewDirection.RIGHT;
				jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "[Direction changed to right]", currentSection);
			}

			else
			{
				//Changing direction to left
				currentDirection = NewDirection.LEFT;
				jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "[Direction changed to left]", currentSection);
            }
		}

		else if (currentDirection == NewDirection.RIGHT || currentDirection == NewDirection.LEFT)
		{
			//Changing direction back to forward
			currentDirection = NewDirection.FORWARD;
            jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "[Direction changed to forward]", currentSection);
        }

		return currentDirection;
	}

	private void spawnBouncer()
	{
		//Spawns a bouncer
		PackedScene bouncerScene = ResourceLoader.Load<PackedScene>("res://levelParts/bouncer.tscn");
		Node3D bouncer = bouncerScene.Instantiate<Node3D>();
		bouncer.Position = new Vector3(currentPosition.X, currentPosition.Y + 2, currentPosition.Z);
		AddChild(bouncer);
		currentPosition += new Vector3(0, 20, 0);
	}

	private bool bouncerDecider()
	{
		//Determines if a bounce platform should be spawned
		uint rng = 1 + GD.Randi() % 100;

		if (rng <= bounceSpawnRate)
		{
			return true;
		}

		return false;
	}



	//Used in level modifier
	public void setPlatformSpawnChances(uint smallPlatform, uint mediumPlatform, uint largePlatform, uint extraLargePlatform)
	{
		smallPlatformSpawnChance = smallPlatform;
		mediumPlatformSpawnChance = mediumPlatform;
		largePlatformSpawnChance = largePlatform;
		extraLargePlatformSpawnChance = extraLargePlatform;
	}

	public void setJumpGapChances(uint smallJumpGap, uint mediumJumpGap, uint largeJumpGap)
	{
		smallJumpGapSpawnChance = smallJumpGap;
		mediumJumpGapSpawnChance = mediumJumpGap;
		largeJumpGapSpawnChance = largeJumpGap;
	}

	public void setPlatformTypeSpawnChances(uint flatPlatformType, uint inclinePlatformType, uint bridgePlatformType)
	{
		flatPlatformTypeSpawnChance = flatPlatformType;
		inclinePlatformTypeSpawnChance = inclinePlatformType;
		bridgePlatformTypeSpawnChance = bridgePlatformType;
	}

	public void settingSections(uint sizeOfSection, uint sectionNumber)
	{
		numberOfSections = sectionNumber;
		sectionSize = (int)sizeOfSection;
	}

	public void setComponentSpawnRate(uint bouncer, uint spikeRate, uint coinRate)
	{
		bounceSpawnRate = bouncer;
		spikeSpawnRate = spikeRate;
		coinSpawnRate = coinRate;

	}

	public void setSpikeDifficulties(uint easy, uint hard)
	{
		easySpikeChance = easy;
		hardSpikeChance = hard;
	}

	public void setSpawnSection(bool sectionSpawn)
	{
		spawnSection = sectionSpawn;
	}

	public void setDirectionChangeChance(uint directionChange)
	{
		directionChangeChance = directionChange;
	}
}
