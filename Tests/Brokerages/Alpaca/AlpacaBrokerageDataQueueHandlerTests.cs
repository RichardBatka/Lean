/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Moq;
using NUnit.Framework;
using QuantConnect.Brokerages.Alpaca;
using QuantConnect.Configuration;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using QuantConnect.Logging;

namespace QuantConnect.Tests.Brokerages.Alpaca
{
    [TestFixture, Ignore("This test requires a configured and testable Alpaca practice account. Since it uses the Polygon API, the account needs to be funded.")]
    public class AlpacaBrokerageDataQueueHandlerBrokerageTests
    {
        private AlpacaBrokerage _brokerage;

        [SetUp]
        public void Setup()
        {
            Log.LogHandler = new ConsoleLogHandler();

            var keyId = Config.Get("alpaca-key-id");
            var secretKey = Config.Get("alpaca-secret-key");
            var tradingMode = Config.Get("alpaca-trading-mode");

            var aggregator = new Mock<IDataAggregator>();
            _brokerage = new AlpacaBrokerage(null, null, keyId, secretKey, tradingMode, true, aggregator.Object);
            _brokerage.Connect();
        }

        [TearDown]
        public void TearDown()
        {
            _brokerage.Disconnect();
            _brokerage.Dispose();
        }

        [Test]
        public void GetsTickData()
        {
            var brokerage = _brokerage;

            brokerage.Subscribe(new List<Symbol>
            {
                Symbol.Create("AAPL", SecurityType.Equity, Market.USA),
                Symbol.Create("FB", SecurityType.Equity, Market.USA),
            });

            brokerage.Subscribe(new List<Symbol>
            {
                Symbol.Create("TSLA", SecurityType.Equity, Market.USA),
                Symbol.Create("MSFT", SecurityType.Equity, Market.USA),
            });

            brokerage.Subscribe(new List<Symbol>
            {
                Symbol.Create("GOOGL", SecurityType.Equity, Market.USA),
            });

            Thread.Sleep(20000);

            brokerage.Unsubscribe(new List<Symbol>
            {
                Symbol.Create("AAPL", SecurityType.Equity, Market.USA),
                Symbol.Create("FB", SecurityType.Equity, Market.USA),
                Symbol.Create("TSLA", SecurityType.Equity, Market.USA),
                Symbol.Create("MSFT", SecurityType.Equity, Market.USA),
                Symbol.Create("GOOGL", SecurityType.Equity, Market.USA),
            });

            Thread.Sleep(20000);

            Thread.Sleep(5000);
        }

        [Test]
        public void SubscribesAndUnsubscribesMultipleSymbols()
        {
            var symbols = new List<string>
            {
                "AAPL", "FB", "MSFT", "GOOGL"
            };

            var brokerage = _brokerage;

            var stopwatch = Stopwatch.StartNew();
            foreach (var symbol in symbols)
            {
                brokerage.Subscribe(new List<Symbol>
                {
                    Symbol.Create(symbol, SecurityType.Equity, Market.USA),
                });
            }
            stopwatch.Stop();
            Console.WriteLine("Subscribe: Elapsed time: " + stopwatch.Elapsed);

            Thread.Sleep(10000);

            stopwatch.Restart();
            foreach (var symbol in symbols)
            {
                brokerage.Unsubscribe(new List<Symbol>
                {
                    Symbol.Create(symbol, SecurityType.Equity, Market.USA),
                });
            }
            Console.WriteLine("Unsubscribe: Elapsed time: " + stopwatch.Elapsed);

            Thread.Sleep(5000);
        }

    }
}