using System.Xml.Linq;

namespace EndpointSystems.BizTalk.Documentation
{
    public class TokenFile
    {
        private static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";
        private static readonly XNamespace ddue = "http://ddue.schemas.microsoft.com/authoring/2003/5";
        private static TokenFile tokenFile;
        public XDocument doc;

        public TokenFile()
        {

            doc = new XDocument(
                new XElement("content",
                             new XAttribute(XNamespace.Xmlns + "ddue", ddue),
                             new XAttribute(XNamespace.Xmlns + "xlink", xlink))
                );
        }

        public static TokenFile GetTokenFile()
        {
            if (tokenFile == null)
            {
                tokenFile = new TokenFile();
            }

            return tokenFile;
        }

        public void AddTopicToken(string tokenId, string topicId)
        {
            if (doc.Root != null)
                doc.Root.Add(
                    new XElement("item",
                                 new XAttribute("id", tokenId),
                                 new XElement(ddue + "link", new XAttribute(xlink + "href", topicId))
                        ));
        }

        public enum ImagePlacement
        {
            near,
            center,
            far
        } ;

        public enum CaptionPlacement
        {
            before,
            after
        } ;

        public void AddImageToken(string tokenId, string caption, CaptionPlacement captionPlacement, string imageId, ImagePlacement imagePlacement)
        {
            /*
             * <mediaLink>
                <caption>Caption Before</caption>
                <image placement="center" xlink:href="6be7079d-a9d8-4189-9021-0f72d1642beb"/>
                </mediaLink>
             */

            XElement media = new XElement(ddue + "mediaLink");
            if (null != caption)
            {
                media.Add(new XElement(ddue + "caption",
                                new XAttribute("placement",captionPlacement.ToString())));

                media.Add(new XElement(ddue + "image",
                                            new XAttribute("placement",imagePlacement.ToString()),
                                            new XAttribute(xlink + "href",imageId)));
                if (doc.Root != null)
                    doc.Root.Add(new XElement("item",
                                              new XAttribute("id",tokenId),
                                              media));
            }

        }

        public void Save(string tokenFilePath)
        {
            doc.Save(tokenFilePath);
        }
    }
}