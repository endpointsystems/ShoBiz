using System;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Developer conceptual topic file for the BizTalk Schema artifact.
    /// </summary>
    /// <remarks>
    ///     Token ID: appName + ".Schemas." + schema.FullName
    /// </remarks>
    public class SchemaTopic: TopicFile
    {
        private readonly string schemaName;
        private XElement root;
        private readonly StringBuilder sb;
        private string schemaTitle;

        /// <summary>
        /// Creates a new Sandcastle schema documentation topic.
        /// </summary>
        /// <param name="btsAppName">The BizTalk application name.</param>
        /// <param name="topicPath">The path to where the topic should be saved.</param>
        /// <param name="btsSchemaName">The name of the schema to document.</param>
        public SchemaTopic(string btsAppName, string topicPath, string btsSchemaName)
        {
            appName = btsAppName;
            schemaName = btsSchemaName;
            topicRelativePath = topicPath;
            tokenId = CleanAndPrep(appName + ".Schemas." + btsSchemaName);
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            sb = new StringBuilder();
        }

        void SaveTopic()
        {
            //var bce = CatalogExplorerFactory.CatalogExplorer();
            try
            {
                //bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                Schema s = null;
                var split = schemaName.Split(new[]{"___"},StringSplitOptions.None);
                foreach (Schema schema in CatalogExplorerFactory.CatalogExplorer().Applications[appName].Schemas)
                {                    
                    if (!schema.FullName.Equals(split[0]) && !schema.RootName.Equals(split[1])) continue;
                    s = schema;
                }

                if (s == null) throw new NullReferenceException("Schema " + schemaName + " was not found in the BizTalk ExplorerOM.");

                schemaTitle = s.RootName == null ? schemaName : split[0] + "#" + split[1];
                sb.Append(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?><topic id=\"" + id + "\" revisionNumber=\"1\">");
                root = CreateDeveloperXmlReference();
                
                var intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText(string.IsNullOrEmpty(s.Description)? "No description was found for the schema." : s.Description)));
                var section = new XElement(xmlns + "section", new XElement(xmlns + "title", new XText("Schema Properties")),
                                                                    new XElement(xmlns + "content",
                                                                        new XElement(xmlns + "table",
                                                                            new XElement(xmlns + "tableHeader",
                                                                                new XElement(xmlns + "row",
                                                                                    new XElement(xmlns + "entry", new XText("Property")),
                                                                                    new XElement(xmlns + "entry", new XText("Value")))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Application")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(s.Application.Name))))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Always Track All Properties")),
                                                                                new XElement(xmlns + "entry", new XText(s.Type == SchemaType.Property ? "does not apply to property schemas." : s.AlwaysTrackAllProperties.ToString()))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Assembly Qualified Name")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Assemblies." + s.BtsAssembly.DisplayName))))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Properties")),
                                                                                new XElement(xmlns + "entry", s.Properties.Count > 0 ? DictionaryToTable(s.Properties,"Property Name", "Property Type") : new XElement(xmlns + "legacyItalic", new XText("(N/A)")))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Root Name")),
                                                                                new XElement(xmlns + "entry", new XText(s.RootName ?? "(None)" ))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Target Namespace")),
                                                                                new XElement(xmlns + "entry", new XText(s.TargetNameSpace ?? "N/A"))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Tracked Property Names")),
                                                                                s.TrackedPropertyNames.Count > 0 ? CollectionToList(s.TrackedPropertyNames) : new XElement(xmlns + "legacyItalic", new XText("(N/A)"))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Type")),
                                                                                new XElement(xmlns + "entry", new XText(s.Type.ToString()))))));

                var content = new XElement(xmlns + "codeExample", 
                    new XElement(xmlns + "code",new XAttribute("language","xml"), new XText(s.XmlContent)));
                
                root.Add(intro,section,content);
                sb.Append(root.ToString());
                sb.Append("</topic>");
            }
                catch(Exception ex)
                {
                    HandleException("SchemaTopic.DoWork", ex);
                }
        }

        /// <summary>
        /// Save the topic.
        /// </summary>
        public override void Save()
        {
            TimerStart();
            SaveTopic();
            Save(sb.ToString());
            TimerStop();
        }

        /// <summary>
        /// Get the Sandcastle content layout for the topic.
        /// </summary>
        /// <returns>An <see cref="XElement"/> containing the content layout information.</returns>
        public XElement GetContentLayout()
        {
            return NewTopicEntry(schemaTitle);
        }

    }
}
