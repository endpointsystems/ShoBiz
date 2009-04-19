using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// BizTalk Application documentation topic.
    /// </summary>
    /// <remarks>
    /// Token id: application name (no spaces)
    ///     <list type="bullet">
    ///         <listheader>BizTalk Admin Console Sequence:</listheader>
    ///         <item>Orchestrations</item>
    ///         <item>Role Links</item>
    ///         <item>Send Port Groups</item>
    ///         <item>Send Ports</item>
    ///         <item>Receive Ports</item>
    ///         <item>Receive Locations</item>
    ///         <item>Policies</item>
    ///         <item>Schemas</item>
    ///         <item>Maps</item>
    ///         <item>Pipelines</item>
    ///         <item>Resources</item>
    ///         <item>BAS Artifacts</item>
    ///     </list>
    /// </remarks>
    public class AppTopic : TopicFile
    {
        private readonly BackgroundWorker appWorker;
        private readonly string imgPath;
        private OrchestrationsTopic orchsTopic;
        private ReceivePortsTopic rpsTopic;
        private SendPortsTopic spsTopic;
        private ReceiveLocationsTopic rlsTopic;
        private SchemasTopic schTopic;
        private AssembliesTopic assyTopic;
        private TransformsTopic transTopic;
        private BusinessRulesTopic bsTopic;
        private readonly string rulesDb;
        public AppTopic(string basePath,string btsImgPath, string btsAppName, string rulesDatabaseName)
        {
            rulesDb = rulesDatabaseName;
            appName = btsAppName;
            path = basePath + appName + @"\";
            imgPath = btsImgPath;
            buildTree();
            tokenId = CleanAndPrep(btsAppName);
            TimerStart();
            TokenFile.GetTokenFile().AddTopicToken(CleanAndPrep(tokenId), id);
            appWorker = new BackgroundWorker();
            appWorker.DoWork += appWorker_DoWork;
            appWorker.RunWorkerCompleted += appWorker_RunWorkerCompleted;
            appWorker.RunWorkerAsync();
        }
        /// <summary>
        /// Build the directory tree for the app.
        /// </summary>
        private void buildTree()
        {
            addFolder(path);
            addFolder(path + @"Orchestrations\");
            addFolder(path + @"Send Ports\");
            addFolder(path + @"Receive Ports\");
            addFolder(path + @"Receive Locations\");
            addFolder(path + @"Policies\");
            addFolder(path + @"Schemas\");
            addFolder(path + @"Maps\");
            addFolder(path + @"Pipelines\");
            addFolder(path + @"Resources\");
        }

        private void appWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (orchsTopic != null)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!orchsTopic.ReadyToSave);
            }

            if (spsTopic != null)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!spsTopic.ReadyToSave);
            }

            if (rpsTopic != null)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!rpsTopic.ReadyToSave);
            }

            if (rlsTopic != null)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!rlsTopic.ReadyToSave);
            }

            if (bsTopic != null)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!bsTopic.ReadyToSave);
            }

            if (schTopic != null)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!schTopic.ReadyToSave);
            }

            lock (this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        private void appWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
            Application app = bce.Applications[appName];
            XElement root = CreateDeveloperOrientationElement();
            XElement intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText(string.IsNullOrEmpty(app.Description) ? "No description was given." : app.Description)));
            XElement inThis = new XElement(xmlns + "inThisSection", new XText("This application contains the BizTalk artifacts listed below."));
            #region orchestrations
            try
            {
                if (null != app.Orchestrations)
                {
                    PrintLine("{0} has {1} orchestrations", appName, app.Orchestrations.Count);
                    if (app.Orchestrations.Count > 0)
                    {
                        inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Orchestrations:"))));
                        foreach (BtsOrchestration orch in app.Orchestrations)
                        {
                            inThis.Add(new XElement(xmlns + "para",
                                                    new XElement(xmlns + "token",
                                                                 new XText(CleanAndPrep(appName + ".Orchestrations." + orch.FullName)))));
                        }
                        orchsTopic = new OrchestrationsTopic(path + @"Orchestrations\", imgPath, appName);
                    }
                }
            }
            catch(Exception ex)
            {
                HandleException("BtsAppTopic.DoWork(orchestrations)",ex);
            }
            #endregion

            #region send ports

            try
            {
                if (app.SendPorts != null)
                {
                    PrintLine("send ports count: {0}", app.SendPorts.Count);
                    if (app.SendPorts.Count > 0)
                    {
                        inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Send Ports:"))));
                        foreach (SendPort port in app.SendPorts)
                        {
                            inThis.Add(new XElement(xmlns + "para",
                                                    new XElement(xmlns + "token",
                                                                 new XText(CleanAndPrep(appName + ".SendPorts." + port.Name)))));
                        }

                        spsTopic = new SendPortsTopic(appName, path + @"Send Ports\");
                    }
                }
            }
            catch(Exception ex)
            {
                HandleException("BtsAppTopic.DoWork(send ports)", ex);
            }

            #endregion

            #region receive ports
            if (app.ReceivePorts != null)
            {
                if (app.ReceivePorts.Count > 0)
                {
                    PrintLine("{0} has {1} receive ports", appName, app.ReceivePorts.Count);
                    XElement rlElement = new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Receive Locations:")));
                    List<XElement> recLocs = new List<XElement>();

                    XElement rpElement = new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Receive Ports:")));
                    foreach (ReceivePort port in app.ReceivePorts)
                    {
                        PrintLine("{0} has {1} receive locations");
                        rpElement.Add(new XElement(xmlns + "para",
                                                new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".ReceivePorts." + port.Name)))));
                        if (port.ReceiveLocations.Count > 0)
                        {
                            foreach (ReceiveLocation rl in port.ReceiveLocations)
                            {
                                recLocs.Add(new XElement(xmlns + "para",
                                                         new XElement(xmlns + "token",
                                                                      new XText(CleanAndPrep(appName + ".ReceiveLocations." + rl.ReceivePort.Name + rl.Name)))));
                            }
                            if (null == rlsTopic)
                            {
                                rlsTopic = new ReceiveLocationsTopic(appName, path + @"Receive Locations\");
                            }
                        }
                        rpsTopic = new ReceivePortsTopic(appName, path + @"Receive Ports\");
                    }

                    inThis.Add(rpElement);

                    if (recLocs.Count > 0)
                    {
                        rlElement.Add(recLocs.ToArray());
                        inThis.Add(rlElement);
                    }
                }
            }

            #endregion

            #region policies

            if (null != app.Policies)
            {
                if (app.Policies.Count > 0)
                {
                    PrintLine("{0} has {1} policies", appName, app.Policies.Count);
                    bsTopic = new BusinessRulesTopic(appName, path + @"Policies\", rulesDb);
                    inThis.Add(new XElement(xmlns + "legacyBold", new XText("Policies:")));
                    foreach (Policy policy in app.Policies)
                    {
                        inThis.Add(new XElement(xmlns + "para",
                                                new XElement(xmlns + "token",
                                                             new XText(
                                                                 CleanAndPrep(appName + ".Policies." + policy.Name)))));
                    }

                }
            }

            #endregion

            #region schemas
            if (null != app.Schemas)
            {
                if (app.Schemas.Count > 0)
                {
                    PrintLine("{0} has {1} schemas", appName, app.Schemas.Count);
                    schTopic = new SchemasTopic(appName, path + @"Schemas\");
                    inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Schemas:"))));

                    foreach (Schema schema in app.Schemas)
                    {
                        inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Schemas." + schema.FullName)))));
                    }
                }
            }

            #endregion

            #region transforms
            if (null != app.Transforms)
            {
                if (app.Transforms.Count > 0)
                {
                    PrintLine("{0} has {1} transforms", appName, app.Transforms.Count);
                    inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Transforms:"))));
                    foreach (Transform transform in app.Transforms)
                    {
                        inThis.Add(new XElement(xmlns + "para",
                                                new XElement(xmlns + "token",
                                                             new XText(CleanAndPrep(appName + ".Transforms." + transform.FullName)))));
                    }

                    transTopic = new TransformsTopic(appName, path + @"Maps\");
                }
                
            }
            #endregion

            #region pipelines
            if (null != app.Pipelines)
            {
                PrintLine("{0} has {1} pipelines", appName, app.Pipelines.Count);
                if (app.Pipelines.Count > 0)
                {
                    inThis.Add(new XElement(xmlns + "para", new XText("Pipelines:")));
                    foreach (Pipeline pipeline in app.Pipelines)
                    {
                        inThis.Add(new XElement(xmlns + "para",
                                                new XElement(xmlns + "token",
                                                             new XText(CleanAndPrep(appName + ".Pipelines." + pipeline.FullName)))));
                    }
                }
            }

            #endregion

            #region assemblies
            if (null != app.Assemblies)
            {
                if (app.Assemblies.Count > 0)
                {
                    PrintLine("{0} has {1} assemblies", appName, app.Assemblies.Count);
                    assyTopic = new AssembliesTopic(path + @"Resources\", appName);
                    inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Resources:"))));
                    foreach (BtsAssembly assembly in app.Assemblies)
                    {
                        inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Assemblies." + assembly.Name)))));
                    }
                }
            }
            #endregion

            root.Add(intro, inThis);
            if (doc.Root != null) doc.Root.Add(root);
        }

        public new void Save()
        {
            do
            {
                Thread.Sleep(100);
            } while (ReadyToSave == false);
            base.Save();
            if (null != orchsTopic)     orchsTopic.Save();
            if (null != rpsTopic)       rpsTopic.Save();
            if (null != rlsTopic)       rlsTopic.Save();
            if (null != bsTopic)        bsTopic.Save();
            if (null != spsTopic)       spsTopic.Save();
            if (null != schTopic)       schTopic.Save();
            if (null != assyTopic)      assyTopic.Save();
            if (null != transTopic)     transTopic.Save();
        }

        public XElement GetContentLayout()
        {
            XElement xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", true),
                                new XAttribute("title", appName));
            if (null != orchsTopic)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!orchsTopic.ReadyToSave);
                xe.Add(orchsTopic.GetContentLayout());
            }
            
            if (null != spsTopic)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!spsTopic.ReadyToSave);

                xe.Add(spsTopic.GetContentLayout());
            }

            if (null != rpsTopic)
            {
                do
                {
                    Thread.Sleep(100);                    
                } while (!rpsTopic.ReadyToSave);
                xe.Add(rpsTopic.GetContentLayout());
            }

            if (null != rlsTopic)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!rlsTopic.ReadyToSave);

                xe.Add(rlsTopic.GetContentLayout());
            }

            if (null != bsTopic)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!bsTopic.ReadyToSave);
                xe.Add(bsTopic.GetContentLayout());
            }

            if (null != schTopic)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!schTopic.ReadyToSave);
                xe.Add(schTopic.GetContentLayout());
            }

            if (null != transTopic)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!transTopic.ReadyToSave);

                xe.Add(transTopic.GetContentLayout());
            }

            if (null != assyTopic)
            {
                do
                {
                    Thread.Sleep(100);
                } while (!assyTopic.ReadyToSave);

                xe.Add(assyTopic.GetContentLayout());
            }
            return xe;
        }

    }
}