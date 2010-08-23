using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Sandcastle Orientation page leading to the orchestration topic files.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>token id: application name + ".Orchestrations"</item>
    ///  </list>
    /// </remarks>
    public class OrchestrationsTopic : TopicFile
    {
        private readonly List<OrchestrationTopic> topics;
        private readonly string[] orchNames;
        /// <summary>
        /// Creates a new instance of the <see cref="OrchestrationsTopic"/> class.
        /// </summary>
        /// <param name="topicPath">The base path of the Orchestrations documentation folder.</param>
        /// <param name="btsAppName">The BizTalk application name.</param>
        /// <param name="orchestrationNames">The <see cref="BtsOrchestration.FullName"/> of all of the orchestrations.</param>
        public OrchestrationsTopic(string topicPath, string btsAppName, string[] orchestrationNames)
        {
            try
            {
                topics = new List<OrchestrationTopic>();
                orchNames = orchestrationNames;
                topicRelativePath = topicPath;
                appName = btsAppName;
                tokenId = CleanAndPrep(appName + ".Orchestrations");
                //add token
                TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
                ReadyToSave = false;                
            }
            catch(Exception ex)
            {
                HandleException("OrchestrationsTopic constructor",ex);
            }
        }
        
        private void BuildTopic()
        {
            //var bce = new BtsCatalogExplorer();
            try
            {
                PrintLine("doing orchestration work");

                //build topic
                var root = CreateDeveloperOrientationElement();
                root.Add(new XElement(xmlns + "introduction",
                                      new XElement(xmlns + "para",
                                                   new XText(
                                                       string.Format(
                                                           "This section contains information about the orchestrations used in the {0} application.",
                                                           appName)))));
                
                var thisSection = new XElement(xmlns + "inThisSection");
                
                var elems = new List<XElement>();

                foreach ( var name in orchNames)
                {
                    var t = new OrchestrationTopic(topicRelativePath, appName, name);                    
                    topics.Add(t);

                    elems.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", 
                        new XText(CleanAndPrep(string.Format("{0}.Orchestrations.{1}",appName,name))))));
                }
                
                //add the orchestration links to the documentation
                thisSection.Add(new XText("This application contains the following orchestrations:"),
                                elems.ToArray());

                //attach section to the root
                root.Add(thisSection);
                
                //add to the document root
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                PrintLine("[OrchestrationsTopic.DoWork] {0}: {1}",ex.GetType(),ex.Message);
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
                                new XAttribute("title", "Orchestrations"));

            xe.Add(t.ToArray());
            return xe;
        }

        /// <summary>
        /// Save the topic.
        /// </summary>
        public override void Save()
        {
            TimerStart();
            BuildTopic();
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