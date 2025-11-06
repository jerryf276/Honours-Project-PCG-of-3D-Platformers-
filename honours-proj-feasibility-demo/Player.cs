using Godot;
using System;

public partial class Player : RigidBody3D
{

	float mouseSensitivity = 0.001f;
	float twistInput = 0.0f;
	float pitchInput = 0.0f;
	float playerSpeed = 1200.0f;
	Node3D twistPivot;
	Node3D pitchPivot;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		twistPivot = GetNode<Node3D>("TwistPivot");
		pitchPivot = GetNode<Node3D>("TwistPivot/PitchPivot");
	}

	public override void _Process(double delta)
	{
		Vector3 input = Vector3.Zero;
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
		//41:14 in reference youtube video
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
