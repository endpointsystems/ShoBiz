using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    class SendPortsTopic: TopicFile
    {
        private readonly List<SendPortTopic> topics;
        private readonly XElement root;
        private readonly string[] sendPorts;

        public SendPortsTopic(string btsAppName, string btsBaseDir, string[] sendPortNames)
        {
            sendPorts = sendPortNames;
            appName = btsAppName;
            topicRelativePath = btsBaseDir;
            tokenId = CleanAndPrep(btsAppName + ".SendPorts");
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            topics = new List<SendPortTopic>();
            root = CreateDeveloperOrientationElement();
        }

        void SaveTopic()
        {
            try
            {
                var intro = new XElement(xmlns + "introduction",
                                              new XText(
                                                  "This section outlines the send ports associated with this application."));

                var paras = new List<XElement>();

                foreach (var name in sendPorts)
                {
                    paras.Add(new XElement(xmlns + "para",
                                           new XElement(xmlns + "token",
                                                        new XText(CleanAndPrep(appName) + ".SendPorts." +
                                                                  CleanAndPrep(name)))));
                    topics.Add(new SendPortTopic(appName,topicRelativePath,name));
                }
                
                var section = new XElement(xmlns + "inThisSection",
                                                new XText("This application contains the following send ports:"), paras);
                root.Add(intro,section);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch (Exception ex)
            {
                PrintLine("[SendPortsTopic]{0}: {1}\r\n{2}", ex.GetType(), ex.Message, ex.StackTrace);
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
                                new XAttribute("title", "Send Ports"));

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
