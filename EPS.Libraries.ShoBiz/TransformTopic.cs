using System.Text;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Developer conceptual topic file for the BizTalk Transform artifact.
    /// </summary>
    /// <remarks>
    ///     Token ID: appName + ".Transforms." + schema.FullName
    /// </remarks>
    class TransformTopic : TopicFile
    {
        private readonly string transformName;
        private XElement root;
        private readonly StringBuilder sb;

        public TransformTopic(string btsAppName, string topicPath, string btsTransformName)
        {
            UsingParallel = false;
            appName = btsAppName;
            transformName = btsTransformName;
            topicRelativePath = topicPath;
            tokenId = CleanAndPrep(appName + ".Transforms." + btsTransformName);
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            sb = new StringBuilder();
        }

        void SaveTopic()
        {
            //var bce = CatalogExplorerFactory.CatalogExplorer();
            //bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
            var transform = CatalogExplorerFactory.CatalogExplorer().Applications[appName].Transforms[transformName];
            sb.Append(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><topic id=\"" + id + "\" revisionNumber=\"1\">");
            root = CreateDeveloperXmlReference();

            var intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText(string.IsNullOrEmpty(transform.Description) ? "No description was found for the schema." : transform.Description)));
            var section = new XElement(xmlns + "section", new XElement(xmlns + "title", new XText("Transform Properties")),
                                                                new XElement(xmlns + "content",
                                                                    new XElement(xmlns + "table",
                                                                        new XElement(xmlns + "tableHeader",
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Property")),
                                                                                new XElement(xmlns + "entry", new XText("Value")))),
                                                                        new XElement(xmlns + "row",
                                                                            new XElement(xmlns + "entry", new XText("Application")),
                                                                            new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(transform.Application.Name))))),
                                                                        new XElement(xmlns + "row",
                                                                            new XElement(xmlns + "entry", new XText("Assembly Qualified Name")),
                                                                            new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Assemblies." + transform.AssemblyQualifiedName))))),
                                                                        new XElement(xmlns + "row",
                                                                            new XElement(xmlns + "entry", new XText("Source Schema")),
                                                                            new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(appName + ".Schemas." + transform.SourceSchema.FullName)))),
                                                                        new XElement(xmlns + "row",
                                                                            new XElement(xmlns + "entry", new XText("Target Schema")),
                                                                            new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(appName + ".Schemas." + transform.TargetSchema.FullName))))
                                                                        )));

            var content = new XElement(xmlns + "codeExample", new XElement(xmlns + "code", new XAttribute("language", "xml"), new XText(transform.XmlContent)));

            root.Add(intro, section, content);
            sb.Append(root.ToString(SaveOptions.None));
            sb.Append("</topic>");
        }

        public override void Save()
        {
            TimerStart();
            SaveTopic();
            Save(sb.ToString());
            TimerStop();
        }

        public XElement GetContentLayout()
        {
            return NewTopicEntry(transformName);
        }

    }
}
