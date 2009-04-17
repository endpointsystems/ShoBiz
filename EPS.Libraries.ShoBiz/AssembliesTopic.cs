using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// A orientation document outlining the assemblies used in the app.
    /// </summary>
    public class AssembliesTopic : TopicFile, IDisposable
    {
        //private BackgroundWorker assemblyWorker;
        private readonly List<AssemblyTopic> assemblyTopics;

        public AssembliesTopic(string basePath, string btsAppName)
        {
            tokenId = CleanAndPrep(appName + ".Resources");
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            TimerStart();
            assemblyTopics = new List<AssemblyTopic>();
            appName = btsAppName;
            path = basePath;
            DoWork();
            Run();
            TimerStop();
        }

        public void Dispose()
        {
            foreach (AssemblyTopic topic in assemblyTopics)
            {                
                topic.Dispose();
            }
        }

        private void Run()
        {
            foreach (AssemblyTopic topic in assemblyTopics)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!topic.ReadyToSave);
            }
            lock (this)
            {
                ReadyToSave = true;
            }
        }

        private void DoWork()
        {
            //build topic
            XElement root = CreateDeveloperOrientationElement();
            root.Add(new XElement(xmlns + "introduction",
                                  new XElement(xmlns + "para",
                                               new XText(
                                                   string.Format(
                                                       "This section contains information about the assemblies used in the {0} application.",
                                                       appName)))));
            XElement thisSection = new XElement(xmlns + "inThisSection");
            List<XElement> elems =
                new List<XElement>(CatalogExplorerFactory.CatalogExplorer().Applications[appName].Assemblies.Count);

            foreach (BtsAssembly assy in CatalogExplorerFactory.CatalogExplorer().Applications[appName].Assemblies)
            {
                assemblyTopics.Add(new AssemblyTopic(appName, assy.Name, path));
                elems.Add(new XElement(xmlns + "para", new XElement(xmlns + "topic", new XText(CleanAndPrep(assy.DisplayName)))));
            }

            thisSection.Add(new XElement(xmlns + "para", new XText("This application contains the following assemblies:")),
                            elems.ToArray());
            root.Add(thisSection);
            if (doc.Root != null) doc.Root.Add(root);
        }

        public new void Save()
        {
            base.Save();
            foreach (AssemblyTopic topic in assemblyTopics)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!topic.ReadyToSave);
                topic.Save();
            }
            doc.Save(path + id + ".aml");
        }

        public XElement GetContentLayout()
        {
            XElement ret = new XElement("Topic",
                                        new XAttribute("id", id),
                                        new XAttribute("visible", "true"),
                                        new XAttribute("title", "Resources"));
            foreach (AssemblyTopic topic in assemblyTopics)
            {
                ret.Add(topic.GetContentLayout());
            }
            return ret;
        }
    }
}