using System;

namespace Assman.Configuration
{
	public interface IAssmanPlugin
	{
		void Initialize(AssmanContext context);
	}
}