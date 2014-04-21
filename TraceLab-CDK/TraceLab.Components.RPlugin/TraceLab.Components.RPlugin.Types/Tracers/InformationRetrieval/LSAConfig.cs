// TraceLab Component Library
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
using TraceLabSDK;

namespace TraceLab.Components.Types.Tracers.InformationRetrieval
{
    [WorkspaceType]
    [Serializable]
    public class LSAConfig : RConfig
    {
        private int _Dimensions;

        public int Dimensions
        {
            get
            {
                return _Dimensions;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Number of topics must be greater than 0.");
                else
                    _Dimensions = value;
            }
        }
    }
}
