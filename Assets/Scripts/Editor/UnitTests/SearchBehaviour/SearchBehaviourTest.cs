using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SearchBehaviourTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SearchBehaviourTestSimplePasses()
        {
            List<Vector2Int> coordinatesToTraverse = new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) };
            //Vector2Int farthestCoordinate = PathSmoothing.FarthestCoordinateToReach(new Vector2(0, 0), coordinatesToTraverse, m_dStarLite.Map, 0.95f);
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator SearchBehaviourTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return true;
        }
    }
}
