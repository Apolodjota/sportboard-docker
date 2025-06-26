using System.Text.Json.Serialization;

public class Group : Component
{
    // Propiedad de la llave primaria
    public string Code { get; set; } // Clave primaria

    // Atributos
    public string? ParentCode { get; set; }

    // Propiedades de navegación
    [JsonIgnore]
    public Group? Parent { get; set; }

    [JsonIgnore]
    public List<Group>? Children { get; set; }
    public List<Catalog>? CatalogList { get; set; }

    // Constructores
    /**
    * Constructor de la clase Group
    */
    public Group()
    {
        this.Children = [];
        this.CatalogList = [];
    }

    /**
    * Constructor de la clase Group
    * @param code Código del grupo
    * @param name Nombre del grupo
    */
    public Group(string code, string name) : this()
    {
        this.Code = code;
        this.Name = name;
    }

    /**
    * Constructor de la clase Group
    * @param code Código del grupo
    * @param name Nombre del grupo
    * @param parent Grupo padre
    */
    public Group(string code, string name, Group parent) : this(code, name)
    {
        this.Parent = parent;
    }

    // Métodos
    /**
    * Método para agregar un hijo a un grupo
    * @param group Grupo hijo
    */
    public void AddChild(Group group)
    {
        this.Children?.Add(group);
        group.Parent = this;
    }

    /**
    * Método para agregar un catálogo a un grupo
    * @param catalog Catálogo a agregar
    */
    public void AddCatalog(Catalog catalog)
    {
        this.CatalogList.Add(catalog);
        catalog.Group = this;
    }



}