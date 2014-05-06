// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 

#region

using System;

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
    public class NullArtifactDataXmlException : ApplicationException
    {
        public NullArtifactDataXmlException() : base("Artifact Xml document must contain a reference")
        {
        }
    }

    public sealed class NullOrchNodeException : NullReferenceException
    {
        public NullOrchNodeException(string nodeName, string orchName, string source)
            : base(
                "Node " + nodeName + " does not exist within orchestration XML source obtained from orchestration " +
                orchName)
        {
            Source = source;
            //m = "Node " + nodeName + " does not exist within orchestration XML source obtained from orchestration " + orchName;
            Data.Add("Node", nodeName);
            Data.Add("Orchestration", orchName);
        }
    }

    /// <summary>
    /// This exception is called whenever a property is found within a component that is not accounted for by the component. It is a library failure that must
    /// be addressed by the author.
    /// </summary>
    internal class UnhandledPropertyException : ApplicationException
    {
        private readonly string _parentName;
        private readonly string _propName;

        /// <summary>
        /// Create the exception
        /// </summary>
        /// <param name="parentName">Name of shape or object containing the unhandled property.</param>
        /// <param name="propName">Name of the unhandled property.</param>
        public UnhandledPropertyException(string parentName, string propName)
            : base("Unhandled property " + propName + " found in " + parentName + " shape.")
        {
            _parentName = parentName;
            _propName = propName;
        }

        public string PropertyName
        {
            get { return _propName; }
        }

        public string ParentName
        {
            get { return _parentName; }
        }
    }
}