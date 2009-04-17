using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// This class takes the topic hierarchy from the objects and saves it to a 
    /// </summary>
    public class ContentFile
    {
        private static ContentFile contentFile;
        private static XDocument doc;
        public static ContentFile GetContentFile()
        {
            if (contentFile == null)
            {
                contentFile = new ContentFile();
                doc = new XDocument(
                    new XElement("Topics")
                    );
            }
            return contentFile;
        }
        /// <summary>
        /// Adds new <c>&lt;Topic&gt;</c> entries to the content file root element (<c>&lt;Topics&gt;</c>).
        /// </summary>
        /// <param name="appTopic">The <c>&lt;Topic&gt;</c> entry to add.</param>
        public void AddApplicationTopic(XElement appTopic)
        {
            if (doc.Root != null) doc.Root.Add(appTopic);
        }

        public void Save(string fullPath)
        {
            doc.Save(fullPath);
        }

    }
}