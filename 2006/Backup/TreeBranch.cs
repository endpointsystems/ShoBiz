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
    public class BtsTreeBranch : BtsBaseComponent
    {
        private readonly string _expression = String.Empty;
        private readonly bool _ghostBranch;
        private readonly List<BtsBaseComponent> _shapes = new List<BtsBaseComponent>();

        public BtsTreeBranch(XmlReader reader)
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
                        if (valName.Equals("IsGhostBranch"))
                            _ghostBranch = Convert.ToBoolean(val);
                        else if (valName.Equals("Expression"))
                            _expression = val;
                        else if (valName.Equals("Name"))
                            _name = val;
                        else
                        {
                            Debug.WriteLine("[BtsDecisionBranchShape.ctor] unhandled property " + valName);
                            Debugger.Break();
                        }
                    }
                }
                else if (reader.Name.Equals("om:Element"))
                    _shapes.Add(BtsShapeFactory.CreateShape(reader.ReadSubtree()));
            }
            reader.Close();
        }


        public List<BtsBaseComponent> Shapes
        {
            get { return _shapes; }
        }

        public bool GhostBranch
        {
            get { return _ghostBranch; }
        }

        public string Expression
        {
            get { return _expression; }
        }
    }

    public class BtsDecisionBranchShape : BtsTreeBranch
    {
        public BtsDecisionBranchShape(XmlReader reader) : base(reader)
        {
        }
    }

    public class BtsListenBranchShape : BtsTreeBranch
    {
        public BtsListenBranchShape(XmlReader reader) : base(reader)
        {
        }
    }

    public class BtsParallelBranchShape : BtsTreeBranch
    {
        public BtsParallelBranchShape(XmlReader reader) : base(reader)
        {
        }
    }
}