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

using RPlugin.Core;
using TraceLab.Components.DevelopmentKit.Metrics.Traceability;
using TraceLab.Components.Types;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Metrics.Traceability
{
    [Component(Name = "Principal Component Analysis (double)",
        Description = "Performs PCA on two TLSimilarityMatrix.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(RConfig))]
    [IOSpec(IOSpecType.Input, "Matrix1", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "Matrix2", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Output, "Matrix1Contribution", typeof(double))]
    [IOSpec(IOSpecType.Output, "Matrix2Contribution", typeof(double))]
    [Tag("RPlugin.Metrics.Traceability")]
    [Tag("Metrics.Traceability")]
    public class DoublePCAComponent : BaseComponent
    {
        private RConfig _config;

        public DoublePCAComponent(ComponentLogger log) : base(log)
        {
            _config = new RConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLSimilarityMatrix matrix1 = (TLSimilarityMatrix)Workspace.Load("Matrix1");
            TLSimilarityMatrix matrix2 = (TLSimilarityMatrix)Workspace.Load("Matrix2");
            REngine engine = new REngine(_config.RScriptPath);
            double[] results = (double[])engine.Execute(new PCAScript(matrix1, matrix2));
            Workspace.Store("Matrix1Contribution", results[0]);
            Workspace.Store("Matrix2Contribution", results[1]);
        }
    }
}
