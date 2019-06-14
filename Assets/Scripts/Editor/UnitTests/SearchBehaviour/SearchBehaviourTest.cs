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
            // 000
            // 000
            // 000
            navigationGraph = new NavigationGraph(completeMap, true, new List<Tile>() { Tile.Wall });
        }

        [Test]
        public void Should_ReturnCornerCoordinates_When_PathsmoothingAroundCorner()
        {
            // Arrange
            InitCornerMap();
            List<Vector2Int> coordinatesToTraverse = new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) };
            Vector2Int expected = new Vector2Int(2, 0);
            // Act
            Vector2Int actual = PathSmoothing.FarthestCoordinateToReach(new Vector2(0, 0), coordinatesToTraverse, navigationGraph, 0.95f);
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_ReturnGoalCoordinates_When_PathsmoothingThroughEmptyRoom()
        {
            // Arrange
            InitOpenMap();
            List<Vector2Int> coordinatesToTraverse = new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) };
            Vector2Int expected = new Vector2Int(2, 2);
            // Act
            Vector2Int actual = PathSmoothing.FarthestCoordinateToReach(new Vector2(0, 0), coordinatesToTraverse, navigationGraph, 0.95f);
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_ReturnEntranceCoordinates_When_PathsmoothingIntoCorridor()
        {
            // Arrange
            InitCorridorMap();
            List<Vector2Int> coordinatesToTraverse = new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2), new Vector2Int(2, 3), new Vector2Int(2, 4) };
            Vector2Int expected = new Vector2Int(2, 2);
            // Act
            Vector2Int actual = PathSmoothing.FarthestCoordinateToReach(new Vector2(0, 0), coordinatesToTraverse, navigationGraph, 0.95f);
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_ReturnCoordinatesAroundPositionVector_When_CallingGetIlluminatedCoordinates()
        {
            // Arrange
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            Vector2Int position = new Vector2Int(2, 3);
            List<Vector2Int> expectedCoordinates = new List<Vector2Int>();
            for (int i = -2; i <= 2; i++) // 2 for the range of the pointlight
            {
                for (int j = -2; j <= 2; j++) // 2 for the range of the pointlight
                {
                    int x = position.x + i;
                    int y = position.y + j;
                    // Check if coordinate is inside the map
                    if (x < 0 || x >= environment.GetMap().TileMap.Length || y < 0 || y >= environment.GetMap().TileMap[0].Length)
                    {
                        continue;
                    }
                    expectedCoordinates.Add(new Vector2Int(x, y));
                }
            }
            // Act
            HashSet<Vector2Int> actualCoordinates = environment.GetIlluminatedCoordinates(position);
            // Assert
            for (int i = 0; i < expectedCoordinates.Count; i++)
            {
                Assert.IsTrue(actualCoordinates.Contains(expectedCoordinates[i]), "Because there are no lights in the scene, the bot should only be able to see 2 coordinates around itself in any direction");
            }
        }

        [Test]
        public void Should_ReturnClosestEnemy_When_OnlyOneEnemyInSight()
        {
            // Arrange
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
            enemy2.transform.position = new Vector3(3, 0, 3);
            Soldier otherEnemy = enemy2.AddComponent<Player>();
            otherEnemy.Team = enemyTeam;
            List<Soldier> enemies = new List<Soldier>() { expectedClosestEnemy, otherEnemy };
            // Act
            Soldier actualClosestEnemy = environment.GetClosestIlluminatedEnemy(me, enemies);
            // Assert
            Assert.AreEqual(expectedClosestEnemy, actualClosestEnemy);
        }

        [Test]
        public void Should_ReturnClosestEnemy_When_TwoEnemiesInSight()
        {
            // Arrange
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
            // Act
            Soldier actualClosestEnemy = environment.GetClosestIlluminatedEnemy(me, enemies);
            // Assert
            Assert.AreEqual(expectedClosestEnemy, actualClosestEnemy);
        }

        [Test]
        public void Should_ReturnNull_When_NoEnemiesExist()
        {
            // Arrange
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            Team myTeam = new Team(1);
            Team enemyTeam = new Team(2);
            GameObject myOb = new GameObject();
            myOb.transform.position = new Vector3(0, 0, 0);
            Soldier me = myOb.AddComponent<Player>();
            me.Team = myTeam;
            List<Soldier> enemies = new List<Soldier>();
            // Act
            Soldier actualClosestEnemy = environment.GetClosestIlluminatedEnemy(me, enemies);
            // Assert
            Assert.IsNull(actualClosestEnemy);
        }

        [Test]
        public void Should_ReturnNull_When_NoEnemiesInSight()
        {
            // Arrange
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            Team myTeam = new Team(1);
            Team enemyTeam = new Team(2);
            GameObject myOb = new GameObject();
            myOb.transform.position = new Vector3(0, 0, 0);
            Soldier me = myOb.AddComponent<Player>();
            me.Team = myTeam;
            GameObject enemy1 = new GameObject();
            enemy1.transform.position = new Vector3(4, 0, 4);
            Soldier expectedClosestEnemy = enemy1.AddComponent<Player>();
            expectedClosestEnemy.Team = enemyTeam;
            GameObject enemy2 = new GameObject();
            enemy2.transform.position = new Vector3(4, 0, 4);
            Soldier otherEnemy = enemy2.AddComponent<Player>();
            otherEnemy.Team = enemyTeam;
            List<Soldier> enemies = new List<Soldier>() { expectedClosestEnemy, otherEnemy };
            // Act
            Soldier actualClosestEnemy = environment.GetClosestIlluminatedEnemy(me, enemies);
            // Assert
            Assert.IsNull(actualClosestEnemy);
        }

        [Test]
        public void Should_ReturnTheCorrectPath_When_KnowingTheMap()
        {
            // Arrange
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            DStarLite dStarLite = new DStarLite(environment, true);
            Vector2Int startNode = new Vector2Int(0, 0);
            Vector2Int endNode = new Vector2Int(2, 4);
            dStarLite.RunDStarLite(startNode, endNode);
            List<Vector2Int> expectedCoordinates = new List<Vector2Int>()
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, 2),
                new Vector2Int(1, 2),
                new Vector2Int(2, 2),
                new Vector2Int(2, 3),
                endNode
            };
            // Act
            List<Vector2Int> actualCoordinates = dStarLite.CoordinatesToTraverse();
            // Assert
            for (int i = 0; i < actualCoordinates.Count; i++)
            {
                Assert.AreEqual(expectedCoordinates[i], actualCoordinates[i]);
            }
        }

        [Test]
        public void Should_ReturnWrongShortestPath_When_NotKnowingTheMap()
        {
            // Arrange
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            DStarLite dStarLite = new DStarLite(environment, false);
            Vector2Int startNode = new Vector2Int(0, 0);
            Vector2Int endNode = new Vector2Int(2, 4);
            dStarLite.RunDStarLite(startNode, endNode);
            List<Vector2Int> expectedCoordinates = new List<Vector2Int>()
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, 2),
                new Vector2Int(0, 3),
                new Vector2Int(0, 4),
                new Vector2Int(1, 4),
                endNode
            };
            // Act
            List<Vector2Int> actualCoordinates = dStarLite.CoordinatesToTraverse();
            // Assert
            for (int i = 0; i < actualCoordinates.Count; i++)
            {
                Assert.AreEqual(expectedCoordinates[i], actualCoordinates[i], "Because the bot doesn't know the map, it will think it can go through the walls. (Since it doesn't know there are walls over there)");
            }
        }

        [Test]
        public void Should_ReturnTheCorrectPath_When_NotKnowingTheMapButBeingAbleToSeeTheWalls()
        {
            // Arrange
            InitCorridorMap();
            environment = GameEnvironment.CreateInstance(completeMap, new List<Tile>() { Tile.Wall });
            DStarLite dStarLite = new DStarLite(environment, true);
            Vector2Int startNode = new Vector2Int(2, 4);
            Vector2Int endNode = new Vector2Int(0, 0);
            dStarLite.RunDStarLite(startNode, endNode);
            List<Vector2Int> expectedCoordinates = new List<Vector2Int>()
            {
                new Vector2Int(2, 3),
                new Vector2Int(2, 2),
                new Vector2Int(1, 2),
                new Vector2Int(0, 2),
                new Vector2Int(0, 1),
                endNode
            };
            // Act
            List<Vector2Int> actualCoordinates = dStarLite.CoordinatesToTraverse();
            // Assert
            for (int i = 0; i < actualCoordinates.Count; i++)
            {
                Assert.AreEqual(expectedCoordinates[i], actualCoordinates[i], "Since the walls are now in range of the bot. The bot can see the walls and plan it's path around them.");
            }
        }

        [Test]
        public void Should_ReturnTrue_When_GettingNodeWithObstacleThroughXY()
        {
            // Arrange
            InitCornerMap();
            // Act
            Node node = navigationGraph.GetNode(0, 1);
            // Assert
            Assert.IsTrue(node.IsObstacle(), "Because this node contains an obstacle, it should return true");
        }

        [Test]
        public void Should_ReturnFalse_When_GettingNodeWithoutObstacleThroughXY()
        {
            // Arrange
            InitCornerMap();
            // Act
            Node node = navigationGraph.GetNode(0, 0);
            // Assert
            Assert.IsFalse(node.IsObstacle(), "Because this node contains no obstacle, it should return false");
        }

        [Test]
        public void Should_ReturnDefaultNode_When_TryGettingNodeOutsideMapThroughXY()
        {
            // Arrange
            InitCornerMap();
            // Act
            Node node = navigationGraph.GetNode(-1, -1);
            // Assert
            Assert.AreEqual(node, default(Node), "Because this node falls outside the map, it should return a default node");
        }

        [Test]
        public void Should_ReturnTrue_When_GettingNodeWithObstacleThroughVector()
        {
            // Arrange
            InitCornerMap();
            // Act
            Node node = navigationGraph.GetNode(new Vector2Int(0, 1));
            // Assert
            Assert.IsTrue(node.IsObstacle(), "Because this node contains an obstacle, it should return true");
        }

        [Test]
        public void Should_ReturnFalse_When_GettingNodeWithoutObstacleThroughVector()
        {
            // Arrange
            InitCornerMap();
            // Act
            Node node = navigationGraph.GetNode(new Vector2Int(0, 0));
            // Assert
            Assert.IsFalse(node.IsObstacle(), "Because this node contains no obstacle, it should return false");
        }

        [Test]
        public void Should_ReturnDefaultNode_When_TryGettingNodeOutsideMapThroughVector()
        {
            // Arrange
            InitCornerMap();
            // Act
            Node node = navigationGraph.GetNode(new Vector2Int(-1, -1));
            // Assert
            Assert.AreEqual(node, default(Node), "Because this node falls outside the map, it should return a default node");
        }
    }
}
