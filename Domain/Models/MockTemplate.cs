using Domain.Enums;

namespace Domain.Models;

/// <summary>
/// Template for Mock
/// </summary>
public sealed class MockTemplate
{
    /// <summary>
    /// Type of a Mock
    /// </summary>
    public MockTypes Type { get; set; }

    /// <summary>
    /// Topic to subscribe
    /// </summary>
    public string Topic { get; set; } = default!;

    /// <summary>
    /// Answer template
    /// </summary>
    public string AnswerTemplate { get; set; } = default!;
}