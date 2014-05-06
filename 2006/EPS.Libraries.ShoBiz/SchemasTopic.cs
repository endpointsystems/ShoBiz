using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    class SchemasTopic: TopicFile
    {
        private readonly List<SchemaTopic> topics;
        private XElement root;
        private readonly string[] schemas;
        
        /// <summary>
        /// Create a new Sandcastle schemas topic.
        /// </summary>
        /// <param name="btsAppName">The BizTalk application name.</param>
        /// <param name="topicPath">The path to save the topic in.</param>
        /// <param name="schemaNames">A list of the full names of schemas to generate.</param>
        public SchemasTopic(string btsAppName, string topicPath, string[] schemaNames)
        {
            topicRelativePath = topicPath;
            appName = btsAppName;
            schemas = schemaNames;
            tokenId = CleanAndPrep(btsAppName + ".Schemas");
            TokenFile.GetTokenFile().AddTopicToken(tokenId,id);            
            topics = new List<SchemaTopic>();
        }

        void SaveTopic()
        {
            try
            {
                var elems = new List<XElement>();

                root = CreateDeveloperOrientationElement();
                var intro = new XElement(xmlns + "introduction",new XElement(xmlns + "para", new XText("This section outlines the XML schemas contained in the BizTalk application.")));
                foreach (var name in schemas)
                {
                    elems.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Schemas." + name)))));
                    topics.Add(new SchemaTopic(appName,topicRelativePath, name));
                }
                
                var inThis = new XElement(xmlns + "inThisSection", new XText("This application contains the following schemas:"));
                
                inThis.Add(elems.ToArray());
                root.Add(intro, inThis);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                HandleException("SchemasTopic.DoWork",ex);
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
                                new XAttribute("title", "Schemas"));

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
            base.Save();
            TimerStop();
        }
    }
}
