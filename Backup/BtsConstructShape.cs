// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 

#region

using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
    internal class BtsConstructShape : BtsBaseComponent
    {
        private List<BtsMsgAssignmentShape> _assignments = new List<BtsMsgAssignmentShape>();
        private List<BtsMessageRef> _refs = new List<BtsMessageRef>();
        private List<BtsTransform> _transforms = new List<BtsTransform>();

        public BtsConstructShape(XmlReader reader)
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
                        if (valName.Equals("AnalystComments"))
                            _comments = val;
                        else
                        {
                            //we're expecting all defaults, so anything else is a surprise...
                            Debug.WriteLine("[BtsConstructShape.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                {
                    if (reader.GetAttribute("Type").Equals("Transform"))
                        _transforms.Add(new BtsTransform(reader.ReadSubtree()));
                    else if (reader.GetAttribute("Type").Equals("MessageRef"))
                        _refs.Add(new BtsMessageRef(reader.ReadSubtree()));
                    else if (reader.GetAttribute("Type").Equals("MessageAssignment"))
                        _assignments.Add(new BtsMsgAssignmentShape(reader.ReadSubtree()));
                    else
                    {
                        Debug.WriteLine("[BtsConstructShape.ctor] unhandled element " + reader.GetAttribute("Type"));
                        Debugger.Break();
                    }
                }
                else
                    continue;
            }
            reader.Close();
        }

        public List<BtsMsgAssignmentShape> MessageAssignments
        {
            get { return _assignments; }
        }

        public List<BtsTransform> Transforms
        {
            get { return _transforms; }
        }


        public List<BtsMessageRef> MessageReferences
        {
            get { return _refs; }
        }
    }
}