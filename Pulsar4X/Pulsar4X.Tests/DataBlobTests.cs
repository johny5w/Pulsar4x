﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Newtonsoft.Json;
//using NUnit.Framework;
//using Pulsar4X.ECSLib;

//namespace Pulsar4X.Tests
//{
//    [TestFixture]
//    [Description("DataBlob Tests")]
//    internal class DataBlobTests
//    {
//        private static readonly List<Type> DataBlobTypes = new List<Type>(Assembly.GetAssembly(typeof(BaseDataBlob)).GetTypes().Where(type => type.IsSubclassOf(typeof(BaseDataBlob)) && !type.IsAbstract));
//        private static EntityManager _manager = new EntityManager();

//        [SetUp]
//        public void Init()
//        {
//            Game game = new Game(new LibProcessLayer(), "TestGame");
//            _manager = new EntityManager();
//        }

//        [Test]
//        public void TypeCount()
//        {
//            Assert.AreEqual(DataBlobTypes.Count, EntityManager.BlankDataBlobMask().Length);
//        }

//        /// <summary>
//        /// This test ensures our DataBlobs have a deep copy constructor. DataBlobs need the ability to be deep copied so they can be passed
//        /// to the UI or used by AI simulations without chaning the original values.
//        /// </summary>
//        [Test]
//        [TestCaseSource("DataBlobTypes")]
//        public void DeepCopyConstructor(Type dataBlobType)
//        {
//            ConstructorInfo constructor = dataBlobType.GetConstructor(new[] {dataBlobType});
//            if (constructor == null)
//            {
//                Assert.Fail(dataBlobType + " does not have a Deep Copy constructor.");
//            }
//        }

//        /// <summary>
//        /// This test ensures our DataBlobs can be created by Json during deserialization.
//        /// Any type that fails this test cannot be instantiated by Json, and thus will throw an exception if you try to deserialize it.
//        /// </summary>
//        [Test]
//        [TestCaseSource("DataBlobTypes")]
//        public void JSONConstructor(Type dataBlobType)
//        {
//            ConstructorInfo[] constructors = dataBlobType.GetConstructors();
//            Attribute jsonConstructorAttribute = new JsonConstructorAttribute();

//            foreach (ConstructorInfo constructorInfo in constructors.Where(constructorInfo => constructorInfo.GetCustomAttributes().Contains(jsonConstructorAttribute)))
//            {
//                Assert.Pass(dataBlobType.ToString() + " will deserialize with the constructor marked with [JsonConstructor]");
//            }

//            foreach (ConstructorInfo constructorInfo in constructors.Where(constructorInfo => constructorInfo.GetParameters().Length == 0 && constructorInfo.IsPublic))
//            {
//                Assert.Pass(dataBlobType.ToString() + " will deserialize with the default parameterless constructor.");
//            }

//            if (constructors.Length == 1)
//            {
//                if (constructors[0].GetParameters().Length != 0)
//                {
//                    Assert.Pass(dataBlobType.ToString() + " will deserialize with the only parameterized constructor available. Make sure parameters match the Json property names saved in the Json file.");
//                }
//            }

//            foreach (ConstructorInfo constructorInfo in constructors.Where(constructorInfo => constructorInfo.GetParameters().Length == 0 && constructorInfo.IsPrivate))
//            {
//                Assert.Pass(dataBlobType.ToString() + " will deserialize with the private default parameterless constructor.");
//            }
//            Assert.Fail(dataBlobType.ToString() + " does not have a Json constructor");

//        }

//        /// <summary>
//        /// This test verifies the integrity of the TreeHierarchy datablob to maintain it's hierarchy during switches.
//        /// </summary>
//        [Test]
//        public void TreeHierarchyTest()
//        {
//            // Create the "Root" entity.
//            Entity rootNode = CreateNode(null);
//            ConcreteTreeHierarchyDB rootDB = rootNode.GetDataBlob<ConcreteTreeHierarchyDB>();

//            // Make sure the root has a root, itself.
//            Assert.AreSame(rootDB, rootDB.RootDB);
//            // Root doesn't have a parent.
//            Assert.IsNull(rootDB.ParentDB);

//            // Create a bunch of children.
//            Entity Parent1_node = CreateNode(rootNode);
//            Entity Parent1_child1 = CreateNode(Parent1_node);
//            Entity Parent1_child2 = CreateNode(Parent1_node);

//            // Store a list of children for later comparison.
//            var Parent1_childEntitys = new List<Entity> { Parent1_child1, Parent1_child2 };

//            // Create a second set of children.
//            Entity Parent2_node = CreateNode(rootNode);
//            Entity Parent2_child1 = CreateNode(Parent2_node);
//            Entity Parent2_child2 = CreateNode(Parent2_node);

//            // Store the second set of children.
//            var Parent2_childEntitys = new List<Entity> { Parent2_child1, Parent2_child2 };

//            // Get the dataBlobs of each child.
//            ConcreteTreeHierarchyDB Parent1DB = Parent1_node.GetDataBlob<ConcreteTreeHierarchyDB>();
//            ConcreteTreeHierarchyDB Parent1_child1DB = Parent1_child1.GetDataBlob<ConcreteTreeHierarchyDB>();
//            ConcreteTreeHierarchyDB Parent1_child2DB = Parent1_child2.GetDataBlob<ConcreteTreeHierarchyDB>();

//            ConcreteTreeHierarchyDB Parent2DB = Parent2_node.GetDataBlob<ConcreteTreeHierarchyDB>();
//            ConcreteTreeHierarchyDB Parent2_child1DB = Parent2_child1.GetDataBlob<ConcreteTreeHierarchyDB>();
//            ConcreteTreeHierarchyDB Parent2_child2DB = Parent2_child2.GetDataBlob<ConcreteTreeHierarchyDB>();

//            // Ensure the root is the same across the branches.
//            Assert.AreSame(rootDB, Parent1_child1DB.RootDB);
//            Assert.AreSame(rootDB, Parent2_child2DB.RootDB);

//            // Ensure children point to their parents.
//            Assert.AreSame(Parent1DB, Parent1_child1DB.ParentDB);
//            Assert.AreSame(Parent2DB, Parent2_child1DB.ParentDB);

//            // Store a list of dataBlobs for later comparison.
//            var Parent1_children = new List<ConcreteTreeHierarchyDB> { Parent1_child1DB, Parent1_child2DB };
//            var Parent2_children = new List<ConcreteTreeHierarchyDB> { Parent2_child1DB, Parent2_child2DB };

//            // Ensure listed child entities concur with our child list.
//            Assert.AreEqual(Parent1_childEntitys, Parent1DB.Children);
//            Assert.AreEqual(Parent2_childEntitys, Parent2DB.Children);

//            // Ensure listen child DBs concur with our stored list.
//            Assert.AreEqual(Parent1_children, Parent1DB.ChildrenDBs);
//            Assert.AreEqual(Parent2_children, Parent2DB.ChildrenDBs);

//            // Change P2C1's parent to P1.
//            Parent2_child1DB.Parent = Parent1DB.OwningEntity;
//            // Make sure P2C1 is owned by P1;
//            Assert.AreEqual(Parent1DB.OwningEntity, Parent2_child1DB.ParentDB.OwningEntity);

//            // Make sure P1's children list updated.
//            Parent1_childEntitys.Add(Parent2_child1);
//            Assert.AreEqual(Parent1_childEntitys, Parent1DB.Children);

//            // Make sure P2's children list updated.
//            Parent2_childEntitys.Remove(Parent2_child1);
//            Assert.AreEqual(Parent2_childEntitys, Parent2DB.Children);


//            // Create a new root.
//            Entity root2Node = CreateNode(null);
//            ConcreteTreeHierarchyDB root2DB = root2Node.GetDataBlob<ConcreteTreeHierarchyDB>();

//            // Assign P2 to the new root.
//            Parent2DB.Parent = root2Node;
//            // Make sure P2's child has a new RootDB.
//            Assert.AreEqual(root2DB.OwningEntity, Parent2_child2DB.RootDB.OwningEntity);

//        }

//        /// <summary>
//        /// Helper function for TreeHierarchyTest
//        /// </summary>
//        private Entity CreateNode(Entity parentEntity)
//        {
//            ConcreteTreeHierarchyDB nodeDB = new ConcreteTreeHierarchyDB(parentEntity);
//            Entity nodeEntity = new Entity(_manager);
//            nodeEntity.SetDataBlob(nodeDB);

//            return nodeEntity;
//        }
//    }
//}