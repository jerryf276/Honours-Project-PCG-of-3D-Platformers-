using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Godot.TextServer;

enum ActionStates {WALK, TURN_LEFT, TURN_RIGHT, JUMP};

//NONE is used for changing direction since a length doesn't exist for a direction change.
enum Lengths {SHORT, MEDIUM, LONG, NONE};

//which direction the platform will be facing
enum Direction {POSITIVE_X, NEGATIVE_X, POSITIVE_Z, NEGATIVE_Z};


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

	//Chance of direction changing, we have made 50 the max 
	[Export(PropertyHint.Range, "0, 50")] uint directionChangeChance;

	//Size of section, which in this case is the size of the level for now.
	//In the future we will make this an array of section sizes when we develop multiple level sections
	[Export] int sectionSize = 20;

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
	  
	}

	public override void _Process(double delta)
	{
		//Ideally, each spawn we could change the actions.

		if (!levelSpawned)
		{
			List<ActionStates> actionsToAdd = new List<ActionStates> { };

            for (int i = 0; i < sectionSize; ++i)
			{
				
				uint directionChange = 1 + GD.Randi() % 100;
				//If random number is less than direction change percentage
				if (directionChange < directionChangeChance) 
					{
					//0 - change direction to left, 1 - change direction to right
						uint direction = GD.Randi() % 2;
						if (direction == 0)
						{
							//actionsToAdd.AddRange(ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.JUMP);
							actionsToAdd.AddRange(new List<ActionStates> {ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.JUMP});
						}
						else
						{
							actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_RIGHT, ActionStates.JUMP });
						}
					}
				else
				{
                    actionsToAdd.AddRange(new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP });
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
				if (actionsToAdd[i] == ActionStates.WALK || actionsToAdd[i] == ActionStates.JUMP)
				{
					//Probability of each length is 1/3
					uint rng = 1 + GD.Randi() % 3;

					switch (rng)
					{
						case 1:
							//Adds short platform or makes the jump length small
							levelComponents.Add(new LevelComponent(actionsToAdd[i], Lengths.SHORT));
							break;
						case 2:
							//Adds medium platform or makes the jump length medium
							levelComponents.Add(new LevelComponent(actionsToAdd[i], Lengths.MEDIUM));
							break;
						case 3:
							//Adds long platform or makes the jump length long
							levelComponents.Add(new LevelComponent(actionsToAdd[i], Lengths.LONG));
							break;
					}

				}
				else
				{
					//Adds turning direction to the section of the level
					levelComponents.Add(new LevelComponent(actionsToAdd[i], Lengths.NONE));
				}

			}
			//Generates level once each component of the level has been defined
			GenerateLevel(levelComponents);
			levelSpawned = true;
		}
	}

	private void GenerateLevel(List<LevelComponent> componentsToAdd)
	{
		//Default direction is +ve x
		Direction direction = Direction.POSITIVE_X;

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

	  for (int i = 0; i < componentsToAdd.Count; ++i)
		{
			if (componentsToAdd[i].action == ActionStates.WALK)
			{
				//Spawns a platform
				SpawnPlatform(componentsToAdd[i].lengthOfComponent, direction, platformSizes);

				//To prevent array errors when checking if the next element is turning left or right
				if (componentsToAdd.Count > (i + 2)) {
					if (componentsToAdd[i + 1].action == ActionStates.TURN_LEFT || componentsToAdd[i + 1].action == ActionStates.TURN_RIGHT)
					{
						//GD.Print("Direction changed!");
						direction = ChangeDirection(direction, componentsToAdd[i + 1].action);
					}
					AddCurrentPosition(direction);
				}
			}

			else if (componentsToAdd[i].action == ActionStates.JUMP)
			{
				AddJumpSpace(componentsToAdd[i].lengthOfComponent, direction, jumpGaps, jumpHeight);
			}

			//else if (componentsToAdd[i].action == ActionStates.TURN_LEFT || componentsToAdd[i].action == ActionStates.TURN_RIGHT)
			//{
			//    //Changing the direction
			//    direction = ChangeDirection(direction, componentsToAdd[i].action);
			//}
 
		}

	}

	void SpawnPlatform(Lengths platformLength, Direction currentDirection, List<float> platSizes)
	{
		var platform = ResourceLoader.Load<PackedScene>("");

		switch (platformLength)
		{
			case Lengths.SHORT:
				//Loads short platform
				platform = ResourceLoader.Load<PackedScene>("res://small_platform.tscn");
				numberToAdd = platSizes[0];
				break;
			case Lengths.MEDIUM:
				//Loads medium platform
				platform = ResourceLoader.Load<PackedScene>("res://medium_platform2.tscn");
				numberToAdd = platSizes[1];
				break;
			case Lengths.LONG:
				//Loads large platform
				platform = ResourceLoader.Load<PackedScene>("res://large_platform.tscn");
				numberToAdd = platSizes[2];
				break;
		}

		Node3D newPlatform = platform.Instantiate<Node3D>();
		//Translate it by currentPosition.
		newPlatform.Position = currentPosition;
		GD.Print(newPlatform.Position);
		GetTree().Root.AddChild(newPlatform);
	}

	void AddCurrentPosition(Direction currentDirection)
	{
		switch (currentDirection)
		{
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

	void AddJumpSpace(Lengths jumpLength, Direction currentDirection, List<float> jumpSizes, List<float> jumpHeight)
	{
		//TO DO: See if you could maybe combine this and the spawn platform functions? (to avoid repetition)
		float numberToAdd = 0;

		//for now we'll do a probability of 1/3 for deciding if the player ascends up, down, or does not.
        uint rng = 1 + GD.Randi() % 3;

		float yPosition = 0.0f;

		switch (rng)
		{
			case 1:
				yPosition = 2.0f;
				break;
			case 2:
				yPosition = -2.0f;
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
				numberToAdd = jumpSizes[2];
				break;
		}

		switch (currentDirection) 
		{
			case Direction.POSITIVE_X:
				currentPosition += new Vector3(numberToAdd, yPosition, 0);
				break;
			case Direction.NEGATIVE_X:
				currentPosition += new Vector3(-numberToAdd, yPosition, 0);
				break;
			case Direction.POSITIVE_Z:
				currentPosition += new Vector3(0, yPosition, numberToAdd);
				break;
			case Direction.NEGATIVE_Z:
				currentPosition += new Vector3(0, yPosition, -numberToAdd);
				break;
		}
	}

	private Direction ChangeDirection(Direction currentDirection, ActionStates currentActionState)
	{
		switch (currentDirection)
		{
			case Direction.POSITIVE_X:
				if (currentActionState == ActionStates.TURN_LEFT)
				{
					//Turning left flips sign and changes axis
					currentDirection = Direction.NEGATIVE_Z;
				}

				else
				{
					//Turning right only changes axis
					currentDirection = Direction.POSITIVE_Z;
				}
				break;
			case Direction.NEGATIVE_X:
				if (currentActionState == ActionStates.TURN_LEFT)
				{
					//Changes axis and flips sign
					currentDirection = Direction.POSITIVE_Z;
				}

				else
				{
					//Changes axis
					currentDirection = Direction.NEGATIVE_Z;
				}
				break;
			case Direction.POSITIVE_Z:
				if (currentActionState == ActionStates.TURN_LEFT)
				{
					//Changes axis and flips sign
					currentDirection = Direction.POSITIVE_X;
				}

				else
				{
					//Changes axis
					currentDirection = Direction.NEGATIVE_X;
				}
				break;
			case Direction.NEGATIVE_Z:
				if (currentActionState == ActionStates.TURN_LEFT)
				{
					//Changes axis and flips sign
					currentDirection = Direction.NEGATIVE_X;
				}

				else
				{
					//Changes axis
					currentDirection = Direction.POSITIVE_X;
				}
				break;
		}

		return currentDirection;
	}
}
