using Godot;
using System;

public partial class Player : RigidBody3D
{
	private float mouseSensitivity = 0.001f;
	private float twistInput = 0.0f;
	private float pitchInput = 0.0f;
	private float playerSpeed = 1200.0f;
	//Change nodes to private and see if it still works?
	public Node3D twistPivot;
	public Node3D pitchPivot;
    Vector3 input;

    public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		twistPivot = GetNode<Node3D>("TwistPivot");
		pitchPivot = GetNode<Node3D>("TwistPivot/PitchPivot");
    //    fallGravity = 0;
        input = Vector3.Zero;
    }

	public override void _Process(double delta)
	{
		input.X = 0;
		input.Z = 0;
		//input = Vector3.Zero;
		input.X = Input.GetAxis("move_left", "move_right");
		input.Z = Input.GetAxis("move_forward", "move_backward");

		ApplyCentralForce(twistPivot.Basis * input * playerSpeed * (float)delta);

		if (Input.IsActionJustPressed("cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		twistPivot.RotateY(twistInput);
		pitchPivot.RotateX(pitchInput);
		var rotation = pitchPivot.Rotation;
		rotation.X = (float)Mathf.Clamp(pitchPivot.Rotation.X, Mathf.DegToRad(-30), Mathf.DegToRad(30));
		pitchPivot.Rotation = rotation;

		twistInput = 0.0f;
		pitchInput = 0.0f;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				twistInput = -eventMouseMotion.Relative.X * mouseSensitivity;
				pitchInput = -eventMouseMotion.Relative.Y * mouseSensitivity;
			}
		}
	}
}
