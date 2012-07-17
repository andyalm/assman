using System;

namespace Assman.Registration
{
	//TODO: Make this interface obsolete and collapse its members into IResourceRegistry
	[Obsolete("Please use IResourceRegistry instead. This interface is going away.")]
	public interface IReadableResourceRegistry : IResourceRegistry
	{}
}