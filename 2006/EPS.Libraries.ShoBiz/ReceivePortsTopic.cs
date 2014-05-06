using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    class ReceivePortsTopic: TopicFile
    {
        private readonly List<ReceivePortTopic> topics;
        private XElement root;
        private readonly string[] ports;

        public ReceivePortsTopic(string btsAppName, string topicPath, string[] receivePorts)
        {
            ports = receivePorts;
            tokenId = CleanAndPrep(btsAppName + ".ReceivePorts");
            topicRelativePath = topicPath;
            appName = btsAppName;
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            topics = new List<ReceivePortTopic>();
        }

        void SaveTopic()
        {
            root = CreateDeveloperOrientationElement();
            var intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para","The following receive ports are associated with this BizTalk application."));
            var section = new XElement(xmlns + "inThisSection");
            var paras = new List<XElement>();
            try
            {
                foreach (var name in ports)
                {
                    paras.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".ReceivePorts." + name)))));
                    topics.Add(new ReceivePortTopic(appName,topicRelativePath,name));
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
            var t = new List<XElement>();
            foreach (var topic in topics)
            {
                t.Add(topic.GetContentLayout());
            }

            var xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", "Receive Ports"));

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
//            loopResult = Parallel.ForEach(topics, SaveAllTopics);
            base.Save();
            TimerStop();            
        }
    }
}
