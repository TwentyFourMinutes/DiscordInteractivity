using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordInteractivity.Enums
{
	/// <summary>
	/// Specifies which options the profanity hanlder uses.
	/// </summary>
	[Flags]
	public enum ProfanityOptions
	{
		/// <summary>
		/// Removes whitespaces from the content to scan.
		/// </summary>
		CheckWithoutWhitespaces = 1,
		/// <summary>
		/// Removes none alphanumeric characters from the contenent to scan.
		/// </summary>
		RemoveNoneAlphanumericCharcaters = 2,
		/// <summary>
		/// If there is a match to a word it won't try again to match anything to it.
		/// </summary>
		IgnoreDuplicateAssumptions = 4,
		/// <summary>
		/// Are the default settings, which are mostly used.
		/// </summary>
		Default = 2 | 4 
	}
}
