// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
    /// <summary>
    /// BtsCorrelationDeclaration is a child object of BtsServiceDeclaration
    /// </summary>
    public class BtsCorrelationDeclaration : BtsBaseComponent, IBtsCorrelationDeclaration
    {
        private MessageDirection _paramType;
        private List<BtsStatementRef> _statementRefs = new List<BtsStatementRef>();
        private string _type;

        public BtsCorrelationDeclaration(XmlReader reader)
            : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    break;

                if (reader.Name.Equals("om:Property"))
                    GetReaderProperties(reader.GetAttribute("Name"), reader.GetAttribute("Value"));
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("StatementRef"))
                        _statementRefs.Add(new BtsStatementRef(reader.ReadSubtree()));
                    else
                    {
                        Debug.WriteLine("[BtsCorrelationDeclaration.ctor] unhandled om:Property " +
                                        reader.GetAttribute("Name"));
                        Debugger.Break();
                    }
                }
            }
        }

        public string CorrelationType
        {
            get { return _type; }
        }

        public MessageDirection ParameterType
        {
            get { return _paramType; }
        }

        public List<BtsStatementRef> StatementReferences
        {
            get { return _statementRefs; }
        }

        internal new void GetReaderProperties(string xmlName, string xmlValue)
        {
            if (!base.GetReaderProperties(xmlName, xmlValue))
            {
                if (xmlName.Equals("ParamDirection"))
                    _paramType = GetMessageDirection(xmlValue);
                else if (xmlName.Equals("AnalystComments"))
                    _comments = xmlValue;
                else if (xmlName.Equals("Type"))
                    _type = xmlValue;
                else
                {
                    Debug.WriteLine("[BtsCorrelationDeclaration.GetReaderProperties] unhandled om:Property " + xmlName);
                    Debugger.Break();
                }
            }
        }
    }

    public class BtsCorrelationType : BtsBaseComponent, IBtsCorrelationType
    {
        private readonly string _modifier;
        private readonly BtsPropertyRef _propRef;

        public BtsCorrelationType(XmlReader reader)
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
                        if (reader.GetAttribute("Name").Equals("TypeModifier"))
                            _modifier = reader.GetAttribute("Value");
                        else
                            Debug.WriteLine("[BtsPropertyRef.ctor] unhandled om:Property " + reader.GetAttribute("Name"));
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("PropertyRef"))
                        _propRef = new BtsPropertyRef(reader.ReadSubtree());
                    else
                    {
                        Debug.WriteLine("[BtsPropertyRef.ctor] unhandled element " + reader.Name);
                        Debugger.Break();
                    }
                }
                else
                {
                    Debug.WriteLine("[BtsPropertyRef.ctor] unhandled element " + reader.Name);
                    Debugger.Break();
                }
            }
        }

        public string Modifier
        {
            get { return _modifier; }
        }

        public BtsPropertyRef PropertyRef
        {
            get { return _propRef; }
        }
    }

    /// <summary>
    /// BtsPropertyRef is a child of BtsCorrelationType
    /// </summary>
    public class BtsPropertyRef : BtsBaseComponent, IBtsPropertyRef
    {
        private string _ref;

        public BtsPropertyRef(XmlReader reader)
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
                        if (reader.GetAttribute("Name").Equals("Ref"))
                            _ref = reader.GetAttribute("Value");
                        else
                        {
                            Debug.WriteLine("[BtsPropertyRef.ctor] unhandled om:Property " + reader.GetAttribute("Name"));
                            Debugger.Break();
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("[BtsPropertyRef.ctor] unhandled element " + reader.Name);
                    Debugger.Break();
                }
            }
        }

        public string Ref
        {
            get { return _ref; }
        }
    }

    /// <summary>
    /// BtsStatementRef is a child of BtsCorrelationDeclaration
    /// </summary>
    public class BtsStatementRef : BtsBaseComponent, IBtsStatementRef
    {
        private bool _initializes;
        private string _ref;

        public BtsStatementRef(XmlReader reader)
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
                        if (reader.GetAttribute("Name").Equals("Initializes"))
                            _initializes = Convert.ToBoolean(reader.GetAttribute("Value"));
                        else if (reader.GetAttribute("Name").Equals("Ref"))
                            _ref = reader.GetAttribute("Value");
                        else
                            Debug.WriteLine("[BtsStatementRef.ctor] unhandled om:Property " +
                                            reader.GetAttribute("Name"));
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                }
                else
                    Debug.WriteLine("[BtsStatementRef.ctor] unhandled element " + reader.Name);
            }
        }

        public string Ref
        {
            get { return _ref; }
        }

        public bool Initializes
        {
            get { return _initializes; }
        }
    }
}