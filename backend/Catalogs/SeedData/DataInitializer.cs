using Catalogs.Data;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SeedData
{
    /// <summary>
    /// Provides methods to initialize data from CSV files.
    /// </summary>
    public static class DataInitializer
    {
        /// <summary>
        /// Seeds the database from CSV files if no groups or catalogs exist.
        /// </summary>
        /// <param name="context">The database context to use for seeding data.</param>
        public static void SeedFromCsv(ApplicationDbContext context)
        {
            if (!context.Groups.Any() && !context.Catalogs.Any())
            {
                SeedGroupsFromCsv(context);
                SeedCatalogsFromCsv(context);
            }
        }

        private static void SeedGroupsFromCsv(ApplicationDbContext context)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => Regex.Replace(args.Header, @"\s", string.Empty),
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            using (var reader = new StreamReader("SeedData/backup_groups.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var groups = csv.GetRecords<Group>().ToList();

                var rootGroups = groups.Where(g => string.IsNullOrEmpty(g.ParentCode)).ToList();
                foreach (var group in rootGroups)
                {
                    var newGroup = new Group
                    {
                        Code = group.Code.ToString(),
                        Name = group.Name,
                        Description = group.Description,
                        IsActive = group.IsActive,
                        Version = group.Version,
                    };
                    context.Groups.Add(newGroup);
                }
                context.SaveChanges();

                var childGroups = groups.Where(g => !string.IsNullOrEmpty(g.ParentCode)).ToList();
                foreach (var group in childGroups)
                {
                    var newGroup = new Group
                    {
                        Code = group.Code.ToString(),
                        Name = group.Name,
                        Description = group.Description,
                        IsActive = group.IsActive,
                        Version = group.Version,
                        ParentCode = group.ParentCode,
                    };
                    context.Groups.Add(newGroup);
                }
                context.SaveChanges();
            }
        }

        private static void SeedCatalogsFromCsv(ApplicationDbContext context)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => Regex.Replace(args.Header, @"\s", string.Empty),
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            using (var reader = new StreamReader("SeedData/backup_catalogs.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var catalogs = csv.GetRecords<Catalog>().ToList();
                System.Console.WriteLine(catalogs.Count);

                var rootCatalogs = catalogs.Where(c => c.IdCatalog == null).ToList();
                foreach (var catalog in catalogs)
                {
                    var group = context.Groups.FirstOrDefault(g => g.Code == catalog.GroupCode.ToString());
                    var newCatalog = new Catalog
                    {
                        Code = catalog.Code,
                        Name = catalog.Name,
                        Description = catalog.Description,
                        IsActive = catalog.IsActive,
                        Version = catalog.Version,
                        GroupCode = catalog.GroupCode.ToString(),
                        Group = group
                    };
                    if (catalog.IdCatalog != null)
                    {
                        newCatalog.IdCatalog = catalog.IdCatalog;
                    }
                    context.Catalogs.Add(newCatalog);
                }

                context.SaveChanges();
            }
        }
    }
}