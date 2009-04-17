using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Developer conceptual topic file for the BizTalk Schema artifact.
    /// </summary>
    /// <remarks>
    ///     Token ID: appName + ".Schemas." + schema.FullName
    /// </remarks>
    public class SchemaTopic: TopicFile, IDisposable
    {
        private readonly string schemaName;
        private readonly BackgroundWorker schWorker;
        private XElement root;
        private StringBuilder sb;
        private string schemaTitle;
        public SchemaTopic(string btsAppName, string baseDir, string btsSchemaName)
        {
            appName = btsAppName;
            schemaName = btsSchemaName;
            path = baseDir;
            tokenId = CleanAndPrep(appName + ".Schemas." + btsSchemaName);
            TimerStart();
            TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
            sb = new StringBuilder();
            schWorker = new BackgroundWorker();
            schWorker.DoWork += schWorker_DoWork;
            schWorker.RunWorkerCompleted += schWorker_RunWorkerCompleted;
            schWorker.RunWorkerAsync();
        }

        public void Dispose()
        {
            schWorker.Dispose();
        }
        void schWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock(this)
            {
                ReadyToSave = true;
            }
            TimerStop();
        }

        void schWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                Schema s = bce.Applications[appName].Schemas[schemaName];
                schemaTitle = schemaName + "#" + s.RootName;
                sb.Append(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?><topic id=\"" + id + "\" revisionNumber=\"1\">");
                root = CreateDeveloperXmlReference();
                
                XElement intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText(string.IsNullOrEmpty(s.Description)? "No description was found for the schema." : s.Description)));
                XElement section = new XElement(xmlns + "section", new XElement(xmlns + "title", new XText("Schema Properties")),
                                                                    new XElement(xmlns + "content",
                                                                        new XElement(xmlns + "table",
                                                                            new XElement(xmlns + "tableHeader",
                                                                                new XElement(xmlns + "row",
                                                                                    new XElement(xmlns + "entry", new XText("Property")),
                                                                                    new XElement(xmlns + "entry", new XText("Value")))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Application")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(s.Application.Name))))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Always Track All Properties")),
                                                                                new XElement(xmlns + "entry", new XText(s.AlwaysTrackAllProperties.ToString()))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Assembly Qualified Name")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Assemblies." + s.BtsAssembly.DisplayName))))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Properties")),
                                                                                new XElement(xmlns + "entry", s.Properties.Count > 0 ? DictionaryToTable(s.Properties,"Property Name", "Property Type") : new XElement(xmlns + "legacyItalic", new XText("(N/A)")))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Root Name")),
                                                                                new XElement(xmlns + "entry", new XText(s.RootName))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Target Namespace")),
                                                                                new XElement(xmlns + "entry", new XText(s.TargetNameSpace ?? "N/A"))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Tracked Property Names")),
                                                                                s.TrackedPropertyNames.Count > 0 ? CollectionToList(s.TrackedPropertyNames) : new XElement(xmlns + "legacyItalic", new XText("(N/A)"))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Type")),
                                                                                new XElement(xmlns + "entry", new XText(s.Type.ToString()))))));

                XElement content = new XElement(xmlns + "codeExample", new XElement(xmlns + "code",new XAttribute("language","xml"), new XText(s.XmlContent)));

                root.Add(intro,section,content);
                sb.Append(root.ToString(SaveOptions.None));
                sb.Append("</topic>");
            }
            finally
            {
                bce.Dispose();
            }
        }

        public new void Save()
        {
            try
            {
                string savePath = path + id + ".aml";
                do
                {
                    Thread.Sleep(100);
                } while (!ReadyToSave);
                using (StreamWriter sw = new StreamWriter(savePath))
                {
                    sw.Write(sb.ToString());
                    sw.Flush();
                    sw.Close();
                    ProjectFile.GetProjectFile().AddTopicItem(savePath);
                }
            }
            catch(Exception ex)
            {
                HandleException("SchemaTopic.Save", ex);
            }
        }

        public XElement GetContentLayout()
        {
            return GetContentLayout(schemaTitle);
        }

    }
}
