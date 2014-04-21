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

using TraceLab.Components.DevelopmentKit.Preprocessors;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Preprocessors
{
    [Component(Name = "Stopwords Remover",
        Description = "Removes common stop words, such as 'a', 'the', 'will', etc. It uses a list of stopwords previously imported to the Workspace.",
        Author = "SAREC",
        Version = "1.0.0.0",
        ConfigurationType = typeof(StopwordsComponentConfig))]
    [IOSpec(IOSpecType.Input, "listOfArtifacts", typeof(TraceLabSDK.Types.TLArtifactsCollection))]
    [IOSpec(IOSpecType.Input, "Stopwords", typeof(TraceLabSDK.Types.TLStopwords))]
    [IOSpec(IOSpecType.Output, "listOfArtifacts", typeof(TraceLabSDK.Types.TLArtifactsCollection))]
    [Tag("Preprocessors")]
    public class StopWordsComponent : BaseComponent
    {
        private StopwordsComponentConfig _config;

        public StopWordsComponent(ComponentLogger log)
            : base(log)
        {
            _config = new StopwordsComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection listOfArtifacts = (TLArtifactsCollection)Workspace.Load("listOfArtifacts");
            TLStopwords stopwords = (TLStopwords)Workspace.Load("Stopwords");
            TLArtifactsCollection removed = StopwordsRemover.ProcessArtifacts(listOfArtifacts, stopwords, _config.MinWordLength, _config.RemoveNumbers);
            Workspace.Store("listOfArtifacts", removed);
        }
    }

    public class StopwordsComponentConfig
    {
        public int MinWordLength { get; set; }
        public bool RemoveNumbers { get; set; }
    }
}
