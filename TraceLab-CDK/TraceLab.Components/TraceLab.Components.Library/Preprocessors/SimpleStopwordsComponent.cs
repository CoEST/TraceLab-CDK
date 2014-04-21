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
using TraceLab.Components.DevelopmentKit.Preprocessors;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Preprocessors
{
    [Component(Name = "Simple Stopwords Remover",
        Description = "Option to remove numbers and words less than a certain length.",
        Author = "SEMERU",
        Version = "1.0.0.0",
        ConfigurationType = typeof(SimpleStopwordsComponentConfig))]
    [IOSpec(IOSpecType.Input, "listOfArtifacts", typeof(TraceLabSDK.Types.TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "listOfArtifacts", typeof(TraceLabSDK.Types.TLArtifactsCollection))]
    [Tag("Preprocessors")]
    public class SimpleStopWordsComponent : BaseComponent
    {
        private SimpleStopwordsComponentConfig _config;

        public SimpleStopWordsComponent(ComponentLogger log)
            : base(log)
        {
            _config = new SimpleStopwordsComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection listOfArtifacts = (TLArtifactsCollection)Workspace.Load("listOfArtifacts");
            TLArtifactsCollection removed = SimpleStopwordsRemover.ProcessArtifacts(listOfArtifacts, _config.MinWordLength, _config.RemoveNumbers);
            Workspace.Store("listOfArtifacts", removed);
        }
    }

    public class SimpleStopwordsComponentConfig
    {
        public int MinWordLength { get; set; }
        public bool RemoveNumbers { get; set; }
    }
}
