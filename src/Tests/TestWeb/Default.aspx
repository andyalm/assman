<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <resource:ClientScriptInclude ScriptUrl="~/Script1.js" runat="server" />
    <resource:ClientScriptInclude ScriptUrl="~/Script2.js" runat="server" />
    <resource:ClientScriptInclude ScriptUrl="~/Script3.js" runat="server" />
    <resource:ClientScriptInclude ScriptUrl="~/Secondary1.js" runat="server" />
    <resource:ClientScriptInclude ScriptUrl="~/ExcludedScript.js" runat="server" />
    <resource:ClientScriptInclude AssemblyName="TestWebLib" ResourceName="TestWebLib.EmbeddedScript1.js" runat="server" />
    <resource:CssInclude StylesheetUrl="~/MyStyles.css" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<input type="button" value="test" onclick="Test1()" />
		<input type="button" value="exclude" onclick="excludedFunction()" />
		<input type="button" value="embedded" onclick="embeddedFunction1()" />
    </div>
    </form>
</body>
</html>
