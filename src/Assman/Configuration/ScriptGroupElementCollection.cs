namespace Assman.Configuration
{
	/// <summary>
	/// Represents a collection of <see cref="ScriptGroupElement"/>.
	/// </summary>
	public class ScriptGroupElementCollection : ResourceGroupElementCollection
	{
	    protected override ResourceGroupElement CreateGroupElement()
	    {
	        return new ScriptGroupElement();
	    }
	}
}
