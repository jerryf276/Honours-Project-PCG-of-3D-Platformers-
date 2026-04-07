using Godot;
using System;
using System.Collections.Generic;

public partial class SpikePatterns : Node
{
    List<List<Vector3>> largePlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> extraLargePlatformPatterns = new List<List<Vector3>> { };


    public void spawnSpikes(Lengths size, Vector3 currentPosition, NewDirection currentDirection)
    {
        switch (size)
        {
            case Lengths.SHORT:
                spawnSmallSpike(currentPosition, currentDirection);
                break;

            case Lengths.MEDIUM:
                spawnMediumSpike(currentPosition, currentDirection);
                break;

            case Lengths.LONG:
                spawnLargeSpikes(currentPosition, currentDirection);
                break;

            case Lengths.LONGEST:
                spawnExtraLargeSpikes(currentPosition, currentDirection);
                break;
        }
    }


    private void spawnSmallSpike(Vector3 currentPosition, NewDirection currentDirection)
    {
        PackedScene spikeScene = ResourceLoader.Load<PackedScene>("res://Level parts/spikeTest.tscn");
        Node3D spike = spikeScene.Instantiate<Node3D>();
        spike.Position = new Vector3(currentPosition.X, currentPosition.Y + 1.5f, currentPosition.Z);
        AddChild(spike);
    }

    private void spawnMediumSpike(Vector3 currentPosition, NewDirection currentDirection)
    {
        PackedScene spikeScene = ResourceLoader.Load<PackedScene>("res://Level parts/spikeTest.tscn");
        Node3D spike = spikeScene.Instantiate<Node3D>();
        spike.Scale *= 2;
        spike.Position = new Vector3(currentPosition.X, currentPosition.Y + 1.5f, currentPosition.Z);
        AddChild(spike);
    }

    private void spawnLargeSpikes(Vector3 currentPosition, NewDirection currentDirection)
    {
        if (largePlatformPatterns.Capacity <= 0)
        {
            createLargeSpikePatterns();
        }

        int index = 0;

        if (largePlatformPatterns.Count > 1)
        {
            //do rng stuff here
        }
        PackedScene spikeScene = ResourceLoader.Load<PackedScene>("res://Level parts/spikeTest.tscn");

        for (int i = 0; i < largePlatformPatterns[index].Count; i++)
        {
            Vector3 positionToUse = largePlatformPatterns[index][i];
            // largePlatformPatterns[index] = 
            if (currentDirection == NewDirection.LEFT)
            {
                positionToUse *= -1;
            }

            else if (currentDirection == NewDirection.FORWARD)
            {
                positionToUse = new Vector3(largePlatformPatterns[index][i].Z, largePlatformPatterns[index][i].Y, largePlatformPatterns[index][i].X);
                //positionToUse *= -1;
            }
            Node3D spike = spikeScene.Instantiate<Node3D>();
            spike.Scale *= 2;
            spike.Position = currentPosition + new Vector3(positionToUse.X, (positionToUse.Y + 1.5f), positionToUse.Z);
            AddChild(spike);
        }

    }
    
    private void spawnExtraLargeSpikes(Vector3 currentPosition, NewDirection currentDirection)
    {
        if (extraLargePlatformPatterns.Capacity <= 0)
        {
            createExtraLargeSpikePatterns();
        }

        int index = 0;

        if (extraLargePlatformPatterns.Count > 1)
        {
            //do rng stuff here
        }
        PackedScene spikeScene = ResourceLoader.Load<PackedScene>("res://Level parts/spikeTest.tscn");

        for (int i = 0; i < extraLargePlatformPatterns[index].Count; i++)
        {
            Vector3 positionToUse = extraLargePlatformPatterns[index][i];
            // largePlatformPatterns[index] = 
            if (currentDirection == NewDirection.LEFT)
            {
                positionToUse *= -1;
            }

            else if (currentDirection == NewDirection.FORWARD)
            {
                positionToUse = new Vector3(extraLargePlatformPatterns[index][i].Z, extraLargePlatformPatterns[index][i].Y, extraLargePlatformPatterns[index][i].X);
            }
            Node3D spike = spikeScene.Instantiate<Node3D>();
            spike.Scale *= 3.0f;
            spike.Position = currentPosition + new Vector3(positionToUse.X, (positionToUse.Y + 1.5f), positionToUse.Z);
            AddChild(spike);
        }
    }

    private void createExtraLargeSpikePatterns()
    {
        //First pattern
        extraLargePlatformPatterns.Add(new List<Vector3>() {new Vector3(0, 0, 0), new Vector3(-5, 0, 0), new Vector3(5, 0, 0) });

        //Second pattern
        extraLargePlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 0, 0), new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(5, 0, 5),
        new Vector3(-5, 0, -5), new Vector3(5, 0, -5), new Vector3(-5, 0, 5)});
    }

    private void createLargeSpikePatterns()
    {
        //First pattern
        largePlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 0, 0), new Vector3(2.5f, 0, 0), new Vector3(-2.5f, 0, 0) });

        //Second pattern
        largePlatformPatterns.Add(new List<Vector3>() {new Vector3(0, 0, 0), new Vector3(2.5f, 0, 0), new Vector3(-2.5f, 0, 0), new Vector3(2.5f, 0, 2.5f), 
            new Vector3(-2.5f, 0, -2.5f), new Vector3(2.5f, 0, -2.5f), new Vector3(-2.5f, 0, 2.5f)});
    }
}
