using System;

public class Range<T>
{
	public T min;
	public T max;

	public Range (T min, T max)
	{
		this.min = min;
		this.max = max;
	}

	public virtual T Get (float normalizedValue)
	{
		if (normalizedValue < 0 || normalizedValue > 1)
			throw new ArgumentOutOfRangeException("normalizedValue");
		if (normalizedValue <= .5f)
			return min;
		else
			return max;
	}
}