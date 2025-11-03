using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

enum ActionStates {WALK, TURN_LEFT, TURN_RIGHT, JUMP};
enum Lengths {SHORT, MEDIUM, LONG};
 
//short - small platform, medium - medium platform, long - large platform
public partial class TestLevel : Node3D
{
    List<ActionStates> levelStates;

    List<ActionStates> actionsToAdd = new List<ActionStates> { ActionStates.WALK, ActionStates.JUMP, ActionStates.TURN_LEFT, ActionStates.WALK, ActionStates.JUMP, ActionStates.WALK, ActionStates.JUMP};

    //int - index of jump or walk in actionsToAdd above, lengths - will be either short, medium or long
    SortedDictionary<int, Lengths> actionSizes;

    public override void _Ready()
    {
    //    levelStates.Add(ActionStates.WALK, ActionStates.JUMP, ActionStates.WALK, ActionStates.TURN_LEFT, ActionStates.WALK);
            for (int i = 0; i < actionsToAdd.Count; i++)
            {
                if (actionsToAdd[i] == ActionStates.WALK || actionsToAdd[i] == ActionStates.JUMP)
                {
                    //Probability of each length is 1/3
                    uint rng = 1 + GD.Randi() % 3;

                    switch (rng)
                    {
                    case 1:
                        //Adds short platform or makes the jump length small
                        actionSizes.Add(i, Lengths.SHORT);
                        break;
                    case 2:
                        //Adds medium platform or makes the jump length medium
                        actionSizes.Add(i, Lengths.MEDIUM);
                        break;
                    case 3:
                        //Adds long platform or makes the jump length long
                        actionSizes.Add(i, Lengths.LONG);
                        break;
                    }

                   //actionSizes.Add(i, )
                }
              
            }
    }

    private void addPlatforms()
    {

    }

    public override void _Process(double delta)
    {
        
    }
}
