using Godot;
using System;
using System.Collections.Generic;
using System.Net;

public partial class CoinPatterns : Node
{
    List<List<Vector3>> largeInclinePatterns = new List<List<Vector3>> { };

    List<List<Vector3>> mediumInclinePatterns = new List<List<Vector3>> { };

    List<List<Vector3>> smallInclinePatterns = new List<List<Vector3>> { };

    List<List<Vector3>> smallPlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> mediumPlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> largePlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> extraLargePlaformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> smallBridgePlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> mediumBridgePlatformPatterns = new List<List<Vector3>> { };

    List<List<Vector3>> largeBridgePlatformPatterns = new List<List<Vector3>> { }; 


    public override void _Ready()
    {
        

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
                mediumInclinePatterns[0].Add(new Vector3(0.9f, 1.4f, 2.0f));
            }

            else
            {
                mediumInclinePatterns[0].Add(new Vector3(0.9f, 1.4f, -2.0f));
            }
                //Adding medium incline pattern 2
                mediumInclinePatterns.Add(new List<Vector3>() { new Vector3(0.9f, 1.4f, 0.0f) });

            for (int j = 0; j < 5; j++)
            {
                if (i == 0)
                {
                    mediumInclinePatterns[0].Add(new Vector3(mediumInclinePatterns[0][j].X + 1.2f, mediumInclinePatterns[0][j].Y + 0.4f, 0.0f));
                }

                else if (i == 1)
                {
                    mediumInclinePatterns[0].Add(new Vector3(mediumInclinePatterns[0][j].X + 1.2f, mediumInclinePatterns[0][j].Y + 0.4f, 2.0f));
                }

                else if (i == 2)
                {
                    mediumInclinePatterns[0].Add(new Vector3(mediumInclinePatterns[0][j].X + 1.2f, mediumInclinePatterns[0][j].Y + 0.4f, -2.0f));
                }
                    //mediumInclinePatterns[0].Add(new Vector3(mediumInclinePatterns[0][j].X + 1.2f, mediumInclinePatterns[0][j].Y + 0.4f, 0.0f));
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
        extraLargePlaformPatterns.Add(new List<Vector3>() { new Vector3(0, 2, 0), new Vector3(1.5f, 2, 0), new Vector3(3, 2, 0), new Vector3(4.5f, 2, 0), new Vector3(6, 2, 0), 
        new Vector3(-1.5f, 2, 0), new Vector3(-3, 2, 0), new Vector3(-4.5f, 2, 0), new Vector3(-6, 2, 0), new Vector3(0, 2, 1.5f), new Vector3(0, 2, 3), new Vector3(0, 2, 4.5f),
        new Vector3(0, 2, 6), new Vector3(0, 2, -1.5f), new Vector3(0, 2, -3), new Vector3(0, 2, -4.5f), new Vector3(0, 2, -6)});


        //Adding the second pattern
        extraLargePlaformPatterns.Add(new List<Vector3>() { new Vector3(0, 2, 0), new Vector3(1.5f, 2, 0), new Vector3(3, 2, 0), new Vector3(4.5f, 2, 0), new Vector3(6, 2, 0),
        new Vector3(-1.5f, 2, 0), new Vector3(-3, 2, 0), new Vector3(-4.5f, 2, 0), new Vector3(-6, 2, 0), new Vector3(0, 2, 1.5f), new Vector3(0, 2, 3), new Vector3(0, 2, 4.5f),
        new Vector3(0, 2, 6), new Vector3(0, 2, -1.5f), new Vector3(0, 2, -3), new Vector3(0, 2, -4.5f), new Vector3(0, 2, -6), new Vector3(1.5f, 2, 1.5f), new Vector3(1.5f, 2, -1.5f),
        new Vector3(-1.5f, 2, 1.5f), new Vector3(-1.5f, 2, -1.5f), new Vector3(3, 2, 3), new Vector3(3, 2, -3), new Vector3(-3, 2, 3), new Vector3(-3, 2, -3), 
        new Vector3(4.5f, 2, 4.5f), new Vector3(4.5f, 2, -4.5f), new Vector3(-4.5f, 2, 4.5f), new Vector3(-4.5f, 2, -4.5f), new Vector3(6, 2, 6), new Vector3(6, 2, -6),
        new Vector3(-6, 2, 6), new Vector3(-6, 2, -6)});
    }



}
