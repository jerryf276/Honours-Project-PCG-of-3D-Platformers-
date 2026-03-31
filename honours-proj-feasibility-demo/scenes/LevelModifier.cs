using Godot;
using System;

public partial class LevelModifier : Control
{
    [Export] Slider smallPlatformSlider;
    [Export] Slider mediumPlatformSlider;
    [Export] Slider largePlatformSlider;
    [Export] Slider extraLargePlatformSlider;

    [Export] Slider smallJumpGapSlider;
    [Export] Slider mediumJumpGapSlider;
    [Export] Slider largeJumpGapSlider;

    [Export] Slider coinSlider;
    [Export] Slider spikeSlider;
    [Export] Slider bouncePadSlider;

    [Export] OptionButton numberOfSections;
    [Export] TextEdit sizePerSection;

    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
           
    }
}
