﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier.InternalMessages
{
    using System;
    using Contracts;


    public class SubscriptionImpl :
        Subscription
    {
        public SubscriptionImpl(Uri address, RoutingSlipEvents events, RoutingSlipEventContents include)
        {
            Include = include;
            Address = address;
            Events = events;
        }

        public Uri Address { get; private set; }
        public RoutingSlipEvents Events { get; private set; }
        public RoutingSlipEventContents Include { get; private set; }
    }
}