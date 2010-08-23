using System;
using System.Collections.Generic;
using System.IO;
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
        private OrchestrationsTopic orchsTopic;
        private ReceivePortsTopic rpsTopic;
        private SendPortsTopic spsTopic;
        private ReceiveLocationsTopic rlsTopic;
        private SchemasTopic schTopic;
        private AssembliesTopic assyTopic;
        private TransformsTopic transTopic;
        private BusinessRulesTopic bsTopic;
        private readonly List<TopicFile> topics;
        /// <summary>
        /// Creates a new Sandcastle topic for a BizTalk application.
        /// </summary>
        /// <param name="topicPath">The base path for the application topic.</param>
        /// <param name="btsAppName">The BizTalk application name.</param>
        public AppTopic(string topicPath, string btsAppName)
        {
            appName = btsAppName;
            topicRelativePath = Path.Combine(topicPath, appName);
            buildTree();
            tokenId = CleanAndPrep(btsAppName);
            topics = new List<TopicFile>();
            TokenFile.GetTokenFile().AddTopicToken(CleanAndPrep(tokenId), id);
        }
        /// <summary>
        /// Build the directory tree for the app.
        /// </summary>
        private void buildTree()
        {
            addFolderToProject(topicRelativePath);
            addFolderToProject(Path.Combine(topicRelativePath, "Orchestrations"));
            addFolderToProject(Path.Combine(topicRelativePath, "Receive Ports"));
            addFolderToProject(Path.Combine(topicRelativePath, "Receive Locations"));
            addFolderToProject(Path.Combine(topicRelativePath, "Policies"));
            addFolderToProject(Path.Combine(topicRelativePath, "Schemas"));
            addFolderToProject(Path.Combine(topicRelativePath, "Maps"));
            addFolderToProject(Path.Combine(topicRelativePath, "Pipelines"));
            addFolderToProject(Path.Combine(topicRelativePath, "Resources"));

            addFolderToFileSystem(Path.Combine(Path.Combine(ProjectConfiguration.BasePath, topicRelativePath), "Orchestrations"));
            addFolderToFileSystem(Path.Combine(Path.Combine(ProjectConfiguration.BasePath, topicRelativePath), "Receive Ports"));
            addFolderToFileSystem(Path.Combine(Path.Combine(ProjectConfiguration.BasePath, topicRelativePath), "Receive Locations"));
            addFolderToFileSystem(Path.Combine(Path.Combine(ProjectConfiguration.BasePath, topicRelativePath), "Policies"));
            addFolderToFileSystem(Path.Combine(Path.Combine(ProjectConfiguration.BasePath, topicRelativePath), "Schemas"));
            addFolderToFileSystem(Path.Combine(Path.Combine(ProjectConfiguration.BasePath, topicRelativePath), "Maps"));
            addFolderToFileSystem(Path.Combine(Path.Combine(ProjectConfiguration.BasePath, topicRelativePath), "Pipelines"));
            addFolderToFileSystem(Path.Combine(Path.Combine(ProjectConfiguration.BasePath, topicRelativePath), "Resources"));
        }

        private void SaveTopic()
        {
            var app = CatalogExplorerFactory.CatalogExplorer().Applications[appName];
            var root = CreateDeveloperOrientationElement();
            var intro = new XElement(xmlns + "introduction", 
                new XElement(xmlns + "para", new XText(string.IsNullOrEmpty(app.Description) 
                    ? "No description was given." 
                    : app.Description)));
            var inThis = new XElement(xmlns + "inThisSection", 
                new XText("This application contains the BizTalk artifacts listed below."));

            #region orchestrations
            try
            {
                if (null != app.Orchestrations)
                {
                    if (app.Orchestrations.Count > 0)
                    {
                        PrintLine("{0} orchestrations count: {1}", appName, app.Orchestrations.Count);
                        var olist = new List<string>();
                        inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Orchestrations:"))));
                        foreach (BtsOrchestration orch in app.Orchestrations)
                        {
                            olist.Add(orch.FullName);
                            inThis.Add(new XElement(xmlns + "para",
                                                    new XElement(xmlns + "token",
                                                                 new XText(CleanAndPrep(appName + ".Orchestrations." + orch.FullName)))));
                        }
                        PrintLine("creating orchestrations topic for {0}", appName);
                        orchsTopic = new OrchestrationsTopic(Path.Combine(topicRelativePath, "Orchestrations"), app.Name, olist.ToArray());
                    }
                    topics.Add(orchsTopic);
                }
            }
            catch (Exception ex)
            {
                HandleException("BtsAppTopic.DoWork(orchestrations)", ex);
            }
            #endregion

            #region send ports

            try
            {
                if (app.SendPorts != null)
                {
                    PrintLine("send ports count: {0}", app.SendPorts.Count);
                    var ports = new List<string>();
                    if (app.SendPorts.Count > 0)
                    {
                        inThis.Add(new XElement(xmlns + "para", 
                            new XElement(xmlns + "legacyBold", new XText("Send Ports:"))));
                        foreach (SendPort port in app.SendPorts)
                        {
                            inThis.Add(new XElement(xmlns + "para",
                                                    new XElement(xmlns + "token",
                                                                 new XText(CleanAndPrep(appName + ".SendPorts." + port.Name)))));
                            ports.Add(port.Name);
                        }
                        PrintLine("creating send ports topic for {0}", appName);
                        spsTopic = new SendPortsTopic(appName, Path.Combine(topicRelativePath, "Send Ports"), ports.ToArray());
                    }
                    topics.Add(spsTopic);
                }
            }
            catch (Exception ex)
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
                    var rlElement = new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Receive Locations:")));
                    var recLocs = new List<XElement>();

                    var rpElement = new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Receive Ports:")));

                    var jarPorts = new string[app.ReceivePorts.Count][];

                    //iterate through the receive ports for the array
                    var jagIterator = 0;
                    var receivePortIterator = 0;
                    var receivePorts = new string[app.ReceivePorts.Count];
                    foreach (ReceivePort port in app.ReceivePorts)
                    {
                        //collect the receive port name
                        receivePorts[receivePortIterator] = port.Name;
                        receivePortIterator++;

                        PrintLine("{0} has {1} receive locations");
                        rpElement.Add(new XElement(xmlns + "para",
                                                new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".ReceivePorts." + port.Name)))));
                        if (port.ReceiveLocations.Count > 0)
                        {
                            jarPorts[jagIterator] = new string[port.ReceiveLocations.Count];
                            for (var j = 0; j < port.ReceiveLocations.Count; j++)
                            {
                                jarPorts[jagIterator][j] = port.ReceiveLocations[j].Name;
                                recLocs.Add(new XElement(xmlns + "para",
                                                         new XElement(xmlns + "token",
                                                                      new XText(CleanAndPrep(appName + ".ReceiveLocations." + port.ReceiveLocations[j].ReceivePort.Name + port.ReceiveLocations[j].Name)))));
                            }
                            jagIterator++;

                            if (null == rlsTopic)
                            {
                                PrintLine("creating receive locations topic for {0}", appName);
                                rlsTopic = new ReceiveLocationsTopic(appName, Path.Combine(topicRelativePath, "Receive Locations"), jarPorts);
                            }

                            topics.Add(rlsTopic);
                        }

                        PrintLine("creating receive ports topic for {0}", appName);
                        rpsTopic = new ReceivePortsTopic(appName, Path.Combine(topicRelativePath, "Receive Ports"), receivePorts);
                        topics.Add(rpsTopic);
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
                    if (string.IsNullOrEmpty(ProjectConfiguration.RulesDatabase))
                        throw new NullReferenceException("The Rules Engine Database configuration setting is null or empty.");
                    inThis.Add(new XElement(xmlns + "legacyBold", new XText("Policies:")));

                    var rules = new string[app.Policies.Count];
                    var rulesIterator = 0;
                    foreach (Policy policy in app.Policies)
                    {
                        rules[rulesIterator] = policy.Name;
                        rulesIterator++;
                        inThis.Add(new XElement(xmlns + "para",
                                                new XElement(xmlns + "token",
                                                             new XText(
                                                                 CleanAndPrep(appName + ".Policies." + policy.Name)))));
                    }

                    PrintLine("creating business rules topic for {0}", appName);
                    bsTopic = new BusinessRulesTopic(appName, Path.Combine(topicRelativePath, "Policies"), rules);
                }
                topics.Add(bsTopic);
            }

            #endregion

            #region schemas
            if (null != app.Schemas)
            {
                if (app.Schemas.Count > 0)
                {
                    PrintLine("{0} has {1} schemas", appName, app.Schemas.Count);
                    var schemaNames = new string[app.Schemas.Count];
                    for (var i = 0; i < app.Schemas.Count; i++)
                    {
                        schemaNames[i] = app.Schemas[i].FullName + "___" + app.Schemas[i].RootName;
                    }

                    PrintLine("Creating schemas topic for {0}...", appName);
                    schTopic = new SchemasTopic(appName, Path.Combine(topicRelativePath, "Schemas"), schemaNames);
                    topics.Add(schTopic);
                    inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Schemas:"))));

                    foreach (var name in schemaNames)
                    {
                        inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Schemas." + name)))));
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
                    var mapNames = new string[app.Transforms.Count];

                    for (var i = 0; i < app.Transforms.Count; i++)
                    {
                        mapNames[i] = app.Transforms[i].FullName;
                    }

                    inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Transforms:"))));
                    foreach (Transform transform in app.Transforms)
                    {
                        inThis.Add(new XElement(xmlns + "para",
                                                new XElement(xmlns + "token",
                                                             new XText(CleanAndPrep(appName + ".Transforms." + transform.FullName)))));
                    }

                    PrintLine("creating transforms (maps) topic for {0}", appName);
                    transTopic = new TransformsTopic(appName, Path.Combine(topicRelativePath, "Maps"), mapNames);
                    topics.Add(transTopic);
                }
            }
            #endregion

            //TODO: Is there no pipeline topic(s)?
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
                    inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Resources:"))));
                    var assys = new string[app.Assemblies.Count];
                    var assyIterator = 0;
                    foreach (BtsAssembly assembly in app.Assemblies)
                    {
                        assys[assyIterator] = assembly.Name;
                        assyIterator++;
                        inThis.Add(new XElement(xmlns + "para", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Assemblies." + assembly.Name)))));
                    }

                    PrintLine("creating assemblies topic for {0}", appName);
                    assyTopic = new AssembliesTopic(Path.Combine(topicRelativePath, "Resources"), appName, assys);
                    topics.Add(assyTopic);
                }
            }
            #endregion

            root.Add(intro, inThis);
            if (doc.Root != null) doc.Root.Add(root);
        }

        /// <summary>
        /// Save the topic and all sub-topics.
        /// </summary>
        public override void Save()
        {
            TimerStart();
            SaveTopic();
            foreach (var file in topics)
            {
                if (file == null) continue;

                if (file is SchemasTopic)
                    PrintLine("saving schemas topic!");

                file.Save();
            }

            //var opts = new ParallelOptions {MaxDegreeOfParallelism = -1, TaskScheduler = null};
            //loopResult = Parallel.ForEach(topics, opts, SaveAllTopics);

            base.Save();
            TimerStop();
        }

        /// <summary>
        /// Get the Sandcastle content layout for the topic.
        /// </summary>
        /// <returns>An <see cref="XElement"/> containing the content layout information.</returns>
        public XElement GetContentLayout()
        {
            var xe = new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", true),
                                new XAttribute("title", appName));
            if (null != orchsTopic)
            {
                xe.Add(orchsTopic.GetContentLayout());
            }

            if (null != spsTopic)
            {
                xe.Add(spsTopic.GetContentLayout());
            }

            if (null != rpsTopic)
            {
                xe.Add(rpsTopic.GetContentLayout());
            }

            if (null != rlsTopic)
            {
                xe.Add(rlsTopic.GetContentLayout());
            }

            if (null != bsTopic)
            {
                xe.Add(bsTopic.GetContentLayout());
            }

            if (null != schTopic)
            {
                xe.Add(schTopic.GetContentLayout());
            }

            if (null != transTopic)
            {
                xe.Add(transTopic.GetContentLayout());
            }

            if (null != assyTopic)
            {
                xe.Add(assyTopic.GetContentLayout());
            }
            return xe;
        }

    }
}