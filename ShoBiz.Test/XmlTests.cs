using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using EndpointSystems.BizTalk.Documentation;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.RuleEngine;
using NUnit.Framework;
using System.Diagnostics;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.RuleEngineExtensions;
using Policy=Microsoft.BizTalk.ExplorerOM.Policy;

namespace ShoBiz.Test
{
    [TestFixture]
    public class XmlTests
    {
        [Test]
        public void TestBre()
        {
            try
            {
                Microsoft.RuleEngine.RuleSetDeploymentDriver driver = new Microsoft.RuleEngine.RuleSetDeploymentDriver("epsbtsvm", "BizTalkRuleEngineDb");
                RuleSetInfoCollection ruleSets = driver.GetRuleStore().GetRuleSets(RuleStore.Filter.All);
                foreach (RuleSetInfo set in ruleSets)
                {
                    driver.ExportRuleSetToFileRuleStore(set, @"C:\temp\" + set.Name);
                }
            }
            catch(Exception ex)
            {
                HandleException("TestBRE", ex);
            }
        }

        [Test]
        public void TestBre2()
        {
            const string connStr = "Data Source=(local);Initial Catalog=BizTalkMgmtDb;Integrated Security=True;Min Pool Size=10;MultipleActiveResultSets=True;Connect Timeout=30;Network Library=dbnmpntw;Application Name=ShoBiz";
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            bce.ConnectionString = connStr;

            SqlRuleStore srs = new SqlRuleStore(connStr.Replace("BizTalkMgmtDb", "BizTalkRuleEngineDb"));

            RuleSetInfoCollection rsic = srs.GetRuleSets(RuleStore.Filter.LatestPublished);

            foreach (Policy p in bce.Applications["EPS Cloud Catalog"].Policies)
            {
                PrintLine("Policy Name: {0}",p.Name);
            }

            foreach (RuleSetInfo info in rsic)
            {
                PrintLine("RuleSet found: {0} ({1}.{2})",info.Name,info.MajorRevision,info.MinorRevision);

                RuleSet rs = srs.GetRuleSet(info);

                RuleDisplayStringExtractor rdse = new RuleDisplayStringExtractor(srs, info);
                
                PrintLine("rules count: {0}",rs.Rules.Count);
                if (rs.Rules.Count == 0) continue;
                IEnumerator iter = rs.Rules.GetEnumerator();
                while (iter.MoveNext())
                {
                    DictionaryEntry de = (DictionaryEntry)iter.Current;
                    Rule r = de.Value as Rule;
                    PrintLine("Rule Display String Extractor ({0}): {1}", r.Name, rdse.ExtractRuleDisplayString(r));
                }
            }
        }

        protected static void HandleException(string title, Exception ex)
        {
            PrintLine("[0] {1}: {2}\r\n{3}", title, ex.GetType(), ex.Message, ex.StackTrace);
        }

        protected static void PrintLine(string message, params object[] args)
        {
            Trace.WriteLine(string.Format(message, args));
        }

        [Test]
        public void TestTopicFile()
        {
            try
            {   
                //initialize catalog explorer witho our optimized connection string
                const string connStr = "Data Source=(local);Initial Catalog=BizTalkMgmtDb;Integrated Security=True;Min Pool Size=10;MultipleActiveResultSets=True;Connect Timeout=30;Network Library=dbnmpntw;Application Name=ShoBiz";
                const string appName = "EPS Cloud Catalog";
                const string baseDirectory = @"C:\Temp\";
                const string imagesDirectory = @"C:\Temp\images\";
                const string contentFile = @"C:\Temp\ContentFile.content";
                const string projectFile = baseDirectory + "ShoBizProject.shfbproj";
                const string tokenFile = baseDirectory + "ShoBizTokens.tokens";
                const string rulesDb = "BizTalkRuleEngineDb";
                CatalogExplorerFactory.CatalogExplorer(connStr, true);

                //what apps do we want to document?
                List<string> apps = new List<string>();
                apps.Add(appName);

                AppsTopic appsTopic = new AppsTopic(baseDirectory,imagesDirectory,apps,rulesDb);

                do
                {
                    Thread.Sleep(250);
                } while (!appsTopic.ReadyToSave);
                
                //save applications topic (and all children)
                appsTopic.Save();                
                
                //save content file and add to the project
                ContentFile.GetContentFile().AddApplicationTopic(appsTopic.GetContentLayout());
                ContentFile.GetContentFile().Save(contentFile);
                ProjectFile.GetProjectFile().AddContentLayoutFile(contentFile);

                //save token file and add to the project
                TokenFile.GetTokenFile().Save(tokenFile);
                ProjectFile.GetProjectFile().AddTokenFile(tokenFile);

                //save the project file
                ProjectFile.GetProjectFile().Save(projectFile);
            }
            catch(Exception e)
            {
                Trace.WriteLine(string.Format("{0}: {1} : {2}",e.GetType(),e.Message,e.StackTrace));
            }
        }
    }
}
