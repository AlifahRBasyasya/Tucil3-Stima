using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tucil3Code;

namespace Tucil3Stima
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OpenFileDialog ofd = new OpenFileDialog();
        private int countNodes;
        private List<string> nodesName;
        private List<Node> listOfNodes;
        private string[] isiFile;
        private List<List<double>> adjMatrix;

        private void button1_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Text Documents|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Clear comboBox Item
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();

                // Munculkan path file
                label4.Text = ofd.FileName;

                // Baca seluruh isi file
                string readFile = System.IO.File.ReadAllText(ofd.FileName);

                // Konversi isi file ke array
                isiFile = readFile.Split(new[] {"\r\n", " " }, StringSplitOptions.RemoveEmptyEntries);
                List<string> listIsiFile = isiFile.ToList<string>();

                // Menyimpan jumlah simpul
                countNodes = Int16.Parse(listIsiFile[0]);
                listIsiFile.RemoveAt(0);

                // List Nama Simpul
                nodesName = new List<string>();

                // List of Nodes, tambahkan di comboBox
                listOfNodes = new List<Node>();
                for (int i = 0; i < countNodes; i++)
                {
                    nodesName.Add(listIsiFile[0]);
                    comboBox1.Items.Add(listIsiFile[0]);
                    comboBox2.Items.Add(listIsiFile[0]);
                    double xValue = Double.Parse(listIsiFile[1], CultureInfo.InvariantCulture);
                    double yValue = Double.Parse(listIsiFile[2], CultureInfo.InvariantCulture);
                    Node eachNode = new Node(listIsiFile[0], xValue, yValue);
                    listOfNodes.Add(eachNode);
                    listIsiFile.RemoveRange(0, 3);
                }

                // Tampilan graf
                Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");

                bool notConnected;
                adjMatrix = new List<List<double>>();
                // Nodes tetangga, graf, matriks ketetanggaan
                for (int j = 0; j < countNodes; j++)
                {
                    notConnected = true;
                    List<double> subList = new List<double>();
                    for (int k = 0; k < countNodes; k++)
                    {
                        Double value = Double.Parse(listIsiFile[k]);
                        subList.Add(value);
                        if (value != 0)
                        {
                            notConnected = false;
                            if (k > j)
                            {
                                graph.AddEdge(nodesName[j], listIsiFile[k], nodesName[k]);
                            }
                            listOfNodes[j].addAdjacent(listOfNodes[k]);
                        }
                    }
                    adjMatrix.Add(subList);
                    if (notConnected)
                    {
                        graph.AddNode(nodesName[j]);
                    }
                    listIsiFile.RemoveRange(0, countNodes);
                }

                // Tambahkan node pada graf
                for (int l = 0; l < countNodes; l++)
                {
                    Microsoft.Msagl.Drawing.Node nodeInGraph = graph.FindNode(nodesName[l]);
                    nodeInGraph.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                }

                // Ubah menjadi graf tidak berarah
                foreach (var edge in graph.Edges)
                {
                    edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;
                }

                // Bind graph to viewer
                gViewer1.Graph = graph;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string inStart = comboBox1.Text;
            string inGoal = comboBox2.Text;
            Node start = listOfNodes.Find(node => node.name == inStart);
            Node goal = listOfNodes.Find(node => node.name == inGoal);

            List<string> result = AStar.AStarAlgorithm(start, goal);

            // Jika terdapat path tampilkan pada graf, jika tidak tampilkan pesan error

            if (result.Count == 0)
            {
                label5.Text = "Tidak terdapat jalur karena tempat tidak terhubung";
            }
            else
            {
                // Tuple rute yang dilalui
                Tuple<string, string>[] coloredEdge = new Tuple<string, string>[result.Count - 2];
                for (int i = 0; i < result.Count - 2; i++)
                {
                    coloredEdge[i] = Tuple.Create(result[i], result[i + 1]);
                }

                // Membuat jalur pada graf
                List<string> listIsiFile = isiFile.ToList<string>();

                // Hapus jumlah simpul di awal dan informasi simpul
                listIsiFile.RemoveRange(0, (countNodes * 3) + 1);

                // Tampilan graf
                Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");


                bool have1, have2, notConnected;
                // Nodes tetangga dan graf
                for (int j = 0; j < countNodes; j++)
                {
                    notConnected = true;
                    for (int k = 0; k < countNodes; k++)
                    {
                        have1 = coloredEdge.Any(t => t.Item1 == nodesName[j] && t.Item2 == nodesName[k]);
                        have2 = coloredEdge.Any(t => t.Item1 == nodesName[k] && t.Item2 == nodesName[j]);
                        Double value = Double.Parse(listIsiFile[k]);
                        if (value != 0)
                        {
                            notConnected = false;
                            if (k > j)
                            {
                                // Mewarnai sisi yang dilewati
                                if (have1 || have2)
                                {
                                    graph.AddEdge(nodesName[j], listIsiFile[k], nodesName[k]).Attr.Color = Microsoft.Msagl.Drawing.Color.Coral;
                                }
                                else
                                {
                                    graph.AddEdge(nodesName[j], listIsiFile[k], nodesName[k]);
                                }
                            }
                            listOfNodes[j].addAdjacent(listOfNodes[k]);
                        }
                    }
                    if (notConnected)
                    {
                        graph.AddNode(nodesName[j]);
                    }
                    listIsiFile.RemoveRange(0, countNodes);
                }

                // Menambahkan node pada graf
                for (int l = 0; l < countNodes; l++)
                {
                    Microsoft.Msagl.Drawing.Node nodeInGraph = graph.FindNode(nodesName[l]);
                    nodeInGraph.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                }

                // Mewarnai node
                for (int n = 0; n < result.Count - 1; n++)
                {
                    graph.FindNode(result[n]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.BlanchedAlmond;
                }

                // Ubah menjadi graf tidak berarah
                foreach (var edge in graph.Edges)
                {
                    edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;
                }

                // Bind graph to viewer
                gViewer1.Graph = graph;

                // Menampilkan jarak total
                label12.Text = result[result.Count - 1];
            }
        }
    }
}
