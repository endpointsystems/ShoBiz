using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.RuleEngineExtensions;
using Microsoft.RuleEngine;
using DictionaryEntry=System.Collections.DictionaryEntry;
using Policy=Microsoft.BizTalk.ExplorerOM.Policy;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Generates documentation for the BRE rule set associated with the specified BizTalk application.
    /// </summary>
    /// <remarks>
    ///  Token ID is appName + ".Policies." + ruleName
    /// </remarks>
    class BusinessRuleTopic: TopicFile
    {
        private readonly string ruleName;
        private XElement root;
        private readonly string rulesDb;

        public BusinessRuleTopic(string btsAppName, string basePath, string btsRuleName)
        {
            appName = btsAppName;
            topicRelativePath = basePath;
            tokenId = CleanAndPrep(appName + ".Policies." + btsRuleName);
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            ruleName = btsRuleName;
            rulesDb = ProjectConfiguration.RulesDatabase;
        }


        void SaveTopic()
        {
            //var bce = CatalogExplorerFactory.CatalogExplorer();
            //bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
            try
            {
                root = CreateDeveloperConceptualElement();
                var srs = new SqlRuleStore(CatalogExplorerFactory.CatalogExplorer().ConnectionString.Replace("BizTalkMgmtDb", rulesDb));
                XElement intro;
                //find our policy in the crowd
                Policy p = null;
                foreach (Policy policy in CatalogExplorerFactory.CatalogExplorer().Applications[appName].Policies)
                {
                    if (!policy.Name.Equals(ruleName)) continue;
                    p = policy;
                    break;
                }
                if (p == null)
                {
                    intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText("Policy was not found!")));
                    root.Add(intro);
                    if (doc.Root != null) doc.Root.Add(root);
                    ReadyToSave = true;
                    return;
                }

                var rs = srs.GetRuleSet(new RuleSetInfo(p.Name, p.MajorRevision, p.MinorRevision));

                root = CreateDeveloperXmlReference();
                intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText(string.Format("This section outlines the properties for the {0} rule set.",p.Name))));
                
                var exeConfInfo = new XElement(xmlns + "section",
                    new XElement(xmlns + "title", new XText("Execution Configuration Properties")),
                    new XElement(xmlns + "content",
                        new XElement(xmlns + "table",
                            new XElement(xmlns + "tableHeader", 
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Property")),
                                    new XElement(xmlns + "entry", new XText("Value")))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Fact Retriever Class Name")),
                                    new XElement(xmlns + "entry", new XText(null == rs.ExecutionConfiguration.FactRetriever ? "N/A" : rs.ExecutionConfiguration.FactRetriever.ClassName ?? "N/A"))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Fact Retriever Assembly Name")),
                                    new XElement(xmlns + "entry", new XText(null == rs.ExecutionConfiguration.FactRetriever ? "N/A" : rs.ExecutionConfiguration.FactRetriever.AssemblyName ?? "N/A"))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Max Execution Loop Depth")),
                                    new XElement(xmlns + "entry", new XText(rs.ExecutionConfiguration.MaxExecutionLoopDepth.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Maximum Working Memory Size")),
                                    new XElement(xmlns + "entry", new XText(rs.ExecutionConfiguration.MaxWorkingMemorySize.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Translation Duration (milliseconds)")),
                                    new XElement(xmlns + "entry", new XText(rs.ExecutionConfiguration.TranslationDuration.ToString()))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Translator Class")),
                                    new XElement(xmlns + "entry", new XText(null == rs.ExecutionConfiguration.Translator ? "N/A" : rs.ExecutionConfiguration.Translator.ClassName ?? "N/A"))),
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Translator Assembly")),
                                    new XElement(xmlns + "entry", new XText(null == rs.ExecutionConfiguration.Translator ? "N/A" : rs.ExecutionConfiguration.Translator.AssemblyName ?? "N/A")))
                        )));                

                var rulesList = new List<XElement>();
                IEnumerator iter = rs.Rules.GetEnumerator();

                while (iter.MoveNext())
                {
                    var de = (DictionaryEntry)iter.Current;
                    var r = de.Value as Rule;
                    if (null == r) continue;
                    var rsi = new RuleSetInfo(p.Name, p.MajorRevision, p.MinorRevision);
                    var rdse = new RuleDisplayStringExtractor(srs, rsi);
                    var s = new XElement(xmlns + "section",
                        new XElement(xmlns + "title", new XText(string.Format("Rule: {0}",r.Name))),
                        new XElement(xmlns + "content",
                            new XElement(xmlns + "para",new XElement(xmlns + "legacyBold", new XText("Rule display:"))),
                            new XElement(xmlns + "code", new XAttribute("language","other"),new XText(rdse.ExtractRuleDisplayString(r))),
                            new XElement(xmlns + "para", new XElement(xmlns + "legacyBold", new XText("Rule properties:"))),
                            new XElement(xmlns + "table",
                            new XElement(xmlns + "tableHeader", 
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Property")),
                                        new XElement(xmlns + "entry", new XText("Value")))),
                            new XElement(xmlns + "row",
                                new XElement(xmlns + "entry", new XText("Active")),
                                new XElement(xmlns + "entry", new XText(r.Active.ToString()))),
                            new XElement(xmlns + "row",
                                new XElement(xmlns + "entry", new XText("Priority")),
                                new XElement(xmlns + "entry", new XText(r.Priority.ToString()))),
                            new XElement(xmlns + "row",
                                new XElement(xmlns + "entry", new XText("Vocabulary Definition Id")),
                                new XElement(xmlns + "entry", new XText(r.VocabularyLink == null ? "N/A" : r.VocabularyLink.DefinitionId ?? "N/A"))),
                            new XElement(xmlns + "row",
                                new XElement(xmlns + "entry", new XText("Vocabulary Id")),
                                new XElement(xmlns + "entry", new XText(r.VocabularyLink == null ? "N/A" : r.VocabularyLink.VocabularyId ?? "N/A")))                                
                                )));
                    
                    rulesList.Add(s);
                }
                
                //parent section
                var section = new XElement(xmlns + "section", 
                    new XElement(xmlns + "title", new XText("Business RuleSet Properties")),
                        new XElement(xmlns + "content", 
                            new XElement(xmlns + "para", new XText(rs.CurrentVersion.Description ?? "The current version of this rule set has no comments associated with it.")),
                            new XElement(xmlns + "table",
                            new XElement(xmlns + "tableHeader", 
                                new XElement(xmlns + "row",
                                    new XElement(xmlns + "entry", new XText("Property")),
                                        new XElement(xmlns + "entry", new XText("Value")))),
                            new XElement(xmlns + "row",
                                new XElement(xmlns + "entry", new XText("Current Version")),
                                new XElement(xmlns + "entry", new XText(rs.CurrentVersion.MajorRevision + "." + rs.CurrentVersion.MinorRevision))),
                            new XElement(xmlns + "row",
                                new XElement(xmlns + "entry", new XText("Last Modified By")),
                                new XElement(xmlns + "entry", new XText(rs.CurrentVersion.ModifiedBy))),
                            new XElement(xmlns + "row",
                                new XElement(xmlns + "entry", new XText("Modification Timestamp")),
                                new XElement(xmlns + "entry", new XText(rs.CurrentVersion.ModifiedTime.ToString())))),
                                new XElement(xmlns + "sections", rulesList.ToArray(),exeConfInfo)
                                ));
                                
                root.Add(intro,section);
                if (doc.Root != null) doc.Root.Add(root);
            }
            catch (Exception ex)
            {                    
                HandleException("BusinessRuleTopic.DoWork",ex);
            }
        }

        public XElement GetContentLayout()
        {
            return NewTopicEntry(ruleName);
        }

        public override void Save()
        {
            TimerStart();
            SaveTopic();
            base.Save();
            TimerStop();
        }
    }
}