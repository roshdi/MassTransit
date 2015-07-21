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
namespace Automatonymous
{
    using MassTransit.Saga;
    using MassTransit.Saga.ConnectorFactories;
    using MassTransit.Saga.Connectors;
    using SubscriptionConnectors;


    public class StateMachineInterfaceType<TInstance, TData> :
        IStateMachineInterfaceType
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TData : class
    {
        readonly ISagaConnectorFactory _connectorFactory;

        public StateMachineInterfaceType(SagaStateMachine<TInstance> machine, EventCorrelation<TInstance, TData> correlation)
        {
            _connectorFactory = new StateMachineEventConnectorFactory<TInstance, TData>(machine, correlation);
        }

        public ISagaMessageConnector GetConnector()
        {
            return _connectorFactory.CreateMessageConnector();
        }
    }
}