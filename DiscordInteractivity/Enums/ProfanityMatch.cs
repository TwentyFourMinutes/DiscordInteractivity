namespace DiscordInteractivity.Enums;

/// <summary>
/// Determines what kind of match the profanity content is.
/// </summary>
public enum ProfanityMatch
{
    /// <summary>
    /// The words match fully
    /// </summary>
    FullMatch,

    /// <summary>
    /// The words are similar to each other.
    /// </summary>
    SimilarMatch,

    /// <summary>
    /// The words match partly.
    /// </summary>
    PartlyMatch,

    /// <summary>
    /// The words do not match.
    /// </summary>
    NoMatch,
}
