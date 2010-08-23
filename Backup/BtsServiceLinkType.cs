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
    internal class BtsServiceLinkType : BtsBaseComponent
    {
        private readonly List<BtsRoleDeclaration> _roleDecs = new List<BtsRoleDeclaration>();

        /// <summary>
        /// TypeModifier
        /// </summary>
        private string _modifier;

        public BtsServiceLinkType(XmlReader reader)
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
                        if (valName.Equals("Name"))
                            _name = val;
                        else if (valName.Equals("Signal"))
                            _signal = Convert.ToBoolean(val);
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else if (valName.Equals("TypeModifier"))
                            _modifier = val;
                        else
                        {
                            Debug.WriteLine("[BtsServiceLinkType.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("RoleDeclaration"))
                        _roleDecs.Add(new BtsRoleDeclaration(reader.ReadSubtree()));
                    else
                    {
                        Debug.WriteLine("[BtsServiceLinkType.ctor] unhandled element " + reader.GetAttribute("Value"));
                        Debugger.Break();
                    }
                }
            }
            reader.Close();
        }

        public string Modifier
        {
            get { return _modifier; }
        }
    }

    public class BtsRoleDeclaration : BtsBaseComponent
    {
        private readonly List<BtsPortTypeRef> _portRefs = new List<BtsPortTypeRef>();

        public BtsRoleDeclaration(XmlReader reader)
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
                        if (valName.Equals("Name"))
                            _name = val;
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else if (valName.Equals("Signal"))
                            _signal = Convert.ToBoolean(val);
                        else
                        {
                            Debug.WriteLine("[BtsRoleDeclaration.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("PortTypeRef"))
                        _portRefs.Add(new BtsPortTypeRef(reader.ReadSubtree()));
                    else
                    {
                        Debug.WriteLine("[BtsRoleDeclaration.ctor] unhandled element " + reader.GetAttribute("Value"));
                        Debugger.Break();
                    }
                }
            }
            reader.Close();
        }

        public List<BtsPortTypeRef> PortTypeRefs
        {
            get { return _portRefs; }
        }
    }

    public class BtsPortTypeRef : BtsBaseComponent
    {
        private readonly string _ref;

        public BtsPortTypeRef(XmlReader reader)
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
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else if (valName.Equals("Name"))
                            _name = val;
                        else if (valName.Equals("Signal"))
                            _signal = Convert.ToBoolean(val);
                        else
                        {
                            Debug.WriteLine("[BtsPortTypeRef.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsPortTypeRef.ctor] unhandled element " + reader.GetAttribute("Value"));
                    Debugger.Break();
                }
            }
            reader.Close();
        }

        public string Ref
        {
            get { return _ref; }
        }
    }
}