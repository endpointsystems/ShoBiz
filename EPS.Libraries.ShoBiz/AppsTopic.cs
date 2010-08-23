using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// The Sandcastle Orientation document for outlining the BizTalk applications that were documented.
    /// </summary>
    public class AppsTopic : TopicFile
    {
        private readonly List<string> appNames;
        private readonly List<AppTopic> topics;
        private XElement root;
        private readonly string tokenFile;
        private readonly string projectFile;
        private readonly string contentFile;

        /// <summary>
        /// Creates a new application topic instance.
        /// </summary>
        /// <param name="basePath">The path the application topics will be persisted to.</param>
        /// <param name="imagePath">The path where all project images will be stored.</param>
        /// <param name="btsAppNames">The list of BizTalk application names that will be documented.</param>
        /// <param name="rulesDb">The name of the BizTalk Rules Engine database.</param>
        public AppsTopic(string basePath, string imagePath, List<string> btsAppNames, string rulesDb)
        {
            topicRelativePath = "Applications";
            ProjectConfiguration.BasePath = basePath;
            ProjectConfiguration.ImagesPath = imagePath;
            ProjectConfiguration.RulesDatabase = rulesDb;
            tokenFile = Path.Combine(basePath, "ShoBizTokens.tokens");
            projectFile = Path.Combine(basePath, "ShoBizProject.shfbproj");
            contentFile = Path.Combine(basePath, "ShoBiz.content");
            addFolderToProject(topicRelativePath);
            addFolderToProject(imagePath);
            addFolderToFileSystem(imagePath);
            appNames = btsAppNames;
            tokenId = "Applications";
            topics = new List<AppTopic>(btsAppNames.Count);
        }

        private void SaveTopic()
        {
            if (null == topics) return;
            if (string.IsNullOrEmpty(ProjectConfiguration.RulesDatabase))
                throw new NullReferenceException("The Rules Engine Database configuration setting is null or empty.");
            foreach (var name in appNames)
            {
                topics.Add(new AppTopic(topicRelativePath, name));
            }

            var paras = new List<XElement>();
            foreach (var topic in topics)
            {
                /*
                 * Get our <inThisSection> paragraphs
                 */
                paras.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", topic.TokenId)));
            }

            /*
             * Build our Apps topic file
             */
            root = CreateDeveloperOrientationElement();
            root.Add(new XElement(xmlns + "introduction",
                                  new XElement(xmlns + "para",
                                               new XText("This section contains documentation for all of the selected BizTalk applications. The BizTalk artifacts, and all documentation captured, has been pulled from the BizTalk Administration Console. For more information " +
                                               "on any of the applications, please select one of the links below.")))
                     , new XElement(xmlns + "inThisSection",
                                    new XText("This section documents the following BizTalk applications:"),
                                    paras.ToArray()));
            if (doc.Root != null) doc.Root.Add(root);
        }

        /// <summary>
        /// Get the Sandcastle content layout for the topic.
        /// </summary>
        /// <returns>An <see cref="XElement"/> containing the content layout information.</returns>
        public XElement GetContentLayout()
        {
            var xe = new XElement("Topic",
                                       new XAttribute("id", id),
                                       new XAttribute("visible", true),
                                       new XAttribute("title", "BizTalk Applications"));
            try
            {
                foreach (var topic in topics)
                {
                    //get the child topics for the ContentFile
                    xe.Add(topic.GetContentLayout());
                }

                return xe;
            }
            catch (Exception ex)
            {
                PrintLine("{0} caught in apps topic: {1}", ex.GetType(), ex.Message);
            }
            return xe;
        }

        /// <summary>
        /// Save the applications topic, all subsequent child topics; also saves the Sandcastle content, 
        /// token and project files.
        /// </summary>
        public override void Save()
        {
            TimerStart();
            SaveTopic();
            
            foreach (var topic in topics)
            {
                if (topic != null) topic.Save();
            }

            //loopResult = Parallel.ForEach(topics, SaveAllTopics);
            base.Save();
            //save content file and add to the project
            ContentFile.GetContentFile().AddApplicationTopic(GetContentLayout());
            ContentFile.GetContentFile().Save(contentFile);
            ProjectFile.GetProjectFile().AddContentLayoutFile(contentFile);

            OrchestrationImage.Instance().StartSavingImages();
            OrchestrationImage.Instance().CancelToken.Cancel();

            //save token file and add to the project
            TokenFile.GetTokenFile().Save(tokenFile);
            ProjectFile.GetProjectFile().AddTokenFile(tokenFile);

            //save the project file
            ProjectFile.GetProjectFile().Save(projectFile);

            TimerStop();
        }

    }
}
