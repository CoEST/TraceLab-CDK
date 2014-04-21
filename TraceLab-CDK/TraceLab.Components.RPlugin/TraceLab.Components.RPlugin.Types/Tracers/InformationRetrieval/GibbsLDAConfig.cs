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

/// SEMERU Component Library - R Plugin
/// Copyright © 2012-2013 SEMERU
/// 
/// This file is part of the SEMERU Component Library - R Plugin.
/// 
/// The SEMERU Component Library - R Plugin is free software: you can redistribute
/// it and/or modify it under the terms of the GNU General Public License as
/// published by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
/// 
/// The SEMERU Component Library - R Plugin is distributed in the hope that it will
/// be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public
/// License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with the
/// SEMERU Component Library - R Plugin.  If not, see <http://www.gnu.org/licenses>.

namespace TraceLab.Components.Types.Tracers.InformationRetrieval
{
    [WorkspaceType]
    [Serializable]
    public class GibbsLDAConfig : RConfig
    {
        #region Private members

        private int _NumTopics;
        private double _Alpha;
        private double _Beta;
        private int _GibbsIterations;
        private int _Seed;

        #endregion

        public double Alpha
        {
            get
            {
                return _Alpha;
            }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Alpha must be greater than 0.");
                else
                    _Alpha = value;
            }
        }

        public double Beta
        {
            get
            {
                return _Beta;
            }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Beta must be greater than 0.");
                else
                    _Beta = value;
            }
        }

        public int NumTopics
        {
            get
            {
                return _NumTopics;
            }
            set
            {
                if (value <= 1)
                    throw new ArgumentException("Number of topics must be greater than 1.");
                else
                    _NumTopics = value;
            }
        }

        public int GibbsIterations
        {
            get
            {
                return _GibbsIterations;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Number of iterations must be greater than 0.");
                else
                    _GibbsIterations = value;
            }
        }

        public int Seed
        {
            get
            {
                return _Seed;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Seed must be greater than 0.");
                else
                    _Seed = value;
            }
        }
    }
}
