using System;
using System.Collections.Generic;

namespace Tucil3Code
{
    class Node
    {
        // Nama, koordinat, serta fgh simpul
        public string name;
        public double x;
        public double y;
        public List<Node> adjacent;
        public double gValue;
        public double hValue;
        public double fValue;
        public Node parent;

        // Constructor
        public Node(string name, double x, double y)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.adjacent = new List<Node>();
            this.gValue = 0;
            this.hValue = 0;
            this.fValue = 0;
            this.parent = null;
        }

        // Copy Constructor
        public Node(Node other)
        {
            this.name = other.name;
            this.x = other.x;
            this.y = other.y;
            this.adjacent = new List<Node>();
            foreach (Node el in other.adjacent)
            {
                this.adjacent.Add(el);
            }
            this.gValue = other.gValue;
            this.hValue = other.hValue;
            this.fValue = other.fValue;
            this.parent = other.parent;
        }

        // Setter
        public void setParent(Node parent)
        {
            this.parent = parent;
        }

        public void setgValue(double gValue)
        {
            this.gValue = gValue;
        }

        public void sethValue(Node goal)
        {
            this.hValue = AStar.EuclideanDistance(this, goal);
        }

        public void setfValue()
        {
            this.fValue = this.gValue + this.hValue;
        }

        public void setAll(double gValue, Node parent, Node goal)
        {
            this.setParent(parent);
            this.setgValue(gValue);
            this.sethValue(goal);
            this.setfValue();
        }

        public void setStart(Node goal)
        {
            this.setgValue(0);
            this.sethValue(goal);
            this.setfValue();
        }

        public void addAdjacent(Node adj)
        {
            this.adjacent.Add(adj);
        }
    }

    class AStar
    {
        // Mencari rute dengan algoritma A*
        public static List<string> AStarAlgorithm(Node start, Node goal)
        {
            List<Node> toCalculate = new List<Node>();
            List<Node> calculated = new List<Node>();
            List<string> result = new List<string>();

            // Tambahkan node start
            Node first = new Node(start);
            first.setStart(goal);
            toCalculate.Add(first);

            // Algoritma A*
            Node min;
            while (toCalculate.Count != 0)
            {
                // Mencari simpul dengan F minimum
                min = FindMinimumF(toCalculate);
                toCalculate.Remove(min);
                calculated.Add(min);

                // Menelusuri simpul yang bertetangga
                foreach (Node adj in min.adjacent)
                {
                    double distance = EuclideanDistance(min, adj) + min.gValue;
                    Node newAdj = new Node(adj);
                    newAdj.setAll(distance, min, goal);

                    if (newAdj.name == goal.name)
                    {
                        calculated.Add(newAdj);
                        break;
                    }

                    if (!isExistLowerF(calculated,newAdj))
                    {
                        toCalculate.Add(newAdj);
                    }
                }

                // Jika sudah sampai goal, hentikan pencarian
                if (calculated[calculated.Count - 1].name == goal.name)
                {
                    break;
                }
            }

            // Jika node tidak terhubung
            if (calculated[calculated.Count - 1].name != goal.name)
            {
                return result;
            }

            // Menambahkan nama node dan jarak total ke dalam list yang berisi path dari start sampai goal
            Node currNode = calculated.Find(el => el.name == goal.name);
            double dist = Math.Round(currNode.gValue * 111, 3);
            string totalDist = dist.ToString();
            result.Add(totalDist);
            while (currNode.name != start.name)
            {
                result.Add(currNode.name);
                currNode = calculated.Find(el => el.name == currNode.parent.name);
            }
            result.Add(currNode.name);
            result.Reverse();

            return result;
        }

        // Pengecekan keberadaan node yang sama, sudah ditelusuri, memiliki nilai f lebih kecil
        public static bool isExistLowerF(List<Node> toCalculate, Node current)
        {
            bool exist = false;
            foreach (Node check in toCalculate)
            {
                if (check.name == current.name && check.fValue < current.fValue)
                {
                    exist = true;
                }
            }
            return exist;
        }

        // Mencari node dengan f terkecil dalam List
        public static Node FindMinimumF(List<Node> toCalculate)
        {
            Node min = toCalculate[0];
            foreach (Node vertex in toCalculate)
            {
                if (vertex.fValue < min.fValue)
                {
                    min = vertex;
                }
            }

            return min;
        }

        // Menghitung jarak Euclidean untuk h(n)
        public static double EuclideanDistance(Node current, Node target)
        {
            return Math.Sqrt(Math.Pow(current.x - target.x, 2) + Math.Pow(current.y - target.y, 2));
        }
    }
}
