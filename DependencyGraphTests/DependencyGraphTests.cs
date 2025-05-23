﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    ///
    /// Author  :          Rohith V
    /// Project Created : Jan 27,2023
    /// Partner :           None
    ///
    /// 
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    ///
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }


        /// <summary>
        ///  Checks if Dependents and Dependees are properly connected and replaced
        /// </summary>
        [TestMethod]
        public void checkIfSimultaneouslyDependentsAndDependees_Replaced()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            dg.AddDependency("e", "b");
            dg.AddDependency("f", "b");

            HashSet<string> dependee = new();
            dependee.Add("a");
            dependee.Add("c");
            dependee.Add("e");
            dependee.Add("f");

            Assert.AreEqual(dependee.Count(), dg.GetDependees("b").Count());
            for (int i = 0; i < dependee.Count(); i++)
                Assert.AreEqual(dependee.ToList()[i].ToString(), dg.GetDependees("b").ToList()[i].ToString());

            Assert.AreEqual(new HashSet<string> { "b" }.ToString(), dg.GetDependents("a").ToString());
            Assert.AreEqual(new HashSet<string> { "b" }.ToString(), dg.GetDependents("c").ToString());
            Assert.AreEqual(new HashSet<string> { "b" }.ToString(), dg.GetDependents("e").ToString());
            Assert.AreEqual(new HashSet<string> { "b" }.ToString(), dg.GetDependents("f").ToString());


            HashSet<string> newDependee = new();
            newDependee.Add("i");
            newDependee.Add("j");
            newDependee.Add("k");

            dg.ReplaceDependees("b", newDependee);

            HashSet<string> dependeeList = dg.GetDependees("b").ToHashSet();

            for (int i = 0; i < 3; i++)
                Assert.AreEqual(newDependee.ToList()[i].ToString(), dependeeList.ToList()[i].ToString());

            //Assert.AreEqual(new List<string> { }.ToArray().ToString(), dg.GetDependees("a").ToArray().ToString());
            //Assert.AreEqual(new List<string> { }.ToArray().ToString(), dg.GetDependees("i").ToArray().ToString());
            //Assert.AreEqual(new List<string> { }.ToArray().ToString(), dg.GetDependees("j").ToArray().ToString());
            //Assert.AreEqual(new List<string> { }.ToArray().ToString(), dg.GetDependees("k").ToArray().ToString());
        }

        [TestMethod]
        public void tryAddDuplicateDependency()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "b");

            Assert.AreEqual(1, dg.Size);
        }

        [TestMethod]
        public void tryRemovingDependencyForEmptyDependee()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "");

            Assert.AreEqual(1, dg.Size);
            dg.RemoveDependency("a", "");
            Assert.AreEqual(0, dg.Size);
        }

        [TestMethod]
        public void checkIndexerMethod()
        {
            DependencyGraph dg = new();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b") ;
            dg.AddDependency("d", "b");

            Assert.AreEqual(3, dg["b"]);

        }

        [TestMethod]
        public void HasDependentsTrue()
        {
            DependencyGraph dg = new();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            dg.AddDependency("d", "b");

            Assert.IsTrue(dg.HasDependents("d"));
            Assert.IsTrue(dg.HasDependents("c"));
            Assert.IsTrue(dg.HasDependents("a"));

        }

        [TestMethod]
        public void HasDependentsFalse()
        {
            DependencyGraph dg = new();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            dg.AddDependency("d", "b");

            Assert.IsFalse(dg.HasDependents("b"));

        }

        /// <summary>
        /// checks HasDependee method for non-empty list
        /// </summary>
        [TestMethod]
        public void HasDependeeTrue()
        {
            DependencyGraph dg = new();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "d");
            dg.AddDependency("e", "f");

            Assert.IsTrue(dg.HasDependees("b"));
            Assert.IsTrue(dg.HasDependees("d"));
            Assert.IsTrue(dg.HasDependees("f"));

        }

        /// <summary>
        ///  Checks HasDependee method for empty list
        /// </summary>
        [TestMethod]
        public void HasDependeeFalse()
        {
            DependencyGraph dg = new();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "d");
            dg.AddDependency("e", "f");

            Assert.IsFalse(dg.HasDependees("a"));
            Assert.IsFalse(dg.HasDependees("c"));
            Assert.IsFalse(dg.HasDependees("e"));

        }

        /// <summary>
        ///  Remove dependency for non-existent connection
        /// </summary>

        [TestMethod]
        public void RemoveDependency_For_NonExistentDependency()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("l", "o");
            Assert.AreEqual(1, dg.Size);
            dg.RemoveDependency("l", "");

            // checks size again after trying to remove non-existent connection
            Assert.AreEqual(1, dg.Size);
        }

        [TestMethod]
        public void replaceDependents_For_NonEmptyList()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("a", "d");

            List<string> newDependent = new List<string>();
            newDependent.Add("e");
            newDependent.Add("f");
            newDependent.Add("g");

            dg.ReplaceDependents("a", newDependent);

            for (int i = 0; i < 3; i++)
                Assert.AreEqual(newDependent[i].ToString(), dg.GetDependents("a").ToArray()[i].ToString());
        }

        [TestMethod]
        public void replaceDependents_For_EmptyList()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("b","a");

            List<string> newDependent = new List<string>();
            newDependent.Add("e");
            newDependent.Add("f");
            newDependent.Add("g");

            dg.ReplaceDependents("a", newDependent);

            for (int i = 0; i < 3; i++)
                Assert.AreEqual(newDependent[i].ToString(), dg.GetDependents("a").ToArray()[i].ToString());
        }

        [TestMethod]
        public void replaceDependees_For_EmptyList()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");

            List<string> newDependeeList = new List<string>();
            newDependeeList.Add("e");
            newDependeeList.Add("f");
            newDependeeList.Add("g");

            dg.ReplaceDependents("a", newDependeeList);

            for (int i = 0; i < 3; i++)
                Assert.AreEqual(newDependeeList[i].ToString(), dg.GetDependents("a").ToArray()[i].ToString());
        }

        [TestMethod]
        public void Add_AndRemovingDependency_AndReplacing()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            Assert.AreEqual(1, dg.Size);
            dg.RemoveDependency("a", "b");
            Assert.AreEqual(0, dg.Size);
            dg.ReplaceDependents("a", new List<string> { "c", "d", "e", "f" });
            Assert.AreEqual(4, dg.Size);
        }

        [TestMethod]
        public void replaceDependents_AndReplaceDependees_checkIfActuallyReplaced()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("a", "d");

            dg.AddDependency("b", "e");
            dg.AddDependency("b", "f");
            dg.AddDependency("b", "g");

            Assert.AreEqual(0, dg["a"]);
            dg.ReplaceDependees("a", new List<string> { "b" });
            Assert.AreEqual(1, dg["a"]);
            dg.ReplaceDependees("b", new List<string> { "", "b", "c" });
            Assert.IsFalse(dg.GetDependees("b").Contains("a"));
            Assert.AreEqual(3,dg.GetDependees("b").Count());

        }

        [TestMethod]
        public void tryReplaceDuplicateDependents()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("a", "d");

            dg.ReplaceDependents("a", new List<string> { "a", "a", "b", "b", "c" });
            Assert.AreEqual(3, dg.GetDependents("a").Count());
        }

        [TestMethod]
        public void tryReplaceDuplicateDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            dg.AddDependency("d", "b");

            dg.ReplaceDependees("b", new List<string> { "e", "d", "e", "b", "c" });
            Assert.AreEqual(4, dg.GetDependees("b").Count());
            Assert.IsFalse(dg.GetDependees("b").Contains("a"));
        }


    }
}

