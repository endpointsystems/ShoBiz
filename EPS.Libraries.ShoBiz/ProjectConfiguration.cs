using Microsoft.BizTalk.ExplorerOM;
namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Configuration information for the Sandcastle project output.
    /// </summary>
    public static class ProjectConfiguration
    {
        /// <summary>
        /// Gets or sets the base path of the documentation project.
        /// </summary>
        public static string BasePath { get; set; }

        /// <summary>
        /// Gets or sets the path to where the images are stored.
        /// </summary>
        public static string ImagesPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the database used by the BizTalk Business Rules Engine (BRE).
        /// </summary>
        public static string RulesDatabase { get; set; }

        /// <summary>
        /// Gets or sets the connection string used by the <see cref="BtsCatalogExplorer"/>.
        /// </summary>
        public static string ConnectionString { get; set; }

    }
}
