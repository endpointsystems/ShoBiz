using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    class TransformsTopic : TopicFile, IDisposable
    {
        private readonly BackgroundWorker transWorker;
        private readonly List<TransformTopic> topics;
        private XElement root;
        public TransformsTopic(string btsAppName, string basePath)
        {
            path = basePath;
            appName = btsAppName;
            tokenId = CleanAndPrep(btsAppName + ".Transforms");
            TimerStart();
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            topics = new List<TransformTopic>();
            transWorker = new BackgroundWorker();
            transWorker.DoWork += schWorker_DoWork;
            transWorker.RunWorkerCompleted += schWorker_RunWorkerCompleted;
            transWorker.RunWorkerAsync();
        }

        public void Dispose()
        {
            if (null != topics)
            {
                foreach (TransformTopic topic in topics)
                {
                    topic.Dispose();
                }
            }
            if (null != transWorker) transWorker.Dispose();
        }

        void schWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (TransformTopic topic in topics)
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
            TimerStop();
        }

        void schWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                List<XElement> elems = new List<XElement>();
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                root = CreateDeveloperOrientationElement();
                XElement intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText("This section outlines the maps contained in this BizTalk application.")));
                foreach (Transform trans in bce.Applications[appName].Transforms)
                {
                    elems.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Transforms." + trans.FullName)))));
                    topics.Add(new TransformTopic(appName, path, trans.FullName));
                }

                XElement inThis = new XElement(xmlns + "inThisSection", new XText("This application contains the following transforms:"));

                inThis.Add(elems.ToArray());
                root.Add(intro, inThis);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch (Exception ex)
            {
                HandleException("TransformsTopic.DoWork", ex);
            }
            finally
            {
                bce.Dispose();
            }
        }

        public XElement GetContentLayout()
        {
            List<XElement> t = new List<XElement>();
            foreach (TransformTopic topic in topics)
            {
                t.Add(topic.GetContentLayout());
            }

            XElement xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Transforms"));

            xe.Add(t.ToArray());
            return xe;
        }

        public new void Save()
        {
            base.Save();
            foreach (TransformTopic topic in topics)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!topic.ReadyToSave);
                topic.Save();
            }
        }
    }
}
