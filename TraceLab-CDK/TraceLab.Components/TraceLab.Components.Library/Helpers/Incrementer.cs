﻿// TraceLab - Software Traceability Instrument to Facilitate and Empower Traceability Research
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

using System;
using TraceLabSDK;
using System.ComponentModel;

namespace TraceLab.Components.Library.Helpers
{
    [Component(Name = "Incrementer By Specified Value",
        Description = "Increments integer by specified value",
        Author = "SAREC",
        Version = "1.0.0.0",
        ConfigurationType=typeof(IncrementerConfig))]
    [IOSpec(IOSpecType.Input, "integer", typeof(int))]
    [IOSpec(IOSpecType.Output, "integer", typeof(int))]
    [Tag("Helper components")]
    public class Incrementer : BaseComponent
    {
        public Incrementer(ComponentLogger log) : base(log) 
        {
            config = new IncrementerConfig();
            Configuration = config;
        }

        private IncrementerConfig config;
        
        public override void Compute()
        {
            int integer = (int)Workspace.Load("integer");
            integer += config.IncrementBy;
            Logger.Info("Value incremented to {0}", integer);
            Workspace.Store("integer", integer);
        }
    }

    public class IncrementerConfig
    {
        [DisplayName("Increment by value")]
        public int IncrementBy
        {
            get;
            set;
        }
    }
}