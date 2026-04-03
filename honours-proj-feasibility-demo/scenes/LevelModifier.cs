using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class LevelModifier : Control
{
    [Export] Slider smallPlatformSlider;
    [Export] Slider mediumPlatformSlider;
    [Export] Slider largePlatformSlider;
    [Export] Slider extraLargePlatformSlider;

    [Export] Slider smallJumpGapSlider;
    [Export] Slider mediumJumpGapSlider;
    [Export] Slider largeJumpGapSlider;

    [Export] Slider flatPlatformTypeSlider;
    [Export] Slider inclinePlatformTypeSlider;
    [Export] Slider bridgePlaformTypeSlider;

    [Export] Slider coinSlider;
    [Export] Slider spikeSlider;
    [Export] Slider bouncePadSlider;

    [Export] OptionButton numberOfSections;
    [Export] TextEdit sizePerSection;

    [Export] Button leftButton;
    [Export] Button rightButton;
    [Export] Button startButton;

    [Export] Node2D pageOne;
    [Export] Node2D pageTwo;

    TestLevel level;


    //struct LevelAttributes
    //{

    //}

    


    public override void _Ready()
    {
        leftButton.Pressed += OnLeftButtonPressed;
        rightButton.Pressed += OnRightButtonPressed;
        startButton.Pressed += OnStartButtonPressed;

        pageTwo.Visible = false;
        pageOne.Visible = true;

        GetTree().Paused = true;
    }

    public override void _Process(double delta)
    {
           
    }


    private void OnLeftButtonPressed()
    {
        if (!pageOne.Visible)
        {
            pageTwo.Visible = false;
            pageOne.Visible = true;
        }
    }

    private void OnRightButtonPressed()
    {
        if (!pageTwo.Visible)
        {
            pageOne.Visible = false;
            pageTwo.Visible = true;
        }

    }

    private void OnStartButtonPressed()
    {
        ////GameManager.startLevel();
        //PackedScene gameManagerScene;
        //gameManagerScene = ResourceLoader.Load<PackedScene>("res://scenes/testGame.tscn");
        //GetTree().Paused = false;
        //GetTree().ChangeSceneToPacked(gameManagerScene);
        GameManager.StartLevel();
        //Node3D level;
        PackedScene LevelScene;

        LevelScene = ResourceLoader.Load<PackedScene>("res://TestLevel.tscn");
        level = LevelScene.Instantiate<TestLevel>();

        level.setPlatformSpawnChances((uint)smallPlatformSlider.Value, (uint)mediumPlatformSlider.Value, (uint)largePlatformSlider.Value, (uint)extraLargePlatformSlider.Value);
        level.setJumpGapChances((uint)smallJumpGapSlider.Value, (uint)mediumJumpGapSlider.Value, (uint)largeJumpGapSlider.Value);
        level.setPlatformTypeSpawnChances((uint)flatPlatformTypeSlider.Value, (uint)inclinePlatformTypeSlider.Value, (uint)bridgePlaformTypeSlider.Value);

        if (uint.TryParse(sizePerSection.Text, out uint result))
        {
            if (result > 1 && result < 256)
            {
                //level.settingSections((uint)numberOfSections.GetItemId(numberOfSections.Selected), result);
                level.settingSections(result, (uint)numberOfSections.GetItemId(numberOfSections.Selected));
            }

            else
            {
                level.settingSections((uint)25, (uint)numberOfSections.GetItemId(numberOfSections.Selected));
            }
        }

        else
        {
            level.settingSections((uint)25, (uint)numberOfSections.GetItemId(numberOfSections.Selected));
        }
        level.setComponentSpawnRate((uint)bouncePadSlider.Value);
        level.setSpawnSection(true);
            //level.settingSections((uint)numberOfSections.Selected, 25);
        // AddChild(level);
        GetTree().Paused = false;
        GameManager.AddLevel(level);
        this.QueueFree();
    }
}
