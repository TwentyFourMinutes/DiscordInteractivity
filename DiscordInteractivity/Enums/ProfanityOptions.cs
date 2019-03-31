using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordInteractivity.Enums
{
	[Flags]
	public enum ProfanityOptions
	{
		CheckWithoutWhitespaces = 1,
		RemoveNoneAlphanumericCharcaters = 2,
		CheckForSimilarity = 4,
		Default = 2 | 4
	}
}
