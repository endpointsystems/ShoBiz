using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;
using System.Xml.Linq;
using Policy=Microsoft.BizTalk.ExplorerOM.Policy;

namespace EndpointSystems.BizTalk.Documentation
{
    public class BusinessRulesTopic: TopicFile, IDisposable
    {
        private readonly BackgroundWorker brWorker;
        private readonly List<BusinessRuleTopic> topics;
        private readonly string rulesDb;
        public BusinessRulesTopic(string btsAppName, string baseDir, string btsRulesDatabase)
        {
            brWorker = new BackgroundWorker();
            appName = btsAppName;
            path = baseDir;
            topics = new List<BusinessRuleTopic>();
            rulesDb = btsRulesDatabase;
            tokenId = appName + ".BusinessRules";
            TimerStart();
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            brWorker.DoWork += brWorker_DoWork;
            brWorker.RunWorkerCompleted += brWorker_RunWorkerCompleted;
            brWorker.RunWorkerAsync();
        }

        void brWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (topics.Count > 0)
            {
                foreach (BusinessRuleTopic topic in topics)
                {
                    do
                    {
                        Thread.Sleep(100);
                    } while (!topic.ReadyToSave);
                }
            }
            lock(this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        void brWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                XElement root = CreateDeveloperOrientationElement();

                XElement intro = new XElement(xmlns + "introduction", 
                    new XText("This section documents the business rule policies for the "), 
                    new XElement(xmlns + "token",new XText(CleanAndPrep(appName))),
                    new XText(" application."));

                List<XElement> paras = new List<XElement>();
                PolicyCollection pc = bce.Applications[appName].Policies;
                
                if (pc == null)
                {
                    intro = new XElement(xmlns + "introduction",
                        new XText("This application contains no policies."));
                    
                    if (doc.Root != null) doc.Root.Add(intro);
                    ReadyToSave = true;
                    return;
                }

                foreach (Policy policy in pc)
                {
                    paras.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Policies." + policy.Name)))));
                    topics.Add(new BusinessRuleTopic(appName,path,policy.Name,rulesDb));
                }

                XElement inThis = new XElement(xmlns + "inThisSection",
                    new XText("This application contains the following policies:"),paras.ToArray());
                
                root.Add(intro, inThis);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                HandleException("BusinessRulesTopic.DoWork",ex);
            }
            finally
            {
                bce.Dispose();
            }            
        }

        public XElement GetContentLayout()
        {
            List<XElement> t = new List<XElement>();
            foreach (BusinessRuleTopic topic in topics)
            {
                t.Add(topic.GetContentLayout());
            }

            XElement xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Policies"));

            xe.Add(t.ToArray());
            return xe;
        }

        public void Dispose()
        {
            brWorker.Dispose();
            foreach (BusinessRuleTopic topic in topics)
            {
                topic.Dispose();
            }
        }

        public new void Save()
        {
            base.Save();
            foreach (BusinessRuleTopic topic in topics)
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
