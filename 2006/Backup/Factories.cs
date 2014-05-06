// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 
#define BTS

#region

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.Win32;

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
#if BTS
    /// <summary>
    /// CatalogExplorerFactory - create CatalogExplorer shapes.
    /// Dependency: Microsoft.BizTalk.ExplorerOM (C:\Program Files\Microsoft BizTalk Server 2006\Developer Tools\Microsoft.BizTalk.ExplorerOM.dll)
    /// This module must run on a BTS server.
    /// </summary>
    public sealed class CatalogExplorerFactory
    {
        public static BtsCatalogExplorer CatalogExplorer()
        {
            BtsCatalogExplorer catalog = new BtsCatalogExplorer();

            if (catalog.ConnectionString.Length < 0)
            {
                RegistryKey key =
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration");
                if (key != null)
                    catalog.ConnectionString = String.Format("Server={0};Database={1};Integrated Security=SSPI",
                                                             key.GetValue("MgmtDBServer"), key.GetValue("MgmtDBName"));
            }

            return catalog;
        }

        public static BtsCatalogExplorer CatalogExplorer(string mgmtDBServer, string mgmtDBName)
        {
            BtsCatalogExplorer catalog = new BtsCatalogExplorer();
            catalog.ConnectionString = String.Format("Server={0};Database={1};Integrated Security=SSPI", mgmtDBServer,
                                                      mgmtDBName);
            return catalog;
        }
    }

#endif

    public sealed class BtsBaseComponentFactory
    {
        public static BtsBaseComponent BtsBaseComponent()
        {
            BtsBaseComponent @base = new BtsBaseComponent();


            return @base;
        }
    }

#if BTS
    /// <summary>
    /// Return System.Reflection.Assembly versions of BizTalk assemblies.
    /// </summary>
    public sealed class BtsAssemblyFactory
    {
        /// <summary>
        /// Extracts XML assembly information from adpl_sat table in BizTalkMgmtDb table, searches for and loads the System.Reflection.Assembly object from its physical location.
        /// </summary>
        /// <param name="assemblyDisplayName">DisplayName property of ExplorerOM.BtsAssembly object.</param>
        /// <returns>loaded System.Reflection.Assembly object.</returns>
        public static Assembly GetAssembly(string assemblyDisplayName)
        {
            string fname = String.Empty;
            SqlConnection conn = new SqlConnection(CatalogExplorerS.GetCatalogExplorer().ConnectionString);
            try
            {
                conn.Open();
                SqlCommand sb =
                    new SqlCommand(
                        String.Format("select properties from adpl_sat where luid='{0}'", assemblyDisplayName), conn);
                XmlReader read = sb.ExecuteXmlReader();

                XmlDocument doc = new XmlDocument();
                doc.Load(read);

                XPathNavigator nav = doc.CreateNavigator();
                if (nav != null)
                {
                    nav.MoveToRoot();
                    XPathNavigator iterator =
                        nav.SelectSingleNode("DictionarySerializer2OfStringObject/dictionary/item[key = \"SourceLocation\"]");
                    XPathNodeIterator fullFileName = iterator.SelectChildren("SourceLocation", "");
                    if (null == fullFileName.Current.Value)
                    {
                        ///TODO: research if %BTAD_Installdir% in properties column expands as needed, or what -- what IF SourceLocation doesn't exist? does that happen?s
                        Debugger.Break();
                    }
                    if (fullFileName.Current.Value != null)
                        fname = fullFileName.Current.Value.Replace("SourceLocation", "");
                }

            }
            catch (Exception)
            {
#if DEBUG
                Debugger.Break();
#endif
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            Debug.WriteLine("loading file " + fname);

            return Assembly.LoadFile(fname);
        }
    }
#endif

    /// <summary>
    /// This class is used to build everything that can be found within a ServiceBody defnition. 
    /// </summary>
    public sealed class BtsShapeFactory
    {
        public static BtsBaseComponent CreateShape(XmlReader reader)
        {
            //gotta initialize it
            reader.Read();

            //we don't read a subtree in any of these because we're receiving one from the invoker.
            string val = reader.GetAttribute("Type");
            switch (val)
            {
                case "AtomicTransaction":
                    return new BtsAtomicTx(reader);
                case "Call":
                    return new BtsCallShape(reader);
                case "CallRules":
                    return new BtsCallRulesShape(reader);
                case "Catch":
                    return new BtsCatchShape(reader);
                case "Compensation":
                    return new BtsCompensation(reader);
                case "Compensate":
                    return new BtsCompensateShape(reader);
                case "Construct":
                    return new BtsConstructShape(reader);
                case "CorrelationDeclaration":
                    return new BtsCorrelationDeclaration(reader);
                case "Decision":
                    return new BtsDecisionShape(reader);
                case "DecisionBranch":
                    return new BtsDecisionShape(reader);
                case "Delay":
                    return new BtsDelayShape(reader);
                case "Listen":
                    return new BtsListenShape(reader);
                case "ListenBranch":
                    return new BtsListenBranchShape(reader);
                case "LongRunningTransaction":
                    return new BtsLongRunningTx(reader);
                case "MessageDeclaration":
                    return new BtsMessageDeclaration(reader);
                case "Parallel":
                    return new BtsParallelShape(reader);
                case "ParallelBranch":
                    return new BtsParallelBranchShape(reader);
                case "Parameter":
                    return new BtsParameter(reader);
                case "Receive":
                    return new BtsReceiveShape(reader);
                case "Scope":
                    return new BtsScopeShape(reader);
                case "Send":
                    return new BtsSendShape(reader);
                case "StatementRef":
                    return new BtsStatementRef(reader);
                case "Suspend":
                    return new BtsSuspendShape(reader);
                case "Task":
                    return new BtsTaskShape(reader);
                case "Terminate":
                    return new BtsTerminateShape(reader);
                case "Throw":
                    return new BtsThrowShape(reader);
                case "TransactionAttribute":
                    return new BtsTransactionAttribute(reader);
                case "VariableAssignment":
                    return new BtsVariableAssignmentShape(reader);
                case "VariableDeclaration":
                    return new BtsVariableDeclaration(reader);
                case "While":
                    return new BtsWhileShape(reader);
                default:
                    {
                        Debug.WriteLine("[BtsShapeFactory.CreateShape] unhandled shape constructor for : " +
                                        reader.GetAttribute("Type"));
                        Debugger.Break();
                        break;
                    }
            }
            return null;
        }
    }
}