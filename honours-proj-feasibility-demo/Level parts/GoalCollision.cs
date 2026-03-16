using Godot;
using System;

public partial class GoalCollision : Area3D
{
    bool goalReached = false;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area3D area)
    {
        if (goalReached == true)
        {
            return;
        }

        if (area.IsInGroup("player"))
        {
            var playerArea = GetTree().GetFirstNodeInGroup("player");
            if (playerArea != null)
            {
                PlayerCharacter player = playerArea.GetParent() as PlayerCharacter;
                if (player != null)
                {

                }


                TestLevel level = GetTree().Root.GetNode<TestLevel>("Game/TestLevel") as TestLevel;
                if (level != null)
                {
                    // GD.Print("Level complete!");

                    //temp ofc, comment out after
                    //GameManager.restartLevel();
                    //GetTree().ReloadCurrentScene();
                    GameManager.displayEndScreen();
                }
            }
        }
    }
}
