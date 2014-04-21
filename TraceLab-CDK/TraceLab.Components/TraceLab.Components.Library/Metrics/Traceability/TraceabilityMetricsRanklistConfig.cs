﻿// TraceLab Component Library
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

namespace TraceLab.Components.Library.Metrics.Traceability
{
    public class TraceabilityMetricsRanklistConfig : MetricsConfig
    {
        public bool Precision { get; set; }
        public bool Recall { get; set; }
        public bool AveragePrecision { get; set; }

        [DisplayName("Precision-Recall Curve")]
        public bool PRCurve { get; set; }
    }
}
