using System;

public static class DieRolls
{
    private static System.Random rand = new System.Random();

    public static int Roll(int dieSize) 
    {
        return rand.Next(dieSize) + 1;
    }
    public static int[] Rolls_Sorted(int numDice, int dieSize) 
    {
        int[] rolls = new int[numDice];

        for (int i = 0; i < numDice; i++)
        {
            rolls[i] = Roll(dieSize);
        }

        Array.Sort(rolls);

        return rolls;
    }
    public static int[] Rolls_SortedAndDropped(int numDice, int dieSize, int dropLow = 0, int dropHigh = 0) 
    {
        int[] rolls = Rolls_Sorted(numDice + dropLow + dropHigh, dieSize);

        if (dropLow > 0 || dropHigh > 0)
        {
            int[] newRolls = new int[numDice];
            int newCount = 0;
            for (int i = 0; i < numDice + dropLow + dropHigh; i++)
            {
                if (i >= dropLow && newCount < numDice)
                {
                    newRolls[newCount] = rolls[i];
                    newCount++;
                }
            }
            return newRolls;
        }

        return rolls;
    }
    public static int Roll_Total(int numDice, int dieSize, int dropLow, int dropHigh) 
    {
        int[] rolls = Rolls_SortedAndDropped(numDice, dieSize, dropLow, dropHigh);

        int total = 0;
        for (int i = 0; i < rolls.Length; i++)
        {
            total += rolls[i];
        }
        return total;
    }
    public static int Roll_Total(DieRollData dieRoll) 
    {
        return Roll_Total(dieRoll.NumberOfDice, dieRoll.DieSize, dieRoll.DropLow, dieRoll.DropHigh);
    }
}

public class DieRollData
{
    public int NumberOfDice;
    public int DieSize;
    public int DropLow;
    public int DropHigh;
    public int Total 
    {
        get
        {
            return DieRolls.Roll_Total(this);
        }
    }

    public DieRollData(int numberOfDice, int dieSize, int dropLow, int dropHigh) 
    {
        NumberOfDice = numberOfDice;
        DieSize = dieSize;
        DropLow = dropLow;
        DropHigh = dropHigh;
    }
}
