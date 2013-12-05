using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR.Hubs;

namespace HubJsProxyMinification
{
    public class AjaxMinMinifier : IJavaScriptMinifier
    {
        public string Minify(string source)
        {
            var minifier = new Minifier();
            return minifier.MinifyJavaScript(source);
        }
    }
}
