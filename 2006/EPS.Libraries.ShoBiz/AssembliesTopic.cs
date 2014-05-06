using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// A orientation document outlining the assemblies used in the app.
    /// </summary>
    class AssembliesTopic : TopicFile
    {
        //private BackgroundWorker assemblyWorker;
        private readonly List<AssemblyTopic> assemblyTopics;
        private readonly string[] resources;

        /// <summary>
        /// Creates a new Sandcastle orientation topic for BizTalk assemblies.
        /// </summary>
        /// <param name="basePath">The path to save the topic files.</param>
        /// <param name="btsAppName">The BizTalk application name.</param>
        /// <param name="assemblies">The BizTalk assemblies to document.</param>
        public AssembliesTopic(string basePath, string btsAppName, string[] assemblies)
        {
            resources = assemblies;
            tokenId = CleanAndPrep(appName + ".Resources");
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            assemblyTopics = new List<AssemblyTopic>();
            appName = btsAppName;
            topicRelativePath = basePath;
        }

        private void SaveTopic()
        {
            //build topic
            var root = CreateDeveloperOrientationElement();
            root.Add(new XElement(xmlns + "introduction",
                                  new XElement(xmlns + "para",
                                               new XText(
                                                   string.Format(
                                                       "This section contains information about the assemblies used in the {0} application.",
                                                       appName)))));
            var elems =
                new List<XElement>(CatalogExplorerFactory.CatalogExplorer().Applications[appName].Assemblies.Count);

            foreach (var assy in resources)
            {
                assemblyTopics.Add(new AssemblyTopic(appName, assy, topicRelativePath));
                elems.Add(new XElement(xmlns + "para", new XElement(xmlns + "topic", new XText(CleanAndPrep(appName + ".Assemblies." + assy)))));
            }

            var thisSection = new XElement(xmlns + "inThisSection", new XText("This application contains the following assemblies:"),elems.ToArray());
            root.Add(thisSection);
            if (doc.Root != null) doc.Root.Add(root);
        }

        /// <summary>
        /// Save the BizTalk Assemblies and all child Assembly topic files.
        /// </summary>
        public override void Save()
        {
            TimerStart();
            SaveTopic();
            foreach (var topic in assemblyTopics)
            {
                topic.Save();
            }
            base.Save();
            TimerStop();
        }

        /// <summary>
        /// Get the Sandcastle content layout for the topic.
        /// </summary>
        /// <returns>An <see cref="XElement"/> containing the content layout information.</returns>
        public XElement GetContentLayout()
        {
            var ret = new XElement("Topic",
                                        new XAttribute("id", id),
                                        new XAttribute("visible", "true"),
                                        new XAttribute("title", "Resources"));
            foreach (var topic in assemblyTopics)
            {
                if (topic != null) ret.Add(topic.GetContentLayout());
            }
            return ret;
        }
    }
}