using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    class BusinessRulesTopic: TopicFile
    {
        private readonly List<BusinessRuleTopic> topics;
        private readonly string[] policies;

        public BusinessRulesTopic(string btsAppName, string baseDir, string[] rules)
        {
            policies = rules;
            appName = btsAppName;
            topicRelativePath = baseDir;
            topics = new List<BusinessRuleTopic>();
            tokenId = appName + ".BusinessRules";
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
        }

        void SaveTopic()
        {
            try
            {
                var root = CreateDeveloperOrientationElement();

                var intro = new XElement(xmlns + "introduction", 
                    new XText("This section documents the business rule policies for the "), 
                    new XElement(xmlns + "token",new XText(CleanAndPrep(appName))),
                    new XText(" application."));

                var paras = new List<XElement>();
                
                if (policies == null)
                {
                    intro = new XElement(xmlns + "introduction",
                        new XText("This application contains no policies."));
                    
                    if (doc.Root != null) doc.Root.Add(intro);
                    ReadyToSave = true;
                    return;
                }

                foreach (var policy in policies)
                {
                    paras.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Policies." + policy)))));
                    topics.Add(new BusinessRuleTopic(appName,topicRelativePath,policy));
                }

                var inThis = new XElement(xmlns + "inThisSection",
                    new XText("This application contains the following policies:"),paras.ToArray());
                
                root.Add(intro, inThis);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                HandleException("BusinessRulesTopic.DoWork",ex);
            }
        }

        public XElement GetContentLayout()
        {
            var t = new List<XElement>();
            foreach (var topic in topics)
            {
                t.Add(topic.GetContentLayout());
            }

            var xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Policies"));

            xe.Add(t.ToArray());
            return xe;
        }

        public override void Save()
        {
            TimerStart();
            SaveTopic();
            foreach (var topic in topics)
            {
                topic.Save();
            }
            //loopResult = Parallel.ForEach(topics, SaveAllTopics);
            base.Save();
            TimerStop();
        }
    }
}
