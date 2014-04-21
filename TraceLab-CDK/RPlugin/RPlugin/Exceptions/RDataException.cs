// RPlugin - A framework for running R scripts in .NET
// Copyright © 2012-2013 SEMERU
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RPlugin.Exceptions
{
    /// <summary>
    /// Exception for invalid data
    /// </summary>
    [Serializable]
    public class RDataException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RDataException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Application message</param>
        public RDataException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Application message</param>
        /// <param name="innerException">Inner exception</param>
        public RDataException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected RDataException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
