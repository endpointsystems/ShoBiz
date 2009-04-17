using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.ComponentModel;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    public class ReceivePortTopic: TopicFile, IDisposable
    {
        private readonly string rpName;
        private readonly BackgroundWorker rpWorker;
        public ReceivePortTopic(string btsAppName, string btsBaseDir, string btsReceivePortName)
        {
            rpName = btsReceivePortName;
            appName = btsAppName;
            tokenId = btsAppName + ".ReceivePorts." + btsReceivePortName;
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            TimerStart();
            path = btsBaseDir;
            rpWorker = new BackgroundWorker();
            rpWorker.DoWork += rpWorker_DoWork;
            rpWorker.RunWorkerCompleted += rpWorker_RunWorkerCompleted;
            rpWorker.RunWorkerAsync();
        }

        public void Dispose()
        {
            rpWorker.Dispose();
        }

        void rpWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock(this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        void rpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                XElement root = CreateDeveloperConceptualElement();
                ReceivePort rp = bce.Applications[appName].ReceivePorts[rpName];

                XElement intro = new XElement(xmlns + "introduction",new XElement(xmlns + "para", string.IsNullOrEmpty(rp.Description) ? new XText("No description was available for this receive location.") : new XText(rp.Description)));
                
                XElement section = new XElement(xmlns + "section",  new XElement(xmlns + "title",new XText(rp.Name + " Properties")),
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
                List<XElement> inTrans = new List<XElement>();
                List<XElement> outTrans = new List<XElement>();

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
                    XElement mapsIn = new XElement(xmlns + "section",
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
                    XElement mapsOut = new XElement(xmlns + "section",
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
            finally
            {
                bce.Dispose();                
            }
        }

        public XElement GetContentLayout()
        {
            return GetContentLayout(rpName);
        }
    }
}
