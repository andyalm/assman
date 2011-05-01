namespace Assman.Mvc.Html
{
#if !NET_40
	///<summary>
	/// Dummy interface defined for .NET 3.5
	/// </summary>
	public interface IHtmlString
	{
		string ToHtmlString();
	}
#endif
}