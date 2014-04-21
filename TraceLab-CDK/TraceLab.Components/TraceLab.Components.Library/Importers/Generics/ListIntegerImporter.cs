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

namespace TraceLab.Components.Library.Importers.Generics
{
    [Component(Name = "Integers Importer",
        Description = "Imports each line of a file as a 32bit integer.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(GenericImporterConfig))]
    [IOSpec(IOSpecType.Output, "ListOfIntegers", typeof(List<int>))]
    [Tag("Importers.Generics")]
    public class ListIntegerImporter : BaseComponent
    {
        private GenericImporterConfig _config;

        public ListIntegerImporter(ComponentLogger log)
            : base(log)
        {
            _config = new GenericImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            List<int> list = new List<int>(DevelopmentKit.IO.Generics.ImportIntegers(_config.FileName.Absolute, false));
            Workspace.Store("ListOfIntegers", list);
        }
    }
}
