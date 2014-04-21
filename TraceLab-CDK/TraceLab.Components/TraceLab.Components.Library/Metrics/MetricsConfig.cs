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

using System.ComponentModel;

namespace TraceLab.Components.Library.Metrics
{
    /// <summary>
    /// Abstract class for defining custom metrics configuration objects
    /// </summary>
    public abstract class MetricsConfig
    {
        [DisplayName("Technique name")]
        [Description("The name of the technique used to compute the results.")]
        public string TechniqueName { get; set; }

        [DisplayName("Dataset name")]
        [Description("The name of the dataset used to compute the results.")]
        public string DatasetName { get; set; }
    }
}
