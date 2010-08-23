// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 
#define BTS //on a BizTalk server

#define reader

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Microsoft.BizTalk.ExplorerOM;
#if BTS
#endif

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
    /// <summary>
    /// BtsOrch is the 'master parent' shape to all orchestration objects. The main child of this object is BtsServiceDeclaration, which in turns has a main child 
    /// of BtsServiceBody, which is the 'meat' of the orchestration. 
    /// 
    /// 12/2/06: We want to build EVERYTHING - let's make the orch the parent of all bts artifacts, but let's build them all out for future reference
    /// Note: Our scope is limited to the objects we want to extract DTS data from - we do NOT want to rebuild every possible shape (port defs, role links, etc) 
    ///     - exclude the *right* shapes/objects, but keep the ones we need!
    /// </summary>
    public sealed class BtsOrch : BtsBaseComponent, IBtsOrch
    {
        //XML variables
        private XmlDocument _xmlViewData;
        //artifact data is the actual orchestration XML that we use to create our shapes with
        private XmlDocument _xmlArtifactData;

        private XmlReader _reader;
/*
        private string _viewData;
*/
#if BTS
        private Application _btsApp;


        /// <summary>
        /// used for extracting basic info about orchestration using MS API
        /// </summary>
        private BtsOrchestration _thisOrch; //Microsoft.BizTalk.ExplorerOM.
#endif

        /// <summary>
        /// child/parent object of orchestration objects
        /// </summary>
        private BtsServiceDeclaration _svcDeclaration;

        //properties
        private Guid _core;
        private Guid _scheduleModel;
        private sbyte _majVersion;
        private sbyte _minVersion;
        private string _modifier;

        private BtsTargetXmlAttribute _xmlAttribute;
        private readonly List<BtsServiceLinkType> _svcLinkTypes = new List<BtsServiceLinkType>();
        private List<BtsServiceLinkDeclaration> _svcLinkDecs = new List<BtsServiceLinkDeclaration>();
        private readonly List<BtsPortType> _portTypes = new List<BtsPortType>();
        private readonly List<IBtsMultiPartMessageType> _mmmsgTypes = new List<IBtsMultiPartMessageType>();
        private readonly List<BtsCorrelationType> _corrTypes = new List<BtsCorrelationType>();
        private readonly List<BtsMethodMessageType> _msgTypes = new List<BtsMethodMessageType>();

        #region ctors

#if BTS

        /// <summary>
        /// The constructor is at the heart of the project. This function does the following:
        /// <list type="bullet">
        /// <item>pulls the orchestration out of the BizTalk runtime assembly</item>
        /// <item>extracts the view and artifact data out of the selected orchestration</item>
        /// <item>gathers base orchestration information</item>
        /// <item>drills into the BtsServiceBody (main orchestration) node and instantiates all child objects</item>
        /// </list>
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="orchName"></param>
        public BtsOrch(string appName, string orchName)
        {
            ExtractOrchestrationFromRuntimeAssembly(appName, orchName);
            GetCommonOrchProperties();
        }
#endif

        public BtsOrch(XmlDocument orchDoc)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.ValidationFlags = XmlSchemaValidationFlags.None;

            StringReader sr = new StringReader(orchDoc.OuterXml);

            _reader = XmlReader.Create(sr, settings);

            _xmlArtifactData = orchDoc;
            GetModuleProperties();
        }

        #endregion

        #region support functions

#if BTS

        #region ExtractOrchestrationFromRuntimeAssembly

        public BtsTargetXmlAttribute XmlAttribute
        {
            get { return _xmlAttribute; }
        }

        /// <summary>
        /// extract orchestration XML from BizTalk runtime using application and orchestration name
        /// </summary>
        /// <param name="appName">application name</param>
        /// <param name="orchName">orchestration name</param>
        private void ExtractOrchestrationFromRuntimeAssembly(string appName, string orchName)
        {
            _btsApp = ApplicationS.GetApplication(appName);
            _thisOrch = _btsApp.Orchestrations[orchName];
            _name = _thisOrch.FullName;
#if DEBUG
            Debug.Assert(null != _btsApp);
            Debug.Assert(null != _thisOrch);
#endif
            try
            {
                Type type1 =
                    BtsAssemblyFactory.GetAssembly(_thisOrch.BtsAssembly.DisplayName).GetType(_thisOrch.FullName);
                object obj =
                    type1.GetField("_symInfo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).
                        GetValue(type1);
                _xmlViewData = new XmlDocument();

                _xmlViewData.LoadXml((string) obj);

                //artifact data - the xml representation of the orchestration
                obj =
                    type1.GetField("_symODXML", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).
                        GetValue(type1);
                _xmlArtifactData = new XmlDocument();
                _xmlArtifactData.LoadXml(((string) obj).Replace("\n", ""));

                //assign Module XmlNode object for gathering BtsOrch properties
                //_module = _xmlArtifactData.SelectSingleNode("om:Element[@Name='Module']");

                GetCommonOrchProperties();

#if DEBUG
                Debug.WriteLine(obj.ToString());

                FieldInfo[] fields = type1.GetFields();
                foreach (FieldInfo fi in fields)
                {
                    Debug.WriteLine(String.Format("[{0}] field {1} ", fi.FieldType, fi.Name));
                }
#endif
            }
            catch (FileLoadException fle)
            {
#if DEBUG
                Debug.WriteLine(fle.FusionLog);
                Debugger.Break();
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
                Debugger.Break();
#endif
            }
        }

        #endregion

#endif //BTS

        /// <summary>
        /// extract base orchestration Module properties - ServiceModel, Core, etc.
        /// This is done at this level instead of the base object due to the fact that we have to pull things out of the base orch at the top level, instead of at the 
        /// XmlNode level, which is done from ServiceDeclaration level on down
        /// </summary>
        private void GetModuleProperties()
        {
            try
            {
#if reader
                _reader.MoveToContent();
                _majVersion = Convert.ToSByte(_reader.GetAttribute("MajorVersion"));
                _minVersion = Convert.ToSByte(_reader.GetAttribute("MinorVersion"));
                _core = new Guid(_reader.GetAttribute("Core"));
                _scheduleModel = new Guid(_reader.GetAttribute("ScheduleModel"));

                //"inner" properties (Module)
                _reader.ReadToDescendant("om:Element");

                _oid = new Guid(_reader.GetAttribute("OID"));

                while (_reader.Read())
                {
                    if (!_reader.HasAttributes)
                        break;
                    if (_reader.Name.Equals("om:Property"))
                        GetReaderProperties(_reader.GetAttribute("Name"), _reader.GetAttribute("Value"));
                    else if (_reader.Name.Equals("om:Element"))
                    {
                        if (_reader.GetAttribute("Type").Equals("PortType"))
                            _portTypes.Add(new BtsPortType(_reader.ReadSubtree()));
                        else if (_reader.GetAttribute("Type").Equals("PrintElement"))
                        {
                            XmlReader r = _reader.ReadSubtree();
                            r.Read();
                            r.Close();
                        }
                        else if (_reader.GetAttribute("Type").Equals("ServiceDeclaration"))
                            _svcDeclaration = new BtsServiceDeclaration(_reader.ReadSubtree());
                        else if (_reader.GetAttribute("Type").Equals("MultipartMessageType"))
                            _mmmsgTypes.Add(new BtsMultiPartMessageType(_reader.ReadSubtree()));
                        else if (_reader.GetAttribute("Type").Equals("CorrelationType"))
                            _corrTypes.Add(new BtsCorrelationType(_reader.ReadSubtree()));
                        else if (_reader.GetAttribute("Type").Equals("ServiceLinkType"))
                            _svcLinkTypes.Add(new BtsServiceLinkType(_reader.ReadSubtree()));
                        else if (_reader.GetAttribute("Type").Equals("TargetXMLNamespaceAttribute"))
                            _xmlAttribute = new BtsTargetXmlAttribute(_reader.ReadSubtree());
                        else if (_reader.GetAttribute("Type").Equals("MethodMessageType"))
                            _msgTypes.Add(new BtsMethodMessageType(_reader.ReadSubtree()));
                        else
                        {
                            Debug.WriteLine("[BtsOrch.GetModuleProperties] unhandled element " +
                                            _reader.GetAttribute("Type") + " received (needs implementation)");
                            Debugger.Break();
                        }
                    }
                    else
                        continue; //what else could we possibly expect??
                }
#endif
            }
            catch (Exception)
            {
#if DEBUG
                Debugger.Break();
#endif
            }
        }

        //used for working with om:Property
        internal new void GetReaderProperties(string xmlName, string xmlValue)
        {
#if DEBUG
            Debug.WriteLine("GetReaderProperties(" + xmlName + ", " + xmlValue + ")");
#endif
            if (!base.GetReaderProperties(xmlName, xmlValue))
            {
                if (xmlName.Equals("AnalystComments"))
                    _comments = xmlValue;
                else if (xmlName.Equals("TypeModifier"))
                    _modifier = xmlValue;
                else
                {
                    Debug.WriteLine("[BtsOrch.ctor] unhandled property " + xmlName);
                    Debugger.Break();
                }
            }
        }

        #endregion

        #region props

        public string TypeModifier
        {
            get { return _modifier; }
        }

        public BtsServiceDeclaration ServiceDeclaration
        {
            get { return _svcDeclaration; }
        }

        public sbyte MinorVersion
        {
            get { return _minVersion; }
        }

        public sbyte MajorVersion
        {
            get { return _majVersion; }
        }


        public Guid ScheduleModel
        {
            get { return _scheduleModel; }
        }


        public Guid Core
        {
            get { return _core; }
        }


        public XmlDocument ViewData
        {
            get { return _xmlViewData; }
            /*
        set 
        { 
            _xmlViewData = value; 
            _viewData = _xmlViewData.OuterXml; 
        }
         */
        }

        public XmlDocument ArtifactData
        {
            get { return _xmlArtifactData; }
            //set { _xmlArtifactData = value;}
        }

        #endregion
    } //BtsOrch
} //namespace