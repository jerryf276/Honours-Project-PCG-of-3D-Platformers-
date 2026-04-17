using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;

public partial class CoinPatterns : Node
{
    //Coin patterns for each platform
    List<List<Vector3>> largeInclinePatterns = new List<List<Vector3>> { };

    List<List<Vector3>> mediumInclinePatterns = new List<List<Vector3>> { };

    List<List<Vector3>> smallInclinePatterns = new List<List<Vector3>> { };

    List<List<Vector3>> smallPlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> mediumPlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> largePlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> extraLargePlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> smallBridgePlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> mediumBridgePlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> largeBridgePlatformPatterns = new List<List<Vector3>> { }; 


    public override void _Ready()
    {
        

    }

    public void spawnCoins(PlatformTypes type, Lengths size, Vector3 position, NewDirection direction)
    {
        
        if (type == PlatformTypes.FLAT)
        {
            chooseFlatPlatformSize(size, position, direction);
        }

        else if (type == PlatformTypes.INCLINE)
        {
            chooseInclinePlatformSize(size, position, direction);
        }

        else
        {
            chooseBridgePlatformSize(size, position, direction);
        }
    }

    private void chooseFlatPlatformSize(Lengths size, Vector3 position, NewDirection direction)
    {
        if (size == Lengths.SHORT)
        {
            spawnFlatSmallCoins(position, direction);
        }

        else if (size == Lengths.MEDIUM)
        {
            spawnFlatMediumCoins(position, direction);
        }

        else if (size == Lengths.LONG)
        {
            spawnFlatLargeCoins(position, direction);
        }

        else
        {
            spawnFlatExtraLargeCoins(position, direction);
        }
    }

    private void chooseInclinePlatformSize(Lengths size, Vector3 position, NewDirection direction)
    {
        if (size == Lengths.SHORT)
        {
            spawnInclineSmallCoins(position, direction);
        }

        else if (size == Lengths.MEDIUM)
        {
            spawnInclineMediumCoins(position, direction);
        }

        else if (size == Lengths.LONG)
        {
            spawnInclineLargeCoins(position, direction);
        }
    }

    private void chooseBridgePlatformSize(Lengths size, Vector3 position, NewDirection direction)
    {
        if (size == Lengths.SHORT)
        {
            spawnBridgeSmallCoins(position, direction);
        }

        else if (size == Lengths.MEDIUM)
        {
            spawnBridgeMediumCoins(position, direction);
        }

        else if (size == Lengths.LONG)
        {
            spawnBridgeLargeCoins(position, direction);
        }
    }

    private void spawnFlatSmallCoins(Vector3 position, NewDirection direction)
    {
        if (smallPlatformPatterns.Capacity <= 0)
        {
            //if patterns list doesn't have anything it will be added
            addSmallPlatformPatterns();
        }

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");
        for (int i = 0; i < smallPlatformPatterns[0].Count; i++)
        {
            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(smallPlatformPatterns[0][i].X, smallPlatformPatterns[0][i].Y, smallPlatformPatterns[0][i].Z);
            AddChild(coin);
        }
    }

    private void spawnFlatMediumCoins(Vector3 position, NewDirection direction)
    {
        if (mediumPlatformPatterns.Capacity <= 0)
        {
            //if patterns list doesn't have anything it will be added
            addMediumPlatformPatterns();
        }

        int index = (int)(GD.Randi() % 2);

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");

        for (int i = 0; i < mediumPlatformPatterns[index].Count; i++)
        {
            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(mediumPlatformPatterns[index][i].X, mediumPlatformPatterns[index][i].Y, mediumPlatformPatterns[index][i].Z);
            AddChild(coin);
        }
    }

    private void spawnFlatLargeCoins(Vector3 position, NewDirection direction)
    {
        if (largePlatformPatterns.Capacity <= 0)
        {
            //if patterns list doesn't have anything it will be added
            addLargePlatformPatterns();
        }
        int index = (int)(GD.Randi() % 2);

        

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");

        for (int i = 0; i < largePlatformPatterns[index].Count; i++)
        {
            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(largePlatformPatterns[index][i].X, largePlatformPatterns[index][i].Y, largePlatformPatterns[index][i].Z);
            AddChild(coin);
        }
    }

    private void spawnFlatExtraLargeCoins(Vector3 position, NewDirection direction)
    {
        if (extraLargePlatformPatterns.Capacity <= 0)
        {
            //if patterns list doesn't have anything it will be added
            addExtraLargePlatformPatterns();
        }

        int index = (int)(GD.Randi() % 2);

        if (extraLargePlatformPatterns.Count > 1)
        {
            //do rng stuff
            //change index here
        }

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");

        for (int i = 0; i < extraLargePlatformPatterns[index].Count; i++)
        {
            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(extraLargePlatformPatterns[index][i].X, extraLargePlatformPatterns[index][i].Y, extraLargePlatformPatterns[index][i].Z);
            AddChild(coin);
        }
    }

    private void spawnInclineSmallCoins(Vector3 position, NewDirection direction)
    {
        if (smallInclinePatterns.Capacity <= 0)
        {
            //if patterns list doesn't have anything it will be added
            addSmallInclinePatterns();
        }

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");

        for (int i = 0; i < smallInclinePatterns[0].Count; i++)
        {
            Vector3 positionToUse = new Vector3(smallInclinePatterns[0][i].X, smallInclinePatterns[0][i].Y, smallInclinePatterns[0][i].Z);
            if (direction == NewDirection.LEFT)
            {
                positionToUse = new Vector3(smallInclinePatterns[0][i].Z * -1, smallInclinePatterns[0][i].Y, smallInclinePatterns[0][i].X * -1);
            }

            else if (direction == NewDirection.RIGHT)
            {
                positionToUse = new Vector3(smallInclinePatterns[0][i].Z, smallInclinePatterns[0][i].Y, smallInclinePatterns[0][i].X);
            }

            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(positionToUse.X, positionToUse.Y, positionToUse.Z);
            AddChild(coin);

        }
    }

    private void spawnInclineMediumCoins(Vector3 position, NewDirection direction)
    {
        if (mediumInclinePatterns.Capacity <= 0)
        {
            //if patterns list doesn't have anything it will be added
            addMediumInclinePatterns();
        }

        int index = (int)(GD.Randi() % 2);

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");


        if (mediumInclinePatterns.Count > 1)
        {
            //do rng stuff
            //change index here
        }

        for (int i = 0; i < mediumInclinePatterns[index].Count; i++)
        {
            Vector3 positionToUse = new Vector3(mediumInclinePatterns[index][i].X, mediumInclinePatterns[index][i].Y, mediumInclinePatterns[index][i].Z);

            if (direction == NewDirection.LEFT)
            {
                positionToUse = new Vector3(mediumInclinePatterns[index][i].Z * -1, mediumInclinePatterns[index][i].Y, mediumInclinePatterns[index][i].X * -1);
            }

            else if (direction == NewDirection.RIGHT)
            {
                positionToUse = new Vector3(mediumInclinePatterns[index][i].Z, mediumInclinePatterns[index][i].Y, mediumInclinePatterns[index][i].X);
            }

            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(positionToUse.X, positionToUse.Y, positionToUse.Z);
            AddChild(coin);

        }
    }

    private void spawnInclineLargeCoins(Vector3 position, NewDirection direction)
    {
        if (largeInclinePatterns.Capacity <= 0)
        {

            //if patterns list doesn't have anything it will be added
            addLargeInclinePatterns();
        }

        int index = (int)(GD.Randi() % 2);

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");

        for (int i = 0; i < largeInclinePatterns[index].Count; i++)
        {
            Vector3 positionToUse = new Vector3(largeInclinePatterns[index][i].X, largeInclinePatterns[index][i].Y, largeInclinePatterns[index][i].Z);

            if (direction == NewDirection.LEFT)
            {
                positionToUse = new Vector3(largeInclinePatterns[index][i].Z * -1, largeInclinePatterns[index][i].Y, largeInclinePatterns[index][i].X * -1);
            }

            else if (direction == NewDirection.RIGHT)
            {
                positionToUse = new Vector3(largeInclinePatterns[index][i].Z, largeInclinePatterns[index][i].Y, largeInclinePatterns[index][i].X);
            }

            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(positionToUse.X, positionToUse.Y, positionToUse.Z);
            AddChild(coin);

        }
    }

    private void spawnBridgeSmallCoins(Vector3 position, NewDirection direction)
    {
        if (smallBridgePlatformPatterns.Capacity <= 0)
        {

            //if patterns list doesn't have anything it will be added
            addSmallBridgePatterns();
        }

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");

        for (int i = 0; i < smallBridgePlatformPatterns[0].Count; i++)
        {
            Vector3 positionToUse = new Vector3(smallBridgePlatformPatterns[0][i].X, smallBridgePlatformPatterns[0][i].Y, smallBridgePlatformPatterns[0][i].Z);
            if (direction == NewDirection.LEFT)
            {
                positionToUse =  new Vector3(smallBridgePlatformPatterns[0][i].Z * -1 + 1.0f, smallBridgePlatformPatterns[0][i].Y, smallBridgePlatformPatterns[0][i].X * -1);
            }

            else if (direction == NewDirection.RIGHT)
            {
                positionToUse = new Vector3(smallBridgePlatformPatterns[0][i].Z - 1.0f, smallBridgePlatformPatterns[0][i].Y, smallBridgePlatformPatterns[0][i].X);
            }

            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(positionToUse.X, positionToUse.Y, positionToUse.Z);
            AddChild(coin);

        }
    }

    private void spawnBridgeMediumCoins(Vector3 position, NewDirection direction)
    {
        if (mediumBridgePlatformPatterns.Capacity <= 0)
        {
            //if patterns list doesn't have anything it will be added
            addMediumBridgePatterns();
        }

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");

        for (int i = 0; i < mediumBridgePlatformPatterns[0].Count; i++)
        {
            Vector3 positionToUse = new Vector3(mediumBridgePlatformPatterns[0][i].X, mediumBridgePlatformPatterns[0][i].Y, mediumBridgePlatformPatterns[0][i].Z);
            if (direction == NewDirection.LEFT)
            {
                positionToUse = new Vector3(mediumBridgePlatformPatterns[0][i].Z * -1 + 1.0f, mediumBridgePlatformPatterns[0][i].Y, mediumBridgePlatformPatterns[0][i].X * -1);
            }

            else if (direction == NewDirection.RIGHT)
            {
                positionToUse = new Vector3(mediumBridgePlatformPatterns[0][i].Z - 1.0f, mediumBridgePlatformPatterns[0][i].Y, mediumBridgePlatformPatterns[0][i].X);
            }

            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(positionToUse.X, positionToUse.Y, positionToUse.Z);
            AddChild(coin);

        }
    }

    private void spawnBridgeLargeCoins(Vector3 position, NewDirection direction)
    {
        if (largeBridgePlatformPatterns.Capacity <= 0)
        {
            //if patterns list doesn't have anything it will be added
            addLargeBridgePatterns();
        }

        PackedScene coinScene = ResourceLoader.Load<PackedScene>("res://levelParts/coin.tscn");

        for (int i = 0; i < largeBridgePlatformPatterns[0].Count; i++)
        {
            Vector3 positionToUse = new Vector3(largeBridgePlatformPatterns[0][i].X, largeBridgePlatformPatterns[0][i].Y, largeBridgePlatformPatterns[0][i].Z);

            if (direction == NewDirection.LEFT)
            {
                positionToUse = new Vector3(largeBridgePlatformPatterns[0][i].Z * -1 + 1.0f, largeBridgePlatformPatterns[0][i].Y, (largeBridgePlatformPatterns[0][i].X * -1));
            }

            else if (direction == NewDirection.RIGHT)
            {
                positionToUse = new Vector3(largeBridgePlatformPatterns[0][i].Z - 1.0f, largeBridgePlatformPatterns[0][i].Y, largeBridgePlatformPatterns[0][i].X);
            }

            Node3D coin = coinScene.Instantiate<Node3D>();
            coin.Position = position + new Vector3(positionToUse.X, positionToUse.Y, positionToUse.Z);
            AddChild(coin);

        }
    }
    private void addLargeInclinePatterns()
    {
        //Adding large incline pattern 1
        //Vector3 

        largeInclinePatterns.Add(new List<Vector3>() { new Vector3(0.9f, 1.4f, 0.0f) });
        for (int i = 0; i < 10; i++)
        {
            largeInclinePatterns[0].Add(new Vector3(largeInclinePatterns[0][i].X + 1.2f, largeInclinePatterns[0][i].Y + 0.4f, 0.0f));
        }

        //Adding large incline pattern 2
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                largeInclinePatterns.Add(new List<Vector3>() { new Vector3(0.9f, 1.4f, 0.0f) });
            }

            else if (i == 1)
            {
                largeInclinePatterns[1].Add( new Vector3(0.9f, 1.4f, 2.0f) );
            }

            else if (i == 2)
            {
                largeInclinePatterns[1].Add(new Vector3(0.9f, 1.4f, -2.0f));
            }
            //largeInclinePatterns.Add(new List<Vector3>() { new Vector3(0.9f, 1.4f, 0.0f) });

            for (int j = 0; j < 10; j++)
            {
                if (i == 0)
                {
                    largeInclinePatterns[1].Add(new Vector3(largeInclinePatterns[0][j].X + 1.2f, largeInclinePatterns[0][j].Y + 0.4f, 0.0f));
                }

                else if (i == 1)
                {
                    largeInclinePatterns[1].Add(new Vector3(largeInclinePatterns[0][j].X + 1.2f, largeInclinePatterns[0][j].Y + 0.4f, 2.0f));
                }

                else if (i == 2)
                {
                    largeInclinePatterns[1].Add(new Vector3(largeInclinePatterns[0][j].X + 1.2f, largeInclinePatterns[0][j].Y + 0.4f, -2.0f));
                }
            }
        }
    }

    private void addMediumInclinePatterns()
    {
        //Adding medium incline pattern 1
        mediumInclinePatterns.Add(new List<Vector3>() { new Vector3(0.9f, 1.4f, 0.0f) });

        for (int i = 0; i < 5; i++)
        {
            mediumInclinePatterns[0].Add(new Vector3(mediumInclinePatterns[0][i].X + 1.2f, mediumInclinePatterns[0][i].Y + 0.4f, 0.0f));
        }

        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                mediumInclinePatterns.Add(new List<Vector3>() { new Vector3(0.9f, 1.4f, 0.0f) });
            }

            else if (i == 1)
            {
                mediumInclinePatterns[1].Add(new Vector3(0.9f, 1.4f, 2.0f));
            }

            else
            {
                mediumInclinePatterns[1].Add(new Vector3(0.9f, 1.4f, -2.0f));
            }
                //Adding medium incline pattern 2
                //mediumInclinePatterns.Add(new List<Vector3>() { new Vector3(0.9f, 1.4f, 0.0f) });

            for (int j = 0; j < 5; j++)
            {
                if (i == 0)
                {
                    mediumInclinePatterns[1].Add(new Vector3(mediumInclinePatterns[0][j].X + 1.2f, mediumInclinePatterns[0][j].Y + 0.4f, 0.0f));
                }

                else if (i == 1)
                {
                    mediumInclinePatterns[1].Add(new Vector3(mediumInclinePatterns[0][j].X + 1.2f, mediumInclinePatterns[0][j].Y + 0.4f, 2.0f));
                }

                else if (i == 2)
                {
                    mediumInclinePatterns[1].Add(new Vector3(mediumInclinePatterns[0][j].X + 1.2f, mediumInclinePatterns[0][j].Y + 0.4f, -2.0f));
                }
            }
        }
    }

    private void addSmallInclinePatterns()
    {
        //Adding small incline pattern 1
        smallInclinePatterns.Add(new List<Vector3>() { new Vector3(0.9f, 1.4f, 0.0f) });

        for (int i = 0; i < 2; i++)
        {
            smallInclinePatterns[0].Add(new Vector3(smallInclinePatterns[0][i].X + 1.2f, smallInclinePatterns[0][i].Y + 0.4f, 0.0f));
        }
    }

    private void addExtraLargePlatformPatterns()

    {
        //Adding the first pattern
        extraLargePlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 2, 0), new Vector3(1.5f, 2, 0), new Vector3(3, 2, 0), new Vector3(4.5f, 2, 0), new Vector3(6, 2, 0), 
        new Vector3(-1.5f, 2, 0), new Vector3(-3, 2, 0), new Vector3(-4.5f, 2, 0), new Vector3(-6, 2, 0), new Vector3(0, 2, 1.5f), new Vector3(0, 2, 3), new Vector3(0, 2, 4.5f),
        new Vector3(0, 2, 6), new Vector3(0, 2, -1.5f), new Vector3(0, 2, -3), new Vector3(0, 2, -4.5f), new Vector3(0, 2, -6)});


        //Adding the second pattern
        extraLargePlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 2, 0), new Vector3(1.5f, 2, 0), new Vector3(3, 2, 0), new Vector3(4.5f, 2, 0), new Vector3(6, 2, 0),
        new Vector3(-1.5f, 2, 0), new Vector3(-3, 2, 0), new Vector3(-4.5f, 2, 0), new Vector3(-6, 2, 0), new Vector3(0, 2, 1.5f), new Vector3(0, 2, 3), new Vector3(0, 2, 4.5f),
        new Vector3(0, 2, 6), new Vector3(0, 2, -1.5f), new Vector3(0, 2, -3), new Vector3(0, 2, -4.5f), new Vector3(0, 2, -6), new Vector3(1.5f, 2, 1.5f), new Vector3(1.5f, 2, -1.5f),
        new Vector3(-1.5f, 2, 1.5f), new Vector3(-1.5f, 2, -1.5f), new Vector3(3, 2, 3), new Vector3(3, 2, -3), new Vector3(-3, 2, 3), new Vector3(-3, 2, -3), 
        new Vector3(4.5f, 2, 4.5f), new Vector3(4.5f, 2, -4.5f), new Vector3(-4.5f, 2, 4.5f), new Vector3(-4.5f, 2, -4.5f), new Vector3(6, 2, 6), new Vector3(6, 2, -6),
        new Vector3(-6, 2, 6), new Vector3(-6, 2, -6)});
    }

    private void addLargePlatformPatterns()
    {
        //Adding first pattern
        largePlatformPatterns.Add(new List<Vector3>() {new Vector3(0, 2, 0),  new Vector3(1.5f, 2, 0), new Vector3(3, 2, 0), new Vector3(-1.5f, 2, 0), new Vector3(-3, 2, 0),
        new Vector3(0, 2, 1.5f), new Vector3(0, 2, 3), new Vector3(0, 2, -1.5f), new Vector3(0, 2, -3)});

        //Adding second pattern
        largePlatformPatterns.Add(new List<Vector3>() {new Vector3(0, 2, 0),  new Vector3(1.5f, 2, 0), new Vector3(3, 2, 0), new Vector3(-1.5f, 2, 0), new Vector3(-3, 2, 0),
        new Vector3(0, 2, 1.5f), new Vector3(0, 2, 3), new Vector3(0, 2, -1.5f), new Vector3(0, 2, -3), new Vector3(1.5f, 2, 1.5f), new Vector3(1.5f, 2, -1.5f), 
        new Vector3(-1.5f, 2, 1.5f), new Vector3(-1.5f, 2, -1.5f), new Vector3(3, 2, 3), new Vector3(3, 2, -3), new Vector3(-3, 2, 3), new Vector3(-3, 2, -3)});
    }

    private void addMediumPlatformPatterns()
    {
        //Adding first pattern
        mediumPlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 2, 0), new Vector3(1.5f, 2, 0), new Vector3(-1.5f, 2, 0), new Vector3(0, 2, 1.5f), new Vector3(0, 2, -1.5f) });
        //Adding second pattern
        mediumPlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 2, 0), new Vector3(1.5f, 2, 0), new Vector3(-1.5f, 2, 0), new Vector3(0, 2, 1.5f), new Vector3(0, 2, -1.5f),
        new Vector3(1.5f, 2, 1.5f), new Vector3(1.5f, 2, -1.5f), new Vector3(-1.5f, 2, 1.5f), new Vector3(-1.5f, 2, -1.5f)});
    }

    private void addSmallPlatformPatterns()
    {
        smallPlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 2, 0) });
    }

    private void addSmallBridgePatterns()
    {
        smallBridgePlatformPatterns.Add(new List<Vector3>() {new Vector3(0, 1, 0.5f), new Vector3(1, 1, 0.5f), new Vector3(2, 1, 0.5f), new Vector3(-1, 1, 0.5f), new Vector3(-2, 1, 0.5f) });
    }

    private void addMediumBridgePatterns()
    {
        mediumBridgePlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 1, 0.5f), new Vector3(1, 1, 0.5f), new Vector3(2, 1, 0.5f), new Vector3(-1, 1, 0.5f), new Vector3(-2, 1, 0.5f),
        new Vector3(3, 1, 0.5f), new Vector3(-3, 1, 0.5f)});
    }

    private void addLargeBridgePatterns()
    {
        largeBridgePlatformPatterns.Add(new List<Vector3>() { new Vector3(0, 1, 0.5f), new Vector3(1, 1, 0.5f), new Vector3(2, 1, 0.5f), new Vector3(-1, 1, 0.5f), new Vector3(-2, 1, 0.5f),
        new Vector3(3, 1, 0.5f), new Vector3(-3, 1, 0.5f), new Vector3(4, 1, 0.5f), new Vector3(-4, 1, 0.5f), new Vector3(5, 1, 0.5f), new Vector3(-5, 1, 0.5f), });
    }
}
