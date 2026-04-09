using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Godot.TextServer;


public enum PlatformTypes {FLAT, INCLINE, BRIDGE};
enum ActionStates {WALK, TURN_LEFT, TURN_RIGHT, JUMP, BOUNCE};

//NONE is used for changing direction since a length doesn't exist for a direction change.
public enum Lengths {SHORT, MEDIUM, LONG, LONGEST, NONE};

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
	/*[Export(PropertyHint.Range, "0, 50")]*/ private uint directionChangeChance;

	//Default values will be 1.
	//[ExportGroup("Spawn Chances")]
	//Chance of spawning small platform
	//[ExportSubgroup("Platform Spawn Chances")]
	/*[Export(PropertyHint.Range, "1, 100")]*/ private uint smallPlatformSpawnChance = 1;
	//Chance of spawning medium platform
	/*[Export(PropertyHint.Range, "1, 100")]*/ private uint mediumPlatformSpawnChance = 1;
	//Chance of spawning large platform
	/*[Export(PropertyHint.Range, "1, 100")]*/ private uint largePlatformSpawnChance = 1;
    //Chance of spawning extra large platform
    /*[Export(PropertyHint.Range, "1, 100")]*/ private uint extraLargePlatformSpawnChance = 1;

    //[ExportSubgroup("Jump Gap Spawn Chances")]
	/*[Export(PropertyHint.Range, "1, 100")]*/ private uint smallJumpGapSpawnChance = 1;
	/*[Export(PropertyHint.Range, "1, 100")]*/ private uint mediumJumpGapSpawnChance = 1;
	/*[Export(PropertyHint.Range, "1, 100")]*/ private uint largeJumpGapSpawnChance = 1;

    //[ExportSubgroup("Platform Type Spawn Chances")]
	/*[Export(PropertyHint.Range, "1, 100")]*/ private uint flatPlatformTypeSpawnChance = 1;
	/*[Export(PropertyHint.Range, "1, 100")]*/ private uint inclinePlatformTypeSpawnChance = 1;
/*	[Export(PropertyHint.Range, "1, 100")]*/ private uint bridgePlatformTypeSpawnChance = 1;

	//[ExportSubgroup("Bouncer Spawn Rate")]
	/*[Export(PropertyHint.Range, "5, 20")]*/ private uint bounceSpawnRate = 5;
	private uint coinSpawnRate = 0;
	private uint spikeSpawnRate = 0;

    //[ExportGroup("Number of Sections")]
	/*[Export(PropertyHint.Range, "1, 10")]*/ private uint numberOfSections = 1;

	


	//Value which combines the three platform spawn chances together
	private uint combinedPlatformSpawnChance;

	//Value which combines three jump gap spawn chances together
	private uint combinedJumpGapSpawnChance;

	private uint combinedPlatformTypeChance;

	private uint easySpikeChance = 1;
	private uint hardSpikeChance = 1;

	//Size of section, which in this case is the size of the level for now.
	//In the future we will make this an array of section sizes when we develop multiple level sections
	//[ExportGroup("Sections")]
/*	[Export]*/ int sectionSize { get; set; }

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

    //we will translate the current position by 2 to prevent the extra large platform being
    //directly above the platform.
    bool smallJumpGapBeforeExLarge = false;

	CoinPatterns coinPatterns;
	SpikePatterns spikePatterns;


	//10000
	//If time is excellent
		//Make time 10000
	//If time is okay
		//Time = 10000 * ( (currentTimeLeft / (maxTime / 2))
	//If time sucks
		//Time bonus = 0
	int maxTimeBonus = 10000;


	//Coin bonus formula
	/// <summary>
	//Coin bonus = maxCoinBonus * (currentCoinCount / maxCoinCount)
	/// </summary>
	int maxCoinBonus = 10000;


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

		//GD.Print("Number: " + smallJumpGapSpawnChance);

		combinedPlatformSpawnChance = smallPlatformSpawnChance + mediumPlatformSpawnChance + largePlatformSpawnChance + extraLargePlatformSpawnChance;

		combinedJumpGapSpawnChance = smallJumpGapSpawnChance + mediumJumpGapSpawnChance + largeJumpGapSpawnChance;

		gameTimer.Start();

		coinPatterns = GetNode<CoinPatterns>("../CoinPatterns");
		spikePatterns = GetNode<SpikePatterns>("../SpikePatterns");

		//gameTimer.Timeout += OnTimeOut;
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
							actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.BOUNCE });

						}

                        else
                        {
                            actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP });
                        }
                    }

					else
					{
						actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP });
					}
						//actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP });
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
            //levelSpawned = true;


            spawnSection = false;
			spawnHealthPack = false;
			sectionsSpawned++;
            currentSection++;
            jsonWriter.outputGeneratedLevelToJson();

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

                    else if (componentsToAdd[i + 1].action == ActionStates.BOUNCE)
                    {
                        spawnBouncer();
						bounceSpawned = true;
                    }

					if (componentsToAdd[i + 1].action != ActionStates.BOUNCE)
					{
                        bounceSpawned = false;
                    }

                    AddCurrentPosition(newDirection);
				}
			}

			else if (componentsToAdd[i].action == ActionStates.JUMP)
			{
				AddJumpSpace(componentsToAdd[i].lengthOfComponent, newDirection, jumpGaps, jumpHeight);
			}

			//else if (componentsToAdd[i].action == ActionStates.BOUNCE)
			//{
			//	spawnBouncer();
			//}
 
		}
        AddCurrentPosition(newDirection);

		if (sectionsSpawned == numberOfSections - 1)
		{
			GenerateGoal();
		}
		else
		{
            GenerateCheckpoint();
        }
			//GenerateCheckpoint();
		//compSpawner.generateCoins();
		compSpawner.generateSpikes();
       // AddCurrentPosition(direction);

    }


	private void SpawnPlatform(Lengths platformLength, NewDirection currentDirection, List<float> platSizes, ActionStates nextAction)
	{
		PackedScene platform;
		PlatformTypes platformType;

		bool bounceMode;
        if (nextAction == ActionStates.BOUNCE) { 
			platformType = PlatformTypes.FLAT;
			bounceMode = true;
		}
		else
		{
            platformType = typeToChoose();
			bounceMode = false;
        }

		platformLength = determinePlatformLength(platformType);
		string platformTypeToSpawn = platformPath(platformLength, platformType, nextAction);
		//string platformTypeToSpawn = typeToChoose(platformLength, nextAction);
        platform = ResourceLoader.Load<PackedScene>(platformTypeToSpawn);

        Node3D newPlatform = platform.Instantiate<Node3D>();

		newPlatform.RotationDegrees = new Vector3(0, determineAngle(currentDirection), 0);

        //Translate it by currentPosition.
        newPlatform.Position = currentPosition;
		//GD.Print(newPlatform.Position);
		AddChild(newPlatform);
		jsonWriter.addPlatform(currentPosition, newPlatform.SceneFilePath, currentSection);
		jsonWriter.addLevelPartToJson(currentPosition, newPlatform.SceneFilePath, currentSection);
		

		if (platformType == PlatformTypes.FLAT && bounceMode == false)
		{
			uint rng = 1 + GD.Randi() % 200;
			if (rng <= spikeSpawnRate)
			{
				if (currentPosition != new Vector3(0, 0, 0))
				{
					uint rng2 = 1 + GD.Randi() % +(easySpikeChance + hardSpikeChance);
					if (rng <= hardSpikeChance)
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
                coinPatterns.spawnCoins(platformType, platformLength, currentPosition, currentDirection);
            }
            //}
		}

		else if (bounceMode == false)
		{
            uint rng = 1 + GD.Randi() % 100;
			if (rng <= coinSpawnRate)
			{
				coinPatterns.spawnCoins(platformType, platformLength, currentPosition, currentDirection);
			}
        }

        if (spawnHealthPack == true)
        {
            PackedScene healthPackScene = ResourceLoader.Load<PackedScene>("res://healthOrb.tscn");

            Node3D healthOrb = healthPackScene.Instantiate<Node3D>();
            healthOrb.Position = new Vector3(currentPosition.X, currentPosition.Y + 2, currentPosition.Z);
			if (bounceMode == true)
			{
				healthOrb.Position += new Vector3(healthOrb.Position.X, healthOrb.Position.Y + 3, healthOrb.Position.Z);
			}
            AddChild(healthOrb);
            spawnHealthPack = false;
            spikeAreaCount = 0;
        }
        
		if (spikeAreaCount == 3)
		{
			//PackedScene healthPackScene = ResourceLoader.Load<PackedScene>("");
			//A health pack will be spawned at the next platform
			spawnHealthPack = true;
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
            // Lengths lenOfPlatform;

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
	private string platformPath(Lengths lenOfPlatform, PlatformTypes type, ActionStates nextAction)
	{
        //uint combinedChanceValues = smallPlatformSpawnChance + mediumPlatformSpawnChance + largePlatformSpawnChance + extraLargeJumpGapSpawnChance;
      //  Lengths lenOfPlatform;
        if (type == PlatformTypes.FLAT)
		{

            if (lenOfPlatform == Lengths.SHORT)
            {
                numberToAdd = 6.0f;
                return "res://small_platform.tscn";
            }

            else if (lenOfPlatform == Lengths.MEDIUM)
            {
                numberToAdd = 9.0f;
                return "res://medium_platform.tscn";
            }

            else if (lenOfPlatform == Lengths.LONGEST)
            {
				if (currentDirection == NewDirection.FORWARD)
				{
					currentPosition.X += 3;

					if (bounceSpawned == true || smallJumpGapBeforeExLarge == true) 
					{
						currentPosition.X += 2;
					}
				}

				else if (currentDirection == NewDirection.LEFT)
				{
					currentPosition.Z -= 3;

                    if (bounceSpawned == true || smallJumpGapBeforeExLarge == true)
                    {
                        currentPosition.Z -= 2;
                    }
                }

				else
				{
					currentPosition.Z += 3;

                    if (bounceSpawned == true || smallJumpGapBeforeExLarge == true)
                    {
                        currentPosition.Z += 2;
                    }
                }
				
				numberToAdd = 21.0f;
                return "res://extraLargePlatform.tscn";

            }

            else
            {
                numberToAdd = 12.0f;
                return "res://large_platform.tscn";
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

                    return "res://smallBridgePlatform.tscn";
                }

                else if (lenOfPlatform == Lengths.MEDIUM)
                {
                    numberToAdd = 13.0f;
                    return "res://mediumBridgePlatform.tscn";
                }

                else if (lenOfPlatform == Lengths.LONG)
                {
                    numberToAdd = 17.0f;
                    return "res://largeBridgePlatform.tscn";
                }
            }

            else if (type == PlatformTypes.INCLINE)
            {
                if (lenOfPlatform == Lengths.SHORT)
                {
                    numberToAdd = 12.0f;
                    if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                    {
                        numberToAdd = 8.5f;
                    }
                    translationVector.Y = 2;
                    return "res://inclinePlatformSmall.tscn";
                }

                else if (lenOfPlatform == Lengths.MEDIUM)
                {
                    numberToAdd = 15.0f;

                    if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                    {
                        numberToAdd = 10.5f;
                    }
                    translationVector.Y = 3;
                    return "res://inclinePlatformMedium.tscn";
                }

                else if (lenOfPlatform == Lengths.LONG)
                {
                    numberToAdd = 21.0f;

                    if (nextAction == ActionStates.TURN_LEFT || nextAction == ActionStates.TURN_RIGHT)
                    {
                        numberToAdd = 12.5f;
                    }

                    translationVector.Y = 5;
                    return "res://inclinePlatformLarge.tscn";
                }
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
				smallJumpGapBeforeExLarge = true;
                numberToAdd = jumpSizes[0];
				jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "Small Jump Gap", currentSection);
				break;
			case Lengths.MEDIUM:
                smallJumpGapBeforeExLarge = false;
                numberToAdd = jumpSizes[1];
				jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "Medium Jump Gap", currentSection);
				break;
			case Lengths.LONG:
                smallJumpGapBeforeExLarge = false;
                yPosition /= 2;
				numberToAdd = jumpSizes[2];
                jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "Long Jump Gap", currentSection);
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
		jsonWriter.addPlatform(currentPosition, newPlatform.SceneFilePath, currentSection);
		jsonWriter.addLevelPartToJson(currentPosition, newPlatform.SceneFilePath, currentSection);

		PackedScene checkpointLoad;


		checkpointLoad = ResourceLoader.Load<PackedScene>("res://Level parts/checkpoint.tscn");
		Node3D checkpoint = checkpointLoad.Instantiate<Node3D>();
		checkpoint.Position = new Vector3(currentPosition.X, currentPosition.Y + 3.0f, currentPosition.Z);
        AddChild(checkpoint);

		numberToAdd = 12.0f;
		translationVector.Y = 0;
		AddCurrentPosition(currentDirection);
        //checkpoint.Position.Y = currentPosition.Y + 1;

    }

	private void GenerateGoal()
	{

        PackedScene platform;

        platform = ResourceLoader.Load<PackedScene>("res://large_platform.tscn");

        Node3D newPlatform = platform.Instantiate<Node3D>();
		//Translate it by currentPosition.
		newPlatform.Position = currentPosition;
		AddChild(newPlatform);
        jsonWriter.addPlatform(currentPosition, newPlatform.SceneFilePath, currentSection);
        jsonWriter.addLevelPartToJson(new Vector3(0, 100, 0), newPlatform.SceneFilePath, currentSection);

        PackedScene goalLoad;

		goalLoad = ResourceLoader.Load<PackedScene>("res://Level parts/goal.tscn");
		Node3D goal = goalLoad.Instantiate<Node3D>();
		goal.Position = new Vector3(currentPosition.X, currentPosition.Y + 3.0f, currentPosition.Z);

        if (currentDirection == NewDirection.FORWARD)
        {
		//	goal.RotateY(Mathf.Pi / 2);

        }

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
				jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "[Direction changed to right]", currentSection);
			}

			else
			{
				currentDirection = NewDirection.LEFT;
				jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "[Direction changed to left]", currentSection);
            }
		}

		else if (currentDirection == NewDirection.RIGHT || currentDirection == NewDirection.LEFT)
		{
			currentDirection = NewDirection.FORWARD;
            jsonWriter.addLevelPartToJson(new Vector3(0, -100, 0), "[Direction changed to forward]", currentSection);
        }

		return currentDirection;
	}
	//private void spawnCoins(NewDirection currentDirection, int platformLength, int platformWidth, string platformType, Vector3 platformPosition)
	//{
	//	PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://Level parts/coin.tscn");
	//	Node3D coin = coinScene.Instantiate<Node3D>();
	//	coin.Position = platformPosition;
	//	Vector3 coinPosition = coin.Position;
	//	coinPosition.Y += 1;
	//	coin.Position = coinPosition;
	//	GetTree().Root.AddChild(coin);


 //   }

	private void spawnBouncer()
	{
		PackedScene bouncerScene = ResourceLoader.Load<PackedScene>("res://Level parts/bouncer.tscn");

		Node3D bouncer = bouncerScene.Instantiate<Node3D>();
		bouncer.Position = new Vector3(currentPosition.X, currentPosition.Y + 2, currentPosition.Z);
		AddChild(bouncer);
		currentPosition += new Vector3(0, 20, 0);
	}

	private bool bouncerDecider()
	{
		uint rng = 1 + GD.Randi() % 100;

		if (rng <= bounceSpawnRate)
		{
			return true;
		}

		return false;
	}

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
