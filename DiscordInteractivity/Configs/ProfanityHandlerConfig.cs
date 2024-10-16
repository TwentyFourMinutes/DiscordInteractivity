﻿using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Configs;

/// <summary>
/// The config for a profanity handler.
/// </summary>
public class ProfanityHandlerConfig
{
    /// <summary>
    /// Get or sets the profanity detection options
    /// </summary>
    public ProfanityOptions ProfanityOptions { get; set; } = ProfanityOptions.Default;

    /// <summary>
    /// Get or sets a Dictionary where the key contains a word which is considered inappropriate and the value sets the rating of the word.
    /// </summary>
    public Dictionary<string, double> ProfanityWords { get; set; }

    /// <summary>
    /// Get or sets a List which contains words which are likely to be used in an inappropriate context.
    /// </summary>
    public List<string> ProfanityIndicators { get; set; }

    /// <summary>
    /// Gets or sets whether content with command prefixes should be checked or not.
    /// </summary>
    public bool CheckCommands { get; set; } = true;

    /// <summary>
    /// Gets or sets a trigger where the profanity filter should immediately hit. If the value is set to -1 it will always check all <see cref="ProfanityWords"/>.
    /// </summary>
    public double EarlyTriggerOn { get; set; } = -1;

    /// <summary>
    /// Gets or sets the minimum where the profanity filter should fire.
    /// </summary>
    public double TriggerOn { get; set; } = 1.5;

    /// <summary>
    /// Gets or sets a value how much an <see cref="ProfanityIndicators"/> should count to the profanity rating.
    /// </summary>
    public double IndicatorAddition { get; set; } = 1;

    /// <summary>
    /// Gets or sets the distance in the Damerau-Levenshtein algorithm.
    /// </summary>
    public double WordDistance { get; set; } = 1;

    /// <summary>
    ///  Gets or sets whether the ProfanityFilter should Scan new messages or not.
    /// </summary>
    public bool ScanNewMessages { get; set; } = true;
}
