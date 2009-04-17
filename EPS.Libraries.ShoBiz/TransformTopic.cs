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
    /// Developer conceptual topic file for the BizTalk Transform artifact.
    /// </summary>
    /// <remarks>
    ///     Token ID: appName + ".Transforms." + schema.FullName
    /// </remarks>
    public class TransformTopic : TopicFile, IDisposable
    {
        private readonly string transformName;
        private readonly BackgroundWorker schWorker;
        private XElement root;
        private StringBuilder sb;
        public TransformTopic(string btsAppName, string baseDir, string btsTransformName)
        {
            appName = btsAppName;
            transformName = btsTransformName;
            path = baseDir;
            tokenId = CleanAndPrep(appName + ".Transforms." + btsTransformName);
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
            if (schWorker != null) schWorker.Dispose();
        }

        void schWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (this)
            {
                ReadyToSave = true;
            }
        }

        void schWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;
                Transform transform = bce.Applications[appName].Transforms[transformName];
                sb.Append(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?><topic id=\"" + id + "\" revisionNumber=\"1\">");
                root = CreateDeveloperXmlReference();
                
                XElement intro = new XElement(xmlns + "introduction", new XElement(xmlns + "para", new XText(string.IsNullOrEmpty(transform.Description) ? "No description was found for the schema." : transform.Description)));
                XElement section = new XElement(xmlns + "section", new XElement(xmlns + "title", new XText("Transform Properties")),
                                                                    new XElement(xmlns + "content",
                                                                        new XElement(xmlns + "table",
                                                                            new XElement(xmlns + "tableHeader",
                                                                                new XElement(xmlns + "row",
                                                                                    new XElement(xmlns + "entry", new XText("Property")),
                                                                                    new XElement(xmlns + "entry", new XText("Value")))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Application")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(transform.Application.Name))))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Assembly Qualified Name")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(CleanAndPrep(appName + ".Assemblies." + transform.AssemblyQualifiedName))))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Source Schema")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(appName + ".Schemas." + transform.SourceSchema.FullName) ))),
                                                                            new XElement(xmlns + "row",
                                                                                new XElement(xmlns + "entry", new XText("Target Schema")),
                                                                                new XElement(xmlns + "entry", new XElement(xmlns + "token", new XText(appName + ".Schemas." + transform.TargetSchema.FullName))))
                                                                            )));

                XElement content = new XElement(xmlns + "codeExample", new XElement(xmlns + "code", new XAttribute("language", "xml"), new XText(transform.XmlContent)));

                root.Add(intro, section, content);
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
            catch (Exception ex)
            {
                HandleException("TransformTopic.Save", ex);
            }
        }

        public XElement GetContentLayout()
        {
            return GetContentLayout(transformName);
        }

    }
}
