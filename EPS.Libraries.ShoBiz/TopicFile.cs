#region
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.BizTalk.ExplorerOM;
#endregion

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// The base class for all topic files.
    /// </summary>
    public class TopicFile: IObservable<TopicFile>, IDisposable
    {
        /// <summary>
        /// The loop result from the <see cref="Parallel"/> operation performed in each topic.
        /// </summary>
        protected ParallelLoopResult loopResult;
        /// <summary>
        /// The topic observer.
        /// </summary>
        /// 
        protected IObserver<TopicFile> observer;
        /// <summary>
        /// The XLink namespace.
        /// </summary>
        protected static XNamespace xlink = "http://www.w3.org/1999/xlink";
        /// <summary>
        /// The xmlns namespace.
        /// </summary>
        protected static XNamespace xmlns = "http://ddue.schemas.microsoft.com/authoring/2003/5";
        /// <summary>
        /// The application name.
        /// </summary>
        protected string appName;
        /// <summary>
        /// The document.
        /// </summary>
        public XDocument doc;
        /// <summary>
        /// The topic ID.
        /// </summary>
        public string id;
        /// <summary>
        /// The path to the document.
        /// </summary>
        protected string topicRelativePath;
        /// <summary>
        /// Indicates the topic is ready to be saved.
        /// </summary>
        public bool ReadyToSave;
        /// <summary>
        /// Indicates whether a <see cref="Parallel"/> task is being performed.
        /// </summary>
        public bool UsingParallel;
        /// <summary>
        /// The topic's token ID.
        /// </summary>
        protected string tokenId;

        private readonly Stopwatch watch;

        /// <summary>
        /// Creates a new <see cref="TopicFile"/> instance.
        /// </summary>
        public TopicFile()
        {
            id = Guid.NewGuid().ToString();
            var topic = new XElement("topic");
            topic.SetAttributeValue("id", id);
            topic.SetAttributeValue("revision", "1");
            doc = new XDocument(
                new XElement("topic",
                             new XAttribute("id", id),
                             new XAttribute("revision", "1")));
            watch = new Stopwatch();
        }

        /// <summary>
        /// Gets the topic's token identifier.
        /// </summary>
        public string TokenId
        {
            get { return tokenId; }
        }

        /// <summary>
        /// The method for performing parallel saves on a topic's child topics.
        /// </summary>
        /// <param name="topic"></param>
        protected void SaveAllTopics(TopicFile topic)
        {
            UsingParallel = true;
            if (topic == null) return;
            PrintLine("Saving topic {0}...", topic.TokenId);
            topic.Save();
        }

        /// <summary>
        /// Cleanup a qualified name, removing non-alphanumeric characters.
        /// </summary>
        /// <param name="qualifiedName">The qualified name to clean.</param>
        /// <returns>A string with only alphanumeric characters.</returns>
        public static string CleanAndPrep(string qualifiedName)
        {
            if (string.IsNullOrEmpty(qualifiedName))
            {
                Trace.WriteLine("CleanAndPrep got a null/empty string!");
                return string.Empty;
            }
            var s = qualifiedName.Replace(",", ".");
            s = s.Replace("=", "-");
            s = s.Replace("#", "-");
            return s.Replace(" ", "");
        }

        /// <summary>
        /// Add a section containing a list.
        /// </summary>
        /// <param name="title">The optional list title.</param>
        /// <param name="listDesc">The list description.</param>
        /// <param name="list">The list.</param>
        /// <returns>The section as an <see cref="XElement"/>.</returns>
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

        /// <summary>
        /// Save the topic file to disk.
        /// </summary>
        public virtual void Save()
        {
            var savePath = Path.Combine(topicRelativePath, id + ".aml");
            ProjectFile.GetProjectFile().AddTopicItem(savePath);
            savePath = Path.Combine(ProjectConfiguration.BasePath, savePath);
            doc.Save(savePath, SaveOptions.None);
        }

        /// <summary>
        /// Save topic file content to disk.
        /// </summary>
        /// <param name="content">The topic file content.</param>
        public void Save(string content)
        {
            var savePath = Path.Combine(topicRelativePath, id + ".aml");
            ProjectFile.GetProjectFile().AddTopicItem(savePath);
            savePath = Path.Combine(ProjectConfiguration.BasePath, savePath);
            File.WriteAllText(savePath, content);
        }

        /// <summary>
        /// Add a folder to the project structure.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        protected static void addFolderToProject(string folderPath)
        {
            ProjectFile.GetProjectFile().AddFolderItem(folderPath);
        }

        /// <summary>
        /// Make a directory in the file system for the project.
        /// </summary>
        /// <param name="dir">The directory to create.</param>
        protected static void addFolderToFileSystem(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// Start a stopwatch timer. 
        /// </summary>
        protected void TimerStart()
        {
            watch.Start();
            PrintLine("Started {0}", tokenId);
        }

        /// <summary>
        /// Stop the stopwatch timer.
        /// </summary>
        protected void TimerStop()
        {
            watch.Stop();
            PrintLine("{0} completed in {1} seconds.", tokenId, watch.Elapsed.TotalSeconds);
            watch.Reset();
        }

        /// <summary>
        /// Perform a trace operation.
        /// </summary>
        /// <param name="message">The format message.</param>
        /// <param name="args">The optional format arguments.</param>
        protected static void PrintLine(string message, params object[] args)
        {
            if (args == null || args.Length < 1)
            {
                Trace.WriteLine(message);
                return;
            }
            Trace.WriteLine(string.Format(message, args));
        }

        /// <summary>
        /// Create a developer XML reference document.
        /// </summary>
        /// <returns>A document.</returns>
        protected static XElement CreateDeveloperXmlReference()
        {
            return getElement("developerXmlReference");
        }

        /// <summary>
        /// Create a developer orientation document.
        /// </summary>
        /// <returns>A document.</returns>
        protected static XElement CreateDeveloperOrientationElement()
        {
            return getElement("developerOrientationDocument");
        }

        /// <summary>
        /// Create a developer conceptual document.
        /// </summary>
        /// <returns>A document.</returns>
        protected static XElement CreateDeveloperConceptualElement()
        {
            return getElement("developerConceptualDocument");
        }

        /// <summary>
        /// Return an element containing possibly <c>null</c> value information.
        /// </summary>
        /// <param name="nullableValue">The possibly <c>null</c> value.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="tokenPrefix">The prefix token of the potentially <c>null</c> object.</param>
        /// <returns>A 'placeholder' element value for the <c>null</c> object</returns>
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
                    var val = nullableValue.GetType().GetProperty(propertyName).GetValue(nullableValue, null);
                    elem = new XElement(xmlns + "entry",
                                        new XElement(xmlns + "token", tokenPrefix + new XText(Convert.ToString(val))));
                }

                return elem;
            }
            catch (Exception ex)
            {
                HandleException("GetNullable", ex);
            }
            return null;
        }

        /// <summary>
        /// Get a document entry for a <see cref="Pipeline"/> reference.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>A document structure entry </returns>
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
                                                                      new XText(
                                                                          CleanAndPrep(pipeline.Application.Name +
                                                                                       ".Pipelines." + pipeline.FullName))));
                    return elem;
                }
                elem = new XElement(xmlns + "entry", new XText(pipeline.FullName));
            }
            catch (Exception ex)
            {
                HandleException("GetPipelineEntry", ex);
            }
            return elem;
        }

        /// <summary>
        /// Create a document entry for a BizTalk Map.
        /// </summary>
        /// <param name="trans">The transform.</param>
        /// <returns>A transform entry for the map.</returns>
        protected static XElement GetTransformEntry(IList trans)
        {
            var elem = new XElement(xmlns + "entry");
            try
            {
                if (null == trans || trans.Count == 0)
                {
                    elem = new XElement(xmlns + "entry", new XText("N/A"));
                    return elem;
                }
                var list = new XElement(xmlns + "list");
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
            catch (Exception ex)
            {
                HandleException("GetTransformEntry", ex);
            }
            return elem;
        }

        /// <summary>
        /// Generic exception handler for writing exceptions into the output.
        /// </summary>
        /// <param name="title">Where the exception occurred.</param>
        /// <param name="ex">The exception.</param>
        protected static void HandleException(string title, Exception ex)
        {
            PrintLine("[{0}] {1}: {2}\r\n{3}", title, ex.GetType(), ex.Message, ex.StackTrace);
        }

        private static XElement getElement(string elemName)
        {
            var xe = new XElement(xmlns + elemName, new XAttribute("xmlns", xmlns),
                                       new XAttribute(XNamespace.Xmlns + "xlink", xlink));
            return xe;
        }

        /// <summary>
        /// Get a new topic.
        /// </summary>
        /// <param name="contentTitle">The topic title.</param>
        /// <returns>A topic document structure.</returns>
        public XElement NewTopicEntry(string contentTitle)
        {
            return new XElement("Topic",
                                new XAttribute("id", id),
                                new XAttribute("visible", "true"),
                                new XAttribute("title", contentTitle));
        }

        /// <summary>
        /// Transform a dictionary object to a table.
        /// </summary>
        /// <param name="dict">The dictionary</param>
        /// <param name="leftColumnTitle">The column name for the key.</param>
        /// <param name="rightColumnTitle">The column name for the value.</param>
        /// <returns>An element containing the table.</returns>
        protected static XElement DictionaryToTable(IDictionary dict, string leftColumnTitle, string rightColumnTitle)
        {
            var el = new XElement(xmlns + "table",
                                  new XElement(xmlns + "tableHeader",
                                               new XElement(xmlns + "row",
                                                            new XElement(xmlns + "entry", new XText(leftColumnTitle)),
                                                            new XElement(xmlns + "entry", new XText(rightColumnTitle)))));
            foreach (DictionaryEntry entry in dict)
            {
                var ex = new XElement(xmlns + "row",
                                      new XElement(xmlns + "entry",
                                                   new XText(entry.Key as string ?? "(N/A)")),
                                      new XElement(xmlns + "entry",
                                                   new XText(entry.Value as string ?? "(N/A)")));
                el.Add(ex);
            }
            return el;
        }

        /// <summary>
        /// Transform an <see cref="ICollection"/> into a list element.
        /// </summary>
        /// <param name="coll">The collection.</param>
        /// <returns>The list element.</returns>
        protected static XElement CollectionToList(ICollection coll)
        {
            var list = new XElement(xmlns + "list");
            foreach (string name in coll)
            {
                list.Add(new XElement(xmlns + "listItem", new XText(name)));
            }
            return list;
        }

        #region Implementation of IObservable<TopicFile>

        /// <summary>
        /// Subscribes an observer to the observable sequence.
        /// </summary>
        public IDisposable Subscribe(IObserver<TopicFile> observer)
        {
            this.observer = observer;
            return this;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!UsingParallel) return;
            do
            {
                PrintLine("Blocking Dispose() call for {0} while waiting for parallel loop to complete.",tokenId);
                Thread.Sleep(100);
            } while (!loopResult.IsCompleted);
            return;
        }

        #endregion
    }
}