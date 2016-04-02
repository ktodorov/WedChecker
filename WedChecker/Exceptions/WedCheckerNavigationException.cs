using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WedChecker.Exceptions
{
	public class WedCheckerNavigationException : Exception
	{

		public WedCheckerNavigationException()
		{
		}

		public WedCheckerNavigationException(string message)
            : base(message)
        {
		}

		public WedCheckerNavigationException(string message, Exception inner)
        : base(message, inner)
        {
		}
	}
}
