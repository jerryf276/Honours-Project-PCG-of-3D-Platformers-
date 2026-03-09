using Godot;
using System;
using System.Collections.Generic;

public partial class ComponentSpawner : Node
{
    private struct componentPlatform
    {
        public PlatformTypes type;
        public Lengths size;
        public Vector3 position;
        public NewDirection direction;
    }

    List<componentPlatform> coinPlatforms = new List<componentPlatform>();
    List<componentPlatform> spikePlatforms = new List<componentPlatform>();


    public void addCoinPlatform(PlatformTypes platformType, Lengths platformSize, Vector3 platformPosition, NewDirection platformDirection)
    {
        //coinPlatforms.Add
        componentPlatform coinPlatformToAdd = new componentPlatform();
        coinPlatformToAdd.type = platformType;
        coinPlatformToAdd.size = platformSize;
        coinPlatformToAdd.position = platformPosition;
        coinPlatformToAdd.direction = platformDirection;
        coinPlatforms.Add(coinPlatformToAdd);
    }

    public void addSpikePlatform(PlatformTypes platformType, Lengths platformSize, Vector3 platformPosition, NewDirection platformDirection)
    {
        componentPlatform spikePlatformToAdd = new componentPlatform();
        spikePlatformToAdd.type = platformType;
        spikePlatformToAdd.size = platformSize;
        spikePlatformToAdd.position = platformPosition;
        spikePlatformToAdd.direction = platformDirection;
        spikePlatforms.Add(spikePlatformToAdd);

    }

    public void generateCoins()
    {
        for (int i = 0; i < coinPlatforms.Count; i++)
        {
            if (coinPlatforms[i].type == PlatformTypes.BRIDGE)
            {
                generateBridgeCoins(coinPlatforms[i]);
            }

            else if (coinPlatforms[i].type == PlatformTypes.INCLINE)
            {

            }

            else
            {

            }

        }

    }

    private void generateBridgeCoins(componentPlatform platform)
    {
        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://Level parts/coin.tscn");
        Node3D coin = coinScene.Instantiate<Node3D>();
        coin.Position = platform.position;
        Vector3 coinPosition = coin.Position;
        coinPosition.Y += 1;

        //Centering coins
        if (platform.direction == NewDirection.LEFT)
        {
            

            coinPosition.X += 0.5f;
        }

        else if (platform.direction == NewDirection.RIGHT)
        {
            coinPosition.X -= 0.5f;
        }

        else
        {
            coinPosition.Z += 0.5f;
        }

            coin.Position = coinPosition;
        GetTree().Root.AddChild(coin);

        int platformSize = 0;
        switch (platform.size)
        {
            case Lengths.SHORT:
                platformSize = 2;
                break;
            case Lengths.MEDIUM:
                platformSize = 3;
                break;

            case Lengths.LONG:
                platformSize = 5;
                break;
        }

        for (int i = 0; i < platformSize; i++)
        {
            Node3D coinToSpawn1 = coinScene.Instantiate<Node3D>();
            Node3D coinToSpawn2 = coinScene.Instantiate<Node3D>();
            coinToSpawn1.Position = platform.position;
            coinToSpawn2.Position = platform.position;
            Vector3 tempPosition;
            if (platform.direction == NewDirection.FORWARD)
            {
                //Straight line of coins
                tempPosition = coinToSpawn1.Position;
                tempPosition.X += i + 1;
                tempPosition.Y += 1;
                tempPosition.Z += 0.5f;
                coinToSpawn1.Position = tempPosition;

                tempPosition = coinToSpawn2.Position;
                tempPosition.X -= i + 1;
                tempPosition.Y += 1;
                tempPosition.Z += 0.5f;
                coinToSpawn2.Position = tempPosition;
            }

            else if (platform.direction == NewDirection.LEFT)
            {
                //Straight line of coins
                tempPosition = coinToSpawn1.Position;
                //Left will use negative Z
                tempPosition.Z -= i + 1;
                tempPosition.X += 0.5f;
                tempPosition.Y += 1;
                coinToSpawn1.Position = tempPosition;

                tempPosition = coinToSpawn2.Position;
                tempPosition.Z += i + 1;
                tempPosition.X += 0.5f;
                tempPosition.Y += 1;
                coinToSpawn2.Position = tempPosition;
            }

            else
            {
                //Straight line of coins
                tempPosition = coinToSpawn1.Position;
                tempPosition.Z += i + 1;
                tempPosition.X -= 0.5f;
                tempPosition.Y += 1;
                coinToSpawn1.Position = tempPosition;

                tempPosition = coinToSpawn2.Position;
                tempPosition.Z -= i + 1;
                tempPosition.X -= 0.5f;
                tempPosition.Y += 1;
                coinToSpawn2.Position = tempPosition;
            }
            GetTree().Root.AddChild(coinToSpawn1);
            GetTree().Root.AddChild(coinToSpawn2);
           // coinToSpawn1.Position.X += 
        }

    }

    private void generateFlatSpikes(componentPlatform platform)
    {
        PackedScene spikes = ResourceLoader.Load<PackedScene>("res://Level parts/spikeTest.tscn");
        Vector3 spikePosition = platform.position;

        if (platform.size == Lengths.SHORT)
        {
            spikePosition.Y += 1.5f;
        }
        else if (platform.size == Lengths.MEDIUM)
        {
            spikePosition.Y += 1;
        }
        else
        {
            spikePosition.Y += 1.5f;

        }
            // spikePosition.Y += 1;
            Node3D spikeToSpawn = spikes.Instantiate<Node3D>();
        spikeToSpawn.Position = spikePosition;
        GetTree().Root.AddChild(spikeToSpawn);
    }

    public void generateSpikes()

    {
        for (int i = 0; i < spikePlatforms.Count; i++)
        {
            

            if (spikePlatforms[i].type == PlatformTypes.BRIDGE)
            {
           
            }

            else if (spikePlatforms[i].type == PlatformTypes.INCLINE)
            {

            }

            else
            {
                generateFlatSpikes(spikePlatforms[i]);
            }

        
        }
    }
}
