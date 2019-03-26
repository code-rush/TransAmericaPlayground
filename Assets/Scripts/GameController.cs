using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GUIText playersTurnText, movesText, gameOverText;            //text shown to indicate players turn, how many moves left and the winner of the game
    string player;                                                      //used to show who's turn it is
    int count;

    /*
     * creates the board map
     * 
     * line - a horizontal line between the points
     * vrline - vertical line slanted on the right side -> /
     * drline - diagonal line slanted to the left side -> \
     * hbridge - horizontal bridge
     * vbridge - vertical bridge
     * dbridge - diagonal bridge
     * 
    */
    public int xSize, zSize;
    public GameObject point, line, sdline, vrline, drline, hbridge, vbridge, dbridge;
    public GameObject redCities, greenCities, yellowCities, blueCities, orangeCities;


    /*
     * players victory points (victory points are basically the cities to connect)
    */
    public GameObject p1vp, p2vp, p3vp;


    int humanPlayers;        //indicates how many human players are playing the game
    int botPlayers;          //indicates how many bot players are playing the game

    private Vector3[] vertices;
    private Vector3[] horizontalLines, verticalLines, diagonalLines;
    //	private Vector3[] players;

    /*
     * Stores the value of the users hubs which is given to them right now.
     * In order to have the user choose the color of their hub, we need to program that when 
     * we work on the UI.
     * 
     * TODO: Assign the color of choice to the user's hub
    */
    public GameObject userHubs1, userHubs2, userHubs3, userHubs4, userHubs5, userHubs6;

    private int maxHubs;            //holds the total number hubs to be placed (i.e. human + bot players)
    private int hubCount;           //indicates the number of hubs placed

    ArrayList definedPoints = new ArrayList();

    /*
     * There are 5 zones on the board, each zone indicates a number of cities.
     * zone1 - green cities
     * zone2 - blue cities
     * zone3 - red cities
     * zone4 - orange cities
     * zone5 - yellow cities
     * 
     * Each zone has 7 cities where 2 of the cities from each zone are exclusive
     * if there are <= 5 players playing the game, but included when there are 6 players playing.
    */
    ArrayList zone1 = new ArrayList();
    ArrayList zone2 = new ArrayList();
    ArrayList zone3 = new ArrayList();
    ArrayList zone4 = new ArrayList();
    ArrayList zone5 = new ArrayList();
    ArrayList zone1_full = new ArrayList();
    ArrayList zone2_full = new ArrayList();
    ArrayList zone3_full = new ArrayList();
    ArrayList zone4_full = new ArrayList();
    ArrayList zone5_full = new ArrayList();

    ArrayList allCities = new ArrayList();

    ArrayList pointCollections = new ArrayList();

    bool placingHubs;

    //	ArrayList quadrant1 = new ArrayList ();
    //	ArrayList quadrant2 = new ArrayList ();
    //	ArrayList quadrant3 = new ArrayList ();
    //	ArrayList quadrant4 = new ArrayList ();

    ArrayList playersHubCollections;

    ArrayList[] players = new ArrayList[6];
    ArrayList[] playersCollections = new ArrayList[6];

    //ArrayList botsvp = new ArrayList();
    ArrayList[] botsvps = new ArrayList[6];

    ArrayList tracks_to_place = new ArrayList(); //bots tracks to place

    Dictionary<int, string> botsDifficultyLevels = new Dictionary<int, string>();

    //private Vector3 playingBotsStartingPoint;

    public ArrayList bridgeNodes = new ArrayList();

    Graph graph;

    public GameObject TrackBlocks;
    public Material originalTrackBlockMaterial;

    private ArrayList botsLastTurnTrackBlocks;

    private ArrayList recentlyConnectedPlayers;

    GameObject lastTrackPlaced;

    Ray ray;
    RaycastHit hit;

    bool gotpaths;

    int turn;
    int layedTracks;

    private bool restart;               //is used to restart the game


    // HOLDS THE POSITION OF THE CITIES ON THE MAP
    Dictionary<Vector3, string> city_zones = new Dictionary<Vector3, string>() {
		//green
		{new Vector3(6f, 1f, 2f), "zone1_full"},
        {new Vector3(5f, 1f, 3f), "zone1"},
        {new Vector3(2f, 1f, 6f), "zone1"},
        {new Vector3(2f, 1f, 7f), "zone1"},
        {new Vector3(1f, 1f, 9f), "zone1"},
        {new Vector3(0f, 1f, 11f), "zone1"},
        {new Vector3(0f, 1f, 12f), "zone1_full"},
		//blue
		{new Vector3(3f, 1f, 11f), "zone2"},
        {new Vector3(7f, 1f, 11f), "zone2"},
        {new Vector3(10f, 1f, 11f), "zone2_full"},
        {new Vector3(10f, 1f, 10f), "zone2"},
        {new Vector3(13f, 1f, 9f), "zone2"},
        {new Vector3(15f, 1f, 10f), "zone2_full"},
        {new Vector3(15f, 1f, 7f), "zone2"},
		//red
		{new Vector3(7f, 1f, 3f), "zone3"},
        {new Vector3(10f, 1f, 1f), "zone3"},
        {new Vector3(13f, 1f, 2f), "zone3"},
        {new Vector3(14f, 1f, 0f), "zone3_full"},
        {new Vector3(16f, 1f, 0f), "zone3"},
        {new Vector3(15f, 1f, 3f), "zone3"},
        {new Vector3(17f, 1f, 2f), "zone3_full"},
		//orange
		{new Vector3(17f, 1f, 10f), "zone4_full"},
        {new Vector3(17f, 1f, 7f), "zone4"},
        {new Vector3(17f, 1f, 8f), "zone4"},
        {new Vector3(18f, 1f, 5f), "zone4_full"},
        {new Vector3(17f, 1f, 4f), "zone4"},
        {new Vector3(19f, 1f, 2f), "zone4"},
        {new Vector3(19f, 1f, 0f), "zone4"},
		//yellow
		{new Vector3(4f, 1f, 8f), "zone5"},
        {new Vector3(7f, 1f, 7f), "zone5_full"},
        {new Vector3(8f, 1f, 4f), "zone5"},
        {new Vector3(9f, 1f, 8f), "zone5"},
        {new Vector3(11f, 1f, 6f), "zone5_full"},
        {new Vector3(11f, 1f, 4f), "zone5"},
        {new Vector3(13f, 1f, 6f), "zone5"},
    };

    // HOLDS THE POSITION OF THE HORIZONTAL BRIDGES ON THE MAP
    Dictionary<Vector3, string> horizontal_bridges = new Dictionary<Vector3, string>() {
        { new Vector3 (0f, 1f, 12f), "hbridge" },
        { new Vector3 (0f, 1f, 11f), "hbridge" },
        { new Vector3 (0f, 1f, 10f), "hbridge" },
        { new Vector3 (1f, 1f, 9f), "hbridge" },
        { new Vector3 (1f, 1f, 8f), "hbridge" },
        { new Vector3 (2f, 1f, 7f), "hbridge" },
        { new Vector3 (3f, 1f, 7f), "hbridge" },
        { new Vector3 (3f, 1f, 6f), "hbridge" },
        { new Vector3 (3f, 1f, 5f), "hbridge" },
        { new Vector3 (4f, 1f, 4f), "hbridge" },
        { new Vector3 (5f, 1f, 4f), "hbridge" },
        { new Vector3 (6f, 1f, 4f), "hbridge" },
        { new Vector3 (5f, 1f, 3f), "hbridge" },
        { new Vector3 (6f, 1f, 3f), "hbridge" },
        { new Vector3 (7f, 1f, 3f), "hbridge" },
        { new Vector3 (6f, 1f, 2f), "hbridge" },
        { new Vector3 (5f, 1f, 5f), "hbridge" },
        { new Vector3 (5f, 1f, 6f), "hbridge" },
        { new Vector3 (7f, 1f, 6f), "hbridge" },
        { new Vector3 (7f, 1f, 5f), "hbridge" },
        { new Vector3 (6f, 1f, 7f), "hbridge" },
        { new Vector3 (5f, 1f, 9f), "hbridge" },
        { new Vector3 (4f, 1f, 10f), "hbridge" },
        { new Vector3 (3f, 1f, 10f), "hbridge" },
        { new Vector3 (3f, 1f, 11f), "hbridge" },
        { new Vector3 (2f, 1f, 11f), "hbridge" },
        { new Vector3 (6f, 1f, 11f), "hbridge" },
        { new Vector3 (7f, 1f, 10f), "hbridge" },
        { new Vector3 (8f, 1f, 9f), "hbridge" },
        { new Vector3 (9f, 1f, 8f), "hbridge" },
        { new Vector3 (10f, 1f, 7f), "hbridge" },
        { new Vector3 (12f, 1f, 7f), "hbridge" },
        { new Vector3 (11f, 1f, 8f), "hbridge" },
        { new Vector3 (11f, 1f, 9f), "hbridge" },
        { new Vector3 (10f, 1f, 10f), "hbridge" },
        { new Vector3 (9f, 1f, 11f), "hbridge" },
        { new Vector3 (13f, 1f, 6f), "hbridge" },
        { new Vector3 (14f, 1f, 6f), "hbridge" },
        { new Vector3 (14f, 1f, 5f), "hbridge" },
        { new Vector3 (14f, 1f, 4f), "hbridge" },
        { new Vector3 (14f, 1f, 3f), "hbridge" },
        { new Vector3 (14f, 1f, 2f), "hbridge" },
        { new Vector3 (15f, 1f, 1f), "hbridge" },
        { new Vector3 (15f, 1f, 0f), "hbridge" },
        { new Vector3 (16f, 1f, 3f), "hbridge" },
        { new Vector3 (16f, 1f, 4f), "hbridge" },
        { new Vector3 (16f, 1f, 5f), "hbridge" },
        { new Vector3 (16f, 1f, 6f), "hbridge" },
        { new Vector3 (16f, 1f, 7f), "hbridge" },
        { new Vector3 (16f, 1f, 8f), "hbridge" }
    };

    // HOLDS THE POSITION OF THE VERTICAL BRIDGES ON THE MAP
    Dictionary<Vector3, string> vertical_bridges = new Dictionary<Vector3, string>() {
        { new Vector3 (2f, 1f, 11f), "vbridge" },
        { new Vector3 (4f, 1f, 11f), "vbridge" },
        { new Vector3 (5f, 1f, 11f), "vbridge" },
        { new Vector3 (6f, 1f, 11f), "vbridge" },
        { new Vector3 (3f, 1f, 10f), "vbridge" },
        { new Vector3 (4f, 1f, 10f), "vbridge" },
        { new Vector3 (7f, 1f, 10f), "vbridge" },
        { new Vector3 (1f, 1f, 9f), "vbridge" },
        { new Vector3 (5f, 1f, 9f), "vbridge" },
        { new Vector3 (8f, 1f, 9f), "vbridge" },
        { new Vector3 (9f, 1f, 8f), "vbridge" },
        { new Vector3 (2f, 1f, 7f), "vbridge" },
        { new Vector3 (3f, 1f, 7f), "vbridge" },
        { new Vector3 (5f, 1f, 7f), "vbridge" },
        { new Vector3 (10f, 1f, 7f), "vbridge" },
        { new Vector3 (12f, 1f, 7f), "vbridge" },
        { new Vector3 (6f, 1f, 6f), "vbridge" },
        { new Vector3 (7f, 1f, 6f), "vbridge" },
        { new Vector3 (11f, 1f, 6f), "vbridge" },
        { new Vector3 (12f, 1f, 6f), "vbridge" },
        { new Vector3 (13f, 1f, 6f), "vbridge" },
        { new Vector3 (15f, 1f, 6f), "vbridge" },
        { new Vector3 (3f, 1f, 5f), "vbridge" },
        { new Vector3 (14f, 1f, 5f), "vbridge" },
        { new Vector3 (6f, 1f, 3f), "vbridge" },
        { new Vector3 (7f, 1f, 3f), "vbridge" },
        { new Vector3 (8f, 1f, 2f), "vbridge" },
        { new Vector3 (15f, 1f, 1f), "vbridge" },
        { new Vector3 (10f, 1f, 10f), "vbridge" },
        { new Vector3 (11f, 1f, 9f), "vbridge" }
    };

    // INDICATES THE POSITION OF THE DIAGONAL BRIDGES ON THE MAP
    Dictionary<Vector3, string> diagonal_bridges = new Dictionary<Vector3, string>() {
        {new Vector3(1f, 1f, 11f), "dbridge"},
        {new Vector3(5f, 1f, 11f), "dbridge"},
        {new Vector3(6f, 1f, 11f), "dbridge"},
        {new Vector3(1f, 1f, 10f), "dbridge"},
        {new Vector3(2f, 1f, 8f), "dbridge"},
        {new Vector3(12f, 1f, 8f), "dbridge"},
        {new Vector3(17f, 1f, 8f), "dbridge"},
        {new Vector3(17f, 1f, 7f), "dbridge"},
        {new Vector3(3f, 1f, 6f), "dbridge"},
        {new Vector3(4f, 1f, 6f), "dbridge"},
        {new Vector3(6f, 1f, 6f), "dbridge"},
        {new Vector3(12f, 1f, 6f), "dbridge"},
        {new Vector3(13f, 1f, 6f), "dbridge"},
        {new Vector3(15f, 1f, 6f), "dbridge"},
        {new Vector3(16f, 1f, 6f), "dbridge"},
        {new Vector3(17f, 1f, 6f), "dbridge"},
        {new Vector3(6f, 1f, 5f), "dbridge"},
        {new Vector3(8f, 1f, 5f), "dbridge"},
        {new Vector3(15f, 1f, 5f), "dbridge"},
        {new Vector3(17f, 1f, 5f), "dbridge"},
        {new Vector3(6f, 1f, 4f), "dbridge"},
        {new Vector3(15f, 1f, 4f), "dbridge"},
        {new Vector3(17f, 1f, 4f), "dbridge"},
        {new Vector3(15f, 1f, 3f), "dbridge"},
        {new Vector3(17f, 1f, 3f), "dbridge"},
        {new Vector3(7f, 1f, 2f), "dbridge"},
        {new Vector3(9f, 1f, 2f), "dbridge"},
        {new Vector3(15f, 1f, 2f), "dbridge"},
        {new Vector3(16f, 1f, 0f), "dbridge"},
    };


    // list of positions of the vertices to remove from the board to create the perfect map
    List<Vector3> delete_vertices = new List<Vector3> {
        new Vector3(0f, 1f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(3f, 1f, 0f),
        new Vector3(4f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(6f, 1f, 0f),
        new Vector3(7f, 1f, 0f),
        new Vector3(8f, 1f, 0f),
        new Vector3(9f, 1f, 0f),
        new Vector3(10f, 1f, 0f),
        new Vector3(0f, 1f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(2f, 1f, 1f),
        new Vector3(3f, 1f, 1f),
        new Vector3(4f, 1f, 1f),
        new Vector3(5f, 1f, 1f),
        new Vector3(6f, 1f, 1f),
        new Vector3(7f, 1f, 1f),
        new Vector3(0f, 1f, 2f),
        new Vector3(1f, 1f, 2f),
        new Vector3(2f, 1f, 2f),
        new Vector3(3f, 1f, 2f),
        new Vector3(4f, 1f, 2f),
        new Vector3(5f, 1f, 2f),
        new Vector3(0f, 1f, 3f),
        new Vector3(1f, 1f, 3f),
        new Vector3(2f, 1f, 3f),
        new Vector3(3f, 1f, 3f),
        new Vector3(4f, 1f, 3f),
        new Vector3(0f, 1f, 4f),
        new Vector3(1f, 1f, 4f),
        new Vector3(2f, 1f, 4f),
        new Vector3(3f, 1f, 4f),
        new Vector3(0f, 1f, 5f),
        new Vector3(1f, 1f, 5f),
        new Vector3(2f, 1f, 5f),
        new Vector3(19f, 1f, 5f),
        new Vector3(0f, 1f, 6f),
        new Vector3(1f, 1f, 6f),
        new Vector3(18f, 1f, 6f),
        new Vector3(19f, 1f, 6f),
        new Vector3(0f, 1f, 7f),
        new Vector3(1f, 1f, 7f),
        new Vector3(18f, 1f, 7f),
        new Vector3(19f, 1f, 7f),
        new Vector3(0f, 1f, 8f),
        new Vector3(18f, 1f, 8f),
        new Vector3(19f, 1f, 8f),
        new Vector3(0f, 1f, 9f),
        new Vector3(18f, 1f, 9f),
        new Vector3(19f, 1f, 9f),
        new Vector3(13f, 1f, 10f),
        new Vector3(14f, 1f, 10f),
        new Vector3(18f, 1f, 10f),
        new Vector3(19f, 1f, 10f),
        new Vector3(12f, 1f, 11f),
        new Vector3(13f, 1f, 11f),
        new Vector3(14f, 1f, 11f),
        new Vector3(17f, 1f, 11f),
        new Vector3(18f, 1f, 11f),
        new Vector3(19f, 1f, 11f),
        new Vector3(10f, 1f, 12f),
        new Vector3(11f, 1f, 12f),
        new Vector3(12f, 1f, 12f),
        new Vector3(13f, 1f, 12f),
        new Vector3(14f, 1f, 12f),
        new Vector3(15f, 1f, 12f),
        new Vector3(16f, 1f, 12f),
        new Vector3(17f, 1f, 12f),
        new Vector3(18f, 1f, 12f),
        new Vector3(19f, 1f, 12f)
    };

    // list of positions of the horizontal lines to remove from the board to create the perfect map
    List<Vector3> delete_horizontalLines = new List<Vector3> {
        new Vector3(0f, 1f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(3f, 1f, 0f),
        new Vector3(4f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(6f, 1f, 0f),
        new Vector3(7f, 1f, 0f),
        new Vector3(8f, 1f, 0f),
        new Vector3(9f, 1f, 0f),
        new Vector3(10f, 1f, 0f),
        new Vector3(0f, 1f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(2f, 1f, 1f),
        new Vector3(3f, 1f, 1f),
        new Vector3(4f, 1f, 1f),
        new Vector3(5f, 1f, 1f),
        new Vector3(6f, 1f, 1f),
        new Vector3(7f, 1f, 1f),
        new Vector3(0f, 1f, 2f),
        new Vector3(1f, 1f, 2f),
        new Vector3(2f, 1f, 2f),
        new Vector3(3f, 1f, 2f),
        new Vector3(4f, 1f, 2f),
        new Vector3(5f, 1f, 2f),
        new Vector3(0f, 1f, 3f),
        new Vector3(1f, 1f, 3f),
        new Vector3(2f, 1f, 3f),
        new Vector3(3f, 1f, 3f),
        new Vector3(4f, 1f, 3f),
        new Vector3(0f, 1f, 4f),
        new Vector3(1f, 1f, 4f),
        new Vector3(2f, 1f, 4f),
        new Vector3(3f, 1f, 4f),
        new Vector3(0f, 1f, 5f),
        new Vector3(1f, 1f, 5f),
        new Vector3(2f, 1f, 5f),
        new Vector3(18f, 1f, 5f),
        new Vector3(0f, 1f, 6f),
        new Vector3(1f, 1f, 6f),
        new Vector3(17f, 1f, 6f),
        new Vector3(18f, 1f, 6f),
        new Vector3(0f, 1f, 7f),
        new Vector3(1f, 1f, 7f),
        new Vector3(17f, 1f, 7f),
        new Vector3(18f, 1f, 7f),
        new Vector3(0f, 1f, 8f),
        new Vector3(17f, 1f, 8f),
        new Vector3(18f, 1f, 8f),
        new Vector3(0f, 1f, 9f),
        new Vector3(17f, 1f, 9f),
        new Vector3(18f, 1f, 9f),
        new Vector3(12f, 1f, 10f),
        new Vector3(13f, 1f, 10f),
        new Vector3(14f, 1f, 10f),
        new Vector3(17f, 1f, 10f),
        new Vector3(18f, 1f, 10f),
        new Vector3(11f, 1f, 11f),
        new Vector3(12f, 1f, 11f),
        new Vector3(13f, 1f, 11f),
        new Vector3(14f, 1f, 11f),
        new Vector3(16f, 1f, 11f),
        new Vector3(17f, 1f, 11f),
        new Vector3(18f, 1f, 11f),
        new Vector3(9f, 1f, 12f),
        new Vector3(10f, 1f, 12f),
        new Vector3(11f, 1f, 12f),
        new Vector3(12f, 1f, 12f),
        new Vector3(13f, 1f, 12f),
        new Vector3(14f, 1f, 12f),
        new Vector3(15f, 1f, 12f),
        new Vector3(16f, 1f, 12f),
        new Vector3(17f, 1f, 12f),
        new Vector3(18f, 1f, 12f)
    };

    // list of positions of the vertical lines to remove from the board to create the perfect map
    List<Vector3> delete_verticalLines = new List<Vector3> {
        new Vector3(0f, 1f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(3f, 1f, 0f),
        new Vector3(4f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(6f, 1f, 0f),
        new Vector3(7f, 1f, 0f),
        new Vector3(8f, 1f, 0f),
        new Vector3(9f, 1f, 0f),
        new Vector3(10f, 1f, 0f),
        new Vector3(0f, 1f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(2f, 1f, 1f),
        new Vector3(3f, 1f, 1f),
        new Vector3(4f, 1f, 1f),
        new Vector3(5f, 1f, 1f),
        new Vector3(6f, 1f, 1f),
        new Vector3(7f, 1f, 1f),
        new Vector3(0f, 1f, 2f),
        new Vector3(1f, 1f, 2f),
        new Vector3(2f, 1f, 2f),
        new Vector3(3f, 1f, 2f),
        new Vector3(4f, 1f, 2f),
        new Vector3(5f, 1f, 2f),
        new Vector3(0f, 1f, 3f),
        new Vector3(1f, 1f, 3f),
        new Vector3(2f, 1f, 3f),
        new Vector3(3f, 1f, 3f),
        new Vector3(4f, 1f, 3f),
        new Vector3(0f, 1f, 4f),
        new Vector3(1f, 1f, 4f),
        new Vector3(2f, 1f, 4f),
        new Vector3(3f, 1f, 4f),
        new Vector3(19f, 1f, 4f),
        new Vector3(0f, 1f, 5f),
        new Vector3(1f, 1f, 5f),
        new Vector3(2f, 1f, 5f),
        new Vector3(18f, 1f, 5f),
        new Vector3(19f, 1f, 5f),
        new Vector3(0f, 1f, 6f),
        new Vector3(1f, 1f, 6f),
        new Vector3(18f, 1f, 6f),
        new Vector3(19f, 1f, 6f),
        new Vector3(0f, 1f, 7f),
        new Vector3(1f, 1f, 7f),
        new Vector3(18f, 1f, 7f),
        new Vector3(19f, 1f, 7f),
        new Vector3(0f, 1f, 8f),
        new Vector3(18f, 1f, 8f),
        new Vector3(19f, 1f, 8f),
        new Vector3(0f, 1f, 9f),
        new Vector3(13f, 1f, 9f),
        new Vector3(14f, 1f, 9f),
        new Vector3(18f, 1f, 9f),
        new Vector3(19f, 1f, 9f),
        new Vector3(12f, 1f, 10f),
        new Vector3(13f, 1f, 10f),
        new Vector3(14f, 1f, 10f),
        new Vector3(17f, 1f, 10f),
        new Vector3(18f, 1f, 10f),
        new Vector3(19f, 1f, 10f),
        new Vector3(10f, 1f, 11f),
        new Vector3(11f, 1f, 11f),
        new Vector3(12f, 1f, 11f),
        new Vector3(13f, 1f, 11f),
        new Vector3(14f, 1f, 11f),
        new Vector3(15f, 1f, 11f),
        new Vector3(16f, 1f, 11f),
        new Vector3(17f, 1f, 11f),
        new Vector3(18f, 1f, 11f),
        new Vector3(19f, 1f, 11f)
    };

    // list of positions of the diagonal lines to remove from the board to create the perfect map
    List<Vector3> delete_diagonalLines = new List<Vector3> {
        new Vector3(1f, 1f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(3f, 1f, 0f),
        new Vector3(4f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(6f, 1f, 0f),
        new Vector3(7f, 1f, 0f),
        new Vector3(8f, 1f, 0f),
        new Vector3(9f, 1f, 0f),
        new Vector3(10f, 1f, 0f),
        new Vector3(1f, 1f, 1f),
        new Vector3(2f, 1f, 1f),
        new Vector3(3f, 1f, 1f),
        new Vector3(4f, 1f, 1f),
        new Vector3(5f, 1f, 1f),
        new Vector3(6f, 1f, 1f),
        new Vector3(7f, 1f, 1f),
        new Vector3(0f, 1f, 2f),
        new Vector3(1f, 1f, 2f),
        new Vector3(2f, 1f, 2f),
        new Vector3(3f, 1f, 2f),
        new Vector3(4f, 1f, 2f),
        new Vector3(5f, 1f, 2f),
        new Vector3(0f, 1f, 3f),
        new Vector3(1f, 1f, 3f),
        new Vector3(2f, 1f, 3f),
        new Vector3(3f, 1f, 3f),
        new Vector3(4f, 1f, 3f),
        new Vector3(0f, 1f, 4f),
        new Vector3(1f, 1f, 4f),
        new Vector3(2f, 1f, 4f),
        new Vector3(3f, 1f, 4f),
        new Vector3(0f, 1f, 5f),
        new Vector3(1f, 1f, 5f),
        new Vector3(2f, 1f, 5f),
        new Vector3(19f, 1f, 5f),
        new Vector3(0f, 1f, 6f),
        new Vector3(1f, 1f, 6f),
        new Vector3(2f, 1f, 6f),
        new Vector3(18f, 1f, 6f),
        new Vector3(19f, 1f, 6f),
        new Vector3(0f, 1f, 7f),
        new Vector3(1f, 1f, 7f),
        new Vector3(18f, 1f, 7f),
        new Vector3(19f, 1f, 7f),
        new Vector3(0f, 1f, 8f),
        new Vector3(1f, 1f, 8f),
        new Vector3(18f, 1f, 8f),
        new Vector3(19f, 1f, 8f),
        new Vector3(0f, 1f, 9f),
        new Vector3(14f, 1f, 9f),
        new Vector3(15f, 1f, 9f),
        new Vector3(18f, 1f, 9f),
        new Vector3(19f, 1f, 9f),
        new Vector3(13f, 1f, 10f),
        new Vector3(14f, 1f, 10f),
        new Vector3(15f, 1f, 10f),
        new Vector3(18f, 1f, 10f),
        new Vector3(19f, 1f, 10f),
        new Vector3(11f, 1f, 11f),
        new Vector3(12f, 1f, 11f),
        new Vector3(13f, 1f, 11f),
        new Vector3(14f, 1f, 11f),
        new Vector3(15f, 1f, 11f),
        new Vector3(16f, 1f, 11f),
        new Vector3(17f, 1f, 11f),
        new Vector3(18f, 1f, 11f),
        new Vector3(19f, 1f, 11f)
    };


    void Awake() {
        int totalPlayers = MainMenu.Playertype.Count;
        foreach(string type in MainMenu.Playertype)
        {
            if (type == "human")
            {
                humanPlayers += 1;
            }
        }

        botPlayers = totalPlayers - humanPlayers;
        botsDifficultyLevels = MainMenu.BotConfig;

        allCities = new ArrayList();

        //botsDifficultyLevels[1] = "medium";
        //botsDifficultyLevels[2] = "easy";
        maxHubs = humanPlayers + botPlayers;        //sets the value of the var maxHubs
        GenerateRhombicalGrid2();                  //generates a map on the board
        hubCount = 0;
        turn = 0;

        // initializing neccessary arraylists for all players
        for (int i = 0; i < maxHubs; i++)
        {
            players[i] = new ArrayList();
            playersCollections[i] = new ArrayList();
        }

        if (botPlayers > 0)
        {
            botsLastTurnTrackBlocks = new ArrayList();
            playersHubCollections = new ArrayList();    //array to store human player's hubs locations to exclude from bot's hub placement options
            //playingBotsStartingPoint = new Vector3();   //if any bots are playing, instantiate this var to store playing bots starting point
        }
    }


    void Start() {
        restart = false;
        gotpaths = false;
        placingHubs = true;
        gameOverText.text = "";
        UpdateCount();
        UpdateTurn();

        layedTracks = 0;


        // Assigning victory points to each player randomly (one city from each zone)
        for (int i = 0; i < maxHubs; i++)
        {
            if (maxHubs >= 5)
            {
                int one = Random.Range(0, zone1_full.Count - 1);
                int two = Random.Range(0, zone2_full.Count - 1);
                int three = Random.Range(0, zone3_full.Count - 1);
                int four = Random.Range(0, zone4_full.Count - 1);
                int five = Random.Range(0, zone5_full.Count - 1);

                players[i].Add(zone1_full[one]);
                players[i].Add(zone2_full[two]);
                players[i].Add(zone3_full[three]);
                players[i].Add(zone4_full[four]);
                players[i].Add(zone5_full[five]);

                zone1_full.RemoveAt(one);
                zone2_full.RemoveAt(two);
                zone3_full.RemoveAt(three);
                zone4_full.RemoveAt(four);
                zone5_full.RemoveAt(five);
            } else 
            {
                int one = Random.Range(0, zone1.Count - 1);
                int two = Random.Range(0, zone2.Count - 1);
                int three = Random.Range(0, zone3.Count - 1);
                int four = Random.Range(0, zone4.Count - 1);
                int five = Random.Range(0, zone5.Count - 1);

                players[i].Add(zone1[one]);
                players[i].Add(zone2[two]);
                players[i].Add(zone3[three]);
                players[i].Add(zone4[four]);
                players[i].Add(zone5[five]);

                zone1.RemoveAt(one);
                zone2.RemoveAt(two);
                zone3.RemoveAt(three);
                zone4.RemoveAt(four);
                zone5.RemoveAt(five);
            }

        }

        //show cities of the first human player with a square above the city
        foreach (Vector3 x in players[0])
        {
            p1vp.GetComponent<Renderer>().sharedMaterial = userHubs1.GetComponent<Renderer>().sharedMaterial;
            GameObject p1 = Instantiate(p1vp, x + new Vector3(0f, -0.3f, 0.3f), Quaternion.identity) as GameObject;
        }

        //foreach (Vector3 x in players[1])
        //{
        //    GameObject p2 = Instantiate(p2vp, x + new Vector3(0f, -0.3f, 0.3f), Quaternion.identity) as GameObject;
        //}

        //printing all players cities to be connected
        //for (int i = 0; i < maxHubs; i++)
        //{
        //    Debug.Log("players size: " + players[i].Count);
        //    foreach (Vector3 x in players[i])
        //    {
        //        Debug.Log("players points: " + x.ToString("F4"));
        //    }
        //}

        ////printing points at the end of bridges whose value is 2
        //Debug.Log("The below are brigde end points");
        //foreach (Vector3 x in bridgeNodes)
        //{
        //    Debug.Log(x);
        //}

        //foreach (Vector3 x in bridgeNodeList())
        //{
        //    Debug.Log("Bridge Node List" + x);
        //}

        //creating graph by adding edges and nodes to the graph
        graph = new Graph();
        graph.addArrayToGraph(bridgeNodes);
        foreach (Vector3 x in definedPoints)
        {
            Vector3 left = x + new Vector3(-1f, 0f, 0f);
            Vector3 right = x + new Vector3(1f, 0f, 0f);
            Vector3 topleft = x + new Vector3(-0.5f, 0f, 1f);
            Vector3 topright = x + new Vector3(0.5f, 0f, 1f);
            Vector3 bottomleft = x + new Vector3(-0.5f, 0f, -1f);
            Vector3 bottomright = x + new Vector3(0.5f, 0f, -1f);
            if (definedPoints.Contains(left))
            {
                graph.insert_edge(x, left);
            }
            if (definedPoints.Contains(right))
            {
                graph.insert_edge(x, right);
            }
            if (definedPoints.Contains(topleft))
            {
                graph.insert_edge(x, topleft);
            }
            if (definedPoints.Contains(topright))
            {
                graph.insert_edge(x, topright);
            }
            if (definedPoints.Contains(bottomleft))
            {
                graph.insert_edge(x, bottomleft);
            }
            if (definedPoints.Contains(bottomright))
            {
                graph.insert_edge(x, bottomright);
            }
        }

        // copy all bots victory points from players array to botsvp array

        for (int i = 0; i < botPlayers; i++)
        {
            botsvps[i] = new ArrayList();
            foreach (Vector3 point in players[humanPlayers + i]) {
                botsvps[i].Add(point);
            }
        }
    }

    bool isPlayerBot(int turn)
    {
        if (MainMenu.Playertype[turn] == "bot")
        {
            return true;
        }
        //if (turn == 1 || turn == 2)
        //{
        //    return true;
        //}

        return false;
    }

    public void restartGame()
    {
        setTracksColorToDefault();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void goToHomeScreen()
    {
        SceneManager.LoadScene(0);
        //GameObject obj = GameObject.FindGameObjectWithTag("music");
        //Destroy(obj);
    }

    // allows a human player to change turn only after placing 1 track
    public void forceChangeTurn()
    {
        if (isPlayerBot(turn) == false)
        {
            if (layedTracks > 0)
            {
                layedTracks = 0;
                UpdateCount();
                if (turn + 1 < maxHubs)
                {
                    turn += 1;
                    UpdateTurn();
                }
                else
                {
                    turn = 0;
                    UpdateTurn();
                }
            }
        }
    }

    // allows human player to undo 1 track placement
    public void undo()
    {
        if (isPlayerBot(turn) == false)
        {
            if (layedTracks == 1)
            {
                Destroy(lastTrackPlaced);
                playersCollections[turn].RemoveAt(playersCollections[turn].Count - 1);
                layedTracks = 0;
                UpdateCount();
            }
        }
    }


    void Update() 
    {
        //Restarts the game if "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            restartGame();
        }

        //changes the players turn if "C" key is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            forceChangeTurn();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            undo();
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        switch (turn)
        {
            case 0:
                if (placingHubs == true)
                {
                    disableTrackInstantiation();

                    if (isPlayerBot(turn))
                    {
                        botwillplaceahub();
                    }
                    else
                    {
                        if (placeHub(turn))
                        {
                            changeTurn();
                        }
                    }
                }
                else
                {
                    try
                    {
                        addPlayersCollectionsIfConnected(turn);
                    }
                    catch
                    {
                        return;
                    }

                    if (checkIfAnyWinner())
                    {
                        disableTrackInstantiation();
                        gameOverText.text = "Player " + winner() + " wins!";
                        highlightAllPlayersCities();
                    }
                    else
                    {
                        if (isPlayerBot(turn))
                        {
                            if (botsDifficultyLevels[turn] == "easy")
                            {
                                easyBotWillPlaceTracks();
                            } else {
                                mediumBotWillPlaceTracks();
                            }
                        }
                        else
                        {
                            humanPlaysTracks(turn);
                        }
                    }
                }

                break;

            case 1:
                if (placingHubs == true)
                {
                    disableTrackInstantiation();

                    if (isPlayerBot(turn))
                    {
                        botwillplaceahub();
                    }
                    else
                    {
                        if (placeHub(turn))
                        {
                            changeTurn();
                        }
                    }
                }
                else
                {
                    try
                    {
                        addPlayersCollectionsIfConnected(turn);
                    }
                    catch
                    {
                        return;
                    }

                    if (checkIfAnyWinner())
                    {
                        disableTrackInstantiation();
                        gameOverText.text = "Player " + winner() + " wins!";
                        highlightAllPlayersCities();
                    }
                    else
                    {
                        if (isPlayerBot(turn))
                        {
                            if (botsDifficultyLevels[turn] == "easy")
                            {
                                easyBotWillPlaceTracks();
                            }
                            else
                            {
                                mediumBotWillPlaceTracks();
                            }
                        }
                        else
                        {
                            humanPlaysTracks(turn);
                        }
                    }
                }
                break;

            case 2:
                if (placingHubs == true)
                {
                    disableTrackInstantiation();

                    if (isPlayerBot(turn))
                    {
                        botwillplaceahub();
                    }
                    else
                    {
                        if (placeHub(turn))
                        {
                            changeTurn();
                        }
                    }
                }
                else
                {
                    try
                    {
                        addPlayersCollectionsIfConnected(turn);
                    }
                    catch
                    {
                        return;
                    }

                    if (checkIfAnyWinner())
                    {
                        disableTrackInstantiation();
                        gameOverText.text = "Player " + winner() + " wins!";
                        highlightAllPlayersCities();
                    }
                    else
                    {
                        if (isPlayerBot(turn))
                        {
                            if (botsDifficultyLevels[turn] == "easy")
                            {
                                easyBotWillPlaceTracks();
                            }
                            else
                            {
                                mediumBotWillPlaceTracks();
                            }
                        }
                        else
                        {
                            humanPlaysTracks(turn);
                        }
                    }
                }

                break;

            case 3:
                if (placingHubs == true)
                {
                    disableTrackInstantiation();

                    if (isPlayerBot(turn))
                    {
                        botwillplaceahub();
                    }
                    else
                    {
                        if (placeHub(turn))
                        {
                            changeTurn();
                        }
                    }
                }
                else
                {
                    try
                    {
                        addPlayersCollectionsIfConnected(turn);
                    }
                    catch
                    {
                        return;
                    }

                    if (checkIfAnyWinner())
                    {
                        disableTrackInstantiation();
                        gameOverText.text = "Player " + winner() + " wins!";
                        highlightAllPlayersCities();
                    }
                    else
                    {
                        if (isPlayerBot(turn))
                        {
                            if (botsDifficultyLevels[turn] == "easy")
                            {
                                easyBotWillPlaceTracks();
                            }
                            else
                            {
                                mediumBotWillPlaceTracks();
                            }
                        }
                        else
                        {
                            humanPlaysTracks(turn);
                        }
                    }
                }

                break;

            case 4:
                if (placingHubs == true)
                {
                    disableTrackInstantiation();

                    if (isPlayerBot(turn))
                    {
                        botwillplaceahub();
                    }
                    else
                    {
                        if (placeHub(turn))
                        {
                            changeTurn();
                        }
                    }
                }
                else
                {
                    try
                    {
                        addPlayersCollectionsIfConnected(turn);
                    }
                    catch
                    {
                        return;
                    }

                    if (checkIfAnyWinner())
                    {
                        disableTrackInstantiation();
                        gameOverText.text = "Player " + winner() + " wins!";
                        highlightAllPlayersCities();
                    }
                    else
                    {
                        if (isPlayerBot(turn))
                        {
                            if (botsDifficultyLevels[turn] == "easy")
                            {
                                easyBotWillPlaceTracks();
                            }
                            else
                            {
                                mediumBotWillPlaceTracks();
                            }
                        }
                        else
                        {
                            humanPlaysTracks(turn);
                        }
                    }
                }

                break;

            case 5:
                if (placingHubs == true)
                {
                    disableTrackInstantiation();

                    if (isPlayerBot(turn))
                    {
                        botwillplaceahub();
                    }
                    else
                    {
                        if (placeHub(turn))
                        {
                            changeTurn();
                        }
                    }
                }
                else
                {
                    try
                    {
                        addPlayersCollectionsIfConnected(turn);
                    }
                    catch
                    {
                        return;
                    }

                    if (checkIfAnyWinner())
                    {
                        disableTrackInstantiation();
                        gameOverText.text = "Player " + winner() + " wins!";
                        highlightAllPlayersCities();
                    }
                    else
                    {
                        if (isPlayerBot(turn))
                        {
                            if (botsDifficultyLevels[turn] == "easy")
                            {
                                easyBotWillPlaceTracks();
                            }
                            else
                            {
                                mediumBotWillPlaceTracks();
                            }
                        }
                        else
                        {
                            humanPlaysTracks(turn);
                        }
                    }
                }

                break;
        }
    }

    void highlightAllPlayersCities()
    {
        for (int i = 0; i < maxHubs; i++)
        {
            if (isPlayerBot(i) == false)
            {
                continue;
            }
            else
            {
                GameObject highlightCity = p1vp;
                if (i == 0)
                {
                    highlightCity.GetComponent<Renderer>().sharedMaterial = userHubs1.GetComponent<Renderer>().sharedMaterial;
                }
                else if (i == 1)
                {
                    highlightCity.GetComponent<Renderer>().sharedMaterial = userHubs2.GetComponent<Renderer>().sharedMaterial;
                }
                else if (i == 2)
                {
                    highlightCity.GetComponent<Renderer>().sharedMaterial = userHubs3.GetComponent<Renderer>().sharedMaterial;
                }
                else if (i == 3)
                {
                    highlightCity.GetComponent<Renderer>().sharedMaterial = userHubs4.GetComponent<Renderer>().sharedMaterial;
                }
                else if (i == 4)
                {
                    highlightCity.GetComponent<Renderer>().sharedMaterial = userHubs5.GetComponent<Renderer>().sharedMaterial;
                }
                else
                {
                    highlightCity.GetComponent<Renderer>().sharedMaterial = userHubs6.GetComponent<Renderer>().sharedMaterial;
                }

                foreach (Vector3 cityPoint in players[i])
                {
                    GameObject city = Instantiate(p1vp, cityPoint + new Vector3(0f, -0.3f, 0.3f), Quaternion.identity) as GameObject;
                }
            }
        }
    }


    Material getHubColor()
    {
        return hub().GetComponent<Renderer>().sharedMaterial;
    }

    void easyBotWillPlaceTracks()
    {
        Vector3 current_point = new Vector3();
        Vector3 hub_point = (Vector3)playersCollections[turn][0];
        //Debug.Log("hub_points is: " + hub_point);

        Dictionary<Vector3, int> distance_to_vpoints = new Dictionary<Vector3, int>();

        if (playersCollections[turn].Count == 1)
        {
            current_point = hub_point;
            //Debug.Log("current_point for bot " + current_point);
        }
        else
        {
            current_point = get_current_point();
            //Debug.Log("current_point for bot " + current_point);
        }

        Dictionary<Vector3, List<Vector3>> path_to_dest = shortest_path(graph, current_point);

        //foreach (Vector3 x in path_to_dest.Keys)
        //{
        //    //                  Debug.Log ("Keys are " + x);
        //    foreach (Vector3 y in path_to_dest[x])
        //    {
        //        Debug.Log("Route to " + x + " is " + y);
        //    }
        //}


        Vector3 track_position = new Vector3();
        Vector3 pt = new Vector3();
        foreach (Vector3 x in path_to_dest.Keys)
        {
            if (path_to_dest[x].Count == 1)
            {
                pt = (Vector3)path_to_dest[x][0];
                //Debug.Log("Point pt is " + pt);
                path_to_dest[x].RemoveAt(0);
            }
            else
            {
                pt = (Vector3)path_to_dest[x][(path_to_dest[x].Count) - 1];
                //Debug.Log("Point pt is " + pt);
                path_to_dest[x].RemoveAt((path_to_dest[x].Count) - 1);
            }
        }

        if (pt.z == current_point.z)
        {
            track_position = new Vector3((pt.x + current_point.x) / 2f, current_point.y, current_point.z);
        }
        else
        {
            if (pt.x < current_point.x)
            {
                track_position = new Vector3((pt.x + 0.25f), 1f, (pt.z + current_point.z) / 2f);
            }
            if (pt.x > current_point.x)
            {
                track_position = new Vector3((pt.x - 0.25f), 1f, (pt.z + current_point.z) / 2f);
            }
        }

        track_position.y += 10f;

        botWillPlaceTracks(track_position, turn);
    }

    void mediumBotWillPlaceTracks()
    {
        System.Threading.Thread.Sleep(1000);

        Debug.Log("******************");
        foreach (Vector3 tracks in tracks_to_place)
        {
            Debug.Log(tracks);
        }

        Vector3 track_position = new Vector3();

        if (tracks_to_place.Count == 0)
        {
            tracks_to_place = efficient_shortes_path();
        }
        else
        {
            track_position = (Vector3)tracks_to_place[0];
            tracks_to_place.RemoveAt(0);
        }
        //ArrayList tracks_to_place = efficient_shortes_path();

        Debug.Log("Track position would be " + track_position.ToString("F2"));

        botWillPlaceTracks(track_position, turn);
    }

    void humanPlaysTracks(int turn)
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                placetrack(turn);
            }
        }
    }


    void botWillPlaceTracks(Vector3 track_position, int turn)
    {
        if (Physics.Raycast(track_position, Vector3.down, out hit, 10))
        {
            Debug.Log("------------->");
            botsLogic(turn);
        }
    }

    // function for placing tracks
    void placetrack(int pturn)
    {
        float z = new float();
        float x = new float();

        Vector3 p1 = new Vector3();
        Vector3 p2 = new Vector3();

        if (hit.collider.gameObject.tag == "dLine")
        {
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            p1 = new Vector3(x + 0.25f, 1f, z - 0.5f);
            p2 = new Vector3(x - 0.25f, 1f, z + 0.5f);

        }
        else if (hit.collider.gameObject.tag == "dBridge" && layedTracks == 0)
        {
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            p1 = new Vector3(x + 0.25f, 1f, z - 0.5f);
            p2 = new Vector3(x - 0.25f, 1f, z + 0.5f);

        }
        else if (hit.collider.gameObject.tag == "hBridge" && layedTracks == 0)
        {
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            p1 = new Vector3(x + 0.5f, 1f, z);
            p2 = new Vector3(x - 0.5f, 1f, z);

        }
        else if (hit.collider.gameObject.tag == "vBridge" && layedTracks == 0)
        {
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            p1 = new Vector3(x - 0.25f, 1f, z - 0.5f);
            p2 = new Vector3(x + 0.25f, 1f, z + 0.5f);

        }
        else if (hit.collider.gameObject.tag == "hLine")
        {
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            p1 = new Vector3(x + 0.5f, 1f, z);
            p2 = new Vector3(x - 0.5f, 1f, z);

        }
        else if (hit.collider.gameObject.tag == "vLine")
        {
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            p1 = new Vector3(x - 0.25f, 1f, z - 0.5f);
            p2 = new Vector3(x + 0.25f, 1f, z + 0.5f);

        }

        if (playersCollections[turn].Contains(p1) && playersCollections[turn].Contains(p2))
        {

        }
        else
        {
            if (playersCollections[pturn].Contains(p1) || playersCollections[pturn].Contains(p2))
            {
                if (playersCollections[pturn].Contains(p1))
                {
                    playersCollections[pturn].Add(p2);
                }
                else
                {
                    playersCollections[pturn].Add(p1);
                }
                //Debug.Log("Players checked if connected");
                TrackBlocks.GetComponent<Renderer>().sharedMaterial = originalTrackBlockMaterial;

                GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;

                if (hit.collider.gameObject.tag == "dBridge" || hit.collider.gameObject.tag == "hBridge" || hit.collider.gameObject.tag == "vBridge")
                {
                    layedTracks += 2;
                }
                else
                {
                    if (layedTracks == 0)
                    {
                        lastTrackPlaced = block;
                    }
                    else
                    {
                        lastTrackPlaced = null;
                    }

                    layedTracks += 1;
                }

                if (botsLastTurnTrackBlocks.Count != 0)
                {
                    foreach (GameObject botsTrack in botsLastTurnTrackBlocks)
                    {
                        botsTrack.GetComponent<Renderer>().sharedMaterial = originalTrackBlockMaterial;
                    }

                    botsLastTurnTrackBlocks.Clear();
                }

                UpdateCount();

                try
                {
                    addPlayersCollectionsIfConnected(pturn);
                }
                catch
                {
                    return;
                }
            }
        }


        if (layedTracks == 2)
        {
            changeTurn();
        }
    }

    void botsLogic(int turn)
    {
        float z = new float();
        float x = new float();

        Vector3 p1 = new Vector3();
        Vector3 p2 = new Vector3();

        if (hit.collider.gameObject.tag == "dLine")
        {
            //Debug.Log("Ray hits the line");
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            //Debug.Log("Made it to through the point calculations");

            p1 = new Vector3(x + 0.25f, 1f, z - 0.5f);
            p2 = new Vector3(x - 0.25f, 1f, z + 0.5f);
        }
        else if (hit.collider.gameObject.tag == "hLine")
        {
            //Debug.Log("Ray hits the line");
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            p1 = new Vector3(x + 0.5f, 1f, z);
            p2 = new Vector3(x - 0.5f, 1f, z);

            //Debug.Log("Made it to through the point calculations");
        }
        else if (hit.collider.gameObject.tag == "vLine")
        {
            //Debug.Log("Ray hits the line");
            z = hit.collider.gameObject.transform.position.z;
            x = hit.collider.gameObject.transform.position.x;

            //Debug.Log("Made it to through the point calculations");

            p1 = new Vector3(x - 0.25f, 1f, z - 0.5f);
            p2 = new Vector3(x + 0.25f, 1f, z + 0.5f);
        }
        else if (hit.collider.gameObject.tag == "dBridge")
        {
            if (layedTracks == 1)
            {
                changeTurn();
            }
            else if (layedTracks == 0)
            {
                //Debug.Log("Ray hits the line");
                z = hit.collider.gameObject.transform.position.z;
                x = hit.collider.gameObject.transform.position.x;

                //Debug.Log("Made it to through the point calculations");

                p1 = new Vector3(x + 0.25f, 1f, z - 0.5f);
                p2 = new Vector3(x - 0.25f, 1f, z + 0.5f);

            }
        }
        else if (hit.collider.gameObject.tag == "hBridge")
        {
            if (layedTracks == 1)
            {
                changeTurn();
            }
            else if (layedTracks == 0)
            {
                //Debug.Log("Ray hits the line");
                z = hit.collider.gameObject.transform.position.z;
                x = hit.collider.gameObject.transform.position.x;

                //Debug.Log("Made it to through the point calculations");

                p1 = new Vector3(x + 0.5f, 1f, z);
                p2 = new Vector3(x - 0.5f, 1f, z);
            }
        }
        else if (hit.collider.gameObject.tag == "vBridge")
        {
            if (layedTracks == 1)
            {
                changeTurn();
            }
            else if (layedTracks == 0)
            {
                //Debug.Log("Ray hits the line");
                z = hit.collider.gameObject.transform.position.z;
                x = hit.collider.gameObject.transform.position.x;

                //Debug.Log("Made it to through the point calculations");

                p1 = new Vector3(x - 0.25f, 1f, z - 0.5f);
                p2 = new Vector3(x + 0.25f, 1f, z + 0.5f);

            }
        }

        if (playersCollections[turn].Contains(p1) && playersCollections[turn].Contains(p2))
        {

        } else 
        {
            if (playersCollections[turn].Contains(p1) || playersCollections[turn].Contains(p2))
            {
                if (playersCollections[turn].Contains(p1))
                {
                    playersCollections[turn].Add(p2);
                }
                else
                {
                    playersCollections[turn].Add(p1);
                }
                //Debug.Log("Made it to the vertical bridge placement");

                Material trackColor = getHubColor();
                GameObject botsTrack = TrackBlocks;
                botsTrack.GetComponent<Renderer>().sharedMaterial = trackColor;

                GameObject block = Instantiate(botsTrack, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                botsLastTurnTrackBlocks.Add(block);

                if (hit.collider.gameObject.tag == "dBridge" || hit.collider.gameObject.tag == "hBridge" || hit.collider.gameObject.tag == "vBridge")
                {
                    layedTracks += 2;
                }
                else
                {
                    layedTracks += 1;
                }
                //Debug.Log("Tracks layed " + layedTracks);
                UpdateCount();
                try
                {
                    addPlayersCollectionsIfConnected(turn);
                }
                catch
                {
                    return;
                }
                finally
                {
                    //prints out all the poins in players collections
                    //foreach (Vector3 p in playersCollections[turn])
                    //{
                    //    Debug.Log("player3sCollection points: " + p);
                    //}
                    if (layedTracks == 2)
                    {
                        changeTurn();
                    }
                }
            }
        }


        try
        {
            foreach (Vector3 vp in botsvps[turn - humanPlayers])
            {
                if (playersCollections[turn].Contains(vp))
                {
                    botsvps[turn - humanPlayers].Remove(vp);
                }
            }
        }
        catch
        {
            return;
        }
    }


    void botwillplaceahub()
    {
        Dictionary<Vector3, int> distances_to_victory_points = new Dictionary<Vector3, int>();
        foreach (Vector3 x in definedPoints)
        {
            Dictionary<Vector3, int> d_to_vps = shortest_distances(graph, x);
            int total_distance = new int();
            foreach (Vector3 y in players[turn])
            {
                total_distance += d_to_vps[y];
            }
            distances_to_victory_points[x] = total_distance;
        }

        //Vector3 hub_point = shortest_distance_node(distances_to_victory_points);

        //foreach (Vector3 x in playersHubCollections)
        //{
        //    if (x == hub_point)
        //    {
        //        distances_to_victory_points.Remove(hub_point);
        //        hub_point = shortest_distance_node(distances_to_victory_points);
        //    }
        //}

        //Debug.Log("The best point for bot to place the hub is: " + hub_point);

        //foreach (Vector3 x in distances_to_victory_points.Keys)
        //{
        //    Debug.Log("Total distance to all victory points from " + x + " is " + distances_to_victory_points[x]);
        //}

        string difficulty = botsDifficultyLevels[turn];

        Vector3 hub_point = getHubLocation(distances_to_victory_points, difficulty);

        GameObject bots_hub = Instantiate(hub(), hub_point + new Vector3(0f, 0.25f, 0f), Quaternion.identity) as GameObject;
        playersCollections[turn].Add(hub_point);
        playersHubCollections.Add(hub_point);
        hubCount++;

        if (botsvps[turn - humanPlayers].Contains(hub_point))
            botsvps[turn - humanPlayers].Remove(hub_point);

        changeTurn();
    }


    GameObject hub()
    {
        switch (turn)
        {
            case 0:
                return userHubs1;
            case 1:
                return userHubs2;
            case 2:
                return userHubs3;
            case 3:
                return userHubs4;
            case 4:
                return userHubs5;
            default:
                return userHubs6;
        }
    }


    bool placeHub(int turn)
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.gameObject.tag == "Point")
                {

                    GameObject startingPoint = Instantiate(hub(), hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                    Vector3 hubPosition = hit.collider.gameObject.transform.position;
                    playersCollections[turn].Insert(0, hubPosition);
                    playersHubCollections.Add(hubPosition);
                    hubCount++;
                    return true;
                }
            }
        }
        return false;
    }

    // changes turns
    void changeTurn()
    {
        //System.Threading.Thread.Sleep(5000);
        if (placingHubs == true)
        {
            if (checkAllPlayersHaveHub() == true)
            {
                placingHubs = false;
                turn = 0;
                UpdateTurn();
            }
            else
            {
                if (turn + 1 < maxHubs)
                {
                    turn += 1;
                    UpdateTurn();
                }
                else
                {
                    turn = 0;
                    UpdateTurn();
                }
            }
        }
        else
        {
            layedTracks = 0;
            UpdateCount();
            if (turn + 1 < maxHubs)
            {
                turn += 1;
                UpdateTurn();
            }
            else
            {
                turn = 0;
                UpdateTurn();
            }
        }
    }

    // return the winner
    int winner()
    {
        for (int i = 0; i < maxHubs; i++)
        {
            if (playersCollections[i].Count > 1)
            {
                if (playersCollections[i].Contains(players[i][0]) && playersCollections[i].Contains(players[i][1]) && playersCollections[i].Contains(players[i][2]) && playersCollections[i].Contains(players[i][3]))
                {
                    return i + 1;
                }
            }
        }
        return 0;
    }

    // checks if any winner
    bool checkIfAnyWinner()
    {
        //		Debug.Log ("entered check winner loop");
        for (int i = 0; i < maxHubs; i++)
        {
            //			Debug.Log ("Checking for player " + i);
            foreach (Vector3 x in players[i])
            {
                //				Debug.Log ("player " + i + " vps: " + x);
            }
            if (playersCollections[i].Count > 1)
            {
                if (playersCollections[i].Contains(players[i][0]) && playersCollections[i].Contains(players[i][1]) && playersCollections[i].Contains(players[i][2]) && playersCollections[i].Contains(players[i][3]))
                {
                    setTracksColorToDefault();
                    return true;
                }
            }
        }
        return false;
    }

    void setTracksColorToDefault()
    {
        TrackBlocks.GetComponent<Renderer>().sharedMaterial = originalTrackBlockMaterial;
    }

    // shows how many moves left for the player
    void UpdateCount()
    {
        if (layedTracks == 0)
        {
            count = 2;
        }
        else if (layedTracks == 1)
        {
            count = 1;
        }
        movesText.text = "Moves left: " + count;
    }

    // updates turn
    void UpdateTurn()
    {
        switch (turn)
        {
            case 0:
                if (MainMenu.humanPlayersName != "")
                {
                    player = MainMenu.humanPlayersName;
                } else {
                    player = "You";
                }
                break;
            case 1:
                player = "Player 2";
                break;
            case 2:
                player = "Player 3";
                break;
            case 3:
                player = "Player 4";
                break;
            case 4:
                player = "Player 5";
                break;
            case 5:
                player = "Player 6";
                break;
        }
        playersTurnText.text = "Turn: " + player;
    }


    // Generates the board map
    void GenerateRhombicalGrid2()
    {
        horizontalLines = new Vector3[xSize * (zSize + 1)];
        Quaternion horizontalLine = Quaternion.Euler(90f, 90f, 0f);

        verticalLines = new Vector3[(xSize + 1) * zSize];
        Quaternion verticalLine = Quaternion.Euler(90f, 26.5f, 0f);

        diagonalLines = new Vector3[xSize * zSize];
        Quaternion diagonalLine = Quaternion.Euler(90f, -26.5f, 0f);

        Vector3 vertexOffset = new Vector3(0.5f, 0f, 0f);
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        Vector3 verticalLineOffset = new Vector3(0.25f, 0f, 0.5f);
        Vector3 diagonalLineOffset = new Vector3(-0.25f, 0f, 0.5f);


        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, 1f, z);
                if (delete_vertices.Contains(vertices[i]))
                {
                    continue;
                }
                else
                {
                    GameObject vertex = Instantiate(point, vertices[i] + (z * vertexOffset), Quaternion.identity) as GameObject;
                    if (city_zones.ContainsKey(vertices[i]))
                    {
                        if (city_zones[vertices[i]] == "zone1")
                        {
                            GameObject green_cities = Instantiate(greenCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone1.Add(vertex.transform.position);
                            allCities.Add(vertex.transform.position);
                            zone1_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone1_full")
                        {
                            if (maxHubs >= 5) 
                            {
                                GameObject green_cities = Instantiate(greenCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                                allCities.Add(vertex.transform.position);
                            }
                            zone1_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone2")
                        {
                            GameObject blue_cities = Instantiate(blueCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone2.Add(vertex.transform.position);
                            allCities.Add(vertex.transform.position);
                            zone2_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone2_full")
                        {
                            if (maxHubs >= 5)
                            {
                                GameObject blue_cities = Instantiate(blueCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                                allCities.Add(vertex.transform.position);
                            }
                            zone2_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone3")
                        {
                            GameObject red_cities = Instantiate(redCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone3.Add(vertex.transform.position);
                            allCities.Add(vertex.transform.position);
                            zone3_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone3_full")
                        {
                            if (maxHubs >= 5)
                            {
                                GameObject red_cities = Instantiate(redCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                                allCities.Add(vertex.transform.position);
                            }
                            zone3_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone4")
                        {
                            GameObject orange_cities = Instantiate(orangeCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone4.Add(vertex.transform.position);
                            allCities.Add(vertex.transform.position);
                            zone4_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone4_full")
                        {
                            if (maxHubs >= 5)
                            {
                                GameObject orange_cities = Instantiate(orangeCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                                allCities.Add(vertex.transform.position);
                            }
                            zone4_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone5")
                        {
                            GameObject yellow_cities = Instantiate(yellowCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone5.Add(vertex.transform.position);
                            allCities.Add(vertex.transform.position);
                            zone5_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone5_full")
                        {
                            if (maxHubs >= 5)
                            {
                                GameObject yellow_cities = Instantiate(yellowCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                                allCities.Add(vertex.transform.position);
                            }
                            zone5_full.Add(vertex.transform.position);
                        }
                    }
                    definedPoints.Add(vertex.transform.position);
                }
            }
        }


        for (int h = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x < xSize; x++, h++)
            {
                horizontalLines[h] = new Vector3(x, 1f, z);
                if (delete_horizontalLines.Contains(horizontalLines[h]))
                {
                    continue;
                }
                else
                {
                    if (horizontal_bridges.ContainsKey(horizontalLines[h]))
                    {
                        if (horizontal_bridges[horizontalLines[h]] == "hbridge")
                        {
                            Vector3 node1 = new Vector3(x, 1f, z) + (z * vertexOffset);
                            Vector3 node2 = new Vector3(x + 1f, 1f, z) + (z * vertexOffset);
                            if (bridgeNodes.Contains(node1) == false)
                            {
                                bridgeNodes.Add(node1);
                            }
                            if (bridgeNodes.Contains(node2) == false)
                            {
                                bridgeNodes.Add(node2);
                            }
                            GameObject horizontal_bridge = Instantiate(hbridge, horizontalLines[h] + new Vector3(0.5f, 0f, 0f) + (z * vertexOffset), horizontalLine) as GameObject;
                            horizontal_bridge.gameObject.tag = "hBridge";
                        }
                    }
                    else
                    {
                        GameObject horizontal_line = Instantiate(line, horizontalLines[h] + new Vector3(0.5f, 0f, 0f) + (z * vertexOffset), horizontalLine) as GameObject;
                        horizontal_line.gameObject.tag = "hLine";
                    }
                }
            }
        }

        for (int v = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, v++)
            {
                verticalLines[v] = new Vector3(x, 1f, z);
                if (delete_verticalLines.Contains(verticalLines[v]))
                {
                    continue;
                }
                else
                {
                    if (vertical_bridges.ContainsKey(verticalLines[v]))
                    {
                        if (vertical_bridges[verticalLines[v]] == "vbridge")
                        {
                            verticalLines[v] = verticalLineOffset + (z * vertexOffset) + new Vector3(x, 1f, z);
                            Vector3 node1 = new Vector3(x, 1f, z) + (z * vertexOffset);
                            Vector3 node2 = new Vector3(x, 1f, z) + (z * vertexOffset) + new Vector3(0.5f, 0f, 1f);
                            if (bridgeNodes.Contains(node1) == false)
                            {
                                bridgeNodes.Add(node1);
                            }
                            if (bridgeNodes.Contains(node2) == false)
                            {
                                bridgeNodes.Add(node2);
                            }
                            GameObject vertical_bridge = Instantiate(vbridge, verticalLines[v], verticalLine) as GameObject;
                            vertical_bridge.gameObject.tag = "vBridge";
                        }
                    }
                    else
                    {
                        verticalLines[v] = verticalLineOffset + (z * vertexOffset) + new Vector3(x, 1f, z);
                        GameObject vertical_line = Instantiate(vrline, verticalLines[v], verticalLine) as GameObject;
                        vertical_line.gameObject.tag = "vLine";
                    }
                }
            }
        }

        for (int d = 0, z = 0; z < zSize; z++)
        {
            for (int x = 1; x <= xSize; x++, d++)
            {
                diagonalLines[d] = new Vector3(x, 1f, z);
                if (delete_diagonalLines.Contains(diagonalLines[d]))
                {
                    continue;
                }
                else
                {
                    if (diagonal_bridges.ContainsKey(diagonalLines[d]))
                    {
                        if (diagonal_bridges[diagonalLines[d]] == "dbridge")
                        {
                            diagonalLines[d] = diagonalLineOffset + (z * vertexOffset) + new Vector3(x, 1f, z);
                            Vector3 node1 = new Vector3(x, 1f, z) + (z * vertexOffset);
                            Vector3 node2 = new Vector3(x, 1f, z) + (z * vertexOffset) + new Vector3(-0.5f, 0f, 1f);
                            if (bridgeNodes.Contains(node1) == false)
                            {
                                bridgeNodes.Add(node1);
                            }
                            if (bridgeNodes.Contains(node2) == false)
                            {
                                bridgeNodes.Add(node2);
                            }
                            GameObject diagonal_bridge = Instantiate(dbridge, diagonalLines[d], diagonalLine) as GameObject;
                            diagonal_bridge.gameObject.tag = "dBridge";
                        }
                    }
                    else
                    {
                        diagonalLines[d] = diagonalLineOffset + (z * vertexOffset) + new Vector3(x, 1f, z);
                        GameObject diagonal_line = Instantiate(drline, diagonalLines[d], diagonalLine) as GameObject;
                        diagonal_line.gameObject.tag = "dLine";
                    }
                }
            }
        }
    }

    //disables clicking on line until all the players have placed their hub
    void disableTrackInstantiation()
    {
        if (gameObject.tag == "hLine" || gameObject.tag == "vLine" || gameObject.tag == "dLine" || gameObject.tag == "hBridge" || gameObject.tag == "dBridge" || gameObject.tag == "vBridge")
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
    }


    //disables clicking on vertex as soon as all the players have place their hub
    void disableHubInstantiation()
    {
        if (gameObject.tag == "Point")
        {
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }


    //checks if all players have layed their hubs
    bool checkAllPlayersHaveHub()
    {
        if (maxHubs == hubCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public ArrayList bridgeNodeList()
    {
        return bridgeNodes;
    }

    // find shortest distances in the graph from a node
    public Dictionary<Vector3, int> shortest_distances(Graph G, Vector3 v)
    {
        Dictionary<Vector3, int> distance_so_far = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> final_distance = new Dictionary<Vector3, int>();
        //		Dictionary<Vector3, List<Vector3>> path_to = new Dictionary<Vector3, List<Vector3>> ();
        distance_so_far[v] = 0;
        //		path_to [v] = null;
        while (final_distance.Count < G.nodes.Count)
        {
            Vector3 w = shortest_distance_node(distance_so_far);
            final_distance[w] = distance_so_far[w];
            distance_so_far.Remove(w);
            foreach (Vector3 x in G.get_neighbour_nodes(w))
            {
                if (final_distance.ContainsKey(x) == false)
                {
                    if (distance_so_far.ContainsKey(x) == false)
                    {
                        distance_so_far[x] = final_distance[w] + G.get_distance_between_nodes(x, w);
                        //						path_to [x] = path_to [w];
                        //						path_to [x].Add (w);
                    }
                    else if (final_distance[w] + G.get_distance_between_nodes(x, w) < distance_so_far[x])
                    {
                        distance_so_far[x] = final_distance[w] + G.get_distance_between_nodes(x, w);
                        //						path_to [x] = path_to [w];
                        //						path_to [x].Add (w);
                    }
                }
            }
        }
        //Debug.Log("final_distance.Count: " + final_distance.Count);
        //Debug.Log("G.nodes.Count: " + G.nodes.Count);

        //		Debug.Log ("path_to: " + path_to.Count);

        return final_distance;

    }


    private Vector3 getHubLocation(Dictionary<Vector3, int> d, string botsDifficulty)
    {
        ArrayList hubLocationsToChooseFrom = new ArrayList();

        Vector3 best_node = new Vector3();
        int best_value = 100000;

        foreach (Vector3 x in d.Keys)
        {
            if (d[x] < best_value)
            {
                best_node = x;
                best_value = d[x];
            }
        }

        foreach (Vector3 x in d.Keys)
        {
            if (d[x] == best_value)
            {
                if (playersHubCollections.Contains(x) == false)
                {
                    if (botsDifficulty == "medium")
                    {
                        if (players[turn].Contains(x))
                        {
                            hubLocationsToChooseFrom.Add(x);
                        } else 
                        {
                            if (allCities.Contains(x) == false)
                            {
                                hubLocationsToChooseFrom.Add(x);
                            }
                        }
                    } else
                    {
                        hubLocationsToChooseFrom.Add(x);
                    }
                }
            }
        }

        best_node = (Vector3)hubLocationsToChooseFrom[Random.Range(0, hubLocationsToChooseFrom.Count - 1)];

        return best_node;

        //foreach (Vector3 x in playersHubCollections)
        //{
        //    if (x == hub_point)
        //    {
        //        distances_to_victory_points.Remove(hub_point);
        //        hub_point = shortest_distance_node(distances_to_victory_points);
        //    }
        //}
    }

    public Vector3 shortest_distance_node(Dictionary<Vector3, int> d)
    {
        Vector3 best_node = new Vector3();
        int best_value = 100000;
        foreach (Vector3 x in d.Keys)
        {
            if (d[x] < best_value)
            {
                best_node = x;
                best_value = d[x];
            }
        }
        return best_node;
    }


    bool isTheTrackPlacedByAnotherPlayer(Vector3 point1, Vector3 point2)
    {
        for (int i = 0; i < maxHubs; i++)
        {
            //if (i == turn) 
            //{
            //    continue;
            //} 
            //else 
            //{
            if (playersCollections[i].Contains(point1) && playersCollections[i].Contains(point2))
            {
                return true;
            }
            //}
        }

        return false;
    }

    int getTotalValueOfThePath(ArrayList path)
    {
        int value = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            int distanceBetween = 0;
            if (isTheTrackPlacedByAnotherPlayer((Vector3)path[i], (Vector3)path[i + 1]) == false)
            {
                distanceBetween = graph.get_distance_between_nodes((Vector3)path[i], (Vector3)path[i + 1]);
            }

            value = value + distanceBetween;
        }

        return value;
    }


    public ArrayList efficient_shortes_path()
    {
        ArrayList tracks_to_place = new ArrayList();

        Dictionary<Vector3, ArrayList> various_paths_to_vps = new Dictionary<Vector3, ArrayList>();

        foreach (Vector3 playersPoint in playersCollections[turn])
        {
            //Debug.Log("playersPoint: " + playersPoint.ToString());
            Dictionary<Vector3, int> distance_so_far = new Dictionary<Vector3, int>();
            Dictionary<Vector3, int> final_distance = new Dictionary<Vector3, int>();
            Dictionary<Vector3, Vector3> path_to = new Dictionary<Vector3, Vector3>();
            distance_so_far[playersPoint] = 0;
            path_to[playersPoint] = playersPoint;
            while (final_distance.Count < graph.nodes.Count)
            {
                Vector3 w = shortest_distance_node(distance_so_far);
                final_distance[w] = distance_so_far[w];
                distance_so_far.Remove(w);
                foreach (Vector3 x in graph.get_neighbour_nodes(w))
                {
                    if (final_distance.ContainsKey(x) == false)
                    {
                        int distance_between_the_nodes = new int();
                        if (isTheTrackPlacedByAnotherPlayer(x, w) == true)
                            distance_between_the_nodes = 0;
                        else
                            distance_between_the_nodes = graph.get_distance_between_nodes(x, w);

                        if (distance_so_far.ContainsKey(x) == false)
                        {
                            distance_so_far[x] = final_distance[w] + distance_between_the_nodes;
                            path_to[x] = w;
                        }
                        else if (final_distance[w] + distance_between_the_nodes < distance_so_far[x])
                        {
                            distance_so_far[x] = final_distance[w] + distance_between_the_nodes;
                            path_to[x] = w;
                        }
                    }
                }
            }

            foreach (Vector3 point in botsvps[turn - humanPlayers])
            {
                //Debug.Log("^^^^^^^^^^^^^^^^^");
                //Debug.Log(point);
                if (path_to.ContainsKey(point))
                {
                    ArrayList path = new ArrayList();
                    if (path_to[point] != playersPoint)
                    {
                        bool source_point_found = false;
                        Vector3 pointer_point = point;
                        //various_paths_to_vps[point] = new ArrayList();
                        //various_paths_to_vps[point].Add(point);
                        //path.Add(point);
                        path.Insert(0, point);
                        while (source_point_found != true)
                        {
                            path.Insert(0, path_to[pointer_point]);
                            //Debug.Log(path_to[pointer_point]);
                            //path.Add(path_to[pointer_point]);
                            //various_paths_to_vps[point].Add(path_to[pointer_point]);
                            pointer_point = path_to[pointer_point];
                            if (pointer_point == playersPoint)
                            {
                                source_point_found = true;
                                //path.Add(pointer_point);
                                //path.Insert(0, pointer_point);
                                //various_paths_to_vps[point].Remove(playersPoint);
                            }
                        }
                    }
                    else
                    {
                        if (point != (Vector3)playersCollections[turn][0])
                        {
                            //Debug.Log("Entered else loop");
                            //path.Add(point);
                            path.Insert(0, path_to[point]);
                            //Debug.Log(point);
                            //various_paths_to_vps[point] = new ArrayList();
                            //various_paths_to_vps[point].Add(point);
                        }
                    }

                    if (various_paths_to_vps.ContainsKey(point))
                    {
                        int newPathValue = getTotalValueOfThePath(path);
                        int oldPathValue = getTotalValueOfThePath(various_paths_to_vps[point]);

                        //Debug.Log("newpathValue: " + newPathValue.ToString());
                        //Debug.Log("oldPathValue: " + oldPathValue.ToString());

                        if (newPathValue <= oldPathValue)
                        {
                            various_paths_to_vps[point].Clear();
                            various_paths_to_vps[point] = path;
                        }
                    }
                    else
                    {
                        various_paths_to_vps[point] = new ArrayList();
                        various_paths_to_vps[point] = path;
                    }
                }
            }
        }


        //Vector3 victoryPoint = new Vector3();
        //Vector3 longestVictoryPoint = new Vector3();

        //foreach(Vector3 vp in various_paths_to_vps.Keys)
        //{
        //    //Debug.Log("++++++++++++");
        //    //Debug.Log(getTotalValueOfThePath(various_paths_to_vps[vp]));
        //    //Debug.Log(vp);
        //    //foreach (Vector3 point in various_paths_to_vps[vp])
        //    //{
        //    //    Debug.Log(point);
        //    //}

        //    if (victoryPoint == new Vector3())
        //    {
        //        victoryPoint = vp;
        //        longestVictoryPoint = vp;
        //    }
        //    else
        //    {
        //        if (getTotalValueOfThePath(various_paths_to_vps[vp]) < getTotalValueOfThePath(various_paths_to_vps[victoryPoint]))
        //        {
        //            victoryPoint = vp;
        //        }

        //        if (getTotalValueOfThePath(various_paths_to_vps[vp]) > getTotalValueOfThePath(various_paths_to_vps[victoryPoint]))
        //        {
        //            longestVictoryPoint = vp;
        //        }
        //    }
        //}


        //Debug.Log("victory point" + victoryPoint.ToString("F2"));


        //if (various_paths_to_vps.ContainsKey(victoryPoint))
        //{
        //    int totalDistance = 0;
        //    int loopThrough = various_paths_to_vps[victoryPoint].Count - 1;
        //    if (various_paths_to_vps[victoryPoint].Count == 1)
        //        loopThrough = 1;

        //    for (int i = 0; i < loopThrough; i++)
        //    {
        //        Debug.Log("----------------");

        //        //if (playersCollections[turn].Contains((Vector3)various_paths_to_vps[victoryPoint][i]))
        //        //{
        //        //    Debug.Log(i);
        //        //    Debug.Log((Vector3)various_paths_to_vps[victoryPoint][i]);
        //        //}

        //        if (totalDistance < 2) 
        //        {

        //            Vector3 point1 = (Vector3)various_paths_to_vps[victoryPoint][i];
        //            Vector3 point2 = new Vector3();

        //            if (loopThrough == 1) {
        //                point2 = victoryPoint;
        //            } else {
        //                point2 = (Vector3)various_paths_to_vps[victoryPoint][i + 1];
        //            }

        //            int distanceBetween2Points = graph.get_distance_between_nodes(point1, point2);

        //            if (distanceBetween2Points == 2 && totalDistance != 0)
        //            {
        //                if (various_paths_to_vps.ContainsKey(longestVictoryPoint))
        //                {
        //                    totalDistance = 0;
        //                    loopThrough = various_paths_to_vps[longestVictoryPoint].Count - 1;
        //                    if (various_paths_to_vps[victoryPoint].Count == 1)
        //                        loopThrough = 1;

        //                    for (int j = 0; j < loopThrough; j++)
        //                    {
        //                        if (totalDistance < 2)
        //                        {
        //                            point1 = (Vector3)various_paths_to_vps[victoryPoint][j];
        //                            point2 = new Vector3();

        //                            if (loopThrough == 1)
        //                            {
        //                                point2 = victoryPoint;
        //                            }
        //                            else
        //                            {
        //                                point2 = (Vector3)various_paths_to_vps[victoryPoint][j + 1];
        //                            }

        //                            distanceBetween2Points = graph.get_distance_between_nodes(point1, point2);

        //                            if (distanceBetween2Points < 2)
        //                            {
        //                                Vector3 trackPlacement = getTrackPlacingPoints(point1, point2);
        //                                //Debug.Log("==========");
        //                                //Debug.Log(point1);
        //                                //Debug.Log(point2);
        //                                //Debug.Log(trackPlacement);

        //                                if (isTheTrackPlacedByAnotherPlayer(point1, point2) == false)
        //                                {
        //                                    tracks_to_place.Add(trackPlacement);
        //                                    totalDistance = totalDistance + distanceBetween2Points;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //            } else {
        //                Vector3 trackPlacement = getTrackPlacingPoints(point1, point2);
        //                //Debug.Log("==========");
        //                //Debug.Log(point1);
        //                //Debug.Log(point2);
        //                //Debug.Log(trackPlacement);

        //                if (isTheTrackPlacedByAnotherPlayer(point1, point2) == false)
        //                {
        //                    tracks_to_place.Add(trackPlacement);
        //                    totalDistance = totalDistance + distanceBetween2Points;
        //                }
        //            }
        //        }
        //    }
        //}

        tracks_to_place = getTracksToPlaceByBot(various_paths_to_vps);

        return tracks_to_place;
    }

    ArrayList getTracksToPlaceByBot(Dictionary<Vector3, ArrayList> various_paths_to_vps)
    {
        ArrayList tracks = new ArrayList();
        int totalDistance = 0;

        Vector3 victoryPoint = getShortestVictoryPoint(various_paths_to_vps);

        //while (tracks.Count < 2)
        //{
        int loopThrough = various_paths_to_vps[victoryPoint].Count - 1;
        if (various_paths_to_vps[victoryPoint].Count == 1)
        {
            loopThrough = 1;
        }

        for (int i = 0; i < loopThrough; i++)
        {
            if (totalDistance < 2)
            {
                Vector3 point1 = (Vector3)various_paths_to_vps[victoryPoint][i];
                Vector3 point2 = new Vector3();

                if (loopThrough == 1)
                {
                    point2 = victoryPoint;
                    botsvps[turn - humanPlayers].Remove(victoryPoint);
                    victoryPoint = getShortestVictoryPoint(various_paths_to_vps);
                }
                else
                {
                    point2 = (Vector3)various_paths_to_vps[victoryPoint][i + 1];
                }

                int distanceBetween2Points = graph.get_distance_between_nodes(point1, point2);

                if (distanceBetween2Points == 2 && totalDistance != 0)
                {
                    if (botsvps[turn - humanPlayers].Count > 1)
                    {
                        victoryPoint = getLongestVictoryPoint(various_paths_to_vps);
                        loopThrough = various_paths_to_vps[victoryPoint].Count - 1;
                        if (various_paths_to_vps[victoryPoint].Count == 1)
                        {
                            loopThrough = 1;
                        }
                        continue;
                    }
                    else
                    {
                        changeTurn();
                    }
                }
                else
                {
                    Vector3 trackPlacement = getTrackPlacingPoints(point1, point2);
                    //Debug.Log("==========");
                    //Debug.Log(point1);
                    //Debug.Log(point2);
                    //Debug.Log(trackPlacement);

                    if (isTheTrackPlacedByAnotherPlayer(point1, point2) == false)
                    {
                        tracks.Add(trackPlacement);
                        totalDistance++;
                        //totalDistance = totalDistance + distanceBetween2Points;
                    }
                }
            }
        }
        //}

        return tracks;
    }


    Vector3 getShortestVictoryPoint(Dictionary<Vector3, ArrayList> various_paths_to_vps)
    {
        Vector3 victoryPoint = new Vector3();

        foreach (Vector3 vp in various_paths_to_vps.Keys)
        {
            //Debug.Log("++++++++++++");
            //Debug.Log(getTotalValueOfThePath(various_paths_to_vps[vp]));
            //Debug.Log(vp);
            //foreach (Vector3 point in various_paths_to_vps[vp])
            //{
            //    Debug.Log(point);
            //}

            if (victoryPoint == new Vector3())
            {
                victoryPoint = vp;
            }
            else
            {
                if (getTotalValueOfThePath(various_paths_to_vps[vp]) < getTotalValueOfThePath(various_paths_to_vps[victoryPoint]))
                {
                    victoryPoint = vp;
                }
            }
        }

        return victoryPoint;
    }


    Vector3 getLongestVictoryPoint(Dictionary<Vector3, ArrayList> various_paths_to_vps)
    {
        Vector3 victoryPoint = new Vector3();

        foreach (Vector3 vp in various_paths_to_vps.Keys)
        {
            //Debug.Log("++++++++++++");
            //Debug.Log(getTotalValueOfThePath(various_paths_to_vps[vp]));
            //Debug.Log(vp);
            //foreach (Vector3 point in various_paths_to_vps[vp])
            //{
            //    Debug.Log(point);
            //}

            if (victoryPoint == new Vector3())
            {
                victoryPoint = vp;
            }
            else
            {
                if (getTotalValueOfThePath(various_paths_to_vps[vp]) > getTotalValueOfThePath(various_paths_to_vps[victoryPoint]))
                {
                    victoryPoint = vp;
                }
            }
        }

        return victoryPoint;
    }

    Vector3 getTrackPlacingPoints(Vector3 startingPoint, Vector3 secondPoint)
    {
        Vector3 track_position = new Vector3();

        if (secondPoint.z == startingPoint.z)
        {
            track_position = new Vector3((secondPoint.x + startingPoint.x) / 2f, startingPoint.y, startingPoint.z);
        }
        else
        {
            if (secondPoint.x < startingPoint.x)
            {
                track_position = new Vector3((secondPoint.x + 0.25f), 1f, (secondPoint.z + startingPoint.z) / 2f);
            }
            if (secondPoint.x > startingPoint.x)
            {
                track_position = new Vector3((secondPoint.x - 0.25f), 1f, (secondPoint.z + startingPoint.z) / 2f);
            }
        }

        track_position.y += 10f;

        return track_position;
    }

    public ArrayList shortestPathToVP(ArrayList paths)
    {
        ArrayList shortest_path = new ArrayList();
        foreach (ArrayList path in paths)
        {
            if (shortest_path.Count == 0)
            {
                shortest_path = path;
            }
            else
            {
                if (path.Count < shortest_path.Count)
                {
                    shortest_path = path;
                }
            }
        }

        return shortest_path;
    }

    //public ArrayList calculate_path_from_vp(Dictionary<Vector3, Vector3> path_to_vp)
    //{
    //    ArrayList path = new ArrayList();

    //    foreach (Vector3 point in path_to_vp.Keys)
    //    {
    //        path.Insert(0, point);
    //    }

    //    return path;
    //}


    // find the path to the victory points from a node
    public Dictionary<Vector3, List<Vector3>> shortest_path(Graph G, Vector3 current_starting_point)
    {
        Dictionary<Vector3, List<Vector3>> path_to_dest = new Dictionary<Vector3, List<Vector3>>();
        Dictionary<Vector3, int> distance_so_far = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> final_distance = new Dictionary<Vector3, int>();
        Dictionary<Vector3, Vector3> path_to = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, List<Vector3>> paths_to_vps = new Dictionary<Vector3, List<Vector3>>();
        distance_so_far[current_starting_point] = 0;
        path_to[current_starting_point] = current_starting_point;
        while (final_distance.Count < G.nodes.Count)
        {
            Vector3 w = shortest_distance_node(distance_so_far);
            final_distance[w] = distance_so_far[w];
            distance_so_far.Remove(w);
            foreach (Vector3 x in G.get_neighbour_nodes(w))
            {
                if (final_distance.ContainsKey(x) == false)
                {
                    if (distance_so_far.ContainsKey(x) == false)
                    {
                        distance_so_far[x] = final_distance[w] + G.get_distance_between_nodes(x, w);
                        path_to[x] = w;
                    }
                    else if (final_distance[w] + G.get_distance_between_nodes(x, w) < distance_so_far[x])
                    {
                        distance_so_far[x] = final_distance[w] + G.get_distance_between_nodes(x, w);
                        path_to[x] = w;
                    }
                }
            }
        }

        foreach (Vector3 point in botsvps[turn - humanPlayers])
        {
            if (path_to.ContainsKey(point))
            {
                if (path_to[point] != current_starting_point)
                {
                    bool source_point_found = false;
                    Vector3 pointer_point = point;
                    paths_to_vps[point] = new List<Vector3>();
                    paths_to_vps[point].Add(point);
                    while (source_point_found != true)
                    {
                        paths_to_vps[point].Add(path_to[pointer_point]);
                        pointer_point = path_to[pointer_point];
                        if (pointer_point == current_starting_point)
                        {
                            source_point_found = true;
                            paths_to_vps[point].Remove(current_starting_point);
                        }
                    }
                }
                else
                {
                    if (point != (Vector3)playersCollections[turn][0])
                    {
                        paths_to_vps[point] = new List<Vector3>();
                        paths_to_vps[point].Add(point);
                    }
                }
            }
        }

        Vector3 shortest_dest = get_shortest_dest(paths_to_vps);
        path_to_dest[shortest_dest] = paths_to_vps[shortest_dest];

        //Debug.Log("path_to: " + path_to.Count);

        return path_to_dest;
    }

    //public Dictionary<Vector3, List<Vector3>> shortest_path (Graph G, Vector3 v, Vector3 dest){
    //	Dictionary<Vector3, int> distance_so_far = new Dictionary<Vector3, int> ();
    //	Dictionary<Vector3, int> final_distance = new Dictionary<Vector3, int> ();
    //	Dictionary<Vector3, Vector3> path_to = new Dictionary<Vector3, Vector3> ();
    //	Dictionary<Vector3, List<Vector3>> path_to_dest = new Dictionary<Vector3, List<Vector3>> ();
    //	distance_so_far [v] = 0;
    //	path_to [v] = v;
    //	while (final_distance.Count < G.nodes.Count) {
    //		Vector3 w = shortest_distance_node (distance_so_far);
    //		final_distance [w] = distance_so_far [w];
    //		distance_so_far.Remove (w);
    //		foreach (Vector3 x in G.get_neighbour_nodes(w)) {
    //			if (final_distance.ContainsKey (x) == false) {
    //				if (distance_so_far.ContainsKey (x) == false) {
    //					distance_so_far [x] = final_distance [w] + G.get_distance_between_nodes (x, w);
    //					path_to [x] = w;
    //				} else if (final_distance [w] + G.get_distance_between_nodes (x, w) < distance_so_far [x]) {
    //					distance_so_far [x] = final_distance [w] + G.get_distance_between_nodes (x, w);
    //					path_to [x] = w;
    //				}
    //			}
    //			if (final_distance.ContainsKey (dest)) {
    //				break;
    //			}
    //		}
    //	}

    //	if (path_to.ContainsKey (dest)) {
    //		if (path_to [dest] != v) {
    //			bool source_point_found = false;
    //			Vector3 pointer_point = dest;
    //			path_to_dest [dest] = new List<Vector3> ();
    //			path_to_dest [dest].Add (dest);
    //			while (source_point_found != true) {
    //				path_to_dest [dest].Add (path_to [pointer_point]);
    //				pointer_point = path_to [pointer_point];
    //				if (pointer_point == v) {
    //					source_point_found = true;
    //				}
    //			}
    //		}
    //	}


    //	Debug.Log ("path_to: " + path_to.Count);

    //	return path_to_dest;
    //}

    //void setStartingPointForBot(Vector3 startingPoint) 
    //{
    //    playingBotsStartingPoint = startingPoint;
    //}

    // gets the point from which the bot has to start laying tracks again
    public Vector3 get_current_point()
    {
        Dictionary<Vector3, Vector3> point_source = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, int> shortest_distances_map = new Dictionary<Vector3, int>();
        foreach (Vector3 point in playersCollections[turn])
        {
            Dictionary<Vector3, int> d_to_vps = shortest_distances(graph, point);
            for (int i = 0; i < botsvps[turn - humanPlayers].Count; i++)
            {
                if (shortest_distances_map.ContainsKey((Vector3)botsvps[turn - humanPlayers][i]) == false)
                {
                    shortest_distances_map[(Vector3)botsvps[turn - humanPlayers][i]] = d_to_vps[(Vector3)botsvps[turn - humanPlayers][i]];
                    point_source[(Vector3)botsvps[turn - humanPlayers][i]] = point;
                }
                else
                {
                    if (shortest_distances_map[(Vector3)botsvps[turn - humanPlayers][i]] > d_to_vps[(Vector3)botsvps[turn - humanPlayers][i]])
                    {
                        shortest_distances_map[(Vector3)botsvps[turn - humanPlayers][i]] = d_to_vps[(Vector3)botsvps[turn - humanPlayers][i]];
                        point_source[(Vector3)botsvps[turn - humanPlayers][i]] = point;
                    }
                }
            }
        }
        Vector3 shortest_point_to_goto = shortest_distance_node(shortest_distances_map);
        Debug.Log("shortest node to goto from the current_point " + shortest_point_to_goto);
        return point_source[shortest_point_to_goto];
    }

    public Vector3 get_shortest_dest(Dictionary<Vector3, List<Vector3>> dict)
    {
        Dictionary<Vector3, int> distances = new Dictionary<Vector3, int>();
        foreach (Vector3 x in dict.Keys)
        {
            distances[x] = dict[x].Count;
        }
        Vector3 shortest_node = shortest_distance_node(distances);
        return shortest_node;
    }


    // checks if the player is connected to other players
    public void addPlayersCollectionsIfConnected(int player)
    {
        //		Debug.Log ("Connection Function Called");
        //		Debug.Log ("Max hubs: " + maxHubs);
        //		Debug.Log ("Player: " + player);
        for (int i = 0; i < maxHubs; i++)
        {
            //Debug.Log("Checking for player inside loop " + i);
            if (i == player)
            {
                continue;
            }
            else
            {
                foreach (Vector3 x in playersCollections[player])
                {
                    //					Debug.Log ("Checking if any players are connected");
                    if (playersCollections[i].Contains(x))
                    {
                        //						Debug.Log ("Player " + player + " is connected to " + i);
                        //						for (int count = 0; count < playersCollections[player].Count; count++) {
                        foreach (Vector3 point in playersCollections[player])
                        {
                            //							Debug.Log ("Points are added to " + player);
                            if (playersCollections[i].Contains(point) == false)
                            {
                                if (isPlayerBot(i) == true)
                                {
                                    if (botsvps[i - humanPlayers].Contains(point))
                                    {
                                        botsvps[i - humanPlayers].Remove(point);
                                    }
                                }
                                playersCollections[i].Add(point);
                            }
                        }
                        //						for(int count = 0; count < playersCollections[i].Count; count++) {
                        foreach (Vector3 point in playersCollections[i])
                        {
                            //							Debug.Log ("Points are added to " + i);
                            if (playersCollections[player].Contains(point) == false)
                            {
                                if (isPlayerBot(player) == true)
                                {
                                    if (botsvps[player - humanPlayers].Contains(point))
                                    {
                                        botsvps[player - humanPlayers].Remove(point);
                                    }
                                }
                                playersCollections[player].Add(point);
                            }
                        }
                    }
                    //					else {
                    //						Debug.Log ("No connection found for " + i);
                    //					}
                }
            }
        }
    }




}