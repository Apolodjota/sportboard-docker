/// <summary>
/// Represents a catalog of items.
/// </summary>
public class Catalog : Component
{

    /// <summary>
    /// Gets or sets the name of the catalog.
    /// </summary>
    public int Id { get; set; }

    // Relaciones

    /// <summary>
    /// Gets or sets the name of the catalog.
    /// </summary>
    public int? IdCatalog { get; set; }
    /// <summary>
    /// Gets or sets the name of the catalog.
    /// </summary>
    public Catalog? Parent { get; set; }

    /// <summary>
    /// Gets or sets the name of the catalog.
    /// </summary>
    public List<Catalog>? Children { get; set; }

    /// <summary>
    /// Gets or sets the name of the catalog.
    /// </summary>
    public string GroupCode { get; set; }
    /// <summary>
    /// Gets or sets the name of the catalog.
    /// </summary>
    public Group Group { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Catalog"/> class.
    /// </summary>
    public Catalog()
    {
        this.Children = new List<Catalog>();
        this.GroupCode = string.Empty;
        this.Group = new Group();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Catalog"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="group"></param>
    public Catalog(string name, Group group) : this()
    {
        this.Name = name;
        this.Group = group;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Catalog"/> class.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="name"></param>
    /// <param name="group"></param>
    public Catalog(string code, string name, Group group) : this(name, group)
    {
        this.Code = code;
    }

    /// <summary>
    /// Adds a child catalog to the catalog.
    /// </summary>
    /// <param name="catalog"></param>
    public void AddChild(Catalog catalog)
    {
        Children?.Add(catalog);
        catalog.Parent = this;
    }

    /// <summary>
    /// Adds a group to the catalog.
    /// </summary>
    /// <param name="group"></param>
    public void AddGroup(Group group)
    {
        this.Group = group;
        group.AddCatalog(this);
    }

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public static List<Catalog> GetCatalogsByGroup(Group group)
    {
        if (group.CatalogList == null)
        {
            return new List<Catalog>();
        }
        return group.CatalogList.ToList();
    }

}