using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    class ReceiveLocationsTopic: TopicFile
    {

        /*
         * 
         *  jagged array sample
            byte[][] scores = new byte[5][];
            for (int x = 0; x < scores.Length; x++) 
            {
               scores[x] = new byte[4];
            }
         */
        private List<ReceiveLocationTopic> topics;
        private readonly XElement root;
        private readonly string[][] names;
        public ReceiveLocationsTopic(string btsAppName, string basePath, string[][] receiveLocations)
        {
            names = receiveLocations;
            appName = btsAppName;
            topicRelativePath = basePath;
            tokenId = CleanAndPrep(btsAppName + ".ReceiveLocations");
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            root = CreateDeveloperOrientationElement();

        }

        void SaveTopic()
        {
            topics = new List<ReceiveLocationTopic>();
            var intro = new XElement(xmlns + "introduction", new XElement("para", new XText("This section outlines the properties for the receive locations for the selected BizTalk application.")));

            var paras = new List<XElement>();
            for (var i = 0; i < names.Length; i++)
            {
                for (var j = 0; j < names[i].Length; j++)
                {
                    topics.Add(new ReceiveLocationTopic(appName, topicRelativePath, names[i][j]));
                    paras.Add(new XElement(xmlns + "para",
                                           new XElement(xmlns + "token",
                                                        new XText(CleanAndPrep(appName + ".ReceiveLocations." + names[i] + names[i][j])))));
                }
                
            }
            
            var inThis = new XElement(xmlns + "inThisSection", new XText("This application contains the following receive locations:"),paras.ToArray());
            root.Add(intro, inThis);
            if (doc.Root != null) doc.Root.Add(root);
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
                                new XAttribute("title", "Receive Locations"));

            xe.Add(t.ToArray());
            return xe;
        }

        public override void Save()
        {
            TimerStart();
            SaveTopic();
            //loopResult = Parallel.ForEach(topics, SaveAllTopics);
            foreach (var topic in topics)
            {
                topic.Save();
            }
            base.Save();
            TimerStop();
        }
    }
}
