using System;
using Engine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Models
{
    public class ComponentTests
    {
        [TestClass]
        public class WhenPopulatingComponentValues
        {
            [TestMethod]
            public void Should_ignore_duplicates()
            {
                const string componentName = "Component1";
                const string characteristicName = "Characteristic1";
                var model = new Model();
                model.AddComponent(componentName, characteristicName);
                model.AddComponent(componentName, characteristicName);
                Assert.AreEqual(1, model.Components.Count);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Should_throw_exception_for_empty_component_name()
            {
                const string componentName = null;
                const string characteristicName = "Characteristic1";
                var model = new Model();
                model.AddComponent(componentName, characteristicName);
                model.AddComponent(componentName, characteristicName);
            }

            [TestMethod]
            public void Should_allow_add_empty_characteristic_name()
            {
                const string componentName = "Component1";
                const string characteristicName = null;
                var model = new Model();
                model.AddComponent(componentName, characteristicName);
            }
        }
    }
}
