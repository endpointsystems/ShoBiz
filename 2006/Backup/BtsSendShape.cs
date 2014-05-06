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
    internal class BtsSendShape : BtsBaseComponent
    {
        private readonly string _msgName;
        private readonly string _opMsgName;
        private readonly string _opName;
        private readonly string _portName;
        private readonly string _svcLinkName;
        private readonly string _svcLinkPortTypeName;
        private readonly string _svcLinkRoleName;

        public BtsSendShape(XmlReader reader)
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
                        if (valName.Equals("PortName"))
                            _portName = val;
                        else if (valName.Equals("MessageName"))
                            _msgName = val;
                        else if (valName.Equals("OperationName"))
                            _opName = val;
                        else if (valName.Equals("OperationMessageName"))
                            _opMsgName = val;
                        else if (valName.Equals("ServiceLinkName"))
                            _svcLinkName = val;
                        else if (valName.Equals("ServiceLinkPortTypeName"))
                            _svcLinkPortTypeName = val;
                        else if (valName.Equals("ServiceLinkRoleName"))
                            _svcLinkRoleName = val;
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else
                        {
                            Debug.WriteLine("[BtsSendShape.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsSendShape.ctor] unhandled element " + reader.GetAttribute("Value"));
                    Debugger.Break();
                }
            }
            reader.Close();
        }


        public string ServiceLinkRoleName
        {
            get { return _svcLinkRoleName; }
        }

        public string ServiceLinkPortTypeName
        {
            get { return _svcLinkPortTypeName; }
        }

        public string ServiceLinkName
        {
            get { return _svcLinkName; }
        }

        public string OperationMessageName
        {
            get { return _opMsgName; }
        }

        public string OperationName
        {
            get { return _opName; }
        }

        public string MessageName
        {
            get { return _msgName; }
        }

        public string PortName
        {
            get { return _portName; }
        }
    }
}