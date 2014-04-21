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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TraceLab.Components.Types.Tracers.InformationRetrieval
{
    public class GibbsLDA_GAConfig : GibbsLDAConfig
    {
        #region Private members

        private int _Run;
        private int _Pop;
        private int _Elite;
        private double _PRate;

        #endregion

        [DisplayName("GA Iterations")]
        [Description("Number of iterations to run the algorithm.")]
        public int GA_Iterations
        {
            get
            {
                return _Run;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Iterations must be greater than 0.");
                else
                    _Run = value;
            }
        }

        [Description("Stop after this many iterations with no improvement.")]
        public int Run
        {
            get
            {
                return _Run;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Run must be greater than 0.");
                else
                    _Run = value;
            }
        }

        [Description("Rate of permutation for each iteration.")]
        public double PermutationRate
        {
            get
            {
                return _PRate;
            }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Permutation rate must be greater than 0.");
                else
                    _PRate = value;
            }
        }

        [Description("Population size per iteration.")]
        public int Population
        {
            get
            {
                return _Pop;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Population must be greater than 0.");
                else
                    _Pop = value;
            }
        }

        [Description("Number of best individuals to carry over to next iteration.")]
        public int Elitism
        {
            get
            {
                return _Elite;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Elitism must be greater than 0.");
                else
                    _Elite = value;
            }
        }
    }
}
