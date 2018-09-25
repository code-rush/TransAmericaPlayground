using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph {

//	GameController gameController;
	public ArrayList nodes, edges, bridgeNodeList;

	public Graph() {
		this.nodes = new ArrayList ();
		this.edges = new ArrayList ();
//		this.bridgeNodeList = new ArrayList ();
//		foreach (Vector3 x in bridgeArray) {
//			this.bridgeNodeList.Add (x);
//		}
	}

	public void addArrayToGraph(ArrayList _array) {
		this.bridgeNodeList = new ArrayList ();

//		foreach (Vector3 x in _array) {
//			Debug.Log ("_array has bridge nodes " + x);
//		}
		foreach (Vector3 x in _array) {
			this.bridgeNodeList.Add (x);
		}
		//foreach (Vector3 x in bridgeNodeList) {
			//Debug.Log ("graph bridge node list has nodes " + x);
		//}
	}

	public void insert_node(Vector3 value) {
		Node new_node = new Node (value);
		this.nodes.Add (new_node);
	}

	public void insert_edge(Vector3 x, Vector3 y) {
		Node nodeA = null;
		Node nodeB = null;

		foreach (Node n in this.nodes) {
			if (x == n.pos) {
				nodeA = n;
			}
			if (y == n.pos) {
				nodeB = n;
			}
		}
		if (nodeA == null) {
			nodeA = new Node (x);
			this.nodes.Add (nodeA);
		}
		if (nodeB == null) {
			nodeB = new Node (y);
			this.nodes.Add (nodeB);
		}

		Vector3 position = new Vector3 ();
		if (nodeA.pos.z == nodeB.pos.z) {
			position = new Vector3 (((nodeA.pos.x + nodeB.pos.x) / 2f), 1f, nodeA.pos.z);
			//			if ((nodeA.pos.x + 1f) == nodeB.pos.x) {
			//				position = new Vector3 (((nodeA.pos.x + nodeB.pos.x) / 2f), 1f, nodeA.pos.z);
			//			}
			//			if ((nodeA.pos.x - 1f) == nodeB.pos.x) {
			//				position = new Vector3 (((nodeA.pos.x + nodeB.pos.x) / 2f), 1f, nodeA.pos.z);
			//			}
		}
		if ((nodeA.pos.z < nodeB.pos.z) || (nodeA.pos.z > nodeB.pos.z)) {
			position = new Vector3 (((nodeA.pos.x + nodeB.pos.x) / 2f), 1f, ((nodeA.pos.z + nodeB.pos.z) / 2f));
			//			if (nodeA.pos.x > nodeB.pos.x) {
			//				position = new Vector3 (((nodeA.pos.x + nodeB.pos.x) / 2f), 1f, ((nodeA.pos.z + nodeB.pos.z) / 2f));
			//			}
			//			if (nodeA.pos.x < nodeB.pos.x) {
			//				position = new Vector3 (((nodeA.pos.x + nodeB.pos.x) / 2f), 1f, ((nodeA.pos.z + nodeB.pos.z) / 2f));
			//			}
		}
		//		if (nodeA.pos.z > nodeB.pos.z) {
		//			if (nodeA.pos.x < nodeB.pos.x) {
		//				position = new Vector3 (((nodeA.pos.x + nodeB.pos.x) / 2f), 1f, ((nodeA.pos.z + nodeB.pos.z) / 2f));
		//			}
		//			if (nodeA.pos.x > nodeB.pos.x) {
		//				position = new Vector3 (((nodeA.pos.x + nodeB.pos.x) / 2f), 1f, ((nodeA.pos.z + nodeB.pos.z) / 2f));
		//			}
		//		}

//		foreach(Vector3 X in this.bridgeNodeList) {
//			Debug.Log ("Game Controller has access to bridge List" + x);
//		}

		if (bridgeNodeList.Contains (x) && bridgeNodeList.Contains (y)) {
			Edge new_edge = new Edge (2, nodeA, nodeB, position);
			nodeA.edges.Add (new_edge);
			nodeB.edges.Add (new_edge);
			this.edges.Add (new_edge);
		} else {
			Edge new_edge = new Edge (1, nodeA, nodeB, position);
			nodeA.edges.Add (new_edge);
			nodeB.edges.Add (new_edge);
			this.edges.Add (new_edge); 
		}
	}

	public ArrayList get_edge_list() {
		ArrayList edge_list = new ArrayList ();
		foreach (Edge x in this.edges) {
			ArrayList edge = new ArrayList ();
			edge.Add (x.value.ToString ());
			edge.Add (x.nodeX.pos.ToString ());
			edge.Add (x.nodeY.pos.ToString ());
			edge.Add (x.position.ToString ("F3"));
			edge_list.Add (edge);
		}
		return edge_list;
	}

	public List<Vector3> get_neighbour_nodes(Vector3 v) {
		List<Vector3> neighbour_nodes = new List<Vector3> ();
		Node node = get_node (v);
		foreach (Edge e in node.edges) {
			if (e.nodeX.pos == node.pos) {
				neighbour_nodes.Add (e.nodeY.pos);
			} else {
				neighbour_nodes.Add (e.nodeX.pos);
			}
		}
		return neighbour_nodes;
	}

	public int get_distance_between_nodes(Vector3 nodeX, Vector3 nodeY) {
		int distance = new int();
		foreach (Edge e in this.edges) {
			if (e.nodeX.pos == nodeX && e.nodeY.pos == nodeY) {
				distance = e.value; 
			}
			if (e.nodeX.pos == nodeY && e.nodeY.pos == nodeX) {
				distance = e.value;
			}
		}
		return distance;
	}

	public bool node_exists(Vector3 vec) {
		foreach (Node x in this.nodes) {
			if (x.pos == vec) {
				return true;
			}
		}
		return false;
	}

	public Node get_node(Vector3 vec) {
		Node y = null;
		foreach (Node x in this.nodes) {
			if (x.pos == vec) {
				y = x;
				return y;
			}
		}
		return y;
	}
}

public class Node {

	public ArrayList edges;
	public Vector3 pos;

	public Node(Vector3 value) {
		this.pos = value;
		this.edges = new ArrayList ();
	}
}

public class Edge {
	public int value;
	public Node nodeX, nodeY;
	public Vector3 position;

	public Edge(int value, Node nodeX, Node nodeY, Vector3 pos_value) {
		this.value = value;
		this.nodeX = nodeX;
		this.nodeY = nodeY;
		this.position = pos_value;
	}
}
