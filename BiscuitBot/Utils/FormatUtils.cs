namespace BiscuitBot.Utils;

public static class FormatUtils
{
	public static string Ordinal(
		int number)
	{
		if (number <= 0)
			return number.ToString();

		switch (number % 100)
		{
			case 11:
			case 12:
			case 13:
				return number + "th";
		}

		return (number % 10) switch
		{
			1 => number + "st",
			2 => number + "nd",
			3 => number + "rd",
			_ => number + "th",
		};
	}
}
