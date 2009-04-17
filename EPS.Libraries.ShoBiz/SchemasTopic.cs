using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    class SchemasTopic: TopicFile, IDisposable
    {
        private readonly BackgroundWorker schsWorker;
        private readonly List<SchemaTopic> topics;
        private XElement root;
        public SchemasTopic(string btsAppName, string basePath)
        {
            path = basePath;
            appName = btsAppName;
            tokenId = CleanAndPrep(btsAppName + ".Schemas");
            TokenFile.GetTokenFile().AddTopicToken(tokenId,id);
            TimerStart();
            topics = new List<SchemaTopic>();
            schsWorker = new BackgroundWorker();
            schsWorker.DoWork += schWorker_DoWork;
            schsWorker.RunWorkerCompleted += schWorker_RunWorkerCompleted;
            schsWorker.RunWorkerAsync();
        }

        public void Dispose()
        {
            foreach (SchemaTopic topic in topics)
            {
                topic.Dispose();
            }
            schsWorker.Dispose();
            
        }

        void schWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (SchemaTopic topic in topics)
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

        void schWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                List<XElement> elems = new List<XElement>();
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                root = CreateDeveloperOrientationElement();
                XElement intro = new XElement(xmlns + "introduction",new XElement(xmlns + "para", new XText("This section outlines the XML schemas contained in the BizTalk application.")));
                foreach (Schema schema in bce.Applications[appName].Schemas)
                {
                    elems.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Schemas." + schema.FullName)))));
                    topics.Add(new SchemaTopic(appName,path,schema.FullName));
                }
                
                XElement inThis = new XElement(xmlns + "inThisSection", new XText("This application contains the following schemas:"));
                
                inThis.Add(elems.ToArray());
                root.Add(intro, inThis);
                PrintLine("schemas topic: {0}",root.ToString(SaveOptions.None));
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                HandleException("SchemasTopic.DoWork",ex);
            }
            finally
            {
                bce.Dispose();
            }
        }

        public XElement GetContentLayout()
        {
            List<XElement> t = new List<XElement>();
            foreach (SchemaTopic topic in topics)
            {
                t.Add(topic.GetContentLayout());
            }

            XElement xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Schemas"));

            xe.Add(t.ToArray());
            return xe;
        }

        public new void Save()
        {
            base.Save();
            foreach (SchemaTopic topic in topics)
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
