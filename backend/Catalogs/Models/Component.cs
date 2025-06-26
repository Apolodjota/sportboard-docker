public abstract class Component : Catalogable
{
    public string Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string Name { get; set; }
    public int Version { get; set; }

}