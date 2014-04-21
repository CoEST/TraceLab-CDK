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

using TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Snowball;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Preprocessors.Stemmers
{
    [Component(Name = "Snowball Stemmer",
        Description = "Stems artifacts using the Snowball stemmer algorithm.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(SnowballStemmerComponentConfig))]
    [IOSpec(IOSpecType.Input, "ListOfArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "ListOfArtifacts", typeof(TLArtifactsCollection))]
    [Tag("Preprocessors.Stemmers")]
    public class SnowballStemmerComponent : BaseComponent
    {
        private SnowballStemmerComponentConfig _config;

        public SnowballStemmerComponent(ComponentLogger log)
            : base(log)
        {
            _config = new SnowballStemmerComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection artifacts = (TLArtifactsCollection)Workspace.Load("ListOfArtifacts");
            Logger.Info("Using " + _config.Language + " stemmer");
            TLArtifactsCollection stemmedArtifacts = SnowballStemmer.ProcessArtifacts(artifacts, _config.Language);
            Workspace.Store("ListOfArtifacts", stemmedArtifacts);
        }
    }

    public class SnowballStemmerComponentConfig
    {
        public SnowballStemmerEnum Language { get; set; }
    }
}
