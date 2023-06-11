namespace Domain.Models;

public class Listener
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Name of listener
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Topic to listen
    /// </summary>
    public string Topic { get; set; } = "";
}