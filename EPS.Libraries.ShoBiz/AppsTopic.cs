using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    public class AppsTopic : TopicFile
    {
        private readonly Thread thread;
        private readonly List<string> appNames;
        private readonly List<AppTopic> appTopics;
        private readonly string imgPath;
        private XElement root;
        private readonly string rulesDb;
        private readonly string tokenFile;
        private readonly string projectFile;
        private readonly string contentFile;

        public AppsTopic(string basePath, string imagePath, List<string> btsAppNames, string businessRulesDatabaseName)
        {
            rulesDb = businessRulesDatabaseName;
            path = basePath + @"Applications\";
            tokenFile = basePath + @"ShoBizTokens.tokens";
            projectFile = basePath + "ShoBizProject.shfbproj";
            contentFile = basePath + "ShoBiz.content";
            addFolder(path);
            imgPath = imagePath;
            addFolder(imgPath);
            appNames = btsAppNames;
            tokenId = "Applications";
            TimerStart();
            thread = new Thread(BuildAppTopics);
            thread.Name = "AppsTopicThread";
            thread.SetApartmentState(ApartmentState.MTA);
            appTopics = new List<AppTopic>(btsAppNames.Count);
            thread.Start();            
        }

        private void BuildAppTopics()
        {
            if (null == appTopics) return;
            foreach (string name in appNames)
            {
                appTopics.Add(new AppTopic(path, imgPath, name,rulesDb));
            }

            List<XElement> paras = new List<XElement>();
            foreach (AppTopic topic in appTopics)
            {
                /*
                 * Get our <inThisSection> paragraphs
                 */
                paras.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", topic.TokenId)));
                do
                {
                    Thread.Sleep(100);
                } while (!topic.ReadyToSave);
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
            lock (this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        public XElement GetContentLayout()
        {
            XElement xe = new XElement("Topic",
                                       new XAttribute("id", id),
                                       new XAttribute("visible", true),
                                       new XAttribute("title", "BizTalk Applications"));
            try
            {
                foreach (AppTopic topic in appTopics)
                {
                    do
                    {
                        Thread.Sleep(100);
                    } while (!topic.ReadyToSave);

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

        public new void Save()
        {
            base.Save();
            foreach (AppTopic topic in appTopics)
            {
                topic.Save();
            }

            //save content file and add to the project
            ContentFile.GetContentFile().AddApplicationTopic(GetContentLayout());
            ContentFile.GetContentFile().Save(contentFile);
            ProjectFile.GetProjectFile().AddContentLayoutFile(contentFile);

            //save token file and add to the project
            TokenFile.GetTokenFile().Save(tokenFile);
            ProjectFile.GetProjectFile().AddTokenFile(tokenFile);

            //save the project file
            ProjectFile.GetProjectFile().Save(projectFile);

        }

    }
}
