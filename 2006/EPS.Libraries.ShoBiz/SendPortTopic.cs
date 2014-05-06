using System;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{

    class SendPortTopic: TopicFile
    {
        private XElement root;
        private readonly string sendPortName;

        public SendPortTopic(string btsAppName, string btsBaseDir, string btsSendPortName)
        {
            tokenId = CleanAndPrep(btsAppName + ".SendPorts." + btsSendPortName);
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            appName = btsAppName;
            topicRelativePath = btsBaseDir;
            sendPortName = btsSendPortName;
        }


        void SaveTopic()
        {
            //var bce = CatalogExplorerFactory.CatalogExplorer();
            //bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
            var sp = CatalogExplorerFactory.CatalogExplorer().Applications[appName].SendPorts[sendPortName];
            root = CreateDeveloperConceptualElement();

            var intro = new XElement(xmlns + "introduction",
                    new XElement(xmlns + "para", new XText(string.IsNullOrEmpty(sp.Description)? "No description was available for this send port." : sp.Description )));
            var section = new XElement(xmlns + "sections", new XElement("section", new XElement(xmlns + "title",new XText("Send Port Properties")),
                    new XElement(xmlns + "content",
                        new XElement(xmlns + "table",
                            new XElement(xmlns + "tableHeader", 
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Property")),
                                    new XElement(xmlns + "entry", new XText("Value")))),
                                new XElement(xmlns + "row", 
                                    new XElement(xmlns + "entry", new XText("Custom Data")),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(sp.CustomData) ? "N/A":  sp.CustomData))),
                                new XElement(xmlns + "row", 
                                    new XElement(xmlns + "entry", new XText("Filter")),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(sp.Filter) ? "N/A" : sp.Filter))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Dynamic")),
                                    new XElement(xmlns + "entry", new XText(sp.IsDynamic.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("TwoWay")),
                                    new XElement(xmlns + "entry", new XText(sp.IsTwoWay.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Ordered Delivery")),
                                    new XElement(xmlns + "entry", new XText(sp.OrderedDelivery.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Priority")),
                                    new XElement(xmlns + "entry", new XText(sp.Priority.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Receive Pipeline"),
                                    GetPipelineEntry(sp.ReceivePipeline))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Receive Pipeline Data")),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(sp.ReceivePipelineData) ? "N/A": sp.ReceivePipelineData))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Send Pipeline")),
                                    GetPipelineEntry(sp.SendPipeline)),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Status")),
                                    new XElement(xmlns + "entry", new XText(sp.Status.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Stop Sending On Failure")),
                                    new XElement(xmlns + "entry", new XText(sp.StopSendingOnFailure.ToString()))),
                                new XElement(xmlns + "row", 
                                    new XElement(xmlns + "entry", new XText("Tracking")),
                                    new XElement(xmlns + "entry", new XText(sp.Tracking.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Inbound Transforms")),
                                    GetTransformEntry(sp.InboundTransforms)),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Outbound Transforms")),
                                    GetTransformEntry(sp.OutboundTransforms))
                                    ))),
                        GetTransportTypeElement(sp.PrimaryTransport,"Primary Transport Properties"),
                        GetTransportTypeElement(sp.SecondaryTransport,"Secondary Transport Properties"));
            root.Add(intro, section);
            if (doc.Root != null) doc.Root.Add(root);
        }

     private static XElement GetTransportTypeElement(TransportInfo ti, string title)
     {
         try
         {
             if (null == ti)
             {
                 return new XElement(xmlns + "section",
                                                   new XElement(xmlns + "title", new XText(title)),
                                                   new XElement(xmlns + "content",
                                                                new XElement(xmlns + "para",
                                                                             new XText(
                                                                                 "Transport information unavailable for this artifact."))));                 
             }
             var nullTrans = null == ti.TransportType;             
             var transport = new XElement(xmlns + "section",
                     new XElement(xmlns + "title", new XText(title)),
                     new XElement(xmlns + "content",
                         new XElement(xmlns + "table",
                             new XElement(xmlns + "tableHeader",
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Property")),
                                     new XElement(xmlns + "entry", new XText("Value")))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Address")),
                                     new XElement(xmlns + "entry", new XText(ti.Address ?? "N/A"))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Delivery Notification")),
                                     new XElement(xmlns + "entry", new XText(ti.DeliveryNotification.ToString()))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("From Time")),
                                     new XElement(xmlns + "entry", new XText(ti.FromTime.ToString()))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("To Time")),
                                     new XElement(xmlns + "entry", new XText(ti.ToTime.ToString()))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Service Window Enabled")),
                                     new XElement(xmlns + "entry", new XText(ti.ServiceWindowEnabled.ToString()))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Ordered Delivery")),
                                     new XElement(xmlns + "entry", new XText(ti.OrderedDelivery.ToString()))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Retry Count")),
                                     new XElement(xmlns + "entry", new XText(ti.RetryCount.ToString())),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Retry Interval")),
                                     new XElement(xmlns + "entry", new XText(ti.RetryInterval.ToString()))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Transport Type Name")),
                                     new XElement(xmlns + "entry", new XText(nullTrans ? "No Transport Type specified" : ti.TransportType.Name))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Transport Type Capabilities")),
                                     new XElement(xmlns + "entry", new XText(nullTrans ? "No Transport Type specified" : ti.TransportType.Capabilities.ToString()))),
                                 new XElement(xmlns + "row",
                                     new XElement(xmlns + "entry", new XText("Transport Type Data")),
                                     new XElement(xmlns + "entry", new XText(ti.TransportTypeData ?? "N/A")))))));
             return transport;
         }
         catch(Exception ex)
         {
             HandleException("GetTransportTypeElement", ex);
         }
         return null;
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
            return NewTopicEntry(sendPortName);
        }

    }
}
