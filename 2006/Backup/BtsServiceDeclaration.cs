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
    /// BtsServiceDeclaration is one of the top child nodes of Module (our BtsServiceBody class) which contains BtsMessageDeclarations and BtsServiceBody, 
    /// the 'heart' of the orchestration. 
    /// NOTE: This object can be null in the event of trying to parse Reference orchestrations!
    /// </summary>
    public class BtsServiceDeclaration : BtsBaseComponent
    {
        /// <summary>
        /// Compensation
        /// </summary>
        private BtsCompensation _comp;

        /// <summary>
        /// CorrelationDeclaration
        /// </summary>
        private List<BtsCorrelationDeclaration> _corrDecs = new List<BtsCorrelationDeclaration>();

        /// <summary>
        /// InitializedTransactionType
        /// </summary>
        private bool _initTxType;

        /// <summary>
        /// IsInvokable
        /// </summary>
        private bool _invokable;

        /// <summary>
        /// TypeModifier
        /// </summary>
        private string _modifier;

        /// <summary>
        /// MessageDeclaration
        /// </summary>
        private List<BtsMessageDeclaration> _msgDecs = new List<BtsMessageDeclaration>();

        /// <summary>
        /// PortDeclaration
        /// </summary>
        private List<BtsPortDeclaration> _portDecs = new List<BtsPortDeclaration>();

        /// <summary>
        /// ServiceLinkDeclaration
        /// </summary>
        private BtsServiceLinkDeclaration _sld;

        /// <summary>
        /// ServiceBody
        /// </summary>
        private BtsServiceBody _svcBody;

        /// <summary>
        /// TargetXMLNamespaceAttribute
        /// </summary>
        private BtsTargetXmlAttribute _target;

        /// <summary>
        /// LongRunningTransaction or AtomicTransaction  
        /// </summary>
        private BtsTx _tx;

        /// <summary>
        /// TransactionAttribute
        /// </summary>
        private BtsTransactionAttribute _txAtt;

        /// <summary>
        /// VariableDeclaration
        /// </summary>
        private List<BtsVariableDeclaration> _varDecs = new List<BtsVariableDeclaration>();

        public BtsServiceDeclaration(XmlReader reader)
            : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    continue;

                if (reader.Name.Equals("om:Property"))
                    GetReaderProperties(reader.GetAttribute("Name"), reader.GetAttribute("Value"));
                else if (reader.Name.Equals("om:Element"))
                {
                    string attr = reader.GetAttribute("Type");
                    if (attr.Equals("ServiceBody"))
                        _svcBody = new BtsServiceBody(reader.ReadSubtree());
                    else if (attr.Equals("PortDeclaration"))
                        _portDecs.Add(new BtsPortDeclaration(reader.ReadSubtree()));
                    else if (attr.Equals("CorrelationDeclaration"))
                        _corrDecs.Add(new BtsCorrelationDeclaration(reader.ReadSubtree()));
                    else if (attr.Equals("MessageDeclaration"))
                        _msgDecs.Add(new BtsMessageDeclaration(reader.ReadSubtree()));
                    else if (attr.Equals("VariableDeclaration"))
                        _varDecs.Add(new BtsVariableDeclaration(reader.ReadSubtree()));
                    else if (attr.Equals("LongRunningTransaction"))
                        _tx = new BtsLongRunningTx(reader.ReadSubtree());
                    else if (attr.Equals("AtomicTransaction"))
                        _tx = new BtsAtomicTx(reader.ReadSubtree());
                    else if (attr.Equals("TransactionAttribute"))
                        _txAtt = new BtsTransactionAttribute(reader.ReadSubtree());
                    else if (attr.Equals("Compensation"))
                        _comp = new BtsCompensation(reader.ReadSubtree());
                    else if (attr.Equals("ServiceLinkDeclaration"))
                        _sld = new BtsServiceLinkDeclaration(reader.ReadSubtree());
                    else if (attr.Equals("TargetXMLNamespaceAttribute"))
                        _target = new BtsTargetXmlAttribute(reader.ReadSubtree());
                    else
                    {
                        Debug.WriteLine("[BtsServiceDeclaration.ctor] unhandled element " + reader.GetAttribute("Type") +
                                        "!!!!");
                        Debugger.Break();
                    }
                }
            }
            reader.Close();
        }

        public BtsServiceLinkDeclaration Sld
        {
            get { return _sld; }
        }

        public bool Invokable
        {
            get { return _invokable; }
        }

        public bool InitTxType
        {
            get { return _initTxType; }
        }

        public BtsTargetXmlAttribute Target
        {
            get { return _target; }
        }

        public BtsServiceBody ServiceBody
        {
            get { return _svcBody; }
        }


        public BtsCompensation Compensation
        {
            get { return _comp; }
        }

        public BtsTransactionAttribute TransactionAttribute
        {
            get { return _txAtt; }
        }

        public BtsTx Transaction
        {
            get { return _tx; }
        }

        public List<BtsVariableDeclaration> VariableDeclarations
        {
            get { return _varDecs; }
        }

        public List<BtsPortDeclaration> PortDeclarations
        {
            get { return _portDecs; }
        }

        public List<BtsCorrelationDeclaration> CorrelationDeclarations
        {
            get { return _corrDecs; }
        }

        public List<BtsMessageDeclaration> MessageDeclarations
        {
            get { return _msgDecs; }
        }

        public string TypeModifier
        {
            get { return _modifier; }
        }

        internal new void GetReaderProperties(string xmlName, string xmlValue)
        {
            if (!base.GetReaderProperties(xmlName, xmlValue))
            {
                if (xmlName.Equals("InitializedTransactionType"))
                    _initTxType = Convert.ToBoolean(xmlValue);
                else if (xmlName.Equals("IsInvokable"))
                    _invokable = Convert.ToBoolean(xmlValue);
                else if (xmlName.Equals("TypeModifier"))
                    _modifier = xmlValue;
                else if (xmlName.Equals("AnalystComments"))
                    _comments = xmlValue;
                else
                    Debug.WriteLine("[BtsServiceDeclaration.GetReaderProperties] TODO: implement handler for " + xmlName);
            }
        }
    }
}