#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick.CLI application.
//
// BlinkStick.CLI application is free software: you can redistribute 
// it and/or modify it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//		
// BlinkStick.CLI application is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick.CLI application. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plossum.CommandLine;
using Plossum;

namespace BlinkStick.CLI
{
    [CommandLineManager(ApplicationName = "BlinkStick CLI",
       Copyright = "Copyright (C) Agile Innovative Ltd 2013",
       EnabledOptionStyles = OptionStyles.Group | OptionStyles.LongUnix)]
    [CommandLineOptionGroup("commands", Name = "Commands",
        Require = OptionGroupRequirement.ExactlyOne)]
    [CommandLineOptionGroup("options", Name = "Options")]
    class Options
    {
        [CommandLineOption(Name = "i", Aliases = "info",
            Description = "Show device info", GroupId = "commands")]
        public Boolean Info { get; set; }

        [CommandLineOption(Name = "h", Aliases = "help",
            Description = "Show this help message", GroupId = "commands")]
        public Boolean Help { get; set; }

        [CommandLineOption(Name = "set-color", 
            Description = "Set color for device. The format must be \"#rrggbb\" or \"random\" where rr, gg, bb is any hexadecimal number from 00 to FF.", GroupId = "commands")]
        public String SetColor { get; set; }

        [CommandLineOption(Name = "get-color", 
            Description = "Get the current color of the device", GroupId = "commands")]
        public Boolean GetColor { get; set; }

        [CommandLineOption(Name = "set-info-block1", 
            Description = "Set info block 1. May contain any string up to 32 chars long.", GroupId = "commands")]
        public String SetInfoBlock1 { get; set; }

        [CommandLineOption(Name = "get-info-block1", 
            Description = "Get the info block 1 from the device", GroupId = "commands")]
        public Boolean GetInfoBlock1 { get; set; }

        [CommandLineOption(Name = "set-info-block2", 
            Description = "Set info block 2. May contain any string up to 32 chars long.", GroupId = "commands")]
        public String SetInfoBlock2 { get; set; }

        [CommandLineOption(Name = "get-info-block2", 
            Description = "Get the info block 2 from the device", GroupId = "commands")]
        public Boolean GetInfoBlock2 { get; set; }

        [CommandLineOption(Name = "d", Aliases = "device",
            Description = "Specify device by serial number. All devices if unspecified or first device found if \"first\" supplied as a parameter.", GroupId = "options")]
        public String Device { get; set; }
    }

}
