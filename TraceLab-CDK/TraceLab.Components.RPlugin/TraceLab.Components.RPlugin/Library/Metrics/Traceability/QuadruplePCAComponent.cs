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
using System.Linq;
using System.Text;
using TraceLabSDK;
using TraceLabSDK.Types;
using TraceLab.Components.Types;
using TraceLab.Components.DevelopmentKit.Metrics.Traceability;
using RPlugin.Core;

namespace TraceLab.Components.Library.Metrics.Traceability
{
    [Component(Name = "Principal Component Analysis (quadruple)",
        Description = "Performs PCA on four TLSimilarityMatrix.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(RConfig))]
    [IOSpec(IOSpecType.Input, "Matrix1", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "Matrix2", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "Matrix3", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "Matrix4", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Output, "Matrix1Contribution", typeof(double))]
    [IOSpec(IOSpecType.Output, "Matrix2Contribution", typeof(double))]
    [IOSpec(IOSpecType.Output, "Matrix3Contribution", typeof(double))]
    [IOSpec(IOSpecType.Output, "Matrix4Contribution", typeof(double))]
    [Tag("RPlugin.Metrics.Traceability")]
    [Tag("Metrics.Traceability")]
    public class QuadruplePCAComponent : BaseComponent
    {
        private RConfig _config;

        public QuadruplePCAComponent(ComponentLogger log)
            : base(log)
        {
            _config = new RConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLSimilarityMatrix matrix1 = (TLSimilarityMatrix)Workspace.Load("Matrix1");
            TLSimilarityMatrix matrix2 = (TLSimilarityMatrix)Workspace.Load("Matrix2");
            TLSimilarityMatrix matrix3 = (TLSimilarityMatrix)Workspace.Load("Matrix3");
            TLSimilarityMatrix matrix4 = (TLSimilarityMatrix)Workspace.Load("Matrix4");
            REngine engine = new REngine(_config.RScriptPath);
            double[] results = (double[])engine.Execute(new PCAScript(matrix1, matrix2, matrix3, matrix4));
            Workspace.Store("Matrix1Contribution", results[0]);
            Workspace.Store("Matrix2Contribution", results[1]);
            Workspace.Store("Matrix3Contribution", results[2]);
            Workspace.Store("Matrix4Contribution", results[3]);
        }
    }
}
