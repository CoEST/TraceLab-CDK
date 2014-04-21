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
    public class LDAConfig : RConfig
    {
        #region Private members

        private int _NumTopics;
        private double _Alpha;
        private double _Eta;
        private double _PredictionBeta;
        private int _NumIterations;
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

        public double Eta
        {
            get
            {
                return _Eta;
            }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Eta must be greater than 0.");
                else
                    _Eta = value;
            }
        }

        public double PredictionBeta
        {
            get
            {
                return _PredictionBeta;
            }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Beta1 must be greater than 0.");
                else
                    _PredictionBeta = value;
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
                if (value <= 0)
                    throw new ArgumentException("Number of topics must be greater than 0.");
                else
                    _NumTopics = value;
            }
        }

        public int NumIterations
        {
            get
            {
                return _NumIterations;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Number of iterations must be greater than 0.");
                else
                    _NumIterations = value;
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
