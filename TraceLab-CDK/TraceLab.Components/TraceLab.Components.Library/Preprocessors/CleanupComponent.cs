// TraceLab - Software Traceability Instrument to Facilitate and Empower Traceability Research
// Copyright © 2012-2013 CoEST - National Science Foundation MRI-R2 Grant # CNS: 0959924
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

using System.ComponentModel;
using TraceLab.Components.DevelopmentKit.Preprocessors;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Preprocessors
{
    [Component(Name = "Cleanup Preprocessor",
        Description = "This Pre-Processor cleans up the text, eliminating all 'non characters' (punctuation marks, numbers, etc).",
        Author = "SAREC",
        Version = "1.0.0.0",
        ConfigurationType = typeof(CleanupComponentConfig))]
    [IOSpec(IOSpecType.Input, "listOfArtifacts", typeof(TraceLabSDK.Types.TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "listOfArtifacts", typeof(TraceLabSDK.Types.TLArtifactsCollection))]
    [Tag("Preprocessors")]
    public class PreprocessorCleanUpComponent : BaseComponent
    {
        private CleanupComponentConfig _config;

        public PreprocessorCleanUpComponent(ComponentLogger log)
            : base(log)
        {
            _config = new CleanupComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection listOfArtifacts = (TLArtifactsCollection)Workspace.Load("listOfArtifacts");
            TLArtifactsCollection cleanArtifacts = Cleanup.ProcessArtifacts(listOfArtifacts, _config.ConvertLowercase);
            Workspace.Store("listOfArtifacts", cleanArtifacts);
        }
    }

    public class CleanupComponentConfig
    {
        [DisplayName("Convert to lowercase?")]
        [Description("Option to convert terms to lowercase")]
        public bool ConvertLowercase { get; set; }
    }
}
