﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WedChecker.Exceptions
{
    class WedCheckerInvalidDataException : Exception
    {
        public WedCheckerInvalidDataException()
        {
        }

        public WedCheckerInvalidDataException(string message)
            : base(message)
        {
        }

        public WedCheckerInvalidDataException(string message, Exception inner)
        : base(message, inner)
        {
        }

    }
}
