using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.BizTalk.ExplorerOM;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Serves as the controlling class to dispatching data coming from the <see cref="BtsCatalogExplorer"/>.
    /// </summary>
    public class CatalogController
    {
        private BtsCatalogExplorer bce;

        /// <summary>
        /// Creates a new instance of the <see cref="CatalogController"/>.
        /// </summary>
        /// <param name="appName"></param>
        public CatalogController(string appName)
        {
            bce = new BtsCatalogExplorer();
                
        }
    }
}
