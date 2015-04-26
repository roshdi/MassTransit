// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    abstract class CompletedExecutionResult<TArguments> :
        ExecutionResult
        where TArguments : class
    {
        readonly IRoutingSlipEventPublisher _publisher;
        readonly Activity _activity;
        readonly IDictionary<string, object> _data;
        readonly TimeSpan _duration;
        readonly ExecuteContext<TArguments> _executeContext;
        readonly RoutingSlip _routingSlip;

        protected CompletedExecutionResult(ExecuteContext<TArguments> executeContext, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip)
            : this(executeContext, publisher, activity, routingSlip, RoutingSlipBuilder.NoArguments)
        {
        }

        protected CompletedExecutionResult(ExecuteContext<TArguments> executeContext, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip,
            IDictionary<string, object> data)
        {
            _executeContext = executeContext;
            _publisher = publisher;
            _activity = activity;
            _routingSlip = routingSlip;
            _data = data;
            _duration = _executeContext.Elapsed;
        }

        protected IDictionary<string, object> Data
        {
            get { return _data; }
        }

        protected ExecuteContext<TArguments> ExecuteContext
        {
            get { return _executeContext; }
        }

        protected Activity Activity
        {
            get { return _activity; }
        }

        protected TimeSpan Duration
        {
            get { return _duration; }
        }

        public async Task Evaluate()
        {
            RoutingSlipBuilder builder = CreateRoutingSlipBuilder(_routingSlip);

            Build(builder);

            RoutingSlip routingSlip = builder.Build();

            await _publisher.PublishRoutingSlipActivityCompleted(_executeContext.ActivityName, _executeContext.ExecutionId, _executeContext.Timestamp, _duration,
                routingSlip.Variables, _activity.Arguments, _data);

            if (HasNextActivity(routingSlip))
            {
                ISendEndpoint endpoint = await _executeContext.GetSendEndpoint(routingSlip.GetNextExecuteAddress());
                await _executeContext.ConsumeContext.Forward(endpoint, routingSlip);
            }
            else
            {
                DateTime completedTimestamp = _executeContext.Timestamp + _duration;
                TimeSpan completedDuration = completedTimestamp - _routingSlip.CreateTimestamp;

                await _publisher.PublishRoutingSlipCompleted(completedTimestamp, completedDuration, routingSlip.Variables);
            }
        }

        static bool HasNextActivity(RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Any();
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityLog(ExecuteContext.Host, Activity.Name, ExecuteContext.ExecutionId, ExecuteContext.Timestamp, Duration);
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, routingSlip.Itinerary.Skip(1), Enumerable.Empty<Activity>());
        }
    }
}