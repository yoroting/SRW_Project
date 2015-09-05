using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace SimpleAStarExample
{
    public class PathFinder
    {
        private int width;
        private int height;
        private Node[,] nodes;

        private Node startNode;
        private Node endNode;
        private SearchParameters searchParameters;

		//path find cache
		private List<Node> openlst ;
		private List<Point> movelst ;

		//===
		public bool bIsDirty = true;		// check if need refresh
		public  int nMaxStep;		// limit of path find
	     /// <summary>
        /// Create a new instance of PathFinder
        /// </summary>
        /// <param name="searchParameters"></param>
        public PathFinder(SearchParameters searchParameters)
        {
            this.searchParameters = searchParameters;
            InitializeNodes(searchParameters.Map);	//  it create node. 
            this.startNode = this.nodes[searchParameters.StartLocation.X, searchParameters.StartLocation.Y];
            this.startNode.State = NodeState.Open;
            this.endNode = this.nodes[searchParameters.EndLocation.X, searchParameters.EndLocation.Y];

			// path find cache
			openlst = new List<Node>();
			movelst = new List<Point>();

			bIsDirty = true;
        }

		public PathFinder( bool [,] map )
		{
			ApplyMap( map );

			// path find cache
			openlst = new List<Node>();
			movelst = new List<Point>();

			bIsDirty = true;
		}

		/// <summary>
		/// Find Direct
		/// </summary>
		/// <param name="searchParameters"></param>
		public List<Point> FindPath(Point Start, Point End )
		{
			this.startNode = this.nodes[Start.X, Start.Y];
			this.startNode.State = NodeState.Open;
			this.startNode.IsWalkable = true;

			this.endNode   = this.nodes[End.X, End.Y];
			this.endNode.IsWalkable = true;

			return FindPath ();
		}

        /// <summary>
        /// Attempts to find a path from the start location to the end location based on the supplied SearchParameters
        /// </summary>
        /// <returns>A List of Points representing the path. If no path was found, the returned list is empty.</returns>
        public List<Point> FindPath()
        {
            // The start node is the first entry in the 'open' list
            List<Point> path = new List<Point>();
			bool success = FastSearch( startNode ); // use fast search first


			if (success == false) {
				success = AStarSearch (startNode);		// use a star when fast is fail
			}

			//bool success = Search(startNode);
            if (success)
            {
                // If a path was found, follow the parents from the end node to build a list of locations
                Node node = this.endNode;
                while (node.ParentNode != null)
                {
                    path.Add(node.Location);
                    node = node.ParentNode;
                }

                // Reverse the list so it's in the correct order when returned
                path.Reverse();
            }

            return path;
        }

        /// <summary>
        /// Builds the node grid from a simple grid of booleans indicating areas which are and aren't walkable
        /// </summary>
        /// <param name="map">A boolean representation of a grid in which true = walkable and false = not walkable</param>
        private void InitializeNodes(bool[,] map)
        {
            this.width = map.GetLength(0);
            this.height = map.GetLength(1);
            this.nodes = new Node[this.width, this.height];
            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    this.nodes[x, y] = new Node(x, y, map[x, y], this.searchParameters.EndLocation);
                }
            }
        }

		public void ApplyMap( bool[,] map  )
		{
			this.width = map.GetLength(0);
			this.height = map.GetLength(1);
			if( this.nodes == null )
				this.nodes = new Node[this.width, this.height];

			for (int y = 0; y < this.height; y++)
			{
				for (int x = 0; x < this.width; x++)
				{
					Node node = this.nodes[x, y];
					if( node  == null )
					{
						node = new Node(x, y, map[x, y] );
						this.nodes[x, y] = node;
					}
					else {
						node.Reset();
						node.IsWalkable = map[x, y];
					}
				}
			}
		}

		public void ApplyMaskNodes(bool[,] mask , bool bWalkAble = false )
		{
			int tw = mask.GetLength(0);
			int th = mask.GetLength(1);
			for (int y = 0; y < th; y++)
			{
				for (int x = 0; x < tw ; x++)
				{
					if( mask[ x , y ] == false  )// false == can't move
					{
						Node node = this.nodes[x, y];
						if( node  != null )
							node.IsWalkable = bWalkAble;
					}
					//this.nodes[x, y] = new Node(x, y, map[x, y], this.searchParameters.EndLocation);
				}
			}

		}

		public void ApplyMaskPoint( List< Point> pool )
		{
			if (pool == null)
				return;
			int w = nodes.GetLength(0);
			int h = nodes.GetLength(1);
			foreach(Point p in  pool )
			{
				if( p.X < 0 || p.X >= w )
					continue;
				if( p.Y < 0 || p.Y >= h )
					continue;

				Node n = this.nodes[ p.X, p.Y ]; 
				if( n != null )
				{
					n.IsWalkable = false;
				}
			}

		}


        /// <summary>
        /// Attempts to find a path to the destination node using <paramref name="currentNode"/> as the starting location
        /// </summary>
        /// <param name="currentNode">The node from which to find a path</param>
        /// <returns>True if a path to the destination has been found, otherwise false</returns>
		/// 
		/// this is not a-star. it only a path finder need a new func
        private bool Search(Node currentNode)
        {
            // Set the current node to Closed since it cannot be traversed more than once
            currentNode.State = NodeState.Closed;
            List<Node> nextNodes = GetAdjacentWalkableNodes(currentNode);

            // Sort by F-value so that the shortest possible routes are considered first
            nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
            foreach (var nextNode in nextNodes)
            {
                // Check whether the end node has been reached
                if (nextNode.Location == this.endNode.Location)
                {
                    return true;
                }
                else
                {
                    // If not, check the next set of nodes
                    if (Search(nextNode)) // Note: Recurses back into Search(Node)
                        return true;
                }
            }

			bIsDirty = true;
            // The method returns false if this path leads to be a dead end
            return false;
        }

		private bool AStarSearch(Node currentNode)
		{
			Node parentnode;
			// open list
			bool bFind = false;
			bool bStop = false;
			int  nNewG = 0;
		//	List<Node> openlst = new List<Node>();
		//	List<Node> closelst = new List<Node>();
		//	List<Node> maxsteplst = new List<Node>();  // record  the max step node
			openlst.Clear();

			openlst.Add (currentNode); // push
			while ( openlst.Count > 0 ) {
				parentnode = openlst[ 0 ] ; // GET FIRST NODE for short len
				if (parentnode.Location == this.endNode.Location){
					bFind = true;
					break; // find
				}

				//openlst.RemoveAt[ openlst.Count-1 ]; // pop
				openlst.Remove( parentnode );
				parentnode.State = NodeState.Closed; // close this 
		//		closelst.Add( parentnode );  // add x to closedset      //将x节点插入已经被估算的节点

				// if need check max step
				if( nMaxStep > 0 )
				{
					if( parentnode.G >= nMaxStep ){
		//				maxsteplst.Add( parentnode );
						continue;
					}
				}

				// find all next node L + G
				List<Node> nextNodes = GetAdjacentWalkableNodes(parentnode); // close node won't return
				foreach (var nextNode in nextNodes)
				{
					//if( closelst.IndexOf(nextNode)>=0 )  		//if y in closedset           //若y已被估值，跳过
					//	continue;
					openlst.Add( nextNode );
					//nextNode.State = NodeState.Closed; // close this 
				}
			}
			bIsDirty = true;
			return bFind;
		}


		private bool FastSearch(Node currentNode)
		{
			//bool bFind = false;
			int nDiffX = (endNode.Location.X - currentNode.Location.X);
			int nDiffY = (endNode.Location.Y - currentNode.Location.Y);
			int nAbsX = Math.Abs ( nDiffX );
			int nAbsY = Math.Abs ( nDiffY );
			Node parentNode = null;


			int nDeltX = nDiffX > 0 ? 1 : -1;
			int nDeltY = nDiffY > 0 ? 1 : -1;
			// try x first
			int x = currentNode.Location.X;
			int y = currentNode.Location.Y;
			parentNode = currentNode;
			for (int i=0 ; i< nAbsX; i++ ,x=x+nDeltX) {
				Node node = this.nodes[x, y];				
				if( node.IsWalkable == false ) {
					break;
				}
				else{
					Node node2 = new Node( x , y , true );	
					node2.ParentNode =  parentNode;
					parentNode = node2;
					
				}

				// check is target node
				if( (parentNode.Location.X==endNode.Location.X) && (parentNode.Location.Y==endNode.Location.Y) )
				{
					endNode = parentNode; // set end to new node for trace parent outside
					return true;
				}
			}

			if (x == endNode.Location.X) {
				for (int j=0; j<= nAbsY; j++ , y+=nDeltY) {
					Node node = this.nodes[x, y];				
					if( node.IsWalkable == false ) {
						break;
					}
					else{
						Node node2 = new Node( x , y , true );	
						node2.ParentNode =  parentNode;
						parentNode = node2;
						
					}
					
					// check is target node
					if( (parentNode.Location.X==endNode.Location.X) && (parentNode.Location.Y==endNode.Location.Y) )
					{
						endNode = parentNode; // set end to new node for trace parent outside
						return true;
					}
				}
			}

			// test y first
			x = currentNode.Location.X;
			y = currentNode.Location.Y;
			parentNode = currentNode;
			for (int j=0; j< nAbsY; j++ , y+=nDeltY) {
				Node node = this.nodes[x, y];				
				if( node.IsWalkable == false ) {
					break;
				}
				else{
					Node node2 = new Node( x , y , true );	
					node2.ParentNode =  parentNode;
					parentNode = node2;
					
				}
				
				// check is target node
				if( (parentNode.Location.X==endNode.Location.X) && (parentNode.Location.Y==endNode.Location.Y) )
				{
					endNode = parentNode; // set end to new node for trace parent outside
					return true;
				}
			}
			if (y == endNode.Location.Y) {
				for (int i=0 ; i<= nAbsX; i++ ,x=x+nDeltX) {
					Node node = this.nodes[x, y];				
					if( node.IsWalkable == false ) {
						break;
					}
					else{
						Node node2 = new Node( x , y , true );	
						node2.ParentNode =  parentNode;
						parentNode = node2;
						
					}
					
					// check is target node
					if( (parentNode.Location.X==endNode.Location.X) && (parentNode.Location.Y==endNode.Location.Y) )
					{
						endNode = parentNode; // set end to new node for trace parent outside
						return true;
					}
				}
			}


			return false;
		}

		/// <summary>
		/// Find Direct
		/// </summary>
		/// <param name="searchParameters"></param>
		public List<Point> MoveAble(Point Start , int nDist )
		{
			this.startNode = this.nodes[Start.X, Start.Y];
			this.startNode.State = NodeState.Open;
			this.startNode.IsWalkable = true;
			

			Node parentnode;
			// open list
			bool bFind = false;
			bool bStop = false;
			int  nNewG = 0;
			openlst.Clear();
			movelst.Clear();

			//	List<Node> closelst = new List<Node>();
			//	List<Node> maxsteplst = new List<Node>();  // record  the max step node
			Node currentNode = startNode;
			openlst.Add (currentNode); // push
			while ( openlst.Count > 0 ) {
				parentnode = openlst[ 0 ] ; // GET FIRST NODE for short len
				
				//openlst.RemoveAt[ openlst.Count-1 ]; // pop
				openlst.Remove( parentnode );
				parentnode.State = NodeState.Closed; // close this 
				//		closelst.Add( parentnode );  // add x to closedset      //将x节点插入已经被估算的节点
				
				// if need check max step

				if( parentnode.G >= nDist ){
						//				maxsteplst.Add( parentnode );
					continue;
				}

				
				// find all next node L + G
				List<Node> nextNodes = GetAdjacentWalkableNodes(parentnode); // close node won't return
				foreach (Node nextNode in nextNodes)
				{
					//if( closelst.IndexOf(nextNode)>=0 )  		//if y in closedset           //若y已被估值，跳过
					//	continue;
//					if( nextNode.State == NodeState.Closed ){
//						continue;
//					}

					openlst.Add( nextNode );
					//nextNode.State = NodeState.Closed; // close this 

					// add to next moveable list
					if( nextNode.G <= nDist ){ // add if it can move
						movelst.Add(  new Point( nextNode.Location.X , nextNode.Location.Y )  );
					}
				}
			}
			bIsDirty = true;
			return movelst;
		}

        /// <summary>
        /// Returns any nodes that are adjacent to <paramref name="fromNode"/> and may be considered to form the next step in the path
        /// </summary>
        /// <param name="fromNode">The node from which to return the next possible nodes in the path</param>
        /// <returns>A list of next possible nodes in the path</returns>
        private List<Node> GetAdjacentWalkableNodes(Node fromNode)
        {
            List<Node> walkableNodes = new List<Node>();
            IEnumerable<Point> nextLocations = GetAdjacentLocations(fromNode.Location);

            foreach (var location in nextLocations)
            {
                int x = location.X;
                int y = location.Y;

                // Stay within the grid's boundaries
                if (x < 0 || x >= this.width || y < 0 || y >= this.height)
                    continue;

                Node node = this.nodes[x, y];
                // Ignore non-walkable nodes
                if (!node.IsWalkable)
                    continue;

                // Ignore already-closed nodes
                if (node.State == NodeState.Closed)
                    continue;

                // Already-open nodes are only added to the list if their G-value is lower going via this route.
                if (node.State == NodeState.Open)
                {
                    float traversalCost = Node.GetTraversalCost(node.Location, node.ParentNode.Location);
                    float gTemp = fromNode.G + traversalCost;
                    if (gTemp < node.G) // change parent if better
                    {
                        node.ParentNode = fromNode;
                        walkableNodes.Add(node);
                    }
                }
                else
                {
                    // If it's untested, set the parent and flag it as 'Open' for consideration
                    node.ParentNode = fromNode;
                    node.State = NodeState.Open;
                    walkableNodes.Add(node);
                }
            }

            return walkableNodes;
        }

        /// <summary>
        /// Returns the eight locations immediately adjacent (orthogonally and diagonally) to <paramref name="fromLocation"/>
        /// </summary>
        /// <param name="fromLocation">The location from which to return all adjacent points</param>
        /// <returns>The locations as an IEnumerable of Points</returns>
        private static IEnumerable<Point> GetAdjacentLocations(Point fromLocation)
        {
            return new Point[]
            {
				// SRW only 4 way to go
             //   new Point(fromLocation.X-1, fromLocation.Y-1),
                new Point(fromLocation.X-1, fromLocation.Y  ),
             //   new Point(fromLocation.X-1, fromLocation.Y+1),
                new Point(fromLocation.X,   fromLocation.Y+1),
             //   new Point(fromLocation.X+1, fromLocation.Y+1),
                new Point(fromLocation.X+1, fromLocation.Y  ),
             //   new Point(fromLocation.X+1, fromLocation.Y-1),
                new Point(fromLocation.X,   fromLocation.Y-1)
            };
        }
    }
}
