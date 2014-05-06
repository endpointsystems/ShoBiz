using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    class TransformsTopic : TopicFile
    {
        private readonly List<TransformTopic> topics;
        private XElement root;
        private readonly string[] maps;

        /// <summary>
        /// Creates a new Sandcastle document for a BizTalk map artifact (transform).
        /// </summary>
        /// <param name="btsAppName">The BizTalk application name.</param>
        /// <param name="topicPath">The path to save the topic.</param>
        /// <param name="transforms">The full name of the BizTalk transforms to save.</param>
        public TransformsTopic(string btsAppName, string topicPath, string[] transforms)
        {
            maps = transforms;
            topicRelativePath = topicPath;
            appName = btsAppName;
            tokenId = CleanAndPrep(btsAppName + ".Transforms");
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            topics = new List<TransformTopic>();
            UsingParallel = true;
        }

        void SaveTopic()
        {
            try
            {
                var elems = new List<XElement>();
                root = CreateDeveloperOrientationElement();
                var intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText("This section outlines the maps contained in this BizTalk application.")));
                foreach (var name in maps)
                {
                    elems.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Transforms." + name)))));
                    PrintLine("creating new TransformTopic for {0}", name);
                    topics.Add(new TransformTopic(appName, topicRelativePath, name));
                }

                var inThis = new XElement(xmlns + "inThisSection", new XText("This application contains the following transforms:"));

                inThis.Add(elems.ToArray());
                root.Add(intro, inThis);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch (Exception ex)
            {
                HandleException("TransformsTopic.DoWork", ex);
            }
        }

        /// <summary>
        /// Get the Sandcastle content layout for the topic.
        /// </summary>
        /// <returns>An <see cref="XElement"/> containing the content layout information.</returns>
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
                                new XAttribute("title", "Transforms"));

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
