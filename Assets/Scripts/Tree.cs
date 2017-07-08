using UnityEngine;
using System.Collections;

namespace IncrementalTree {

	public class Node {

		public Vector3 value;
		public Node topRight;
		public Node right;
		public Node bottomRight;
		public Node bottomLeft;
		public Node left;
		public Node topLeft;

		public Node(Vector3 value) {
			this.value = value;
			this.topLeft = null;
			this.topRight = null;
			this.left = null;
			this.right = null;
			this.bottomLeft = null;
			this.bottomRight = null;
		}

	}

	public class Tree{

		public Node root;
		public int count;

		public Tree() {
			root = null;
//			count = 0;
		}

		public Tree(Vector3 value) {
			root = new Node (value);
		}

		public void addToTree(Vector3 value) {
			if (root == null) {
				Node newNode = new Node (value);
				root = newNode;
				return;
			}

			Node currentNode = root;
			bool added = false;

			do {
				if (currentNode == root) {
					if ((value.z == currentNode.value.z) && (value.x == (currentNode.value.x - 1))) {
						if (currentNode.left == null) {
							Node newNode = new Node(value);
							currentNode.left = newNode;
							added = true;
						} else {
							currentNode = currentNode.left;
						}
					}

					if ((value.z == currentNode.value.z) && (value.x == (currentNode.value.x + 1))) {
						if (currentNode.right == null) {
							Node newNode = new Node(value);
							currentNode.right = newNode;
							added = true;
						} else {
							currentNode = currentNode.right;
						}
					}

					if ((value.z == (currentNode.value.z - 1)) && (value.x == (currentNode.value.x - 0.5))) {
						if (currentNode.bottomLeft == null) {
							Node newNode = new Node(value);
							currentNode.bottomLeft = newNode;
							added = true;
						} else {
							currentNode = currentNode.bottomLeft;
						}
					}

					if ((value.z == (currentNode.value.z - 1)) && (value.x == (currentNode.value.x + 0.5))) {
						if (currentNode.bottomRight == null) {
							Node newNode = new Node(value);
							currentNode.bottomRight = newNode;
							added = true;
						} else {
							currentNode = currentNode.bottomRight;
						}
					}

					if ((value.z == (currentNode.value.z + 1)) && (value.x == (currentNode.value.x + 0.5))) {
						if (currentNode.topRight == null) {
							Node newNode = new Node(value);
							currentNode.topRight = newNode;
							added = true;
						} else {
							currentNode = currentNode.topRight;
						}
					}

					if ((value.z == (currentNode.value.z + 1)) && (value.x == (currentNode.value.x - 0.5))) {
						if (currentNode.topLeft == null) {
							Node newNode = new Node(value);
							currentNode.topLeft = newNode;
							added = true;
						} else {
							currentNode = currentNode.topLeft;
						}
					}
				} else {
					if (isParent(currentNode, value) == true) {
						added = true;
					} else {
						if ((value.z == currentNode.value.z) && (value.x == (currentNode.value.x - 1))) {
							if (currentNode.left == null) {
								Node newNode = new Node(value);
								currentNode.left = newNode;
								added = true;
							} else {
								currentNode = currentNode.left;
							}
						}

						if ((value.z == currentNode.value.z) && (value.x == (currentNode.value.x + 1))) {
							if (currentNode.right == null) {
								Node newNode = new Node(value);
								currentNode.right = newNode;
								added = true;
							} else {
								currentNode = currentNode.right;
							}
						}

						if ((value.z == (currentNode.value.z - 1)) && (value.x == (currentNode.value.x - 0.5))) {
							if (currentNode.bottomLeft == null) {
								Node newNode = new Node(value);
								currentNode.bottomLeft = newNode;
								added = true;
							} else {
								currentNode = currentNode.bottomLeft;
							}
						}

						if ((value.z == (currentNode.value.z - 1)) && (value.x == (currentNode.value.x + 0.5))) {
							if (currentNode.bottomRight == null) {
								Node newNode = new Node(value);
								currentNode.bottomRight = newNode;
								added = true;
							} else {
								currentNode = currentNode.bottomRight;
							}
						}

						if ((value.z == (currentNode.value.z + 1)) && (value.x == (currentNode.value.x + 0.5))) {
							if (currentNode.topRight == null) {
								Node newNode = new Node(value);
								currentNode.topRight = newNode;
								added = true;
							} else {
								currentNode = currentNode.topRight;
							}
						}

						if ((value.z == (currentNode.value.z + 1)) && (value.x == (currentNode.value.x - 0.5))) {
							if (currentNode.topLeft == null) {
								Node newNode = new Node(value);
								currentNode.topLeft = newNode;
								added = true;
							} else {
								currentNode = currentNode.topLeft;
							}
						}
					}

				}

			} while(!added);

		}

		bool isParent(Node node, Vector3 val) {

			Node current = root;
				
			bool flag = false;

			do {
				if (current.value == val && (node.value == current.left.value || node.value == current.right.value || node.value == current.bottomLeft.value || node.value == current.bottomRight.value || node.value == current.topLeft.value || node.value == current.topRight.value)) {
					return true;
				} else if (current.value.z == val.z) {
					if (current.value.x < val.x) {
						current = current.right;
					} else {
						current = current.left;
					}
				} else if (current.value.z < val.z) {
					if (current.value.x < val.x) {
						current = current.topRight;
					} else {
						current = current.topLeft;
					}
				} else if (current.value.z > val.z){
					if (current.value.x < val.x) {
						current = current.bottomRight;
					} else {
						current = current.bottomLeft;
					}
				}
			} while(!flag);
			return false;
		}

	}

}

