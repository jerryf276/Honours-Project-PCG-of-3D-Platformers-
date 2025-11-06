using Godot;
using System;

public partial class Player : RigidBody3D
{

    float mouseSensitivity = 0.001f;
    float twistInput = 0.0f;
    float pitchInput = 0.0f;
    float playerSpeed = 1200.0f;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        Vector3 input = Vector3.Zero;
        input.X = Input.GetAxis("move_left", "move_right");
        input.Z = Input.GetAxis("move_forward", "move_backward");

        ApplyCentralForce(input * playerSpeed * (float)delta);

        if (Input.IsActionJustPressed("cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                twistInput = -eventMouseMotion.Relative.X * mouseSensitivity;
                pitchInput = -eventMouseMotion.Relative.Y * mouseSensitivity;
                //38:01 in referenced YouTube video, continue from here!
            }
        }
    }
}
