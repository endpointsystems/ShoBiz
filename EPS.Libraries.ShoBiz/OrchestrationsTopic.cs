using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Orientation page leading to the orchestration topic files.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>token id: application name + ".Orchestrations"</item>
    ///  </list>
    /// </remarks>
    public class OrchestrationsTopic : TopicFile, IDisposable
    {
        private readonly BackgroundWorker orchWorker;
        private readonly List<OrchestrationTopic> topics;
        private readonly string imagesPath;

        public OrchestrationsTopic(string basePath, string btsImagesPath, string btsAppName)
        {
            try
            {
                topics = new List<OrchestrationTopic>();
                path = basePath;
                appName = btsAppName;
                imagesPath = btsImagesPath;
                tokenId = CleanAndPrep(appName + ".Orchestrations");
                TimerStart();
                //add token
                TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
                ReadyToSave = false;
                PrintLine("[OrchestrationsTopic] initializing worker thread...");
                orchWorker = new BackgroundWorker();
                
                orchWorker.DoWork += orchWorker_DoWork;
                orchWorker.RunWorkerCompleted += orchWorker_RunWorkerCompleted;
                PrintLine("[OrchestrationsTopic] starting worker...");
                
                orchWorker.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                HandleException("OrchestrationsTopic constructor",ex);
            }
        }


        private void orchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (OrchestrationTopic topic in topics)
            {
                do
                {
                    Thread.Sleep(2000);
                } while (topic.ReadyToSave == false);
            }
            lock(this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        private void orchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                PrintLine("doing orchestration work");

                //build topic
                XElement root = CreateDeveloperOrientationElement();
                root.Add(new XElement(xmlns + "introduction",
                                      new XElement(xmlns + "para",
                                                   new XText(
                                                       string.Format(
                                                           "This section contains information about the orchestrations used in the {0} application.",
                                                           appName)))));
                
                XElement thisSection = new XElement(xmlns + "inThisSection");
                
                List<XElement> elems = new List<XElement>();

                foreach (
                    BtsOrchestration orch in
                        bce.Applications[appName].Orchestrations)
                {
                    topics.Add(new OrchestrationTopic(path, imagesPath, appName, orch.FullName));
                    elems.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(string.Format("{0}.Orchestrations.{1}",appName,orch.FullName))))));
                    Thread.Sleep(5000);
                }

                thisSection.Add(new XText("This application contains the following orchestrations:"),
                                elems.ToArray());
                root.Add(thisSection);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                PrintLine("[OrchestrationsTopic.DoWork] {0}: {1}",ex.GetType(),ex.Message);
            }
            finally
            {
                bce.Dispose();
            }
        }
        
        public XElement GetContentLayout()
        {
            List<XElement> t = new List<XElement>();            
            foreach (OrchestrationTopic topic in topics)
            {
               t.Add(topic.GetContentLayout());
            }

            XElement xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Orchestrations"));

            xe.Add(t.ToArray());
            return xe;
        }

        public new void Save()
        {
            base.Save();
            foreach (OrchestrationTopic topic in topics)
            {
                topic.Save();
            }
        }

        public void Dispose()
        {
            orchWorker.Dispose();
        }
    }
}