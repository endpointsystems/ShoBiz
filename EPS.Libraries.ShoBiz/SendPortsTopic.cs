using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    public class SendPortsTopic: TopicFile, IDisposable
    {
        private readonly BackgroundWorker spWorker;
        private readonly List<SendPortTopic> topics;
        private readonly XElement root;


        public SendPortsTopic(string btsAppName, string btsBaseDir)
        {
            appName = btsAppName;
            path = btsBaseDir;
            tokenId = CleanAndPrep(btsAppName + ".SendPorts");
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            TimerStart();
            spWorker = new BackgroundWorker();
            spWorker.DoWork += spWorker_DoWork;
            spWorker.RunWorkerCompleted += spWorker_RunWorkerCompleted;
            topics = new List<SendPortTopic>();
            root = CreateDeveloperOrientationElement();
            spWorker.RunWorkerAsync();
        }

        public void Dispose()
        {
            if (spWorker != null) spWorker.Dispose();
        }

        void spWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock(this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        void spWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                XElement intro = new XElement(xmlns + "introduction",
                                              new XText(
                                                  "This section outlines the send ports associated with this application."));

                List<XElement> paras = new List<XElement>();

                foreach (SendPort sendPort in bce.Applications[appName].SendPorts)
                {
                    paras.Add(new XElement(xmlns + "para",
                                           new XElement(xmlns + "token",
                                                        new XText(CleanAndPrep(appName) + ".SendPorts." +
                                                                  CleanAndPrep(sendPort.Name)))));
                    topics.Add(new SendPortTopic(appName,path,sendPort.Name));
                }
                XElement section = new XElement(xmlns + "inThisSection",
                                                new XText("This application contains the following send ports:"), paras);
                root.Add(intro,section);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch (Exception ex)
            {

                PrintLine("[SendPortsTopic]{0}: {1}\r\n{2}", ex.GetType(), ex.Message, ex.StackTrace);
            }
            finally
            {
                bce.Dispose();
            }
        }

        public XElement GetContentLayout()
        {
            List<XElement> t = new List<XElement>();
            foreach (SendPortTopic topic in topics)
            {
                t.Add(topic.GetContentLayout());
            }

            XElement xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Send Ports"));

            xe.Add(t.ToArray());
            return xe;
            
        }

        public new void Save()
        {
            base.Save();
            foreach (SendPortTopic topic in topics)
            {
                topic.Save();
            }
        }
    }
}
