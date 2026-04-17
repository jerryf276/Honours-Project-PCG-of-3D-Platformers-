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
        string minuteText = "00";
        string secondText = "00";
        if (seconds > 59)
        {
            minutes += 1;
            seconds = 0;
        }

        if (minutes < 10)
        {
            //To display "0X" for minutes
            if (minutes <= 0)
            {
                minuteText = "00";
            }
            else
            {
                minuteText = "0" + minutes;
            }
        }

        else
        {
            minuteText = minutes.ToString();
        }

        if (seconds < 10) 
        {
            //To display "0X" for seconds
            if (seconds <=0) 
            {
                secondText = "00";
            }

            else
            {
                secondText = "0" + seconds;
            }
        }

        else
        {
            secondText = seconds.ToString();
        }

        //Example: "Time: 01:05"
        string textToPrint = "Time: " + minuteText + ":" + secondText;

        GameManager.updateTimeText(textToPrint);
    }

    private string displaySeconds()
    {
        if (seconds < 10)
        {
            return "0" + seconds;
        }
        else
        {
            return seconds.ToString();
        }
    }

    public int getMinutes()
    {
        return minutes;
    }

    public int getSeconds()
    {
        return seconds;
    }

    private string displayMinutes()
    {
        if (minutes < 10)
        {
            return "0" + minutes;
        }
        else
        {
            return minutes.ToString();
        }
    }

    public string displayCurrentTime()
    {
        return displayMinutes() + ":" + displaySeconds();
    }
}
