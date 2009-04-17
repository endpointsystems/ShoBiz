using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    class ReceivePortsTopic: TopicFile, IDisposable
    {
        private readonly List<ReceivePortTopic> topics;
        private readonly BackgroundWorker rpWorker;
        private XElement root;
        public ReceivePortsTopic(string btsAppName, string btsBaseDir)
        {
            tokenId = CleanAndPrep(btsAppName + ".ReceivePorts");
            TimerStart();
            path = btsBaseDir;
            appName = btsAppName;
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            rpWorker = new BackgroundWorker();
            rpWorker.DoWork += rpWorker_DoWork;
            rpWorker.RunWorkerCompleted += rpWorker_RunWorkerCompleted;
            topics = new List<ReceivePortTopic>();
            rpWorker.RunWorkerAsync();

        }

        public void Dispose()
        {
            if (null != topics)
            {
                foreach (ReceivePortTopic topic in topics)
                {
                    topic.Dispose();
                }
            }
            rpWorker.Dispose();
        }

        void rpWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (ReceivePortTopic topic in topics)
            {
                do
                {
                    Trace.WriteLine("[ReceivePortsTopic] Waiting for a receive port topic..");
                    Thread.Sleep(100);                    
                } while (!topic.ReadyToSave);
            }
            lock(this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        void rpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            root = CreateDeveloperOrientationElement();
            XElement intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para","The following receive ports are associated with this BizTalk application."));
            XElement section = new XElement(xmlns + "inThisSection");
            List<XElement> paras = new List<XElement>();
            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;

                foreach (ReceivePort port in bce.Applications[appName].ReceivePorts)
                {
                    paras.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".ReceivePorts." + port.Name)))));
                    topics.Add(new ReceivePortTopic(appName,path,port.Name));
                }

                section.Add(new XText("This application contains the following receive ports:"),paras.ToArray());
                root.Add(intro, section);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                PrintLine("[ReceivePortsTopic] {0}: {1}\r\n{2}",ex.GetType(),ex.Message,ex.StackTrace);
            }
        }

        public XElement GetContentLayout()
        {
            List<XElement> t = new List<XElement>();
            foreach (ReceivePortTopic topic in topics)
            {
                t.Add(topic.GetContentLayout());
            }

            XElement xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Receive Ports"));

            xe.Add(t.ToArray());
            return xe;
        }

        public new void Save()
        {
            base.Save();

            foreach (ReceivePortTopic topic in topics)
            {
                topic.Save();
            }
        }
    }
}
