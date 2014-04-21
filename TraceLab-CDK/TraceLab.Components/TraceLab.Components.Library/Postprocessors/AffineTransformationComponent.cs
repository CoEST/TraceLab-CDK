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
using System.ComponentModel;
using TraceLab.Components.DevelopmentKit.Postprocessors;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Postprocessors
{
    [Component(Name = "Affine Tranformation",
                Description = "Performs an affine transformation combining two distributions.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(AffineTransformationConfig))]
    [IOSpec(IOSpecType.Input, "LargeExpert", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "SmallExpert", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Output, "CombinedSimilarities", typeof(TLSimilarityMatrix))]
    [Tag("Postprocessors")]
    public class AffineTransformationComponent : BaseComponent
    {
        private AffineTransformationConfig _config;

        public AffineTransformationComponent(ComponentLogger log)
            : base(log)
        {
            _config = new AffineTransformationConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLSimilarityMatrix large = (TLSimilarityMatrix)Workspace.Load("LargeExpert");
            TLSimilarityMatrix small = (TLSimilarityMatrix)Workspace.Load("SmallExpert");
            TLSimilarityMatrix combined = AffineTranformation.Transform(large, small, _config.Lambda);
            Workspace.Store("CombinedSimilarities", combined);
        }
    }

    public class AffineTransformationConfig
    {
        private double _lambda;

        [DisplayName("Lambda")]
        [Description("Weight percentage given to LargeExpert")]
        public double Lambda
        {
            get
            {
                return _lambda;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException("Lambda must be between 0 and 1");
                }
                _lambda = value;
            }
        }
    }
}
