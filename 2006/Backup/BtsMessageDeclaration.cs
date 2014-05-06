// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 

#region

using System.Diagnostics;
using System.Xml;

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
    /// <summary>
    /// BtsMessageDeclaration is a child of BtsServiceDeclaration and is used to declare message types
    /// </summary>
    public class BtsMessageDeclaration : BtsBaseComponent, IBtsMessageDeclaration
    {
        private MessageDirection _paramDirection;
        private string _type;

        public BtsMessageDeclaration(XmlReader reader)
            : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    break;
                if (reader.Name.Equals("om:Property"))
                {
                    if (!GetReaderProperties(reader.GetAttribute("Name"), reader.GetAttribute("Value")))
                    {
                        if (reader.GetAttribute("Name").Equals("ParamDirection"))
                            _paramDirection = GetMessageDirection(reader.GetAttribute("Value"));
                        else if (reader.GetAttribute("Name").Equals("Type"))
                            _type = reader.GetAttribute("Value");
                        else if (reader.GetAttribute("Name").Equals("AnalystComments"))
                            _comments = reader.GetAttribute("Value");
                        else
                        {
                            Debug.WriteLine("[BtsMessageDeclaration.ctor] unhandled property " +
                                            reader.GetAttribute("Name"));
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsMessageDeclaration.ctor] unhandled element " + reader.GetAttribute("Type") +
                                    " not supported in class! (TODO)");
                }
            }
            reader.Close();
        }

        public string Type
        {
            get { return _type; }
        }

        public MessageDirection ParamDirection
        {
            get { return _paramDirection; }
        }
    }
}