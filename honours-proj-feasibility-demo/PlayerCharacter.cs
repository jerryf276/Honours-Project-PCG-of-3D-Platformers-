using Godot;
using System;

public partial class PlayerCharacter : CharacterBody3D
{
    [ExportGroup("Camera")]
    [Export(PropertyHint.Range, "0.0, 1.0")] private float mouseSensitivity = 0.25f;

    [ExportGroup("Movement")]
    [Export] private float moveSpeed = 8.0f;
    [Export] private float acceleration = 20.0f;
    [Export] private float rotationSpeed = 12.0f;

    private Vector2 cameraInputDirection = Vector2.Zero;
    private Vector3 lastMovementDirection = Vector3.Back;
    private Node3D cameraPivot;
    private Node3D camera;
    private Node3D gobotSkin;

    public override void _Ready()
    {
        cameraPivot = GetNode<Node3D>("CameraPivot");
        camera = GetNode<Camera3D>("CameraPivot/SpringArm3D/Camera3D");
        gobotSkin = GetNode<Node3D>("GobotSkin");
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("left_click"))
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }

        if (inputEvent.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }
    public override void _UnhandledInput(InputEvent inputEvent) 
    {
        bool isCameraMotion = false; 
        if (inputEvent is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            isCameraMotion = true;
        }

        if (isCameraMotion)
        {
            InputEventMouseMotion inputEventMouseMotion = (InputEventMouseMotion)inputEvent;

            cameraInputDirection = inputEventMouseMotion.ScreenRelative * mouseSensitivity;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 rotation = cameraPivot.Rotation;
        rotation.X += (float)(cameraInputDirection.Y * delta);
        rotation.X = Mathf.Clamp(rotation.X, -Mathf.Pi / 6.0f, Mathf.Pi / 3.0f);
        rotation.Y -= (float)(cameraInputDirection.X * delta);
        cameraPivot.Rotation = rotation;
        cameraInputDirection = Vector2.Zero;
        // cameraPivot.Rotation = rotation;

        Vector2 rawInput = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 forward = camera.GlobalBasis.Z;
        Vector3 right = camera.GlobalBasis.X;

        Vector3 moveDirection = forward * rawInput.Y + right * rawInput.X;
        moveDirection.Y = 0.0f;
        moveDirection = moveDirection.Normalized();

        Velocity = Velocity.MoveToward(moveDirection * moveSpeed, acceleration * (float)delta);
        MoveAndSlide();

        if (moveDirection.Length() > 0.2f)
        {
            lastMovementDirection = moveDirection;
        }

        float targetAngle = Vector3.Back.SignedAngleTo(lastMovementDirection, Vector3.Up);
        Vector3 gobotGlobalRotation = gobotSkin.GlobalRotation;
        gobotGlobalRotation.Y = Mathf.LerpAngle(gobotSkin.Rotation.Y, targetAngle, rotationSpeed * (float)delta);
        gobotSkin.GlobalRotation = gobotGlobalRotation;

        float groundSpeed = Velocity.Length();

        if (groundSpeed > 0.0f)
        {
            gobotSkin.Call("run");
        }
        else
        {
            gobotSkin.Call("idle");
        }
    }
}

