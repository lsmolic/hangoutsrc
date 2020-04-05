using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Hangout.Client;
using Hangout.Shared;

namespace Hangout.Shared.UnitTest
{

    public class TestMediator : Mediator
    {
    }

    public class TestProxy : Proxy
    {
    }

    public class UnregisteredTestMediator : Mediator
    {
    }

    public class UnregisteredTestProxy : Proxy
    {
    }


    [TestFixture]
    public class PureMVCTest
    {
        [Test]
        public void RegisterAndRetrieveMediator()
        {
            // Make sure registering and retrieving a mediator works

            IMediator mediator = new TestMediator();

            Facade.Instance.RegisterMediator(mediator);

            // Make sure facade has mediator
            Assert.IsTrue(Facade.Instance.HasMediator(typeof(TestMediator).Name));

            // Get a mediator and make sure it's not null
            Assert.IsNotNull(Facade.Instance.RetrieveMediator<TestMediator>());
        }

        [Test]
        public void RegisterAndRetrieveProxy()
        {
            // Make sure registering and retrieving a proxy works
            IProxy proxy = new TestProxy();

            Facade.Instance.RegisterProxy(proxy);

            // Make sure facade has proxy
            Assert.IsTrue(Facade.Instance.HasProxy(typeof(TestProxy).Name));

            // Get the test proxy and make sure it's not null
            Assert.IsNotNull(Facade.Instance.RetrieveProxy<TestProxy>());
        }

        [Test]
        public void RetrieveUnregisteredMediator()
        {
            // Try retrieving a mediator that hasn't been registered and make sure we get an exception
            // Make sure we use a different type as the above tests in case the facade is still hanging on to it
            bool exceptionThrown = false;
            try
            {
                Facade.Instance.RetrieveMediator<UnregisteredTestMediator>();
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Facade should throw exception if we are calling RetrieveMediator for a mediator that hasn't been registered yet");
        }

        [Test]
        public void RetrieveUnregisteredProxy()
        {
            // Try retrieving a mediator that hasn't been registered and make sure we get an exception
            // Make sure we use a different type as the above tests in case the facade is still hanging on to it
            bool exceptionThrown = false;
            try
            {
                Facade.Instance.RetrieveProxy<UnregisteredTestProxy>();
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Facade should throw exception if we are calling RetrieveProxy for a proxy that hasn't been registered yet");
        }

    }
}
