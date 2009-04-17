using System;
using System.ComponentModel;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    public class ReceiveLocationTopic: TopicFile, IDisposable
    {
        private readonly BackgroundWorker rlWorker;
        private readonly string recLocName;
        private XElement root;
        public ReceiveLocationTopic(string btsAppName, string basePath, string btsRelName)
        {
            ///HACK: assign 'temporary' token id for timer
            tokenId = CleanAndPrep(appName + ".ReceiveLocations." + btsRelName);
            TimerStart();
            appName = btsAppName;
            rlWorker = new BackgroundWorker();
            path = basePath;
            recLocName = btsRelName;
            rlWorker.DoWork += rlWorker_DoWork;
            rlWorker.RunWorkerCompleted += rlWorker_RunWorkerCompleted;
            rlWorker.RunWorkerAsync();
        }

        public void Dispose()
        {
            rlWorker.Dispose();
        }
        void rlWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock(this)
            {
                ReadyToSave = true;
            }
            ///HACK: set token back for timer consistency
            tokenId = CleanAndPrep(appName + ".ReceiveLocations." + recLocName);
            TimerStop();
        }

        void rlWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();

            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                ReceiveLocation rl = null;
                foreach (ReceivePort port in bce.Applications[appName].ReceivePorts)
                {
                    foreach (ReceiveLocation rloc in port.ReceiveLocations)
                    {
                        if (rloc.Name.Equals(recLocName))
                        {
                            rl = rloc;
                            break;
                        }                            
                    }
                }

                root = CreateDeveloperConceptualElement();
                if (null == rl) return;

                tokenId = CleanAndPrep(appName + ".ReceiveLocations." + rl.ReceivePort.Name + rl.Name);
                TokenFile.GetTokenFile().AddTopicToken(tokenId, id);

                XElement intro = new XElement(xmlns + "introduction", 
                    new XElement(xmlns + "para", new XText(string.IsNullOrEmpty(rl.Description) ? "No description was available for this receive location." : rl.Description)));

                XElement section = new XElement(xmlns + "section",
                    new XElement(xmlns + "title", new XText("Receive Location Properties")),
                    new XElement(xmlns + "content", 
                        new XElement(xmlns + "table",
                            new XElement(xmlns + "tableHeader",
                                new XElement(xmlns + "row", 
                                    new XElement(xmlns + "entry", new XText("Property")),
                                    new XElement(xmlns + "entry", new XText("Value")))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Address")),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(rl.Address) ? "N/A" : rl.Address))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Custom Data")),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(rl.CustomData) ? "N/A" : rl.CustomData))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Enabled")),
                                    new XElement(xmlns + "entry", new XText(rl.Enable.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("End Date")),
                                    new XElement(xmlns + "entry", new XText(rl.EndDate.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("End Date Enabled")),
                                    new XElement(xmlns + "entry", new XText(rl.EndDateEnabled.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Fragment Messages")),
                                    new XElement(xmlns + "entry", new XText(rl.FragmentMessages.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("From Time")),
                                    new XElement(xmlns + "entry", new XText(rl.FromTime.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Public Address")),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(rl.PublicAddress) ? "N/A" : rl.PublicAddress))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Receive Handler Transport")),
                                    new XElement(xmlns + "entry", new XText( null == rl.ReceiveHandler ? "N/A" : rl.ReceiveHandler.TransportType.Name))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Receive Pipeline")),
                                    GetPipelineEntry(rl.ReceivePipeline)),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Receive Pipeline Data") ),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(rl.ReceivePipelineData) ? "N/A" : rl.ReceivePipelineData))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Receive Port")),
                                    new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".ReceivePorts." + rl.ReceivePort.Name))))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Send Pipeline")),
                                    GetPipelineEntry(rl.SendPipeline)),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Send Pipeline Data") ),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(rl.SendPipelineData) ? "N/A" : rl.SendPipelineData))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Service Window Enabled") ),
                                    new XElement(xmlns + "entry", new XText(rl.ServiceWindowEnabled.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Start Date") ),
                                    new XElement(xmlns + "entry", new XText(rl.StartDate.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Start Date Enabled") ),
                                    new XElement(xmlns + "entry", new XText(rl.StartDateEnabled.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Transport Type Name")),
                                    new XElement(xmlns + "entry", new XText(null == rl.TransportType ? "N/A" : rl.TransportType.Name))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Transport Type Capabilities") ),
                                    new XElement(xmlns + "entry", new XText(null == rl.TransportType ? "N/A" : rl.TransportType.Capabilities.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Transport Type Data")),
                                    new XElement(xmlns + "entry", new XText(string.IsNullOrEmpty(rl.TransportTypeData) ? "N/A" : rl.TransportTypeData)))
                                )));

                root.Add(intro, section);
                if (doc.Root != null) doc.Root.Add(root);
            }
        catch (Exception ex)
        {
            HandleException("ReceiveLocationTopic.DoWork", ex);
        }
        finally
        {
            bce.Dispose();
        }
    }

        public XElement GetContentLayout()
        {
            return GetContentLayout(recLocName);
        }
    }
}
