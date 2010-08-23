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
    public class BtsMultiPartMessageType : BtsBaseComponent, IBtsMultiPartMessageType
    {
        private readonly string _modifier;
        private readonly List<BtsPartDeclaration> _parts = new List<BtsPartDeclaration>();

        public BtsMultiPartMessageType(XmlReader reader)
            : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    continue;
                if (reader.Name.Equals("om:Property"))
                {
                    string valName = reader.GetAttribute("Name");
                    string val = reader.GetAttribute("Value");
                    if (!GetReaderProperties(valName, val))
                    {
                        if (valName.Equals("TypeModifier"))
                            _modifier = val;
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else
                        {
                            Debug.WriteLine("[BtsMultiPartMessageType.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("PartDeclaration"))
                        _parts.Add(new BtsPartDeclaration(reader.ReadSubtree()));
                }
                else
                    continue;
            }
            reader.Close();
        }

        public string Modifier
        {
            get { return _modifier; }
        }

        public bool Signal
        {
            get { return _signal; }
        }


        public List<BtsPartDeclaration> PartDeclarations
        {
            get { return _parts; }
        }
    } //BtsMultiPartMessageType

    public class BtsPartDeclaration : BtsBaseComponent, IBtsPartDeclaration
    {
        private readonly bool _bodyPart;
        private readonly string _className;

        public BtsPartDeclaration(XmlReader reader)
            : base(reader)
        {
            while (reader.Read())
            {
                if (!reader.HasAttributes)
                    continue;
                if (reader.Name.Equals("om:Property"))
                {
                    string valName = reader.GetAttribute("Name");
                    string val = reader.GetAttribute("Value");
                    if (!GetReaderProperties(valName, val))
                    {
                        if (valName.Equals("ClassName"))
                        {
                            _className = val;
                        }
                        else if (valName.Equals("IsBodyPart"))
                            _bodyPart = Convert.ToBoolean(val);
                        else
                        {
                            Debug.WriteLine("[BtsPartDeclaration.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
            }
            reader.Close();
        }

        public bool Signal
        {
            get { return _signal; }
        }

        public bool IsBodyPart
        {
            get { return _bodyPart; }
        }

        public string ClassName
        {
            get { return _className; }
        }
    }
} //namespace