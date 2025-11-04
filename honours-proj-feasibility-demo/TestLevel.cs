using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Godot.TextServer;

enum ActionStates {WALK, TURN_LEFT, TURN_RIGHT, JUMP};
enum Lengths {SHORT, MEDIUM, LONG, NONE};

enum Direction {POSITIVE_X, NEGATIVE_X, POSITIVE_Z, NEGATIVE_Z};



//short - small platform, medium - medium platform, long - large platform
public partial class TestLevel : Node3D
{
    //Current position so that we can translate the platform when spawning it
    Vector3 currentPosition;
    bool levelSpawned = false;
    private struct LevelComponent
    {
        public ActionStates action;
        //Will make it none by default as turning left or turning right would have no length
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
            //Do logic in here if _Ready doesn't work (very likely btw)
            List<ActionStates> actionsToAdd = new List<ActionStates> { ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.JUMP, ActionStates.WALK, ActionStates.JUMP, ActionStates.WALK, ActionStates.TURN_RIGHT, ActionStates.JUMP, ActionStates.WALK};

            //int - index of jump or walk in actionsToAdd above, lengths - will be either short, medium or long
            SortedDictionary<int, Lengths> actionSizes = new SortedDictionary<int, Lengths> { };

            //Do we change the name of this variable to levelPart instead?
            //A part of a level would have many components
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
            GenerateLevel(levelComponents);
            levelSpawned = true;
        }
    }

    private void GenerateLevel(List<LevelComponent> componentsToAdd)
    {
        //Default direction is +ve x
        Direction direction = Direction.POSITIVE_X;

        //How long the jump is
        //Index 0 - short, Index 1 - medium, index 2 - long

        //List<float> jumpGaps = new List<float> { 3.0f, 6.0f, 9.0f };
        List<float> jumpGaps = new List<float> { 3.0f, 6.0f, 9.0f };

        //Float since the platform size could not be a whole number
        //Index 0 - short, Index 1 - medium, index 2 - long
        List<float> platformSizes = new List<float> { 6.0f, 9.0f, 12.0f };

      for (int i = 0; i < componentsToAdd.Count; ++i)
        {
            if (componentsToAdd[i].action == ActionStates.WALK)
            {
                //Spawns a platform
                SpawnPlatform(componentsToAdd[i].lengthOfComponent, direction, platformSizes);
            }

            else if (componentsToAdd[i].action == ActionStates.JUMP)
            {
                AddJumpSpace(componentsToAdd[i].lengthOfComponent, direction, jumpGaps);
            }

            else if (componentsToAdd[i].action == ActionStates.TURN_LEFT || componentsToAdd[i].action == ActionStates.TURN_RIGHT)
            {
                //Changing the direction
                direction = ChangeDirection(direction, componentsToAdd[i].action);
            }
 
        }

    }

    void SpawnPlatform(Lengths platformLength, Direction currentDirection, List<float> platSizes)
    {
        float numberToAdd = 0;
        var platform = ResourceLoader.Load<PackedScene>("");

        switch (platformLength)
        {
            case Lengths.SHORT:
                //Do packed scene thing here
                platform = ResourceLoader.Load<PackedScene>("res://small_platform.tscn");
                numberToAdd = platSizes[0];
                break;
            case Lengths.MEDIUM:
                //Do packed scene thing here
                platform = ResourceLoader.Load<PackedScene>("res://medium_platform.tscn");
                numberToAdd = platSizes[1];
                break;
            case Lengths.LONG:
                //Do packed scene thing here
                platform = ResourceLoader.Load<PackedScene>("res://large_platform.tscn");
                numberToAdd = platSizes[2];
                break;
        }

        Node3D newPlatform = platform.Instantiate<Node3D>();
        //Translate it by currentPosition.
        newPlatform.Position = currentPosition;
        GD.Print(newPlatform.Position);
        GetTree().Root.AddChild(newPlatform);

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

    void AddJumpSpace(Lengths jumpLength, Direction currentDirection, List<float> jumpSizes)
    {
        //TO DO: See if you could maybe combine this and the spawn platform functions? (to avoid repetition)
        float numberToAdd = 0;

        switch (jumpLength)
        {
            case Lengths.SHORT:
                numberToAdd = jumpSizes[0];
                break;
            case Lengths.MEDIUM:
                numberToAdd = jumpSizes[0];
                break;
            case Lengths.LONG:
                numberToAdd = jumpSizes[0];
                break;
        }

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
                    currentDirection = Direction.NEGATIVE_X;
                }

                else
                {
                    //Changes axis
                    currentDirection = Direction.POSITIVE_X;
                }
                break;
            case Direction.NEGATIVE_Z:
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
        }

        return currentDirection;
    }
}

