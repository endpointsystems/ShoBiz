// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 

#region

using System;
using System.Diagnostics;
using System.Xml;

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
    /// <summary>
    /// BtsPortDeclaration is a child of BtsServiceDeclaration
    /// </summary>
    public class BtsPortDeclaration : BtsBaseComponent, IBtsPortDeclaration
    {
        private BtsBindingAttribute _binding;
        private MessageDirection _direction;
        private string _modifier;
        private string _notification;
        private bool _orderedDelivery;
        private string _orientation;
        private short _portIdx;
        private string _type;
        private bool _webPort;

        public BtsPortDeclaration(XmlReader reader) : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    break;

                int i = 0;
                if (reader.Name.Equals("om:Property"))
                    GetReaderProperties(reader.GetAttribute("Name"), reader.GetAttribute("Value"));
                else if (reader.AttributeCount != 0 && reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("LogicalBindingAttribute"))
                    {
                        if (i < 1)
                            _binding = new BtsLogicalBindingAttribute(reader.ReadSubtree());
                        else
                            Debugger.Break(); //checking to make sure only 
                    }
                    else if (reader.GetAttribute("Type").Equals("DirectBindingAttribute"))
                    {
                        if (i < 1)
                            _binding = new BtsDirectBindingAttribute(reader.ReadSubtree());
                        else
                            Debugger.Break();
                    }
                    else if (reader.GetAttribute("Type").Equals("PhysicalBindingAttribute"))
                    {
                        if (i < 1)
                            _binding = new BtsPhysicalBindingAttribute(reader.ReadSubtree());
                        else
                            Debugger.Break();
                    }
                    else
                    {
                        Debug.WriteLine("[BtsPortDeclaration.ctor] unhandled element type: " +
                                        reader.GetAttribute("Type"));
                        Debugger.Break();
                    }
                }
            }
            reader.Close();
        }

        public string Type
        {
            get { return _type; }
        }

        public BtsBindingAttribute BindingAttribute
        {
            get { return _binding; }
        }

        public string PortModifier
        {
            get { return _modifier; }
        }

        public MessageDirection ParamDirection
        {
            get { return _direction; }
        }

        public string DeliveryNotification
        {
            get { return _notification; }
        }

        public bool OrderedDelivery
        {
            get { return _orderedDelivery; }
        }

        public bool IsWebPort
        {
            get { return _webPort; }
        }

        public int PortIndex
        {
            get { return _portIdx; }
        }

        public string Orientation
        {
            get { return _orientation; }
        }

        internal new void GetReaderProperties(string xmlName, string xmlValue)
        {
            if (!base.GetReaderProperties(xmlName, xmlValue))
            {
                if (xmlName.Equals("PortModifier"))
                    _modifier = xmlValue;
                else if (xmlName.Equals("Orientation"))
                    _orientation = xmlValue;
                else if (xmlName.Equals("PortIndex"))
                    _portIdx = Convert.ToInt16(xmlValue);
                else if (xmlName.Equals("IsWebPort"))
                    _webPort = Convert.ToBoolean(xmlValue);
                else if (xmlName.Equals("OrderedDelivery"))
                    _orderedDelivery = Convert.ToBoolean(xmlValue);
                else if (xmlName.Equals("DeliveryNotification"))
                    _notification = xmlValue;
                else if (xmlName.Equals("ParamDirection"))
                    _direction = GetMessageDirection(xmlValue);
                else if (xmlName.Equals("Type"))
                    _type = xmlValue;
                else if (xmlName.Equals("AnalystComments"))
                    _comments = xmlValue;
                else
                    Debug.WriteLine("[BtsPortDeclaration.GetReaderProperties] unhandled om:Property " + xmlName);
                return;
            }
        }
    }

    public class BtsBindingAttribute : BtsBaseComponent
    {
        internal BindingAttributeType _bindType;

        public BtsBindingAttribute(XmlReader reader) : base(reader)
        {
        }

        public BindingAttributeType BindingAttributeType
        {
            get { return _bindType; }
        }

        public bool Signal
        {
            get { return _signal; }
        }
    }

    public class BtsLogicalBindingAttribute : BtsBindingAttribute
    {
        public BtsLogicalBindingAttribute(XmlReader reader)
            : base(reader)
        {
            _bindType = BindingAttributeType.Logical;
            //no, really - nothing to do here
            reader.Close();
        }
    }

    public class BtsDirectBindingAttribute : BtsBindingAttribute
    {
        private readonly string _dirBindType;
        private readonly string _partnerPort;
        private readonly string _partnerSvc;

        public BtsDirectBindingAttribute(XmlReader reader) : base(reader)
        {
            _bindType = BindingAttributeType.Direct;

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
                        if (valName.Equals("PartnerPort"))
                            _partnerPort = val;
                        else if (valName.Equals("PartnerService"))
                            _partnerSvc = val;
                        else if (valName.Equals("DirectBindingType"))
                            _dirBindType = val;
                        else if (valName.Equals("Signal"))
                            _signal = Convert.ToBoolean(val);
                        else
                        {
                            Debug.WriteLine("[BtsDirectBindingAttribute.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsDirectBindingAttribute.ctor] unhandled element " + reader.GetAttribute("Value"));
                    Debugger.Break();
                }
            }
            reader.Close();
        }


        public string DirectBindingType
        {
            get { return _dirBindType; }
        }

        public string PartnerService
        {
            get { return _partnerSvc; }
        }

        public string PartnerPort
        {
            get { return _partnerPort; }
        }
    }

    public class BtsPhysicalBindingAttribute : BtsBindingAttribute
    {
        /// <summary>
        /// <see cref="IsDynamic"/>
        /// </summary>
        private readonly bool _dynamic;

        /// <summary>
        /// <see cref="InPipeline"/>
        /// </summary>
        private readonly string _inPipe;

        /// <summary>
        /// <see cref="OutPipeline"/>
        /// </summary>
        private string _outPipe;

        /// <summary>
        /// TransportType
        /// </summary>
        private string _transType;

        /// <summary>
        /// URI
        /// </summary>
        private string _uri;

        public BtsPhysicalBindingAttribute(XmlReader reader)
            : base(reader)
        {
            _bindType = BindingAttributeType.Physical;

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
                        if (valName.Equals("InPipeline"))
                            _inPipe = val;
                        else if (valName.Equals("OutPipeline"))
                            _outPipe = val;
                        else if (valName.Equals("TransportType"))
                            _transType = val;
                        else if (valName.Equals("URI"))
                            _uri = val;
                        else if (valName.Equals("IsDynamic"))
                            _dynamic = Convert.ToBoolean(val);
                        else
                        {
                            Debug.WriteLine("[BtsPhysicalBindingAttribute.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsPhysicalBindingAttribute.ctor] unhandled element " +
                                    reader.GetAttribute("Value"));
                    Debugger.Break();
                }
            }
            reader.Close();
        }

        public bool IsDynamic
        {
            get { return _dynamic; }
        }

        public string URI
        {
            get { return _uri; }
        }

        public string TransportType
        {
            get { return _transType; }
        }

        public string OutPipeline
        {
            get { return _outPipe; }
        }

        public string InPipeline
        {
            get { return _inPipe; }
        }
    }
}