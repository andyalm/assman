using System;
using System.Web.UI;

[assembly: WebResource("TestWebLib.EmbeddedScript1.js", "text/javascript")]
[assembly: WebResource("TestWebLib.EmbeddedScript2.js", "text/javascript")]
[assembly: WebResource("TestWebLib.EmbeddedCss.css", "text/css")]
namespace TestWebLib
{
    public static class ResourceClass
    {
    }
}
