﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Telemetry
{
    public enum LogEventSeverity
    {
        /// <summary>
        /// All the things!
        /// </summary>
        Verbose,

        /// <summary>
        /// Enough to know what is happening inside
        /// </summary>
        Debug,

        /// <summary>
        /// Enough to know what is happening outside
        /// </summary>
        Information,

        /// <summary>
        /// Things are in danger of going sideway quickly
        /// </summary>
        Warning,

        /// <summary>
        /// Okay, this is a bad thing - we aren't dead yet but it's coming
        /// </summary>
        Error,

        /// <summary>
        /// Boom, is there a doctor in house?
        /// </summary>
        Fatal
    }
}