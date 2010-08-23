using System;
using System.Xml.Linq;
namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Represents the Sandcastle Help File Builder (SHFB) project file (<c>.shfbproj</c>) document.
    /// </summary>
    public class ProjectFile
    {
        private static ProjectFile projectFile;
        private static XElement foldersGroup;
        private static XElement topicsGroup;
        private static XElement imagesGroup;
        private static XElement contentLayoutGroup;
        private static XElement tokenGroup;
        private readonly XNamespace xmlns;

        /// <summary>
        /// Creates a new instance of the project file.
        /// </summary>
        private ProjectFile()
        {
            xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
            foldersGroup = new XElement(xmlns + "ItemGroup");
            topicsGroup = new XElement(xmlns + "ItemGroup");
            imagesGroup = new XElement(xmlns + "ItemGroup");
            contentLayoutGroup = new XElement(xmlns + "ItemGroup");
            tokenGroup = new XElement(xmlns + "ItemGroup");

        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="projectFile"/>.
        /// </summary>
        /// <returns></returns>
        public static ProjectFile GetProjectFile()
        {
            if (projectFile == null)
            {
                projectFile = new ProjectFile();
            }

            return projectFile;
        }

        /// <summary>
        /// Adds a folder to the list of directories to include.
        /// </summary>
        /// <param name="folderPath">The path of the folder to include.</param>
        public void AddFolderItem(string folderPath)
        {
            foldersGroup.Add(new XElement(xmlns + "Folder", new XAttribute("Include", folderPath)));
        }

        /// <summary>
        /// Adds a Sandcastle topic item to the project.
        /// </summary>
        /// <param name="topicFilePath">The path to the topic file.</param>
        public void AddTopicItem(string topicFilePath)
        {            
            topicsGroup.Add(new XElement(xmlns + "None", new XAttribute("Include", 
                topicFilePath.Replace(ProjectConfiguration.BasePath + @"\",string.Empty))));
        }
        /// <summary>
        /// Adds an image to the project.
        /// </summary>
        /// <param name="imageFilePath">The path to the image object.</param>
        /// <param name="imageId">The ID of the image.</param>
        public void AddImageItem(string imageFilePath, string imageId)
        {
            imagesGroup.Add(new XElement(xmlns + "Image",
                        new XAttribute("Include",imageFilePath),
                        new XElement(xmlns + "ImageId", imageId)));

        }
        /// <summary>
        /// Adds a Sandcastle topics layout file to the project.
        /// </summary>
        /// <param name="layoutFilePath">The path to the content layout file.</param>
        public void AddContentLayoutFile(string layoutFilePath)
        {
            contentLayoutGroup.Add(new XElement(xmlns + "ContentLayout", new XAttribute("Include", layoutFilePath)));
        }

        /// <summary>
        /// Adds a token file reference to the project.
        /// </summary>
        /// <param name="tokenFilePath">The path to the token file.</param>
        public void AddTokenFile(string tokenFilePath)
        {
            tokenGroup.Add(new XElement(xmlns + "Tokens", new XAttribute("Include", tokenFilePath)));
        }
        /// <summary>
        /// Save the project file.
        /// </summary>
        /// <param name="projectFilePath">The path to save the project file to.</param>
        public void Save(string projectFilePath)
        {
            var proj = new XElement(xmlns + "Project",
                new XAttribute("xmlns","http://schemas.microsoft.com/developer/msbuild/2003"),
                new XAttribute("Targets","Build"),
                new XElement(xmlns + "PropertyGroup",
                    new XElement(xmlns + "Configuration",
                        new XAttribute("Condition", " '$(Configuration)' == '' "), "Debug"),
                    new XElement(xmlns + "Platform", new XAttribute("Condition", " '$(Platform)' == '' "), "AnyCPU"),
                    new XElement(xmlns + "SchemaVersion", "2.0"),
                    new XElement(xmlns + "ProjectGuid", Guid.NewGuid().ToString("B")),
                    new XElement(xmlns + "SHFBSchemaVersion", "1.8.0.0"),
                    new XElement(xmlns + "AssemblyName", "BizTalkDocumentation"),
                    new XElement(xmlns + "RootNamespace", "BizTalkDocumentation"),
                    new XElement(xmlns + "Name", "Documentation"),
                    new XElement(xmlns + "OutputPath", @".\Help\"),
                    new XElement(xmlns + "HtmlHelpName", @"Documentation"),
                    new XElement(xmlns + "PlugInConfigurations",
                            new XElement("PlugInConfig",
                                new XAttribute("id","Additional Content Only"), new XAttribute("enabled","True"), new XElement("configuration"))),
                    new XElement(xmlns + "ProjectSummary",new XText("Generated by ShoBiz, a documentation tool developed by Endpoint Systems.")),
                    new XElement(xmlns + "FooterText", new XText("Endpoint Systems")),
                    new XElement(xmlns + "HelpTitle", new XText("BizTalk Applications Documentation  - Endpoint Systems")),
                    new XElement(xmlns + "IncludeFavorites", new XText("true"))),
                new XElement(xmlns + "PropertyGroup", new XAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ")),
                new XElement(xmlns + "PropertyGroup", new XAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ")),
                foldersGroup,topicsGroup,imagesGroup,contentLayoutGroup,tokenGroup,
                new XElement(xmlns + "Import", new XAttribute("Project", @"$(SHFBROOT)\SandcastleHelpFileBuilder.targets")));
            var doc = new XDocument();
            doc.Add(proj);
            doc.Save(projectFilePath);
        }
    }

}
