using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    class ReceivePortTopic: TopicFile
    {
        private readonly string rpName;
        public ReceivePortTopic(string btsAppName, string btsBaseDir, string btsReceivePortName)
        {
            rpName = btsReceivePortName;
            appName = btsAppName;
            tokenId = btsAppName + ".ReceivePorts." + btsReceivePortName;
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            topicRelativePath = btsBaseDir;
        }



        void SaveTopic()
        {
            //var bce = CatalogExplorerFactory.CatalogExplorer();
            try
            {
                //bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                var root = CreateDeveloperConceptualElement();
                var rp = CatalogExplorerFactory.CatalogExplorer().Applications[appName].ReceivePorts[rpName];

                var intro = new XElement(xmlns + "introduction",new XElement(xmlns + "para", string.IsNullOrEmpty(rp.Description) ? new XText("No description was available for this receive location.") : new XText(rp.Description)));
                
                var section = new XElement(xmlns + "section",  new XElement(xmlns + "title",new XText(rp.Name + " Properties")),
                                                                    new XElement(xmlns + "content",
                                                                        new XElement(xmlns + "table", 
                                                                            new XElement(xmlns + "tableHeader", 
                                                                                new XElement(xmlns + "row",
                                                                                    new XElement(xmlns + "entry",new XText("Property"),
                                                                                    new XElement(xmlns + "entry", new XText("Value"))))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry",new XText("Application")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(rp.Application.Name))))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Authentication")),
                                                                                new XElement(xmlns + "entry", new XText(rp.Authentication.ToString()))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Custom Data    ")),
                                                                                new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(rp.CustomData) ? "N/A" : rp.CustomData ))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Primary Receive Location")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(appName + ".ReceiveLocations." + rp.PrimaryReceiveLocation.Name)))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Is Two Way?")),
                                                                                new XElement(xmlns + "entry", new XText(rp.IsTwoWay.ToString()))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Route Failed Message?")),
                                                                                new XElement(xmlns + "entry", new XText(rp.RouteFailedMessage.ToString()))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Send Pipeline")),
                                                                                GetPipelineEntry(rp.SendPipeline)),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Tracking")),
                                                                                new XElement(xmlns + "entry", new XText(rp.Tracking.ToString()))))));
                root.Add(intro, section);
                var inTrans = new List<XElement>();
                var outTrans = new List<XElement>();

                if (null != rp.InboundTransforms)
                {
                    foreach (Transform map in rp.InboundTransforms)
                    {
                        inTrans.Add(new XElement(xmlns + "listItem", new XElement(xmlns + "token", new XText(CleanAndPrep(map.Application.Name) + ".Transforms." + map.FullName))));
                    }
                }

                if (null != rp.OutboundTransforms)
                {
                    foreach (Transform map in rp.OutboundTransforms)
                    {
                        outTrans.Add(new XElement(xmlns + "listItem", new XElement(xmlns + "token", new XText(CleanAndPrep(map.Application.Name) + ".Transforms." + map.FullName))));
                    }
                }

                if (inTrans.Count > 0)
                {
                    var mapsIn = new XElement(xmlns + "section",
                                                   new XElement(xmlns + "title", new XText("Inbound Transforms")),
                                                   new XElement(xmlns + "content",
                                                                new XElement(xmlns + "para",
                                                                             new XText(
                                                                                 "The following inbound transforms are associated with this receive port:")),
                                                                new XElement(xmlns + "list", inTrans.ToArray())));
                    root.Add(mapsIn);
                }
                if (outTrans.Count > 0)
                {
                    var mapsOut = new XElement(xmlns + "section",
                                                    new XElement(xmlns + "title", new XText("Outbound Transforms")),
                                                    new XElement(xmlns + "content",
                                                                 new XElement(xmlns + "para",
                                                                              new XText(
                                                                                  "The following outbound transforms are associated with this receive port:")),
                                                                 new XElement(xmlns + "list", inTrans.ToArray())));
                    root.Add(mapsOut);
                }

                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                HandleException("ReceivePortTopic.DoWork",ex);
            }
        }

        public override void Save()
        {
            TimerStart();
            SaveTopic();
            base.Save();
            TimerStop();            
        }

        public XElement GetContentLayout()
        {
            return NewTopicEntry(rpName);
        }
    }
}
