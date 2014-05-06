// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 
#define BTS //on a BizTalk server

#region

using System;
using System.Diagnostics;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.Win32;
#if BTS
#endif

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
#if BTS
    /// <summary>
    /// CatalogExplorerS - CatalogExplorer Singleton class
    /// Dependency: Microsoft.BizTalk.ExplorerOM (C:\Program Files\Microsoft BizTalk Server 2006\Developer Tools\Microsoft.BizTalk.ExplorerOM.dll)
    /// This module must run on a BTS server.
    /// </summary>
    public sealed class CatalogExplorerS
    {
        private static readonly BtsCatalogExplorer Catalog = new BtsCatalogExplorer();

        public static BtsCatalogExplorer GetCatalogExplorer()
        {
            if (null == Catalog.ConnectionString)
            {
                RegistryKey key =
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration");
                if (key != null)
                    Catalog.ConnectionString = String.Format("Server={0};Database={1};Integrated Security=SSPI",
                                                             key.GetValue("MgmtDBServer"), key.GetValue("MgmtDBName"));
            }

            return Catalog;
        }

        /// <summary>
        /// the "remote" version?
        /// </summary>
        /// <param name="mgmtDBServer">Database server containing BTS MgmtDBServer</param>
        /// <param name="mgmtDBName">BTS Mgmt DB Name</param>
        /// <returns>BtsCatalogExplorer object (ExplorerOM)</returns>
        public static BtsCatalogExplorer CatalogExplorer(string mgmtDBServer, string mgmtDBName)
        {
            BtsCatalogExplorer catalog = new BtsCatalogExplorer();
            catalog.ConnectionString = String.Format("Server={0};Database={1};Integrated Security=SSPI", mgmtDBServer,
                                                      mgmtDBName);
            return catalog;
        }
    } //CatalogExplorerS

    public sealed class ApplicationS
    {
        private static Application _app;

        public static Application GetApplication(string appName)
        {
            if (null != CatalogExplorerS.GetCatalogExplorer().Applications[appName])
                _app = CatalogExplorerS.GetCatalogExplorer().Applications[appName];

            else
            {
#if DEBUG
                ///TODO: Exception Handling
                Debugger.Break();
#endif
                _app = null;
            }
            return _app;
        }
    } //ApplicationS
#endif //BTS
} //namespace