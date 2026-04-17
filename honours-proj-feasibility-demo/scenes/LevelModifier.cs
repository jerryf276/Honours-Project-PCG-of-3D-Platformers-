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
    [Export] Slider bridgePlatformTypeSlider;

    [Export] Slider coinSlider;
    [Export] Slider spikeSlider;
    [Export] Slider bouncePadSlider;

    [Export] Slider easySpikeSlider;
    [Export] Slider hardSpikeSlider;

    [Export] OptionButton numberOfSections;
    [Export] TextEdit sizePerSection;

    [Export] Button leftButton;
    [Export] Button rightButton;
    [Export] Button startButton;
    [Export] Slider directionChange;

    [Export] Node2D pageOne;
    [Export] Node2D pageTwo;

    TestLevel level;

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
        GameManager.StartLevel();
        PackedScene LevelScene;
        JsonWriter jsonWriter = GetNode<JsonWriter>("../JsonWriter");

        LevelScene = ResourceLoader.Load<PackedScene>("res://scenes/TestLevel.tscn");
        level = LevelScene.Instantiate<TestLevel>();

        level.setPlatformSpawnChances((uint)smallPlatformSlider.Value, (uint)mediumPlatformSlider.Value, (uint)largePlatformSlider.Value, (uint)extraLargePlatformSlider.Value);
        level.setJumpGapChances((uint)smallJumpGapSlider.Value, (uint)mediumJumpGapSlider.Value, (uint)largeJumpGapSlider.Value);
        level.setPlatformTypeSpawnChances((uint)flatPlatformTypeSlider.Value, (uint)inclinePlatformTypeSlider.Value, (uint)bridgePlatformTypeSlider.Value);

        if (jsonWriter == null)
        {
            jsonWriter = GetNode<JsonWriter>("../JsonWriter");
        }
        jsonWriter.addChosenLevelAttributes("PLATFORM SIZES:");
        jsonWriter.addChosenLevelAttributes("Small Platform " + "Value: " +  smallPlatformSlider.Value);
        jsonWriter.addChosenLevelAttributes("Medium Platform " + "Value: " + mediumPlatformSlider.Value);
        jsonWriter.addChosenLevelAttributes("Large Platform " + "Value: " + largePlatformSlider.Value);
        jsonWriter.addChosenLevelAttributes("Extra Large Platform " + "Value: " + extraLargePlatformSlider.Value);
        jsonWriter.addChosenLevelAttributes("");
        jsonWriter.addChosenLevelAttributes("JUMP GAPS:");
        jsonWriter.addChosenLevelAttributes("Small Jump Gap " + "Value: " + smallJumpGapSlider.Value);
        jsonWriter.addChosenLevelAttributes("Medium Jump Gap " + "Value: " + mediumJumpGapSlider.Value);
        jsonWriter.addChosenLevelAttributes("Large Jump Gap " + "Value: " + largeJumpGapSlider.Value);
        jsonWriter.addChosenLevelAttributes("");
        jsonWriter.addChosenLevelAttributes("PLATFORM TYPES:");
        jsonWriter.addChosenLevelAttributes("Flat Platform" + "Value: " + flatPlatformTypeSlider.Value);
        jsonWriter.addChosenLevelAttributes("Bridge Platform" + "Value: " + bridgePlatformTypeSlider.Value);
        jsonWriter.addChosenLevelAttributes("Incline Platform" + "Value: " + inclinePlatformTypeSlider.Value);
        jsonWriter.addChosenLevelAttributes("");
        jsonWriter.addChosenLevelAttributes("LEVEL LAYOUT:");
        jsonWriter.addChosenLevelAttributes("Number of sections: " + (uint)numberOfSections.GetItemId(numberOfSections.Selected));

        if (uint.TryParse(sizePerSection.Text, out uint result))
        {
            if (result > 1 && result < 101)
            {
                level.settingSections(result, (uint)numberOfSections.GetItemId(numberOfSections.Selected));
                jsonWriter.addChosenLevelAttributes("Platforms per section: " + result);
            }

            else
            {
                level.settingSections((uint)25, (uint)numberOfSections.GetItemId(numberOfSections.Selected));
                jsonWriter.addChosenLevelAttributes("Platforms per section: " + 25);
            }
        }

        else
        {
            level.settingSections((uint)25, (uint)numberOfSections.GetItemId(numberOfSections.Selected));
            jsonWriter.addChosenLevelAttributes("Platforms per section: " + 25);
        }
        level.setDirectionChangeChance((uint)directionChange.Value);
        jsonWriter.addChosenLevelAttributes("Direction change chance: " + directionChange.Value);
        jsonWriter.addChosenLevelAttributes("");
        jsonWriter.addChosenLevelAttributes("LEVEL COMPONENTS:");
        jsonWriter.addChosenLevelAttributes("Bounce pad rate: " + bouncePadSlider.Value);
        jsonWriter.addChosenLevelAttributes("Spikes rate: " + spikeSlider.Value);
        jsonWriter.addChosenLevelAttributes("Coins rate: " + coinSlider.Value);
        jsonWriter.addChosenLevelAttributes("");
        jsonWriter.addChosenLevelAttributes("SPIKE DIFFICULTY:");
        jsonWriter.addChosenLevelAttributes("Spike ratios: " + "Easy: " + easySpikeSlider.Value + " Hard: " + hardSpikeSlider.Value);
        level.setComponentSpawnRate((uint)bouncePadSlider.Value, (uint)spikeSlider.Value, (uint)coinSlider.Value);
        level.setSpikeDifficulties((uint)easySpikeSlider.Value, (uint)hardSpikeSlider.Value);
        level.setSpawnSection(true);
        jsonWriter.displayChosenLevelAttributes();
        GetTree().Paused = false;
        GameManager.AddLevel(level);
        this.QueueFree();
    }
}
