// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 
#define reader
//#define DOM

#region

using System;
using System.Diagnostics;
using System.Xml;

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
    /// <summary>
    /// Base component - parent component to all orchestration shapes
    /// </summary>
    public class BtsBaseComponent : IBtsShape
    {
        #region vars

        internal bool _report = true;
        internal ParentLink _parentLink;
        //internal string _parentLinkRef;
        internal string _desc;
        internal bool _signal;
        internal string _name;
        internal string _scope;
        internal string _comments;
        internal Guid _oid;
        internal string _typeId;
#if reader
//        internal XmlReader _reader;
#endif
        //internal XmlNode _baseNode;

        #endregion

        #region properties

        /// <summary>
        /// Initialize base component and extract base shape properties 
        /// </summary>
        /// <param name="shape">XmlNode for child shape created</param>
        internal BtsBaseComponent(XmlNode shape)
        {
            return;
        }

        internal BtsBaseComponent(XmlReader reader)
        {
            try
            {
                //we only want to do this if we haven't initialized yet
                if (reader.HasAttributes && reader.AttributeCount < 1)
                    reader.Read();

                _oid = new Guid(reader.GetAttribute("OID"));
                _parentLink = GetParentLink(reader.GetAttribute("ParentLink"));
                _typeId = reader.GetAttribute("Type");
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("[BtsBaseComponent.ctor] An exception of type " + e.GetType().ToString() +
                                " has occured: " + e.Message);
                Debugger.Break();
#endif
            }
        }

        internal BtsBaseComponent()
        {
        }

        public string Description
        {
            get { return _desc; }
        }

        public string Name
        {
            get { return _name; }
        }


        public string Scope
        {
            get { return _scope; }
        }

        /// <summary>
        /// simple existence test. used by child objects.
        /// </summary>
        /// <param name="node">XmlNode to test.</param>
        /// <param name="nodeName">name of node being tested. Used for exception throwing.</param>
        /// <param name="source">name of source throwing the (possible) exception.</param>
        internal void NullNodeExcTest(XmlNode node, string nodeName, string source)
        {
            if (null == node)
                throw new NullOrchNodeException(nodeName, _name, source);
        }

        public ParentLink ParentLink
        {
            get { return _parentLink; }
        }

        #endregion

        /// <summary>
        /// check for and set base orchestration om:Property values
        /// </summary>
        /// <param name="xmlName">name of the om:Property evaluated</param>
        /// <param name="xmlValue">value associated with Value attribute</param>
        /// <returns><c>true</c> if we set a property in our parent function.</returns>
        protected bool GetReaderProperties(string xmlName, string xmlValue)
        {
            bool ret = true;

            if (xmlName.Equals("ReportToAnalyst"))
                _report = Convert.ToBoolean(xmlValue);
            else if (xmlName.Equals("Name"))
                _name = xmlValue;
            else if (xmlValue.Equals("AnalystComments"))
                _desc = xmlValue;
            else if (xmlName.Equals("Signal"))
                _signal = Convert.ToBoolean(xmlValue);
            else
                ret = false;
            return ret;
        }

        /// <summary>
        /// The following properties are picked up in BtsBaseComponent:
        /// 
        /// BaseNode attributes:
        /// OID
        /// ParentLink
        /// Type
        /// Signal
        /// 
        /// BaseNode child elements:
        /// ReportToAnalyst
        /// AnalystComments
        /// Name
        /// 
        /// </summary>
        internal void GetCommonOrchProperties()
        {
#if DOM
            _oid = new Guid(this._baseNode.Attributes["OID"].Value);            
            _typeId = this._baseNode.Attributes["Type"].Value;
            _report = this.GetNamedItemValueAsBool("om:Property[@Name='ReportToAnalyst']", String.Empty);
            
            XmlAttribute attr;

            //returns empty string if null, which can happen also if AnalystComments doesn't exist?
            this._desc = this.GetNamedItemValue("om:Property[@Name='AnalystComments']");            

            //shape name
            this._name = this.GetNamedItemValue("om:Property[@Name='Name']");

            this._signal = this.GetNamedItemValueAsBool("om:Property[@Name='Signal']");

            //parent link
            try
            {
                attr = this._baseNode.Attributes["ParentLink"];
                if (null == attr)
                {
                    _parentLinkRef = "(none)";
                    _parentLink = ParentLink.None;
                }
                else
                {
                    _parentLinkRef = attr.Value;
                    _parentLink = this.DetermineParentLink(_parentLinkRef);
                }

            }
            catch (Exception ee)
            {
                ///TODO: standardized exception handling here
#if DEBUG
                Debugger.Break();
#endif
            }
#endif //dom
        }

#if DOM
    /// <summary>
    /// return attribute values from orchestration nodes. If a node is null, an empty string is returned.
    /// </summary>
    /// <param name="xpath">xpath of the property to retrieve.</param>
    /// 
    /// <returns>string value of named attribute.</returns>
        protected string GetNamedItemValue(string xpath)
        {
            //_type = orchNode.SelectSingleNode("om:Property[@Name='Type']").Attributes.GetNamedItemValue("Value").Value;

            XmlNode HelperNode = this._baseNode.SelectSingleNode(xpath);
            if (null == HelperNode)
                return String.Empty;
            else
                return HelperNode.Attributes.GetNamedItem("Value").Value;            

        }

        protected bool GetNamedItemValueAsBool(string xpath)
        {
            return this.GetNamedItemValueAsBool(xpath, String.Empty);
        }
        protected bool GetNamedItemValueAsBool(string xpath, string namedItem)
        {
            string ret = this.GetNamedItemValue(xpath);
            
            if (null == ret)
                return false;
            else if (ret.Length > 0 && ret.ToUpper().Contains("TRUE"))
                return true;
            else
                return false;

        }

        protected int GetNamedItemAsInt(string xpath)
        {
            return this.GetNamedItemAsInt(xpath, String.Empty);
        }
        protected int GetNamedItemAsInt(string xpath, string namedItem)
        {
            string ret = this.GetNamedItemValue(xpath);
            if (ret.Length > 0)
                return Convert.ToInt32(ret);
            else
                return 0;

        }

        internal XmlNodeList SelectNodes(string xpath)
        {
            return _baseNode.SelectNodes(xpath);
        }

        internal XmlNode SelectNode(string xpath)
        {
            return _baseNode.SelectSingleNode(xpath);
        }
#endif

        internal IsolationType GetIsolation(string isolation)
        {
            if (isolation.Contains("Commit"))
                return IsolationType.ReadCommitted;
            if (isolation.Contains("Repeat"))
                return IsolationType.ReadRepeatable;
            return IsolationType.Serializable; //default
        }

        internal MessageDirection GetMessageDirection(string direction)
        {
            if (direction.ToUpper().Equals("IN"))
                return MessageDirection.In;
            if (direction.ToUpper().Equals("OUT"))
                return MessageDirection.Out;
            if (direction.ToUpper().Equals("INOUT"))
                return MessageDirection.InOut;
            return MessageDirection.Indeterminant;
        }

        internal ParentLink GetParentLink(string parentLink)
        {
            if (parentLink.Contains("Module"))
                return ParentLink.Module;
            if (parentLink.Contains("Service"))
                return ParentLink.Service;
            if (parentLink.Contains("Scope"))
                return ParentLink.Scope;
            if (parentLink.Contains("Port") || parentLink.Contains("PortDeclaration"))
                return ParentLink.Port;
            if (parentLink.Contains("Multipart"))
                return ParentLink.MultiPartMessage;
            if (parentLink.Contains("Construct_MessageRef"))
                return ParentLink.ConstructMessageRef;
            if (parentLink.Contains("DNFPredicate"))
                return ParentLink.FilterPredicate;
            if (parentLink.Contains("Transform_InputMessagePartRef"))
                return ParentLink.InputMessagePart;
            if (parentLink.Contains("Transform_OutputMessagePartRef"))
                return ParentLink.OutputMessagePart;
            if (parentLink.Contains("CallRules_RulesParameterRef"))
                return ParentLink.RulesParameterRef;
            if (parentLink.Contains("CorrelationType_PropertyRef"))
                return ParentLink.CorrelationProperty;
            if (parentLink.Contains("Operation"))
            {
                Debug.WriteLine("[BtsBaseComponent.GetParentLink] assigning 'Statement' enum to " + parentLink);
                return ParentLink.Operation;
            }
            if (parentLink.Contains("Statement"))
                return ParentLink.Statement;
            
            
#if DEBUG
                Debug.WriteLine("[BtsBaseComponent.GetParentLink] unhandled ParentLink enum " + parentLink);
                Debugger.Break();
#endif
                return ParentLink.Unknown;
        }
    } //BtsBaseComponent
}