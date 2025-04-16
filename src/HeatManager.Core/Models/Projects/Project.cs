namespace HeatManager.Core.Models.Projects;

public class Project
{
    public Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public DateTime LastOpened { get; set; } = DateTime.UtcNow;

    public ProjectData ProjectData { get; } = new();
}