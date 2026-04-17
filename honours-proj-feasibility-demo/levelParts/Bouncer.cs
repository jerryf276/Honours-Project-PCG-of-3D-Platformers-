using Godot;
using System;

public partial class Bouncer : Area3D
{
        [Export] private AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            AreaEntered += OnAreaEntered;
        }

        public override void _Process(double delta)
        {
        
        }

        private void OnAreaEntered(Area3D area)
        {
            if (area.IsInGroup("player"))
            {
                var playerArea = GetTree().GetFirstNodeInGroup("player");
                if (playerArea != null)
                {
                    PlayerCharacter player = playerArea.GetParent() as PlayerCharacter;
                    if (player != null)
                    {
                        player.bouncedOnSpring();
                        animationPlayer.Play("Bouncer_Bounce");
                    }
                }
            }
        }

}
