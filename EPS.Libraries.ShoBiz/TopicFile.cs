using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// The base class for all topic files.
    /// </summary>
    public class TopicFile
    {
        protected static XNamespace xlink = "http://www.w3.org/1999/xlink";
        protected static XNamespace xmlns = "http://ddue.schemas.microsoft.com/authoring/2003/5";
        protected string appName;
        public XDocument doc;
        public string id;
        protected string path;
        public bool ReadyToSave;
        protected Stopwatch watch;
        protected string tokenId;

        public TopicFile()
        {
            id = Guid.NewGuid().ToString();
            XElement topic = new XElement("topic");
            topic.SetAttributeValue("id", id);
            topic.SetAttributeValue("revision", "1");
            doc = new XDocument(
                new XElement("topic",
                             new XAttribute("id", id),
                             new XAttribute("revision", "1")));
            watch = new Stopwatch();
        }

        public static string CleanAndPrep(string qualifiedName)
        {
            if (string.IsNullOrEmpty(qualifiedName))
            {
                Trace.WriteLine("CleanAndPrep got a null/empty string!");
                return string.Empty;
            }
            string s = qualifiedName.Replace(",", ".");
            s = s.Replace("=", "-");
            s = s.Replace("#", "-");
            return s.Replace(" ", "");
        }

        protected static XElement AddListSection(string title, string listDesc, XElement list)
        {
            return new XElement(xmlns + "section",
                                new XElement(xmlns + "title", new XText(title)),
                                new XElement(xmlns + "content",
                                             new XElement(xmlns + "para",
                                                          new XText(
                                                              listDesc)),
                                             list));
        }

        public void Save()
        {
            string savePath = path + id + ".aml";
            do
            {
                Thread.Sleep(100);
            } while (!ReadyToSave);
            ProjectFile.GetProjectFile().AddTopicItem(savePath);
            doc.Save(savePath,SaveOptions.None);
        }
        protected static void addFolder(string folderPath)
        {
            makeDir(folderPath);
            ProjectFile.GetProjectFile().AddFolderItem(folderPath);
        }
        
        protected static void makeDir(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        protected void TimerStart()
        {
            watch.Start();
            PrintLine("Started {0}", tokenId);
        }
        protected void TimerStop()
        {
            watch.Stop();
            PrintLine("{0} completed in {1} seconds.",tokenId,watch.Elapsed.TotalSeconds);
        }

        protected static void PrintLine(string message, params object[] args)
        {
            if (args == null || args.Length < 1)
            {
                Trace.WriteLine(message);
                return;
            }
            Trace.WriteLine(string.Format(message, args));
        }

        protected static XElement CreateDeveloperXmlReference()
        {
            return getElement("developerXmlReference");
        }

        protected static XElement CreateDeveloperOrientationElement()
        {
            return getElement("developerOrientationDocument");
        }

        protected static XElement CreateDeveloperConceptualElement()
        {
            return getElement("developerConceptualDocument");
        }

        protected static XElement GetNullable(object nullableValue, string propertyName, string tokenPrefix)
        {
            try
            {
                XElement elem;
                if (nullableValue == null)
                {
                    elem = new XElement(xmlns + "entry", new XText("N/A"));
                }
                else
                {
                    object val = nullableValue.GetType().GetProperty(propertyName).GetValue(nullableValue, null);
                    elem = new XElement(xmlns + "entry", new XElement(xmlns + "token", tokenPrefix +  new XText(Convert.ToString(val))));
                }

                return elem;
            }
            catch(Exception ex)
            {
                HandleException("GetNullable", ex);
            }
            return null;
        }

        protected static XElement GetPipelineEntry(Pipeline pipeline)
        {
            XElement elem = null;
            try
            {
                if (null == pipeline)
                {
                    elem = new XElement(xmlns + "entry", new XText("N/A"));
                    return elem;
                }
                if (!pipeline.FullName.Contains("Microsoft"))
                {
                    elem = new XElement(xmlns + "entry", new XElement(xmlns + "token",
                                          new XText(CleanAndPrep(pipeline.Application.Name + ".Pipelines." + pipeline.FullName))));
                    return elem;
                }
                elem = new XElement(xmlns + "entry", new XText(pipeline.FullName));
            }
            catch(Exception ex)
            {
                HandleException("GetPipelineEntry", ex);
            }
            finally
            {
                //PrintLine("[GetPipelineEntry] element XML: {0}", elem.ToString(SaveOptions.None));
            }
            return elem;
        }

        protected static XElement GetTransformEntry(IList trans)
        {
            XElement elem = new XElement(xmlns + "entry");
            try
            {
                if (null == trans || trans.Count == 0)
                {
                    elem = new XElement(xmlns + "entry", new XText("N/A"));
                    return elem;
                }
                XElement list = new XElement(xmlns + "list");
                foreach (Transform tran in trans)
                {
                    list.Add(new XElement(xmlns + "listItem",
                                          new XElement(xmlns + "token",
                                                       new XText(
                                                           CleanAndPrep(tran.Application.Name + ".Transforms." +
                                                                        tran.FullName)))));
                }
                elem.Add(list);
            }
            catch(Exception ex)
            {
                HandleException("GetTransformEntry", ex);
            }
            finally
            {
                //PrintLine("[GetTransformEntry] element xml: {0}", elem.ToString(SaveOptions.None));
            }
            return elem;
        }
        
        protected static void HandleException (string title, Exception ex)
        {
            PrintLine("[{0}] {1}: {2}\r\n{3}",title,ex.GetType(),ex.Message,ex.StackTrace);
        }

        private static XElement getElement(string elemName)
        {
            XElement xe = new XElement(xmlns + elemName,new XAttribute("xmlns",xmlns),new XAttribute(XNamespace.Xmlns + "xlink",xlink));
            return xe;
        }

        public string TokenId { get { return tokenId; } }

        public XElement GetContentLayout(string contentTitle)
        {
            return new XElement("Topic",
                                    new XAttribute("id", id),
                                    new XAttribute("visible", "true"),
                                    new XAttribute("title", contentTitle));
        }

        protected static XElement DictionaryToTable(IDictionary dict, string leftColumnTitle, string rightColumnTitle)
        {
            XElement el = new XElement(xmlns + "table", new XElement(xmlns + "tableHeader", new XElement(xmlns + "row", new XElement(xmlns + "entry", new XText(leftColumnTitle)),
                new XElement(xmlns + "entry", new XText(rightColumnTitle)))));
            foreach (DictionaryEntry entry in dict)
            {
                XElement ex = new XElement(xmlns + "row", new XElement(xmlns + "entry", new XText(entry.Key == null ? "(N/A)" : entry.Key as string)),
                    new XElement(xmlns + "entry", new XText(entry.Value == null ? "(N/A)" : entry.Value as string)));
                el.Add(ex);
            }
            return el;
        }

        protected static XElement CollectionToList(ICollection coll)
        {
            XElement list = new XElement(xmlns + "list");
            foreach (string name in coll)
            {
                list.Add(new XElement(xmlns + "listItem", new XText(name)));
            }
            return list;
        }
    }
}