// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SpreadsheetUtilities
{

    /// <summary>
    ///
    /// Author      : Rohith Veeramachaneni
    /// Partner     : None
    /// Date Created: Jan 27,2023
    ///
    ///
    /// 
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<string>> nodesGraphDependents;
        private Dictionary<string, HashSet<string>> nodesGraphDependees;


        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            nodesGraphDependents = new();
            nodesGraphDependees = new();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            
            get
            {
                int totalOrderedPairsCount = 0;
                // gets all the keys in the dictionary
                foreach (string key in nodesGraphDependents.Keys)
                {
                    // gets all the dependents for the string key and adds their count to
                    // get total number of pairs that exist in the dictionary
                    totalOrderedPairsCount += nodesGraphDependents[key].Count();
                }
                return totalOrderedPairsCount;
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { return this.GetDependees(s).Count(); }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            // checks if the given string s exists as a key in the dictionary
            if (nodesGraphDependents.ContainsKey(s))
            {
                if (nodesGraphDependents[s].Count != 0)
                    return true;
                return false;
            }
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return this[s]!=0; // checks if size of dependees is not zero
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if(nodesGraphDependents.ContainsKey(s))
                return nodesGraphDependents[s];
            return new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s) =>getDependeeList(s);


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            // checks if string s is already existent in dictionary
            if (!nodesGraphDependents.ContainsKey(s))
            {
                nodesGraphDependents.Add(s, new HashSet<string>());
                nodesGraphDependents[s].Add(t);
                // adds another empty dependency connection for string t given 
                if (!nodesGraphDependees.ContainsKey(t))
                {
                    nodesGraphDependees.Add(t, new HashSet<string>());
                    nodesGraphDependees[t].Add(s);
                }
                else
                    nodesGraphDependees[t].Add(s);
            }
            else
            {
                nodesGraphDependents[s].Add(t);
                if (!nodesGraphDependees.ContainsKey(t))
                {
                    nodesGraphDependees.Add(t, new HashSet<string>());
                    nodesGraphDependees[t].Add(s);
                }
                else
                    nodesGraphDependees[t].Add(s);

            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            // checks if both given string s and t are existent in dictionary before trying
            // to delete them
            if (nodesGraphDependents.ContainsKey(s) && nodesGraphDependents[s].Contains(t))
            {
                nodesGraphDependents[s].Remove(t);
                if (nodesGraphDependents[s].Count() == 0)
                    nodesGraphDependents.Remove(s);
                if (nodesGraphDependees.ContainsKey(t) && nodesGraphDependees[t].Contains(s))
                {
                    nodesGraphDependees[t].Remove(s);
                    if (nodesGraphDependees[t].Count() == 0)
                        nodesGraphDependees.Remove(t);
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (nodesGraphDependents.ContainsKey(s))
            {
                // resests the dependents of key string s if any exists
                if (nodesGraphDependents[s].Count() != 0)
                {
                    foreach (string dependent in GetDependents(s))
                        RemoveDependency(s, dependent);
                    nodesGraphDependents[s] = new HashSet<string>();
                }
                // checks if needed to iterate through given IEnumerable and adds them to the
                // string key s into the dictionary
                if (newDependents.Count() != 0)
                {
                    foreach (string toBeReplacedValue in newDependents)
                    {
                        this.AddDependency(s, toBeReplacedValue);
                    }
                }
            }
            else
            {
                // checks if needed to iterate through given IEnumerable and adds them to the
                // string key s into the dictionary
                if (newDependents.Count() != 0)
                {
                    foreach (string newDependent in newDependents)
                        AddDependency(s, newDependent);
                }
                else if (newDependents.Count()==0)
                    nodesGraphDependents.Add(s,new HashSet<string>());
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            // removes all the dependee connection for the given dependee string s
            //nodesGraphDependees[s] = new HashSet<string>();
            foreach(string dependee in GetDependees(s))
            {
                RemoveDependency(dependee, s);
            }
                foreach (string newDependee in newDependees)
                {
                AddDependency(newDependee, s);
                }
        }

        /// <summary>
        ///  Helper method to retrieve all the dependents for the given dependee string s and stores them in
        ///  a list
        /// </summary>
        /// <param name="s"></param> Dependee for which all dependents will have to be found
        /// <returns></returns> List of all the dependents of the given dependee
        private IEnumerable<string> getDependeeList(string s)
        {
            if (nodesGraphDependees.ContainsKey(s))
                return nodesGraphDependees[s];
            //    List<string> dependeeList = new List<string>();
            //    // gets all keys in the dictionary
            //        List<string> keyLists = new(nodesGraphDependents.Keys);
            //        foreach (string eachKey in keyLists)
            //        {
            //        // for each key checks if string s is a dependent and if so,
            //        // then key is considered as a dependee and added in dependeeList
            //            if (nodesGraphDependents[eachKey].Contains(s))
            //                dependeeList.Add(eachKey);
            //        }
            //    return dependeeList;
            return new HashSet<string>();
        }


    }
}

