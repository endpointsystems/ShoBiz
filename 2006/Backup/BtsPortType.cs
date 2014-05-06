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
    /// PortType is a top-level child of an orchestration. It contains an OperationDeclaration, which in turn contains a MessageRef
    /// </summary>
    internal class BtsPortType : BtsBaseComponent, IBtsPortType
    {
        /// <summary>
        /// TypeModifier (NOTE: this property is used by orchestration as well - if multiple others, we need to move
        /// </summary>
        private string _mod = String.Empty;

        /// <summary>
        /// OperationDeclaration found within PortType
        /// </summary>
        private List<BtsOperationDeclaration> _opDecs = new List<BtsOperationDeclaration>();

        /// <summary>
        /// Synchronous
        /// </summary>
        private bool _sync = false;

        public BtsPortType(XmlReader reader)
            : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    break;
                if (reader.Name.Equals("om:Property"))
                {
                    string valName = reader.GetAttribute("Name");
                    string val = reader.GetAttribute("Value");
                    if (!GetReaderProperties(valName, val))
                    {
                        if (valName.Equals("Synchronous"))
                            _sync = Convert.ToBoolean(val);
                        else if (valName.Equals("TypeModifier"))
                            _mod = val;
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else
                        {
                            Debug.WriteLine("[BtsPortType.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("OperationDeclaration"))
                        _opDecs.Add(new BtsOperationDeclaration(reader.ReadSubtree()));
                    else
                    {
                        Debug.WriteLine("[BtsPortType.ctor] unhandled element " + reader.GetAttribute("Value"));
                        Debugger.Break();
                    }
                }
                else
                    continue;
            }
            reader.Close();
        }

        public bool Signal
        {
            get { return _signal; }
        }

        public string Modifier
        {
            get { return _mod; }
        }

        public bool Synchronous
        {
            get { return _sync; }
        }

        public List<BtsOperationDeclaration> OperationDeclarations
        {
            get { return _opDecs; }
        }
    } //BtsPortType

    /// <summary>
    /// BtsOperationDeclaration is a single-child of BtsPortType.
    /// </summary>
    public class BtsOperationDeclaration : BtsBaseComponent, IBtsOperationDeclaration
    {
        private readonly List<BtsMessageRef> _msgRefs = new List<BtsMessageRef>();
        private readonly OperationType _opType;

        public BtsOperationDeclaration(XmlReader reader)
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
                        if (reader.GetAttribute("Name").Equals("OperationType"))
                            _opType = DetermineOpType(reader.GetAttribute("Value"));
                        else
                        {
                            Debug.WriteLine("[BtsOperationDeclaration.ctor] unhandled property " +
                                            reader.GetAttribute("Name"));
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("MessageRef"))
                        _msgRefs.Add(new BtsMessageRef(reader.ReadSubtree()));
                }
                else
                    continue;
            }
            reader.Close();
        }

        public OperationType OperationType
        {
            get { return _opType; }
        }

        public bool Signal
        {
            get { return _signal; }
        }

        public List<BtsMessageRef> MessageRef
        {
            get { return _msgRefs; }
        }

        private OperationType DetermineOpType(string opType)
        {
            Debug.WriteLine("[BtsPortType.DetermineOpType] Operation Type: " + opType);

            if (opType.Equals("OneWay"))
                return OperationType.OneWay;
            if (opType.Equals("RequestResponse"))
                return OperationType.RequestResponse;
            Debug.WriteLine("ERROR! OperationType " + opType +
                            " not supported by OperationType enum, and needs to be added!!");
#if DEBUG
            Debug.Fail("ERROR! OperationType " + opType +
                       " not supported by OperationType enum, and needs to be added!!");
#endif
            return OperationType.None;
        }
    } //BtsOperationDeclaration

    /// <summary>
    /// BtsMessageRef is a single child of BtsOperationDeclaration.
    /// </summary>
    public class BtsMessageRef : BtsBaseComponent, IBtsMessageRef
    {
        private string _ref;


        public BtsMessageRef(XmlReader reader)
            : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    break;
                if (reader.Name.Equals("om:Property"))
                {
                    string valName = reader.GetAttribute("Name");
                    string val = reader.GetAttribute("Value");
                    if (!GetReaderProperties(valName, val))
                    {
                        if (valName.Equals("Ref"))
                            _ref = val;
                        else
                        {
                            Debug.WriteLine("[BtsMessageRef.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsMessageRef.ctor] unhandled element " + reader.GetAttribute("Type"));
                    Debugger.Break();
                }
                else
                    continue;
            }
            reader.Close();
        }

        public string Ref
        {
            get { return _ref; }
        }

        public bool Signal
        {
            get { return _signal; }
        }
    }

    public class BtsMessagePartRef : BtsBaseComponent, IBtsMessagePartRef
    {
        private string _messageRef;
        private string _partRef;

        public BtsMessagePartRef(XmlReader reader)
            : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    break;
                if (reader.Name.Equals("om:Property"))
                {
                    string valName = reader.GetAttribute("Name");
                    string val = reader.GetAttribute("Value");
                    if (!GetReaderProperties(valName, val))
                    {
                        if (valName.Equals("MessageRef"))
                            _messageRef = val;
                        else if (valName.Equals("PartRef"))
                            _partRef = val;
                        else
                        {
                            Debug.WriteLine("[BtsMessagePartRef.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsMessagePartRef.ctor] unhandled element " + reader.GetAttribute("Type"));
                    Debugger.Break();
                }
                else
                    continue;
            }
            reader.Close();
        }

        public string PartRef
        {
            get { return _partRef; }
        }

        public string MessageRef
        {
            get { return _messageRef; }
        }
    }
}