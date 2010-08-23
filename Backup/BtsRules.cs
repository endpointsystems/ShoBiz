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
    public class BtsCallRulesShape : BtsBaseComponent
    {
        private readonly List<BtsRulesParameterRef> _params = new List<BtsRulesParameterRef>();
        private readonly string _policyName;
        private readonly short _policyVersion;

        public BtsCallRulesShape(XmlReader reader)
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
                        if (valName.Equals("PolicyName"))
                            _policyName = val;
                        else if (valName.Equals("PolicyVersion"))
                            _policyVersion = Convert.ToInt16(val);
                        else if (valName.Equals("Name"))
                            _name = val;
                        else if (valName.Equals("Signal"))
                            _signal = Convert.ToBoolean(val);
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else
                        {
                            Debug.WriteLine("[BtsCallRulesShape.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("RulesParameterRef"))
                        _params.Add(new BtsRulesParameterRef(reader.ReadSubtree()));
                    else
                    {
                        Debug.WriteLine("[BtsCallRulesShape.ctor] unhandled element " + reader.GetAttribute("Value"));
                        Debugger.Break();
                    }
                }
            }
            reader.Close();
        }

        public short PolicyVersion
        {
            get { return _policyVersion; }
        }

        public string PolicyName
        {
            get { return _policyName; }
        }
    }

    public class BtsRulesParameterRef : BtsBaseComponent
    {
        private readonly string _alias;
        private readonly string _ref;

        public BtsRulesParameterRef(XmlReader reader)
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
                        if (valName.Equals("Reference"))
                            _ref = val;
                        else if (valName.Equals("Alias"))
                            _alias = val;
                        else
                        {
                            Debug.WriteLine("[BtsRulesParameterRef.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsRulesParameterRef.ctor] unhandled element " + reader.GetAttribute("Value"));
                    Debugger.Break();
                }
            }
            reader.Close();
        }

        public string Alias
        {
            get { return _alias; }
        }

        public string Reference
        {
            get { return _ref; }
        }
    }
}