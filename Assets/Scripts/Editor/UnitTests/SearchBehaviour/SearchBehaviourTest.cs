using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SearchBehaviourTest
    {
        Map completeMap;
        NavigationGraph navigationGraph;
        GameEnvironment environment;
        private void InitCornerMap()
        {
            completeMap = Map.GenerateEmptyMap(Tile.Wall, 3, 3, 0);
            completeMap.TileMap[0][0] = Tile.Floor;  // Testmap:
            completeMap.TileMap[1][0] = Tile.Floor;  // 110
            completeMap.TileMap[2][0] = Tile.Floor;  // 110
            completeMap.TileMap[2][1] = Tile.Floor;  // 000
            completeMap.TileMap[2][2] = Tile.Floor;
            navigationGraph = new NavigationGraph(completeMap, true, new List<Tile>() { Tile.Wall });
        }
      
        private void InitCorridorMap()
        {
            completeMap = Map.GenerateEmptyMap(Tile.Floor, 5, 5, 0);
            completeMap.TileMap[0][4] = Tile.Wall;  // Testmap:
            completeMap.TileMap[1][4] = Tile.Wall;  // 11011
            completeMap.TileMap[3][4] = Tile.Wall;  // 11011
            completeMap.TileMap[4][4] = Tile.Wall;  // 00000
            completeMap.TileMap[0][3] = Tile.Wall;  // 00000
            completeMap.TileMap[1][3] = Tile.Wall;  // 00000
            completeMap.TileMap[3][3] = Tile.Wall;
            completeMap.TileMap[4][3] = Tile.Wall;
            navigationGraph = new NavigationGraph(completeMap, true, new List<Tile>() { Tile.Wall });
        }

        private void InitOpenMap()
        {
            completeMap = Map.GenerateEmptyMap(Tile.Floor, 3, 3, 0);
            // Testmap:
            // 110
            // 110
            // 000
            navigationGraph = new NavigationGraph(completeMap, true, new List<Tile>() { Tile.Wall });
        }

        [Test]
        public void PathSmoothingAroundCorner()
        {
            InitCornerMap();
            List<Vector2Int> coordinatesToTraverse = new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) };
            Vector2Int farthestCoordinate = PathSmoothing.FarthestCoordinateToReach(new Vector2(0, 0), coordinatesToTraverse, navigationGraph, 0.95f);
            Assert.AreEqual(new Vector2Int(2, 0), farthestCoordinate);
        }

        [Test]
        public void PathSmoothingThroughOpenRoom()
        {
            InitOpenMap();
            List<Vector2Int> coordinatesToTraverse = new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) };
            Vector2Int farthestCoordinate = PathSmoothing.FarthestCoordinateToReach(new Vector2(0, 0), coordinatesToTraverse, navigationGraph, 0.95f);
            Assert.AreEqual(new Vector2Int(2, 2), farthestCoordinate);
        }

        [Test]
        public void PathSmoothingWithCorridor()
        {
            InitCorridorMap();
            List<Vector2Int> coordinatesToTraverse = new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2), new Vector2Int(2, 3), new Vector2Int(2, 4) };
            Vector2Int farthestCoordinate = PathSmoothing.FarthestCoordinateToReach(new Vector2(0, 0), coordinatesToTraverse, navigationGraph, 0.95f);
            Assert.AreEqual(new Vector2Int(2, 2), farthestCoordinate);
        }

        [Test]
        public void GameEnvironmentIlluminatedCoordinates()
        {
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            HashSet<Vector2Int> actualCoordinates = environment.GetIlluminatedCoordinates(new Vector2Int(2,3));

            List<Vector2Int> expectedCoordinates = new List<Vector2Int>()
            {
                new Vector2Int(1,2),
                new Vector2Int(2,2),
                new Vector2Int(3,2),
                new Vector2Int(1,3),
                new Vector2Int(2,3),
                new Vector2Int(3,3),
                new Vector2Int(1,4),
                new Vector2Int(2,4),
                new Vector2Int(3,4),
            };

            for(int i = 0; i < expectedCoordinates.Count; i++)
            {
                Assert.IsTrue(actualCoordinates.Contains(expectedCoordinates[i]));
            }
        }

        [Test]
        public void GameEnvironmentGetClosestIlluminatedEnemyWithOneEnemyInSight()
        {
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            Team myTeam = new Team(1);
            Team enemyTeam = new Team(2);
            GameObject myOb = new GameObject();
            myOb.transform.position = new Vector3(0, 0, 0);
            Soldier me = myOb.AddComponent<Player>();
            me.Team = myTeam;
            GameObject enemy1 = new GameObject();
            enemy1.transform.position = new Vector3(1, 0, 1);
            Soldier expectedClosestEnemy = enemy1.AddComponent<Player>();
            expectedClosestEnemy.Team = enemyTeam;
            GameObject enemy2 = new GameObject();
            enemy2.transform.position = new Vector3(2, 0, 2);
            Soldier otherEnemy = enemy2.AddComponent<Player>();
            otherEnemy.Team = enemyTeam;
            List<Soldier> enemies = new List<Soldier>() { expectedClosestEnemy, otherEnemy };

            Soldier actualClosestEnemy = environment.GetClosestIlluminatedEnemy(me, enemies);
            Assert.AreEqual(expectedClosestEnemy.transform.position.x, actualClosestEnemy.transform.position.x);
            Assert.AreEqual(expectedClosestEnemy.transform.position.z, actualClosestEnemy.transform.position.z);
        }

        [Test]
        public void GameEnvironmentGetClosestIlluminatedEnemyWithtwoEnemyInSight()
        {
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            Team myTeam = new Team(1);
            Team enemyTeam = new Team(2);
            GameObject myOb = new GameObject();
            myOb.transform.position = new Vector3(0, 0, 0);
            Soldier me = myOb.AddComponent<Player>();
            me.Team = myTeam;
            GameObject enemy1 = new GameObject();
            enemy1.transform.position = new Vector3(1, 0, 0);
            Soldier expectedClosestEnemy = enemy1.AddComponent<Player>();
            expectedClosestEnemy.Team = enemyTeam;
            GameObject enemy2 = new GameObject();
            enemy2.transform.position = new Vector3(1, 0, 1);
            Soldier otherEnemy = enemy2.AddComponent<Player>();
            otherEnemy.Team = enemyTeam;
            List<Soldier> enemies = new List<Soldier>() { expectedClosestEnemy, otherEnemy };

            Soldier actualClosestEnemy = environment.GetClosestIlluminatedEnemy(me, enemies);
            Assert.AreEqual(expectedClosestEnemy.transform.position.x, actualClosestEnemy.transform.position.x);
            Assert.AreEqual(expectedClosestEnemy.transform.position.z, actualClosestEnemy.transform.position.z);
        }

        [Test]
        public void DStarLiteCoordinatesToTraverseWhenKnowingTheMap()
        {
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });

            DStarLite dStarLite = new DStarLite(environment, true);
            Vector2Int startNode = new Vector2Int(0, 0);
            Vector2Int endNode = new Vector2Int(2, 4);
            dStarLite.RunDStarLite(startNode, endNode);
            List<Vector2Int> actualCoordinates = dStarLite.CoordinatesToTraverse();
            List<Vector2Int> expectedCoordinates = new List<Vector2Int>()
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, 2),
                new Vector2Int(1, 2),
                new Vector2Int(2, 2),
                new Vector2Int(2, 3),
                endNode
            };

            for (int i = 0; i < actualCoordinates.Count; i++)
            {
                Assert.AreEqual(expectedCoordinates[i], actualCoordinates[i]);
            }
        }

        [Test]
        public void DStarLiteCoordinatesToTraverseWhenNotKnowingTheMap()
        {
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });

            DStarLite dStarLite = new DStarLite(environment, false);
            Vector2Int startNode = new Vector2Int(0, 0);
            Vector2Int endNode = new Vector2Int(2, 4);
            dStarLite.RunDStarLite(startNode, endNode);
            List<Vector2Int> actualCoordinates = dStarLite.CoordinatesToTraverse();
            List<Vector2Int> expectedCoordinates = new List<Vector2Int>()
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, 2),
                new Vector2Int(0, 3),
                new Vector2Int(0, 4),
                new Vector2Int(1, 4),
                endNode
            };

            for (int i = 0; i < actualCoordinates.Count; i++)
            {
                Assert.AreEqual(expectedCoordinates[i], actualCoordinates[i]);
            }
        }

        [Test]
        public void DStarLiteCoordinatesToTraverseWhenKnowingTheMapButWithWallsInSight()
        {
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });

            DStarLite dStarLite = new DStarLite(environment, true);
            Vector2Int startNode = new Vector2Int(2, 4);
            Vector2Int endNode = new Vector2Int(0, 0);
            dStarLite.RunDStarLite(startNode, endNode);
            List<Vector2Int> actualCoordinates = dStarLite.CoordinatesToTraverse();
            List<Vector2Int> expectedCoordinates = new List<Vector2Int>()
            {
                new Vector2Int(2, 3),
                new Vector2Int(2, 2),
                new Vector2Int(1, 2),
                new Vector2Int(0, 2),
                new Vector2Int(0, 1),
                endNode
            };

            for (int i = 0; i < actualCoordinates.Count; i++)
            {
                Assert.AreEqual(expectedCoordinates[i], actualCoordinates[i]);
            }
        }

        [Test]
        public void NavigationGraphGetNodeXY()
        {
            InitCornerMap();
            Assert.IsFalse(navigationGraph.GetNode(0, 0).IsObstacle());
            Assert.IsTrue(navigationGraph.GetNode(0, 1).IsObstacle());
            Assert.AreEqual(navigationGraph.GetNode(-1, -1), default(Node));
        }

        [Test]
        public void NavigationGraphGetNodeVector()
        {
            InitCornerMap();
            Assert.IsFalse(navigationGraph.GetNode(new Vector2Int(0, 0)).IsObstacle());
            Assert.IsTrue(navigationGraph.GetNode(new Vector2Int(0, 1)).IsObstacle());
            Assert.AreEqual(navigationGraph.GetNode(new Vector2Int(-1, -1)), default(Node));
        }
    }
}
