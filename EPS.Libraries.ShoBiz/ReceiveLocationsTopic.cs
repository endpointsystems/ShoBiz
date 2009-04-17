using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    public class ReceiveLocationsTopic: TopicFile, IDisposable
    {
        private readonly BackgroundWorker rlWorker;
        private List<ReceiveLocationTopic> topics;
        private readonly XElement root;
        public ReceiveLocationsTopic(string btsAppName, string basePath)
        {
            appName = btsAppName;
            path = basePath;
            tokenId = CleanAndPrep(btsAppName + ".ReceiveLocations");
            TimerStart();
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            rlWorker = new BackgroundWorker();
            root = CreateDeveloperOrientationElement();
            rlWorker.DoWork += rlWorker_DoWork;
            rlWorker.RunWorkerCompleted += rlWorker_RunWorkerCompleted;
            rlWorker.RunWorkerAsync();

        }

        public void Dispose()
        {
            if (topics != null)
            {
                foreach (ReceiveLocationTopic topic in topics)
                {
                    topic.Dispose();
                }
            }
            rlWorker.Dispose();
        }

        void rlWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (ReceiveLocationTopic topic in topics)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!topic.ReadyToSave);
            }
            lock(this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        void rlWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            topics = new List<ReceiveLocationTopic>();
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
            XElement intro = new XElement(xmlns + "introduction", new XElement("para", new XText("This section outlines the properties for the receive locations for the selected BizTalk application.")));

            List<XElement> paras = new List<XElement>();
            foreach (ReceivePort port in bce.Applications[appName].ReceivePorts)
            {
                foreach (ReceiveLocation rl in port.ReceiveLocations)
                {
                    topics.Add(new ReceiveLocationTopic(appName,path,rl.Name));
                    paras.Add(new XElement(xmlns + "para",
                                           new XElement(xmlns + "token",
                                                        new XText(CleanAndPrep(appName + ".ReceiveLocations." + rl.ReceivePort.Name + rl.Name)))));
                }
            }
            XElement inThis = new XElement(xmlns + "inThisSection", new XText("This application contains the following receive locations:"),paras.ToArray());
            root.Add(intro, inThis);
            if (doc.Root != null) doc.Root.Add(root);
        }

        public XElement GetContentLayout()
        {
            List<XElement> t = new List<XElement>();
            foreach (ReceiveLocationTopic topic in topics)
            {
                t.Add(topic.GetContentLayout());
            }

            XElement xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Receive Locations"));

            xe.Add(t.ToArray());
            return xe;
        }

        public new void Save()
        {
            base.Save();
            foreach (ReceiveLocationTopic topic in topics)
            {
                topic.Save();
            }
        }
    }
}
