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
    /// ServiceLink - ref for ServiceLinkDeclaration (ports?) in orchestration
    /// ServiceDeclaration_ServiceLinkDeclaration (parentlink)
    /// </summary>
    public class BtsServiceLinkDeclaration : BtsBaseComponent
    {
        #region vars

        private readonly MessageDirection _direction;

        /// <summary>
        /// DeliveryNotification
        /// </summary>
        private readonly string _notification;

        /// <summary>
        /// OrderedDelivery
        /// </summary>
        private readonly bool _ordered;

        /// <summary>
        /// Orientation
        /// </summary>
        private readonly string _orientation;

        /// <summary>
        /// PortIndex
        /// </summary>
        private readonly short _portIdx;

        /// <summary>
        /// PortModifier
        /// </summary>
        private readonly string _portModifier;

        /// <summary>
        /// RoleName
        /// </summary>
        private readonly string _roleName;

        private readonly string _type;

        #endregion

        public BtsServiceLinkDeclaration(XmlReader reader)
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
                        if (valName.Equals("Orientation"))
                            _orientation = val;
                        else if (valName.Equals("RoleName"))
                            _roleName = val;
                        else if (valName.Equals("PortIndex"))
                            _portIdx = Convert.ToInt16(val);
                        else if (valName.Equals("PortModifier"))
                            _portModifier = val;
                        else if (valName.Equals("OrderedDelivery"))
                            _ordered = Convert.ToBoolean(val);
                        else if (valName.Equals("DeliveryNotification"))
                            _notification = val;
                        else if (valName.Equals("Type"))
                            _type = val;
                        else if (valName.Equals("ParamDirection"))
                            _direction = GetMessageDirection(val);
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else
                        {
                            Debug.WriteLine("[ServiceLink.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[ServiceLink.ctor] unhandled element " + reader.GetAttribute("Value"));
                    Debugger.Break();
                }
            }
            reader.Close();
        }


        public MessageDirection Direction
        {
            get { return _direction; }
        }

        public string Type
        {
            get { return _type; }
        }

        public string DeliveryNotification
        {
            get { return _notification; }
        }

        public bool IsOrdered
        {
            get { return _ordered; }
        }

        public string RoleName
        {
            get { return _roleName; }
        }

        public string PortModifier
        {
            get { return _portModifier; }
        }

        public short PortIndex
        {
            get { return _portIdx; }
        }

        public string Orientation
        {
            get { return _orientation; }
        }
    }
}