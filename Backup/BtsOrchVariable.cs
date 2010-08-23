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
    /// generic container for extracting 
    /// </summary>
    public class BtsOrchVariable : BtsBaseComponent, IBtsOrchVariable
    {
        private readonly MessageDirection _direction;
        private readonly string _initVal;
        private readonly string _type;


        public BtsOrchVariable(XmlReader reader)
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
                        if (valName.Equals("Type"))
                            _type = val;
                        else if (valName.Equals("InitialValue"))
                            _initVal = val;
                        else if (valName.Equals("UseDefaultConstructor"))
                            Convert.ToBoolean(val);
                        else if (valName.Equals("ParamDirection"))
                            _direction = GetMessageDirection(val);
                        else if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else
                        {
                            Debug.WriteLine("[BtsOrchVariable.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    Debug.WriteLine("[BtsOrchVariable.ctor] unhandled element " + reader.GetAttribute("Value"));
                    Debugger.Break();
                }
            }
            reader.Close();
        }

        public string InitialValue
        {
            get { return _initVal; }
        }

        public MessageDirection ParamDirection
        {
            get { return _direction; }
        }

        public string TypeName
        {
            get { return _type; }
        }
    }
}