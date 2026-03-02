using Godot;
using System;

public partial class GameTimer : Timer
{

    int minutes = 0;
    int seconds = 0;

    public override void _Ready()
    {
        Timeout += OnTimeOut;
    }

    private void OnTimeOut()
    {
        seconds += 1;

        if (seconds > 59)
        {
            minutes += 1;
            seconds = 0;
        }

    }
}
