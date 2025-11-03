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
        List<ActionStates> actionsToAdd = new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP, ActionStates.TURN_LEFT, ActionStates.WALK, ActionStates.JUMP, ActionStates.WALK, ActionStates.JUMP };

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
    }

    public override void _Process(double delta)
    {
        //Do logic in here if _Ready doesn't work (very likely btw)
    }

    private void GenerateLevel(List<LevelComponent> componentsToAdd)
    {
        //Default direction is +ve z
        Direction direction = Direction.POSITIVE_Z;

        //How long the jump is
        float longGap = 3.0f;
        float mediumGap = 2.0f;
        float shortGap = 1.0f;


        //Float since the platform size could not be a whole number
        float largePlatformSize = 9.0f;
        float mediumPlatformSize = 6.0f;
        float smallPlatformSize = 3.0f;


        //Current position so that we can translate the platform when spawning it
        Vector3 currentPosition = new Vector3(0, 0, 0);

      for (int i = 0; i < componentsToAdd.Count; ++i)
        {
            if (componentsToAdd[i].action == ActionStates.WALK)
            {
                //TODO: Make this into a function?
                switch (componentsToAdd[i].lengthOfComponent)
                {
                    case Lengths.SHORT:
                        break;
                    case Lengths.MEDIUM:
                        break;
                    case Lengths.LONG:
                        break;

                }

                
            }

            else if (componentsToAdd[i].action == ActionStates.JUMP)
            {
                //TODO: Make this into a function
                switch (componentsToAdd[i].lengthOfComponent)
                {
                    case Lengths.SHORT:
                        break;
                    case Lengths.MEDIUM:
                        break;
                    case Lengths.LONG:
                        break;

                }
            }

            else if (componentsToAdd[i].action == ActionStates.TURN_LEFT || componentsToAdd[i].action == ActionStates.TURN_RIGHT)
            {
                direction = ChangeDirection(direction, componentsToAdd[i].action);
            }
 
        }

    }

    private Vector3 UpdateCurrentPosition() 
    {
        return new Vector3();
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

