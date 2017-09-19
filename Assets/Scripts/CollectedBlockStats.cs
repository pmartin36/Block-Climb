using System;

/// <summary>
/// Keeps track of the blocks collected in a level as well as the amount of blocks the the level contains at start
/// </summary>
[Serializable]
public class CollectedBlockStats : ICloneable
{
	public int Collected { get; set; }
	public int Available { get; set; }
	public float Percent
	{
		get
		{
			return (float)Collected / (float)Available;
		}
	}
	public CollectedBlockStats() : this(-1,-1)
	{
		
	}
	public CollectedBlockStats(int collected, int available)
	{
		Collected = collected;
		Available = available;
	}

	public static CollectedBlockStats operator + (CollectedBlockStats c, int num) {
		return new CollectedBlockStats(c.Collected + num, c.Available);
	}

	public static CollectedBlockStats operator -(CollectedBlockStats c, int num)
	{
		return new CollectedBlockStats(c.Collected - num, c.Available);
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public override string ToString()
	{
		return string.Format("{0}/{1}",Collected,Available);
	}
}
